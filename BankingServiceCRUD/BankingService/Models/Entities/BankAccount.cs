using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankingService.Models.Entities
{
    public class BankAccount
    {
        [Key]
        public int ClientID { get; set; }
        public string AccountName { get; set; }
        public string IBAN { get; set; }
        public IList<Statement> Statements { get; set; }
    }
}
