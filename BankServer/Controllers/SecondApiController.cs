using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using Newtonsoft.Json;
using BankServer.Models;
using System.Threading;

namespace BankServer.Controllers
{
    public class SecondApiController : ApiController
    {
        
        // POST: https://localhost:44318/api/SecondApi/addOwner
        // Add owner, when registering in system
        [HttpPost]
        public string AddOwner([FromBody] Owner owner)
        {
            //var _owner = JsonConvert.DeserializeObject<Owner>(owner);
            if (owner.Email != null && owner.Name != null && owner.Surname != null)
            {
                using (var db = new BankContext())
                {
                    db.Owners.Add(owner);
                    db.SaveChanges();
                }
                return JsonConvert.SerializeObject(owner.OwnerId);
            }
            else
            {
                return JsonConvert.SerializeObject("The data does not exist or non correct data format!");
            }  
        }

        // POST: https://localhost:44318/api/SecondApi/AddAccount
        // add account for the last added to system owner
        [HttpPost]
        public string AddAccount([FromBody] Account account)
        {
            using (var db = new BankContext())
            {
                //var owner = db.Owners.OrderByDescending(o => o.OwnerId).FirstOrDefault();
                var owner = db.Owners.Where(o => o.OwnerId == account.OwnerId).FirstOrDefault();
                var acc = db.Accounts.Where(a => a.AccountId == account.AccountId).FirstOrDefault();
                if (account.ConfirmCode == 0)
                {
                    HomeController controller = new HomeController();
                    int code = controller.GenerateCode();
                    controller.SendMail(owner.Email, code);
                    //account.Owner = owner;
                    return JsonConvert.SerializeObject(account);
                }
                if (account.Username != null && account.Password != null && account.ConfirmPassword == account.Password && account.Currency != null && account.ConfirmCode != 0)
                {
                    //Exeption is here, tomorrow should to fix it
                    if (acc == null)
                    {
                        account.Owner = owner;
                        account.OwnerId = owner.OwnerId;
                        db.Accounts.Add(account);
                        db.SaveChanges();
                        return JsonConvert.SerializeObject(account);
                    }
                    else if (account.ConfirmCode != acc.ConfirmCode)
                    {
                        
                        acc.Username = account.Username;
                        acc.Password = account.Password;
                        acc.ConfirmPassword = account.ConfirmPassword;
                        acc.ConfirmCode = account.ConfirmCode;
                        db.SaveChanges();
                        return JsonConvert.SerializeObject(acc);
                    }
                    return JsonConvert.SerializeObject("The data does not exist or non correct data format!");
                }
                else
                {
                    return JsonConvert.SerializeObject("The data does not exist or non correct data format!");
                }
            }
            
        }

        // POST : https://localhost:44318/api/SecondApi/SignIn
        [HttpPost]
        public string SignIn([FromBody] RegisterParams registerParams)
        {
            using (var db = new BankContext())
            {
                
                var account = db.Accounts.Where(a => a.Password == registerParams.Password).FirstOrDefault();
                var owner = db.Owners.Where(o => o.OwnerId == account.OwnerId).FirstOrDefault();
                account.Owner = owner;
                IEnumerable<Card> cards = account.Cards;
                if (account.Username == registerParams.Username)
                {
                    
                    return JsonConvert.SerializeObject(account);
                }
                else
                {
                    return JsonConvert.SerializeObject("The data does not exist or non correct data format!");
                }
            }
        }

        //Post : https://localhost:44318/api/SecondApi/ForgotPass
        [HttpPost]
        public string ForgotPass(ForgotPasswordParams forgotPasswordParams)
        {
            using (var db = new BankContext())
            {
                var acc = db.Accounts.Where(a => a.Username == forgotPasswordParams.Username).FirstOrDefault();
                var owner = db.Owners.Where(o => o.OwnerId == acc.OwnerId).FirstOrDefault();

                if (acc != null && forgotPasswordParams.Password != null && forgotPasswordParams.Password == forgotPasswordParams.ConfirmPassword)
                {
                    acc.Owner = owner;

                    if (forgotPasswordParams.Email == acc.Owner.Email)
                    {
                        HomeController homeController = new HomeController();
                        int otp_code = homeController.GenerateCode();
                        acc.ConfirmCode = otp_code;
                        homeController.SendMail(forgotPasswordParams.Email, otp_code);
                        
                    }
                    return JsonConvert.SerializeObject(acc);

                }
                else
                {
                    return JsonConvert.SerializeObject("The data does not exist or non correct data format!");
                }

            }

            
        }

        //Post : https://localhost:44318/api/SecondApi/AddCard
        public void AddCard([FromBody] Card card)
        {
            using (var db = new BankContext())
            {
                var acc = db.Accounts.Where(a => a.AccountId == card.AccountId).FirstOrDefault();
                acc.Cards = db.Cards;
                acc.Cards.ToList().Add(card);
                db.Cards.Add(card);
                db.SaveChanges();

            }
            
        }
    }
}
