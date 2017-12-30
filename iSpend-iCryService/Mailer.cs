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
        public static void SendEmail(bool? failed = false)
        {
            MailAddress fromAddress = new MailAddress("fikus88@gmail.com", "iSpendiCry Administrator");
            MailAddress toAddress = new MailAddress("fikus88@gmail.com", "to iSpend_iCry User");
            string fromPass = ConfigurationManager.AppSettings["GmailPass"];
            string subject = "Accounts Updated";

            string body_part = System.IO.File.ReadAllText("Html\\body.html");

            if (failed == false)
            {
                using (ispend_icryEntities db = new ispend_icryEntities())
                {
                    string update_Date = db.accounts.First().update_timestamp.ToString();

                    body_part = body_part.Replace("<<**DATE**>>", update_Date);

                    var joined = from b in db.balances
                                 join a in db.accounts on b.account_id equals a.account_id
                                 select new
                                 {
                                     a.display_name,
                                     b.available,
                                     b.current
                                 };

                    string accs_html = "";
                    foreach (var detail in joined)
                    {
                        accs_html += System.IO.File.ReadAllText("Html\\account.html");
                        // body += "\n - " + detail.display_name + "Current: " + detail.current + ", Available : " + "<bold>" + detail.available + "</bold>";
                        accs_html = accs_html.Replace("<<**ACCOUNT_NAME**>>", detail.display_name)
                                             .Replace("<<**CURRENT**>>", detail.current.ToString())
                                             .Replace("<<**AVAILABLE**>>", detail.available.ToString());

                        accs_html = accs_html.Replace("<<**BG_COLOR**>>", (detail.available > 0M ? "#009900" : "#9C010F"));
                    }

                    accs_html += System.IO.File.ReadAllText("Html\\total.html");
                    accs_html = accs_html.Replace("<<**TOTAL_AVAILABLE**>>", joined.Sum(x => x.available).ToString());

                    body_part = body_part.Replace("<<**ACCOUNTS_HTML**>>", accs_html);
                }
            }
            else
            {
                subject = "Update Failed";
                body_part = "UPDATE FAILED";
            };

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPass),
                Timeout = 20000
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body_part,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}