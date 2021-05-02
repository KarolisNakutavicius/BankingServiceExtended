using BankingService.Functional;
using BankingService.Models.Contexts;
using BankingService.Models.DTOs;
using BankingService.Models.Entities;
using BankingService.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BankingService.Services
{
    public class ContactsService : IContactsService
    {
        private readonly BankAccountsContext _context;
        private readonly IBankAccountService _accountService;
        private readonly HttpClient _httpClient;

        public ContactsService(
            BankAccountsContext context,
            IBankAccountService bankAccountService,
            HttpClient httpClient)
        {
            _context = context;
            _accountService = bankAccountService;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5000/");
        }

        public async Task<Result<IList<Contact>>> GetAllContacts(int accountId)
        {
            var result = await _accountService.GetAccount(accountId);

            if (!result.Success)
            {
                return Result.Fail<IList<Contact>>(result.StatusCode, result.Error);
            }

            BankAccount account = result.Value;

            IList<Contact> contacts = new List<Contact>();

            foreach(int contactId in account.ContactIds)
            {
                Contact contact = await _httpClient.GetFromJsonAsync<Contact>($"contacts/{contactId}");
                contacts.Add(contact);
            }

            return Result.Ok(contacts);
        }

        public async Task<Result<Contact>> CreateContact(int accountId, Contact contact)
        {
            var result = await _accountService.GetAccount(accountId);

            if (!result.Success)
            {
                return Result.Fail<Contact>(result.StatusCode, result.Error);
            }

            var contactCreationResponse = await _httpClient.PostAsJsonAsync<Contact>("contacts", contact);

            if (!contactCreationResponse.IsSuccessStatusCode)
            {
                var message = await contactCreationResponse.Content.ReadAsStringAsync();
                return Result.Fail<Contact>(contactCreationResponse.StatusCode, message);
            }

            try
            {
                BankAccount account = result.Value;

                if (account.ContactIds == null)
                {
                    account.ContactIds = new List<int>();
                }

                account.ContactIds.Add((int)contact.Id);
                _context.Entry(account).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Result.Fail<Contact>(HttpStatusCode.InternalServerError, "Unexpected server error while updating an account");
            }

            return Result.Ok((Contact)contact);
        }

        public async Task<Result<Contact>> GetContact(int accountId, int contactId)
        {
            var result = await _accountService.GetAccount(accountId);

            if (!result.Success)
            {
                return Result.Fail<Contact>(result.StatusCode, result.Error);
            }

            var response = await _httpClient.GetAsync($"contacts/{contactId}");

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Result.Fail<Contact>(response.StatusCode, message);
            }

            var contact =  await response.Content.ReadFromJsonAsync<Contact>();

            return Result.Ok(contact);
        }

        public async Task<Result> UpdateContact(int accountID, int contactID, ContactDTO updatedContact)
        {
            var result = await _accountService.GetAccount(accountID);

            if (!result.Success)
            {
                return Result.Fail(result.StatusCode, result.Error);
            }

            BankAccount account = result.Value;

            if (account.ContactIds == null || !account.ContactIds.Any(c => c == contactID))
            {
                return Result.Fail(HttpStatusCode.BadRequest, $"Account does not have a contact - {contactID}");
            }

            var response = await _httpClient.PutAsJsonAsync<ContactDTO>($"contacts/{contactID}", updatedContact);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Result.Fail<Contact>(response.StatusCode, message);
            }

            return Result.Ok();
        }

        public async Task<Result> DeleteContact(int accountID, int contactID)
        {
            var result = await _accountService.GetAccount(accountID);

            if (!result.Success)
            {
                return Result.Fail(result.StatusCode, result.Error);
            }

            BankAccount account = result.Value;

            if (account.ContactIds == null || !account.ContactIds.Any(c => c == contactID))
            {
                return Result.Fail(HttpStatusCode.BadRequest, $"Account does not have a contact - {contactID}");
            }

            var response = await _httpClient.DeleteAsync($"contacts/{contactID}");

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Result.Fail<Contact>(response.StatusCode, message);
            }

            account.ContactIds.Remove(contactID);

            return Result.Ok();
        }

    }
}
