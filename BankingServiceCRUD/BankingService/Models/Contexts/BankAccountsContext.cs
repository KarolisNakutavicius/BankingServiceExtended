using BankingService.Enums;
using BankingService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankingService.Models.Contexts
{
    public class BankAccountsContext : DbContext
    {
        public DbSet<BankAccount> BankAccounts { get; set; }

        public BankAccountsContext(DbContextOptions<BankAccountsContext> options) : base(options)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>().Property(ba => ba.ClientID).ValueGeneratedOnAdd();

            modelBuilder.Entity<BankAccount>().Property(ba => ba.ContactIds)
                .HasConversion(
                     v => JsonConvert.SerializeObject(v),
                     v => JsonConvert.DeserializeObject<List<int>>(v));

            modelBuilder.Entity<Statement>()
                .HasOne(s => s.BankAccount)
                .WithMany(b => b.Statements)
                .HasForeignKey(s => s.BankAccountID);

            modelBuilder.Entity<BankAccount>().HasData(
                new BankAccount 
                { 
                    AccountName = "UAB Vilniaus Transportas",
                    IBAN = "LT47 2121 1009 0000 0002 3569 87411",
                    ClientID = 1, ContactIds = new List<int>() { 12345, 11234 }
                },
                new BankAccount 
                { 
                    AccountName = "MB Viešųjų ryšių ekspertai",
                    IBAN = "LT47 4445 1263 4542 4512 411 2255",
                    ClientID = 2,
                    ContactIds = new List<int>() { 74638 }
                },
                new BankAccount 
                {
                    AccountName = "Algimantas Malinauskas",
                    IBAN = "LT47 4564 111 45687 45641 455 236",
                    ClientID = 3,
                    ContactIds = new List<int>()
                });
        }
    }
}
