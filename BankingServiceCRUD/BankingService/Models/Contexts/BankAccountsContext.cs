using BankingService.Enums;
using BankingService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingService.Models.Contexts
{
    public class BankAccountsContext : DbContext
    {
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Statement> Statements { get; set; }

        public BankAccountsContext(DbContextOptions<BankAccountsContext> options) : base(options)
        { Database.EnsureCreated(); }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>().Property(ba => ba.ClientID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Statement>().Property(ba => ba.StatementID).ValueGeneratedOnAdd();

            modelBuilder.Entity<Statement>()
                .HasOne(s => s.BankAccount)
                .WithMany(b => b.Statements)
                .HasForeignKey(s => s.BankAccountID);

            modelBuilder.Entity<BankAccount>().HasData(
                new BankAccount { AccountName = "UAB Vilniaus Transportas", IBAN = "LT47 2121 1009 0000 0002 3569 87411", ClientID = 1 },
                new BankAccount { AccountName = "MB Viešųjų ryšių ekspertai", IBAN = "LT47 4445 1263 4542 4512 411 2255", ClientID = 2 },
                new BankAccount { AccountName = "Algimantas Malinauskas", IBAN = "LT47 4564 111 45687 45641 455 236", ClientID = 3 });

            modelBuilder.Entity<Statement>().HasData(
                new Statement { StatementID = 1,
                    BankAccountID = 1,
                    Date = DateTime.Now,
                    Amount = 240,
                    OperationType = OperationEnum.Expense,
                    Transactor = "Jonas Makaronas" },
                new Statement
                {
                    StatementID = 2,
                    BankAccountID = 1,
                    Date = DateTime.Now,
                    Amount = 310,
                    OperationType = OperationEnum.Income,
                    Transactor = "Antanas Markaitis"
                },
                new Statement
                {
                    StatementID = 3,
                    BankAccountID = 2,
                    Date = DateTime.Now,
                    Amount = 640,
                    OperationType = OperationEnum.Expense,
                    Transactor = "Kęstutis Vaišvila"
                },
                new Statement
                {
                    StatementID = 4,
                    BankAccountID = 2,
                    Date = DateTime.Now,
                    Amount = 20000,
                    OperationType = OperationEnum.Income,
                    Transactor = "Karolis Karalius"
                });
        }
    }
}
