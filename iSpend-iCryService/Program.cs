using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankData;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace iSpend_iCryService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string param_from = "";
            string param_to = "";
            if (args.Length < 2)
            {
                param_from = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                param_to = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else if (args.Length == 2)
            {
                param_from = args[0];
                param_to = args[1];
            }

            Accounts Acc = new Accounts();
            Balances Bal = new Balances();

            Acc = (Accounts)HttpParser.GetAPIResponse("accounts", null, HttpParser.ResponseType.Account);

            foreach (AccountsResult ac in Acc.Results)
            {
                using (ispend_icryEntities db = new ispend_icryEntities())
                {
                    if (db.accounts.Where(x => x.account_id == ac.AccountId).ToList().Count == 0)
                    {
                        db.accounts.Add(new account()
                        {
                            account_id = ac.AccountId,
                            account_number_iban = ac.AccountNumber.Iban,
                            account_number_number = ac.AccountNumber.Number,
                            account_number_sort_code = ac.AccountNumber.SortCode,
                            account_number_swift_bic = ac.AccountNumber.SwiftBic,
                            account_type = ac.AccountType,
                            currency = ac.Currency,
                            display_name = ac.DisplayName,
                            logo_uri = ac.Provider.LogoUri,
                            provider_display_name = ac.Provider.DisplayName,
                            provider_id = ac.Provider.ProviderId,
                            update_timestamp = ac.UpdateTimestamp
                        });
                    }
                    else
                    {
                        account tmp_acc = db.accounts.Where(x => x.account_id == ac.AccountId).FirstOrDefault();
                        tmp_acc.update_timestamp = ac.UpdateTimestamp;
                        db.Entry(tmp_acc).State = System.Data.Entity.EntityState.Modified;
                    }

                    db.SaveChanges();
                }
            }

            using (ispend_icryEntities db = new ispend_icryEntities())
            {
                foreach (account ac in db.accounts)
                {
                    Bal = (Balances)HttpParser.GetAPIResponse("balance",
                       new List<KeyValuePair<string, string>> {
                            new KeyValuePair<string, string>("acc_id", ac.account_id) },
                       HttpParser.ResponseType.Balance);

                    foreach (BalancesResult b in Bal.Results)
                    {
                        if (db.balances.Where(x => x.account_id == ac.account_id).ToList().Count == 0)
                        {
                            db.balances.Add(new balance
                            {
                                account_id = ac.account_id,
                                available = Convert.ToDecimal(b.Available),
                                curency = b.Currency,
                                current = Convert.ToDecimal(b.Current),
                                update_timestamp = b.UpdateTimestamp
                            });
                        }
                        else
                        {
                            balance tmp_bal = db.balances.Where(x => x.account_id == ac.account_id).FirstOrDefault();
                            tmp_bal.available = Convert.ToDecimal(b.Available);
                            tmp_bal.current = Convert.ToDecimal(b.Current);
                            tmp_bal.update_timestamp = b.UpdateTimestamp;
                            db.Entry(tmp_bal).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }

                db.SaveChanges();
            }
            using (ispend_icryEntities db = new ispend_icryEntities())
            {
                foreach (account ac in db.accounts)
                {
                    Transactions Transacts = (Transactions)HttpParser.GetAPIResponse("transactions",
                         new List<KeyValuePair<string, string>> {
                            new KeyValuePair<string, string>("acc_id", ac.account_id),
                            new KeyValuePair<string,string>("from",param_from),
                            new KeyValuePair<string, string>("to", param_to) }, HttpParser.ResponseType.Transaction);

                    foreach (TransactionsResult T in Transacts.Results)
                    {
                        if (db.transactions.Where(x => x.transaction_id == T.TransactionId).ToList().Count == 0)
                        {
                            db.transactions.Add(new transaction
                            {
                                transaction_id = T.TransactionId,
                                account_id = ac.account_id,
                                timestamp = T.Timestamp,
                                description = T.Description,
                                transaction_type = T.TransactionType,
                                transaction_category = T.TransactionCategory,
                                amount = Convert.ToDecimal(T.Amount),
                                currency = T.Currency,
                                meta_provider_transaction_category = (T.Meta != null ? T.Meta.ProviderTransactionCategory : null),
                                meta_transaction_reference = (T.Meta != null ? T.Meta.TransactionReference : null)
                            });
                        }
                    }
                }

                db.SaveChanges();
            }

            Mailer.SendEmail();
        }
    }
}

//TODO: Hide email passwords on git ignore
//TODO : SQL ADO Helper Class
//TODO : Convert to service and execute every 5 minutes ?
//TODO : Run in task scheduler
//TODO : Confirm through email when updated
//TODO : Get Email Notiicatons for transactions, balances,
//TODO : Create Service Settings App for Notiications email addresses intevals
//TODO : Retrieve settings and use for notifications in service
//TODO : Web UI ?
//TODO : Figure out hosting or desktop ?