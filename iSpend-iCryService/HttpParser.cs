using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace iSpend_iCryService
{
    internal class HttpParser

    {
        public enum ResponseType
        {
            Account,
            Balance,
            Transaction
        }

        public static bool ResponseFailed = false;

        public static object GetAPIResponse(string route, List<KeyValuePair<string, string>> paramList, ResponseType type)

        {
            string reqString = "http://localhost:5000/" + route;

            if (paramList != null)
            {
                foreach (KeyValuePair<string, string> kvp in paramList)
                {
                    reqString += "/" + kvp.Value;
                }
            }
            WebRequest req = WebRequest.Create(reqString);
            WebResponse res = null;
            try
            {
                res = req.GetResponse();

            } catch (WebException e)
            {
                ResponseFailed = true;

                return null;
            }
            

            Stream s = res.GetResponseStream();
            StreamReader sr = new StreamReader(s);
            string respString = sr.ReadToEnd();
            sr.Close();
            s.Close();
            object result = null;

            switch (type)
            {
                case ResponseType.Account:
                    result = BankData.Accounts.FromJson(respString);
                    break;

                case ResponseType.Balance:
                    result = BankData.Balances.FromJson(respString);
                    break;

                case ResponseType.Transaction:
                    result = BankData.Transactions.FromJson(respString);
                    break;
            }

            return result;
        }
    }
}