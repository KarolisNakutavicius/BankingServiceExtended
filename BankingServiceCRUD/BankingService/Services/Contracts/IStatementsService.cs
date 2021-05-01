using BankingService.Functional;
using BankingService.Models.DTOs;
using BankingService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingService.Services.Contracts
{
    public interface IStatementsService
    {
        Task<Result<Statement>> CreateStatement(int accountID, StatementDTO newStatement);
        Task<Result<IEnumerable<Statement>>> GetStatements(int accountID);
        Task<Result<Statement>> GetStatement(int accountID, int statementID);
        Task<Result> UpdateStatement(int accountID, int statementID, StatementDTO updatedStatement);
        Task<Result> DeleteStatement(int accountID, int statementID);
    }
}
