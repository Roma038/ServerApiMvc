using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankServer.Models
{
    public class ForgotPasswordParams
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}