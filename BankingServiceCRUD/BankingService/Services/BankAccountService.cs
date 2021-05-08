using BankingService.Enums;
using BankingService.Functional;
using BankingService.Models.Contexts;
using BankingService.Models.DTOs;
using BankingService.Models.Entities;
using BankingService.Services.Contracts;
using BankingService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BankingService.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly BankAccountsContext _context;
        private readonly IContactsService _contactsService;

        public BankAccountService(BankAccountsContext context,
            IContactsService contactsService)
        {
            _context = context;
            _contactsService = contactsService;
        }

        public async Task<IEnumerable<BankAccountViewModel>> GetAccounts()
        {
            IList<BankAccount> accounts = await _context.BankAccounts.ToListAsync();

            IList<BankAccountViewModel> accountsWithContacts = new List<BankAccountViewModel>();

            foreach (var account in accounts)
            {
                var contacts = await _contactsService.GetAllContacts(account);

                var accountViewModel = new BankAccountViewModel(account);

                if (contacts.Success)
                {
                    accountViewModel.Contacts = contacts.Value;
                }

                accountsWithContacts.Add(accountViewModel);
            }

            return accountsWithContacts;
        }

        public async Task<Result<BankAccountViewModel>> GetAccount(int id)
        {
            BankAccount bankAccount = null;

            try
            {
                bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.ClientID == id);
            }
            catch (Exception ex)
            {
                //account was not found 
            }

            if (bankAccount == null)
            {
                return Result.Fail<BankAccountViewModel>(HttpStatusCode.NotFound, "Bank account was not found");
            }

            var accountViewModel = new BankAccountViewModel(bankAccount);

            var contacts = await _contactsService.GetAllContacts(bankAccount);

            if (contacts.Success)
            {
                accountViewModel.Contacts = contacts.Value;
            }

            return Result.Ok(accountViewModel);
        }

        public async Task<Result<BankAccountViewModel>> CreateAccount(BankAccountDTO account)
        {
            var newAccount = new BankAccount()
            {
                AccountName = account.AccountName,
                IBAN = account.IBAN,
                ContactIds = new List<int>()
            };

            if (account.Contacts != null)
            {
                foreach (var contact in account.Contacts)
                {
                    var result = await _contactsService.CreateContact(contact);

                    if (!result.Success)
                    {
                        return Result.Fail<BankAccountViewModel>(result.StatusCode, result.Error);
                    }

                    newAccount.ContactIds.Add(contact.Id);
                }
            }

            _context.BankAccounts.Add(newAccount);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Result.Fail<BankAccountViewModel>(HttpStatusCode.InternalServerError, "Unexpected server error while saving new account");
            }

            var viewModel = new BankAccountViewModel(newAccount);

            viewModel.Contacts = account.Contacts != null ?
                account.Contacts :
                new List<Contact>();


            return Result.Ok(viewModel);
        }

        public async Task<Result> UpdateAccount(int id, BankAccountDTO newAccount)
        {
            var currentAccount = await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.ClientID == id);

            var updatedAccount = new BankAccount()
            {
                ClientID = id,
                AccountName = newAccount.AccountName,
                IBAN = newAccount.IBAN,
                ContactIds = new List<int>()
            };

            var result = await UpdateContacts(newAccount.Contacts, currentAccount, updatedAccount);
            if (!result.Success)
            {
                return result;
            }

            try
            {
                _context.Entry(currentAccount).State = EntityState.Detached;
                _context.Entry(updatedAccount).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
                {
                    return Result.Fail(HttpStatusCode.NotFound, "Account was not found");
                }
                else
                {
                    return Result.Fail(HttpStatusCode.InternalServerError, "Unexpected server error while updating an account");
                }
            }

            return Result.Ok();
        }

        private async Task<Result> UpdateContacts(List<Contact> contacts, BankAccount currentAccount, BankAccount updatedAccount)
        {
            Result result = null;

            foreach (var contact in contacts)
            {
                if (currentAccount.ContactIds.Contains(contact.Id))
                {
                    result = await _contactsService.UpdateContact(contact);
                    currentAccount.ContactIds.Remove(contact.Id);
                }
                else
                {
                    result = await _contactsService.CreateContact(contact);
                }

                if (!result.Success)
                {
                    return result;
                }

                updatedAccount.ContactIds.Add(contact.Id);
            }

            foreach (int contactId in currentAccount.ContactIds)
            {
                await _contactsService.DeleteContact(contactId);
            }

            return Result.Ok();
        }

        public async Task<Result> DeleteAccount(int id)
        {
            BankAccount bankAccount = null;

            try
            {
                bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.ClientID == id);
            }
            catch
            {
                //account was not found 
            }

            if (bankAccount == null)
            {
                return Result.Fail<BankAccountViewModel>(HttpStatusCode.NotFound, "Bank account was not found");
            }

            foreach (int contactId in bankAccount.ContactIds)
            {
                var result = await _contactsService.DeleteContact(contactId);

                if (!result.Success)
                {
                    return Result.Fail<BankAccountViewModel>(result.StatusCode, result.Error);
                }
            }

            try
            {
                _context.BankAccounts.Remove(bankAccount);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return Result.Fail(HttpStatusCode.InternalServerError, "Unexpected server error while deleting an account");
            }

            return Result.Ok();
        }


        private bool BankAccountExists(int id)
        {
            return _context.BankAccounts.Any(e => e.ClientID == id);
        }
    }
}
