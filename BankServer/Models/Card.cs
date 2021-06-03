using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankServer.Models
{
    public class Card
    {
        public int CardId { get; set; }

        public string Owner { get; set; }

        public string Number { get; set; }

        public string CV { get; set; }
        
        public string Date { get; set; }

        public double Balance { get; set; }

        public int AccountId { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }


    }
}