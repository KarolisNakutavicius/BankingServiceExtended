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
        Task<Result<IList<Contact>>> GetAllContacts(int accountId);
        Task<Result<Contact>> GetContact(int accountId, int contactId);
        Task<Result<Contact>> CreateContact(int accountId, Contact contactDTO);
        Task<Result> UpdateContact(int accountID, int contactID, ContactDTO updatedContact);
        Task<Result> DeleteContact(int accountID, int contactID);
    }
}
