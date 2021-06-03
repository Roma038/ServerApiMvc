using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BankServer.Models
{
    public class Owner
    {
        public int OwnerId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        [EmailAddress]
        public string Email { get; set; }


        //public Account Account { get; set; }
    }
}