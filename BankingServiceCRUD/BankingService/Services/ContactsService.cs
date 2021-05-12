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
        private readonly HttpClient _httpClient;

        public ContactsService(
            BankAccountsContext context,
            HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://contacts-service:5000/");
        }

        public async Task<Result<List<Contact>>> GetAllContacts(BankAccount account)
        {
            List<Contact> contacts = new List<Contact>();

            try
            {
                foreach (int contactId in account.ContactIds)
                {
                    Contact contact = await _httpClient.GetFromJsonAsync<Contact>($"contacts/{contactId}");
                    contacts.Add(contact);
                }
            }
            catch
            {
                //do nothing
            }

            return Result.Ok(contacts);
        }

        public async Task<Result<Contact>> CreateContact(Contact contact)
        {
            try
            {
                var contactCreationResponse = await _httpClient.PostAsJsonAsync<Contact>("contacts", contact);
                if (!contactCreationResponse.IsSuccessStatusCode)
                {
                    var message = await contactCreationResponse.Content.ReadAsStringAsync();
                    return Result.Fail<Contact>(contactCreationResponse.StatusCode, message);
                }
            }
            catch (Exception ex)
            {
                return Result.Fail<Contact>(HttpStatusCode.InternalServerError, "Could not create contact. Contacts api cannot be reached");
            }


            return Result.Ok((Contact)contact);
        }

        public async Task<Result<Contact>> GetContact(int contactId)
        {
            var response = await _httpClient.GetAsync($"contacts/{contactId}");

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Result.Fail<Contact>(response.StatusCode, message);
            }

            var contact =  await response.Content.ReadFromJsonAsync<Contact>();

            return Result.Ok(contact);
        }

        public async Task<Result> UpdateContact(Contact updatedContact)
        {
            var response = await _httpClient.PutAsJsonAsync<Contact>($"contacts/{updatedContact.Id}", updatedContact);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Result.Fail<Contact>(response.StatusCode, message);
            }

            return Result.Ok();
        }

        public async Task<Result> DeleteContact(int contactID)
        {
            var response = await _httpClient.DeleteAsync($"contacts/{contactID}");

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Result.Fail<Contact>(response.StatusCode, message);
            }

            return Result.Ok();
        }

    }
}
