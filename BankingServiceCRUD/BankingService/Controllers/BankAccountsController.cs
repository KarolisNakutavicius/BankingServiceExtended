using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingService.Models.Contexts;
using BankingService.Models.Entities;
using BankingService.Services.Contracts;
using BankingService.Models.DTOs;
using BankingService.ViewModels;

namespace BankingService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountsController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        [HttpPost]
        public async Task<ActionResult<BankAccountViewModel>> CreateBankAccount(BankAccountDTO bankAccount)
        {
            var result = await _bankAccountService.CreateAccount(bankAccount);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return CreatedAtAction("GetBankAccount", new { id = result.Value.ClientID }, new BankAccountViewModel(result.Value));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccountViewModel>>> GetBankAccounts()
        {
            return (await _bankAccountService.GetAccounts()).Value.Select(b => new BankAccountViewModel(b)).ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccountViewModel>> GetBankAccount(int id)
        {
            var result = await _bankAccountService.GetAccount(id);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return Ok(new BankAccountViewModel(result.Value));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankAccount(int id, BankAccountDTO updatedAccount)
        {
            var result = await _bankAccountService.UpdateAccount(id, updatedAccount);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankAccount(int id)
        {
            var result = await _bankAccountService.DeleteAccount(id);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return NoContent();
        }

    }
}
