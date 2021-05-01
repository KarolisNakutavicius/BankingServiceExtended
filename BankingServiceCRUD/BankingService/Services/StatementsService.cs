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
using System.Threading.Tasks;

namespace BankingService.Services
{
    public class StatementsService : IStatementsService
    {
        private readonly BankAccountsContext _context;
        private readonly IBankAccountService _accountService;

        public StatementsService(BankAccountsContext context, IBankAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public async Task<Result<Statement>> CreateStatement(int accountID, StatementDTO statement)
        {
            var result = await _accountService.GetAccount(accountID);

            if (!result.Success)
            {
                return Result.Fail<Statement>(result.StatusCode, result.Error);
            }

            var newStatement = new Statement
            {
                Amount = statement.Amount,
                OperationType = statement.OperationType,
                Transactor = statement.Transactor,
                Date = statement.Date,
                BankAccount = result.Value
            };

            try
            {
                _context.Statements.Add(newStatement);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return Result.Fail<Statement>(HttpStatusCode.InternalServerError, $"Unexpected server error while saving a new statement - {ex.Message}");
            }

            return Result.Ok(newStatement);
        }

        public async Task<Result<IEnumerable<Statement>>> GetStatements(int accountID)
        {
            var statements = (await _context.BankAccounts.Include(b => b.Statements).FirstOrDefaultAsync(b => b.ClientID == accountID))?.Statements;

            if (statements == null)
            {
                return Result.Fail<IEnumerable<Statement>>(HttpStatusCode.NotFound, "Account was not found");
            }

            return Result.Ok<IEnumerable<Statement>>(statements);
        }

        public async Task<Result<Statement>> GetStatement(int accountID, int statementID)
        {
            var statements = (await _context.BankAccounts.Include(b => b.Statements).FirstOrDefaultAsync(b => b.ClientID == accountID))?.Statements;

            if (statements == null)
            {
                return Result.Fail<Statement>(HttpStatusCode.NotFound, "Account was not found");
            }

            var statement = statements.FirstOrDefault(s => s.StatementID == statementID);

            if (statement == null)
            {
                return Result.Fail<Statement>(HttpStatusCode.NotFound, "Statement with specified ID was not found");
            }

            return Result.Ok(statement);
        }

        public async Task<Result> UpdateStatement(int accountID, int statementID, StatementDTO updatedStatement)
        {
            var result = await _accountService.GetAccount(accountID);

            if (!result.Success)
            {
                return Result.Fail<Statement>(result.StatusCode, result.Error);
            }

            var statement = new Statement
            {
                StatementID = statementID,
                Amount = updatedStatement.Amount,
                OperationType = updatedStatement.OperationType,
                Transactor = updatedStatement.Transactor,
                Date = updatedStatement.Date,
                BankAccount = result.Value
            };

            try
            {
                _context.Entry(statement).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return Result.Fail(HttpStatusCode.InternalServerError, $"Unexpected server error while updating a statement - {ex.Message}");
            }

            return Result.Ok();
        }

        public async Task<Result> DeleteStatement(int accountID, int statementID)
        {
            var result = await GetStatement(accountID, statementID);

            if (!result.Success)
            {
                return Result.Fail<Statement>(result.StatusCode, result.Error);
            }

            try
            {
                _context.Statements.Remove(result.Value);
                await _context.SaveChangesAsync();
            }
            catch
            {
                return Result.Fail(HttpStatusCode.InternalServerError, "Unexpected server error while deleting a statement");
            }

            return Result.Ok();
        }

    }
}
