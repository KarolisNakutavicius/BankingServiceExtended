using BankingService.Enums;
using BankingService.Models.Entities;
using System;


namespace BankingService.ViewModels
{
    public class StatementViewModel
    {
        public int StatementID { get; set; }
        public OperationEnum OperationType { get; set; }
        public string Transactor { get; set; }
        public float Amount { get; set; }
        public string Date { get; set; }

        public StatementViewModel(Statement statement)
        {
            StatementID = statement.StatementID;
            OperationType = statement.OperationType;
            Transactor = statement.Transactor;
            Amount = statement.Amount;
            Date = statement.Date.ToString();
        }
    }
}
