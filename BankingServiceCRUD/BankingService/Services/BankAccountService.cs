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
        public BankAccountService(BankAccountsContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<BankAccount>>> GetAccounts()
        {
            return await _context.BankAccounts.ToListAsync();
        }

        public async Task<Result<BankAccount>> CreateAccount(BankAccountDTO account)
        {
            var newAccount = new BankAccount()
            {
                AccountName = account.AccountName,
                IBAN = account.IBAN
            };

            _context.BankAccounts.Add(newAccount);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Result.Fail<BankAccount>(HttpStatusCode.InternalServerError, "Unexpected server error while saving new account");
            }

            return Result.Ok(newAccount);

        }

        public async Task<Result<BankAccount>> GetAccount(int id)
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
                return Result.Fail<BankAccount>(HttpStatusCode.NotFound, "Bank account was not found");
            }

            return Result.Ok(bankAccount);
        }

        public async Task<Result> UpdateAccount(int id, BankAccountDTO newAccount)
        {
            var updatedAccount = new BankAccount()
            {
                ClientID = id,
                AccountName = newAccount.AccountName,
                IBAN = newAccount.IBAN
            };

            try
            {
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
