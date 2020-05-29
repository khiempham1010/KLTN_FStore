using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace AtomStore.PayPal
{
    public class AtomPP
    {
        public double GrossTotal { get; set; }
        public int InvoiceNumber { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentFirstName { get; set; }
        public double PaymentFee { get; set; }
        public string BusinessEmail { get; set; }
        public string PayerEmail { get; set; }
        public string TxToken { get; set; }
        public string PayerLastName { get; set; }
        public string ReceiverEmail { get; set; }
        public string ItemName { get; set; }
        public string Currency { get; set; }
        public string TransactionId { get; set; }
        public string SubcriberId { get; set; }
        public string Custom { get; set; }

        private static string authToken, txToken, query, strResponse;

        public static AtomPP Success(string tx)
        {
            PayPalConfig payPalConfig = PayPalService.getPayPalConfig();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            authToken = payPalConfig.AuthToken;
            txToken = tx;
            query = string.Format("cmd=_notify-synch&tx={0}&at={1}", txToken, authToken);
            string url = payPalConfig.PostUrl;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = query.Length;
            StreamWriter stOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            stOut.Write(query);
            stOut.Close();
            StreamReader stIn = new StreamReader(req.GetResponse().GetResponseStream());
            strResponse = stIn.ReadToEnd();
            stIn.Close();
            if (strResponse.StartsWith("SUCCESS"))
                return AtomPP.Parse(strResponse);
            return null;
        }

        public static AtomPP Parse(string postData)
        {
            String sKey, sValue;
            AtomPP atomPP = new AtomPP();
            try
            {
                String[] StringArray = postData.Split('\n');
                int i;
                for (i = 1; i < StringArray.Length - 1; i++)
                {
                    String[] StringArray1 = StringArray[i].Split('=');
                    sKey = StringArray1[0];
                    sValue = HttpUtility.UrlDecode(StringArray1[1]);
                    switch (sKey)
                    {
                        case "mc_gross":
                            atomPP.GrossTotal = Convert.ToDouble(sValue);
                            break;
                        case "invoice":
                            atomPP.InvoiceNumber = Convert.ToInt32(sValue);
                            break;
                        case "payment_status":
                            atomPP.PaymentStatus = Convert.ToString(sValue);
                            break;
                        case "first_name":
                            atomPP.PaymentFirstName = Convert.ToString(sValue);
                            break;
                        case "mc_fee":
                            atomPP.PaymentFee = Convert.ToDouble(sValue);
                            break;
                        case "business":
                            atomPP.BusinessEmail = Convert.ToString(sValue);
                            break;
                        case "payer_email":
                            atomPP.PayerEmail = Convert.ToString(sValue);
                            break;
                        case "Tx Token":
                            atomPP.TxToken = Convert.ToString(sValue);
                            break;
                        case "last_name":
                            atomPP.PayerLastName = Convert.ToString(sValue);
                            break;
                        case "receiver_email":
                            atomPP.ReceiverEmail = Convert.ToString(sValue);
                            break;
                        case "item_name":
                            atomPP.ItemName = Convert.ToString(sValue);
                            break;
                        case "mc_currency":
                            atomPP.Currency = Convert.ToString(sValue);
                            break;
                        case "txn_id":
                            atomPP.TransactionId = Convert.ToString(sValue);
                            break;
                        case "custom":
                            atomPP.Custom = Convert.ToString(sValue);
                            break;
                        case "subscr_id":
                            atomPP.SubcriberId = Convert.ToString(sValue);
                            break;
                    }
                }
                return atomPP;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

