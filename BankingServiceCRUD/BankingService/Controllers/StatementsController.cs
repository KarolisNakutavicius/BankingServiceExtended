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
    [Route("BankAccounts/{accountID}/[controller]")]
    [ApiController]
    public class StatementsController : ControllerBase
    {
        private readonly IStatementsService _statementsService;

        public StatementsController(IStatementsService statementsService)
        {
            _statementsService = statementsService;
        }

        [HttpPost]
        public async Task<ActionResult<StatementViewModel>> CreateStatement(int accountID, StatementDTO newStatement)
        {
            var result = await _statementsService.CreateStatement(accountID, newStatement);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return CreatedAtAction("GetStatement", new { accountID = accountID,  statementID = result.Value.StatementID }, new StatementViewModel(result.Value));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatementViewModel>>> GetStatements(int accountID)
        {
            var result = await _statementsService.GetStatements(accountID);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            var statementsToReturn = result.Value.Select(s => new StatementViewModel(s));

            return Ok(statementsToReturn);
        }

        [HttpGet("{statementID}")]
        public async Task<ActionResult<StatementViewModel>> GetStatement(int accountID, int statementID)
        {
            var result = await _statementsService.GetStatement(accountID, statementID);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return Ok(new StatementViewModel(result.Value));
        }

        [HttpPut("{statementID}")]
        public async Task<IActionResult> PutStatement(int accountID, int statementID, StatementDTO updatedStatement)
        {
            var result = await _statementsService.UpdateStatement(accountID, statementID, updatedStatement);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return NoContent();           
        }

        [HttpDelete("{statementID}")]
        public async Task<IActionResult> DeleteStatement(int accountID, int statementID)
        {
            var result = await _statementsService.DeleteStatement(accountID, statementID);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return NoContent();
        }
    }
}
