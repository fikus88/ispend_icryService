using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using BankData;
using System.Configuration;

namespace iSpend_iCryService
{
    internal class Mailer
    {
        public static void SendEmail()
        {
            MailAddress fromAddress = new MailAddress("fikus88@gmail.com", "iSpendiCry Administrator");
            MailAddress toAddress = new MailAddress("fikus88@gmail.com", "to iSpend_iCry User");
            string fromPass = ConfigurationManager.AppSettings["GmailPass"];
            const string subject = "Accounts Updated";

            string body = "Your accounts have been updated";

            using (ispend_icryEntities db = new ispend_icryEntities())
            {
                string update_Date = db.accounts.First().update_timestamp.ToString();
                body += " on " + update_Date;
                body += "\n Current Balances are : ";

                var joined = from b in db.balances
                             join a in db.accounts on b.account_id equals a.account_id
                             select new
                             {
                                 a.display_name,
                                 b.available,
                                 b.current
                             };

                foreach (var detail in joined)
                {
                    body += "\n - " + detail.display_name + "Current: " + detail.current + ", Available : " + "<bold>" + detail.available + "</bold>";
                }
            }

            body += "\n\n Thank you for using iSpend_iCry";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPass),
                Timeout = 20000
            };

            body = System.IO.File.ReadAllText("email.html");
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}