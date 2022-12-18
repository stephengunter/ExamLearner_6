using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ECPay.Payment.Integration
{
    public class AllInOne : AllInOneMetadata, IDisposable
    {
        public AllInOne()
           : base()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        }

        public IEnumerable<string> CheckOutFeedback(HttpRequest request, ref Hashtable feedback)
        {
            string szParameters = String.Empty;
            string szCheckMacValue = String.Empty;

            List<string> errList = new List<string>();
            if (null == feedback) feedback = new Hashtable();

            var keys = request.Form.Keys.ToArray();
            Array.Sort(keys);

            foreach (string szKey in keys)
            {
                string szValue = request.Form[szKey];

                if (szKey != "CheckMacValue")
                {
                    szParameters += String.Format("&{0}={1}", szKey, szValue);

                    if (szKey == "PaymentType")
                    {
                        szValue = szValue.Replace("_CVS", String.Empty);
                        szValue = szValue.Replace("_BARCODE", String.Empty);
                        szValue = szValue.Replace("_CreditCard", String.Empty);
                    }

                    if (szKey == "PeriodType")
                    {
                        szValue = szValue.Replace("Y", "Year");
                        szValue = szValue.Replace("M", "Month");
                        szValue = szValue.Replace("D", "Day");
                    }

                    feedback.Add(szKey, szValue);
                }
                else
                {
                    szCheckMacValue = szValue;
                }
            }

            // 比對驗證檢查碼。
            errList.AddRange(this.CompareCheckMacValue(szParameters, szCheckMacValue));

            return errList;
        }

        


        private string BuildCheckMacValue(string parameters, int encryptType = 0)
        {
            string szCheckMacValue = String.Empty;
            // 產生檢查碼。
            szCheckMacValue = String.Format("HashKey={0}{1}&HashIV={2}", this.HashKey, parameters, this.HashIV);
            szCheckMacValue = HttpUtility.UrlEncode(szCheckMacValue).ToLower();
            if (encryptType == 1)
            {
                szCheckMacValue = SHA256Encoder.Encrypt(szCheckMacValue);
            }
            else
            {
                szCheckMacValue = MD5Encoder.Encrypt(szCheckMacValue);
            }

            return szCheckMacValue;
        }

        private IEnumerable<string> CompareCheckMacValue(string parameters, string checkMacValue)
        {
            List<string> errList = new List<string>();

            if (!String.IsNullOrEmpty(checkMacValue))
            {
                // 產生檢查碼。
                string szConfirmMacValueMD5 = this.BuildCheckMacValue(parameters, 0);

                // 產生檢查碼。
                string szConfirmMacValueSHA256 = this.BuildCheckMacValue(parameters, 1);

                // 比對檢查碼。
                if (checkMacValue != szConfirmMacValueMD5 && checkMacValue != szConfirmMacValueSHA256)
                {
                    errList.Add("CheckMacValue verify fail.");
                }

            }
            // 查無檢查碼時，拋出例外。
            else
            {
                if (String.IsNullOrEmpty(checkMacValue)) errList.Add("No CheckMacValue parameter.");
            }

            return errList;
        }


        public void Dispose()
        {
            GC.Collect();
        }
    }
}
