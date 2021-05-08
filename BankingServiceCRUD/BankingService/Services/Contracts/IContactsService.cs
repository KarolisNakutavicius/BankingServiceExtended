using BankingService.Functional;
using BankingService.Models.DTOs;
using BankingService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingService.Services.Contracts
{
    public interface IContactsService
    {
        Task<Result<List<Contact>>> GetAllContacts(BankAccount bankAccount);
        Task<Result<Contact>> GetContact(int contactId);
        Task<Result<Contact>> CreateContact(Contact contact);
        Task<Result> UpdateContact(Contact updatedContact);
        Task<Result> DeleteContact(int contactID);
    }
}
