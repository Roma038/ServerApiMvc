using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using BankServer.Models;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace BankServer.Controllers
{
    public class HomeController : Controller
    {
        private Owner owner_;
        int _confirmCode;
        IEnumerable<Card> Cards;
        private double sumBalance;

        public ActionResult HomePage()
        {

            return View();
        }

        public ActionResult SignIntoAcc()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {

            return View();
        }

        [HttpPost]
        public ActionResult SignintoAcc(string Username, string Password)
        {
            try
            {
                using (var db = new BankContext())
                {
                    var account = db.Accounts.Where(a => a.Password == Password).FirstOrDefault();

                    if (account != null)
                    {
                        if (account.Username != Username && account.Password != Password)
                        {
                            ViewBag.Message = "Cannot find this account.Incorrect username or password!";
                            return RedirectToAction("SignIntoAcc");
                        }

                        else if (account.Username != Username && account.Password == Password)
                        {
                            ViewBag.Message = "Cannot find this account.Incorrect username or password!";
                            return RedirectToAction("SignIntoAcc");
                        }
                        else if (account.Username == Username && account.Password != Password)
                        {
                            ViewBag.Message = "Cannot find this account.Incorrect username or password!";
                            return RedirectToAction("SignIntoAcc");
                        }
                        else if (account.Username == Username && account.Password == Password)
                        {
                            return RedirectToAction("AccountPage", account);
                        }
                        else
                        {
                            ViewBag.Message = "Cannot find this account.Incorrect username or password!";
                            return RedirectToAction("SignIntoAcc");
                        }
                    }
                    else
                    {
                        string s = "Cannot find this account.Incorrect username or password!";
                        ViewBag.Message = s;
                        return RedirectToAction("SignIntoAcc");
                    }

                }
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
                string s = "Cannot find this account.Incorrect username or password!";
                ViewBag.Message = s;
                return RedirectToAction("SignIntoAcc");
            }
            
            
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(Owner owner)
        {
            using (BankContext db = new BankContext())
            {
                
                db.Owners.Add(owner);
                db.SaveChanges();
                
            }
            return RedirectToAction("CreateAccount");
        }

        public ActionResult CreateAccount()
        {
            using (var db = new BankContext())
            {
                ViewBag.OwnerId = db.Owners.ToList().LastOrDefault().OwnerId;

                Currency currency = new Currency();
                Currency currency1 = new Currency();
                currency.Name = "AZN";
                ViewBag.AZN = currency.Name;
                currency1.Name = "USD";
                ViewBag.USD = currency1.Name;

            }
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmForgotPass(string Email, string Username, string NewPassword)
        {

            using (var db = new BankContext())
            {
                var acc = db.Accounts.Where(a => a.Username == Username).FirstOrDefault();
                var owner = db.Owners.Where(o => o.OwnerId == acc.OwnerId).FirstOrDefault();
                int randCode = GenerateCode();
                acc.ConfirmCode = randCode;
                if (acc != null && owner != null)
                {
                    SendMail(Email, randCode);
                    acc.Password = NewPassword;
                    acc.ConfirmPassword = NewPassword;
                    db.SaveChanges();
                }
                

                return View(acc);
            }
            
        }
        public ActionResult UpdateAccountPass(Account account, int Code)
        {

            using (var db = new BankContext())
            {
                
                if (account.ConfirmCode == Code)
                {
                    db.SaveChanges();
                }
            }
            return RedirectToAction("HomePage");

        }

        [HttpPost]
        public ActionResult GetConfirmCode(Account account)
        {
            Owner owner;
            _confirmCode = GenerateCode();
            account.ConfirmCode = _confirmCode;

            using (var db = new BankContext())
            {
                owner = db.Owners.Where(o => o.OwnerId == account.OwnerId).FirstOrDefault();
                if (account.Password == account.ConfirmPassword)
                {
                    SendMail(owner.Email, _confirmCode);
                }
                else
                {
                    ViewBag.Message = "Cannot add account to database";
                    return RedirectToAction("CreateAccount");
                }
            }
            return View(account);
        }

        [HttpPost]
        public ActionResult CreateAccount(Account account, int Code)
        {
            
            using (var db = new BankContext())
            {
                owner_ = db.Owners.Where(o => o.OwnerId == account.OwnerId).FirstOrDefault();
                if (owner_ != null && account.ConfirmCode == Code)
                {
                    account.Owner = owner_;
                    db.Accounts.Add(account);
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.Message = "Cannot add account to database";
                    return RedirectToAction("CreateAccount");
                }
                
            }
            return RedirectToAction("AccountPage",account);
        }

        public ActionResult AccountPage(Account account)
        {
            using (var db = new BankContext())
            {
                var cards = db.Cards.Where(c => c.AccountId == account.AccountId).ToList();
                var owner = db.Owners.Where(o => o.OwnerId == account.OwnerId).FirstOrDefault();
                account.Cards = cards;
                
                foreach (var item in cards)
                {
                    sumBalance += item.Balance;
                }
                account.OverAllBalance = sumBalance;
                if (owner != null)
                {
                    ViewBag.OwnerId = account.OwnerId;
                    ViewBag.Name = owner.Name;
                    ViewBag.Surname = owner.Surname;
                    ViewBag.Email = owner.Email;
                    ViewBag.Username = account.Username;
                    ViewBag.Cards = account.Cards;
                }
            }
            
            return View(account);
        }

        public ActionResult DeleteAccount(int id)
        {
            using (var db = new BankContext())
            {
                var acc = db.Accounts.Where(a => a.AccountId == id).FirstOrDefault();
                var cards = db.Cards.Where(c => c.AccountId == id).ToList();
                db.Accounts.Remove(acc);
                foreach (var item in cards)
                {
                    db.Cards.Remove(item);
                }
                db.SaveChanges();
            }
            return RedirectToAction("HomePage");
        }

        public ActionResult DeleteCard(int id)
        {
            using (var db = new BankContext())
            {
                var card = db.Cards.Where(c => c.CardId == id).FirstOrDefault();
                var acc = db.Accounts.Where(a => a.AccountId == card.AccountId).FirstOrDefault();
                db.Cards.Remove(card);
                db.SaveChanges();
                return RedirectToAction("AccountPage", acc);
            }
            
        }

        public ActionResult TransferMoney(int id)
        {

            using (var db = new BankContext())
            {
                var fromcard = db.Cards.Where(c => c.CardId == id).FirstOrDefault();
                var acc = db.Accounts.Where(ac => ac.AccountId == fromcard.AccountId).FirstOrDefault();

                if (fromcard != null && fromcard.Balance > 0)
                {
                    ViewBag.AZN = "AZN";
                    ViewBag.USD = "USD";
                    return View(fromcard);
                }
                else
                {
                    return RedirectToAction("AccountPage", acc);
                }
            }
            
        }

        [HttpPost]
        public ActionResult TransferMoney(Card card, string money, string Currency, int id)
        {

            Currency _azn = new Currency();
            _azn.Name = "AZN";
            Currency _usd = new Currency();
            _usd.Name = "USD";

            double _money = Convert.ToDouble(money);
            using (var db = new BankContext())
            {
                var toCard = db.Cards.Where(c => c.Number == card.Number).FirstOrDefault();
                var fromCard = db.Cards.Where(c => c.CardId == id).FirstOrDefault();
                var acc = db.Accounts.Where(a => a.AccountId == fromCard.AccountId).FirstOrDefault();

                if (toCard.Number == card.Number && toCard.CV == card.CV && toCard.Date == card.Date)
                {
                    if (Currency.Equals(_azn.Name) && acc.Currency.Equals(_azn.Name))
                    {
                        if (_money <= fromCard.Balance)
                        {
                            fromCard.Balance -= _money;
                            toCard.Balance += _money;
                            
                        }
                    }
                    if (Currency.Equals(_usd.Name) && acc.Currency.Equals(_azn.Name))
                    {
                        _money = _money * 1.7;

                        if (_money <= acc.OverAllBalance)
                        {
                            fromCard.Balance -= _money;
                            toCard.Balance += _money;
                        }
                    }
                    if (Currency.Equals(_azn.Name) && acc.Currency.Equals(_usd.Name))
                    {
                        _money = _money / 1.7;

                        if (_money <= acc.OverAllBalance)
                        {
                            fromCard.Balance -= _money;
                            toCard.Balance += _money;

                        }
                    }
                    if (Currency.Equals(_usd.Name) && acc.Currency.Equals(_usd.Name))
                    {
                        
                        if (_money <= acc.OverAllBalance)
                        {
                            fromCard.Balance -= _money;
                            toCard.Balance += _money;

                        }
                    }
                }
                db.SaveChanges();

                return RedirectToAction("AccountPage", acc);
            }
            
        }

        public ActionResult SignOut(int id)
        {

            return RedirectToAction("SignintoAcc");
        }

        public ActionResult AddCard(int id)
        {
            using (var db = new BankContext())
            {
                var account = db.Accounts.Where(a => a.AccountId == id).FirstOrDefault();
                
                
                return View(account);
            }
        }

        [HttpPost]
        public ActionResult AddCard(Card card)
        {

                var db = new BankContext();
            
                if (card != null && card.Number.Length == 16 && card.CV.Length == 3)
                {
                    
                    
                    var acc = db.Accounts.Where(a => a.AccountId == card.AccountId).FirstOrDefault();
                    if (acc != null)
                    {
                        acc.OverAllBalance += card.Balance;
                        card.Account = acc;
                        db.Cards.Add(card);
                        Cards = db.Cards;
                        acc.Cards = Cards;

                    }
                    db.SaveChanges();

                    return RedirectToAction("AccountPage", acc);
                }
                return RedirectToAction("AccountPage");

        }

        public int GenerateCode()
        {
            Random random = new Random();
            int code = random.Next(1000, 9999);
            return code;
        }

        public void SendMail(string Address, int code)
        {
            var fromMail = new MailAddress("johnueak654@gmail.com");
            const string password = "0507903730";
            var toMail = new MailAddress(Address);
            MailMessage message = new MailMessage();
            message.Subject = "Confirmation";
            message.From = fromMail;
            message.To.Add(toMail);
            message.Body = "Hi there!" +
                           "Please, confirm your email address." +
                           $"Your confirmation code is {code}!";

            using (var client = new SmtpClient())
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("johnueak654@gmail.com", password);
                client.EnableSsl = true;
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 10000;
                client.Send(message);

            }

        }
    }
}
