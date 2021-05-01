using BankingService.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace BankingService.Models.Entities
{
    public class Statement
    {
        [Key]
        public int StatementID { get; set; }
        public OperationEnum OperationType { get; set; }
        public string Transactor { get; set; }
        public float Amount { get; set; }
        public DateTime Date { get; set; }

        public int BankAccountID { get; set; }
        public BankAccount BankAccount { get; set; }
    }
}
