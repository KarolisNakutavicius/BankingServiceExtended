using BankingService.Functional;
using BankingService.Models.DTOs;
using BankingService.Models.Entities;
using BankingService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingService.Services.Contracts
{
    public interface IBankAccountService
    {
        Task<Result<BankAccountViewModel>> CreateAccount(BankAccountDTO newAccount);
        Task<IEnumerable<BankAccountViewModel>> GetAccounts();
        Task<Result<BankAccountViewModel>> GetAccount(int id);
        Task<Result> UpdateAccount(int id, BankAccountDTO newAccount);
        Task<Result> DeleteAccount(int id);

    }
}
