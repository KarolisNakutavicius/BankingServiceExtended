using BankingService.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingService.Models.DTOs
{
    public class StatementDTO
    {
        public OperationEnum OperationType { get; set; }
        public float Amount { get; set; }
        public string Transactor { get; set; }
        public DateTime Date { get; set; }
    }
}
