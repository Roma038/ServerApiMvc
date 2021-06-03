using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BankServer.Models
{
    public class Account
    {
        public int AccountId { get; set; }

        public int OwnerId { get; set; }

        public double OverAllBalance { get; set; }

        [ForeignKey("OwnerId")]
        public Owner Owner { get; set; }
        
        public string Currency { get; set; }

        public int ConfirmCode { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public IEnumerable<Card> Cards { get; set; }
    }
}