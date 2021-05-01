using BankingService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingService.ViewModels
{
    public class BankAccountViewModel
    {
        public int ClientID { get; set; }
        public string AccountName { get; set; }
        public string IBAN { get; set; }

        public BankAccountViewModel(BankAccount bankAccount)
        {
            ClientID = bankAccount.ClientID;
            AccountName = bankAccount.AccountName;
            IBAN = bankAccount.IBAN;
        }
    }
}
