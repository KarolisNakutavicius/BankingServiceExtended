using BankingService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingService.Models.DTOs
{
    public class BankAccountDTO
    {
        public string AccountName { get; set; }

        public string IBAN { get; set; }
    }
}
