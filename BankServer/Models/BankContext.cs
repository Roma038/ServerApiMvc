using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankServer.Models
{
    public class BankContext : DbContext
    {
        public BankContext() : base("Bank_Database")
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Card> Cards { get; set; }
    }
}