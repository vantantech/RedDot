using Newtonsoft.Json;
using NLog;
using RedDot.Models.CardConnect;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RedDot.Models
{
    public class CardConnectModel
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public static decimal GetTip(decimal amount)
        {
            Connect();


            if (GlobalSettings.Instance.SessionKey == null)
            {
                TouchMessageBox.Show("Not able to retrieve session key!!");
                return -1;
            }

            string host = GlobalSettings.Instance.BoltBaseURL;
            string urlstring = "{0}/v3/tip"; // also need to get from database
            string url = string.Format(urlstring, host);


            string tippercent = GlobalSettings.Instance.TipSuggestion;


            var tiprequestbody = new CCTipRequest
            {
                merchantId = GlobalSettings.Instance.MerchantID,
                hsn = GlobalSettings.Instance.HardwareSerialNumber,
                amount = Decimal.ToInt32(Math.Round(amount, 2) * 100).ToString(),
                tipPercentPresets = Array.ConvertAll(tippercent.Split(','),int.Parse),
                prompt = GlobalSettings.Instance.TipPrompt

            };

            logger.Debug("url=" + url);
            var client = new RestClient(url);
            client.Timeout = 30 * 1000; //30 seconds
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", GlobalSettings.Instance.CardConnectAuthorization);
            request.AddHeader("X-CardConnect-SessionKey", GlobalSettings.Instance.SessionKey);

            var body = JsonConvert.SerializeObject(tiprequestbody);
            logger.Debug("body=" + body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            string cont = response.Content;

            logger.Debug("response body:" + cont);

            TipResponse resp = JsonConvert.DeserializeObject<TipResponse>(cont);

            int tipamount = int.Parse(resp.tip);
            logger.Debug("tip=" + tipamount);
            return (decimal)(tipamount / 100.0);
        }
        public static CCSaleResponse authCard(int salesid, decimal amount, bool capture = true)
        {


            Connect();

            if (GlobalSettings.Instance.SessionKey == null)
            {
                TouchMessageBox.Show("Not able to retrieve session key!!");
                return null;
            }


            string host = GlobalSettings.Instance.BoltBaseURL;
            string urlstring = "{0}/v3/authCard"; // also need to get from database
            string url = string.Format(urlstring, host);

            var salesrequestbody = new CCSaleRequest
            {
                merchantId = GlobalSettings.Instance.MerchantID,
                hsn = GlobalSettings.Instance.HardwareSerialNumber,
                amount = Decimal.ToInt32(Math.Round(amount, 2) * 100).ToString(),
                orderId = salesid.ToString(),
                capture = capture ? "true" : "false",
                includeSignature = capture ? "true" : "false"
            };



            logger.Debug("url=" + url);

            var client = new RestClient(url);
            client.Timeout = 60 * 1000; //60 seconds
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", GlobalSettings.Instance.CardConnectAuthorization);
            request.AddHeader("X-CardConnect-SessionKey", GlobalSettings.Instance.SessionKey);

            var body = JsonConvert.SerializeObject(salesrequestbody);
            logger.Debug("body=" + body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            string cont = response.Content;
            logger.Debug("response body:" + cont);


            CCSaleResponse resp = JsonConvert.DeserializeObject<CCSaleResponse>(cont);
            if (resp == null)
            {
                TouchMessageBox.Show(response.ErrorMessage);
                logger.Error(response.ErrorMessage);
                return null;
            }

            if (resp.emvTagData != null)
                resp.EMV_Data = JsonConvert.DeserializeObject<EMV>(resp.emvTagData);

            logger.Info("Response:" + resp.resptext);

            return resp;

        }


        public static bool ReadConfirmation(string prompt, int timeout=60)
        {
            Connect();


            if (GlobalSettings.Instance.SessionKey == null)
            {
                TouchMessageBox.Show("Not able to retrieve session key!!");
                return false;
            }

            string host = GlobalSettings.Instance.BoltBaseURL;
            string urlstring = "{0}/v2/readConfirmation"; // also need to get from database
            string url = string.Format(urlstring, host);



            var tiprequestbody = new CCConfirmationRequest
            {
                merchantId = GlobalSettings.Instance.MerchantID,
                hsn = GlobalSettings.Instance.HardwareSerialNumber,
                beep = false,
                prompt = prompt

            };

            logger.Debug("url=" + url);
            var client = new RestClient(url);
            client.Timeout = timeout * 1000; 
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", GlobalSettings.Instance.CardConnectAuthorization);
            request.AddHeader("X-CardConnect-SessionKey", GlobalSettings.Instance.SessionKey);

            var body = JsonConvert.SerializeObject(tiprequestbody);
            logger.Debug("body=" + body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            string cont = response.Content;

            if (cont == "" )
            {
                if(timeout >= 30)  TouchMessageBox.Show("No Response from Customer -- Time Out");
                return false;
            }

            logger.Debug("response body:" + cont);

            CCConfirmationResponse resp = JsonConvert.DeserializeObject<CCConfirmationResponse>(cont);

            return resp.confirmed;
        }

        public static void DisplayWelcome()
        {
            Display("Welcome to " + GlobalSettings.Instance.Shop.Name + "\n\n\n Station No: " + GlobalSettings.Instance.StationNo);
        }
        public static void Display(string text)
        {
            Connect();


            if (GlobalSettings.Instance.SessionKey == null)
            {
                TouchMessageBox.Show("Not able to retrieve session key!!");

            }

            string host = GlobalSettings.Instance.BoltBaseURL;
            string urlstring = "{0}/v2/display"; // also need to get from database
            string url = string.Format(urlstring, host);



            var tiprequestbody = new
            {
                merchantId = GlobalSettings.Instance.MerchantID,
                hsn = GlobalSettings.Instance.HardwareSerialNumber,
                text = text

            };

            logger.Debug("url=" + url);
            var client = new RestClient(url);
            client.Timeout = 30 * 1000; //30 seconds
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", GlobalSettings.Instance.CardConnectAuthorization);
            request.AddHeader("X-CardConnect-SessionKey", GlobalSettings.Instance.SessionKey);

            var body = JsonConvert.SerializeObject(tiprequestbody);
            logger.Debug("body=" + body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            string cont = response.Content;



        }
        public static void ClearPinPad()
        {
            Connect();


            if (GlobalSettings.Instance.SessionKey == null)
            {
                TouchMessageBox.Show("Not able to retrieve session key!!");
            
            }

            string host = GlobalSettings.Instance.BoltBaseURL;
            string urlstring = "{0}/v3/clearDisplay"; // also need to get from database
            string url = string.Format(urlstring, host);



            var tiprequestbody = new
            {
                merchantId = GlobalSettings.Instance.MerchantID,
                hsn = GlobalSettings.Instance.HardwareSerialNumber
             
            };

            logger.Debug("url=" + url);
            var client = new RestClient(url);
            client.Timeout = 30 * 1000; //30 seconds
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", GlobalSettings.Instance.CardConnectAuthorization);
            request.AddHeader("X-CardConnect-SessionKey", GlobalSettings.Instance.SessionKey);

            var body = JsonConvert.SerializeObject(tiprequestbody);
            logger.Debug("body=" + body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            string cont = response.Content;

    
          
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }


        public static CCCaptureResponse Capture(string retref, string authcode, decimal amount)
        {


            string host = GlobalSettings.Instance.CardConnectURL;
            string urlstring = "{0}/capture";
            string url = string.Format(urlstring, host);
            string usernamepassword = Base64Encode(GlobalSettings.Instance.CardConnectUsername + ":" + GlobalSettings.Instance.CardConnectPassword);

            var requestbody = new CCCaptureRequest
            {
                merchid = GlobalSettings.Instance.MerchantID,
                authcode = authcode,
                retref = retref,
                amount = amount.ToString(),
            };

            logger.Debug("url=" + url);
            var client = new RestClient(url);
            client.Timeout = 32 * 1000; //30 seconds
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic " + usernamepassword);


            var body = JsonConvert.SerializeObject(requestbody);
            logger.Debug("body=" + body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            string cont = response.Content;

            logger.Debug("response body:" + cont);

            CCCaptureResponse resp = JsonConvert.DeserializeObject<CCCaptureResponse>(cont);


            logger.Debug("capture amount=" + resp.amount);
            return resp;
        }

        public static CCVoidResponse Void(string retref, string authcode, decimal amount)
        {


            string host = GlobalSettings.Instance.CardConnectURL;
            string urlstring = "{0}/void";
            string url = string.Format(urlstring, host);
            string usernamepassword = Base64Encode(GlobalSettings.Instance.CardConnectUsername + ":" + GlobalSettings.Instance.CardConnectPassword);

            var requestbody = new CCVoidRequest
            {
                merchid = GlobalSettings.Instance.MerchantID,
                retref = retref,
                amount = amount.ToString(),
                receipt = "Y"
            };

            logger.Debug("url=" + url);
            var client = new RestClient(url);
            client.Timeout = 32 * 1000; //30 seconds
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic " + usernamepassword);


            var body = JsonConvert.SerializeObject(requestbody);
            logger.Debug("body=" + body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            string cont = response.Content;

            logger.Debug("response body:" + cont);

            CCVoidResponse resp = JsonConvert.DeserializeObject<CCVoidResponse>(cont);


            logger.Debug("void amount=" + resp.amount);
            return resp;
        }

        public static CCRefundResponse Refund(string retref)
        {


            string host = GlobalSettings.Instance.CardConnectURL;
            string urlstring = "{0}/refund";
            string url = string.Format(urlstring, host);
            string usernamepassword = Base64Encode(GlobalSettings.Instance.CardConnectUsername + ":" + GlobalSettings.Instance.CardConnectPassword);

            var requestbody = new CCRefundRequest
            {
                merchid = GlobalSettings.Instance.MerchantID,
                retref = retref

            };

            logger.Debug("url=" + url);
            var client = new RestClient(url);
            client.Timeout = 32 * 1000; //30 seconds
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic " + usernamepassword);


            var body = JsonConvert.SerializeObject(requestbody);
            logger.Debug("body=" + body);

            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            string cont = response.Content;

            logger.Debug("response body:" + cont);

            CCRefundResponse resp = JsonConvert.DeserializeObject<CCRefundResponse>(cont);
            resp.receiptdata = JsonConvert.DeserializeObject<CCReceiptData>(resp.receipt);

            logger.Debug("Refund Response=" + resp.resptext);
            return resp;
        }



        public static bool Connect()
        {

            string host = GlobalSettings.Instance.BoltBaseURL;
            string urlstring = "{0}/v2/connect"; // also need to get from database
            string url = string.Format(urlstring, host);


            GlobalSettings.Instance.SessionKey = null;

            logger.Debug("url=" + url);

            var requestbody = new
            {
                merchantId = GlobalSettings.Instance.MerchantID,
                hsn = GlobalSettings.Instance.HardwareSerialNumber,
                force = "true"
            };


            var client = new RestClient(url);
            client.Timeout = 30 * 1000; //30 seconds
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", GlobalSettings.Instance.CardConnectAuthorization);
            var body = JsonConvert.SerializeObject(requestbody);
            logger.Debug("body=" + body);


            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response == null) return false;

            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                TouchMessageBox.Show("Not able to connect to PIN PAD, please check Bolt Status");
                return false;
            }

            var responsestr = response.Headers.ToList().Find(x => x.Name == "X-CardConnect-SessionKey");

            logger.Debug("response header:" + responsestr);

            if (responsestr == null)
            {
                TouchMessageBox.Show(response.Content);
                //log error
                logger.Error(response.Content);
                return false;
            }

            var arrayValues = responsestr.Value.ToString().Split(';');
            GlobalSettings.Instance.SessionKey = arrayValues[0];
            string datestr = arrayValues[1].Replace("expires=", "").Replace("T", " ").Replace("Z", "");
            GlobalSettings.Instance.SessionExpire = DateTime.Parse(datestr);
            return true;

        }

        public static CCReceiptData PrintReceipt(int salesid)
        {
            Connect();

            if (GlobalSettings.Instance.SessionKey == null)
            {
                TouchMessageBox.Show("Not able to retrieve session key!!");
                return null;
            }

            string host = GlobalSettings.Instance.BoltBaseURL;
            string urlstring = "{0}/v3/printReceipt";
            string url = string.Format(urlstring, host);

            logger.Debug("url=" + url);

            var requestbody = new
            {
                merchantId = GlobalSettings.Instance.MerchantID,
                hsn = GlobalSettings.Instance.HardwareSerialNumber,
                orderId = salesid,
                printExtraReceipt = "false",
                printDelay = "2000"
            };


            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", GlobalSettings.Instance.CardConnectAuthorization);
            request.AddHeader("X-CardConnect-SessionKey", GlobalSettings.Instance.SessionKey);


            var body = JsonConvert.SerializeObject(requestbody);
            logger.Debug("body=" + body);


            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string cont = response.Content;

            logger.Debug("response body:" + cont);

            CCReceiptData resp = JsonConvert.DeserializeObject<CCReceiptData>(cont);

            return resp;
        }

        public static bool ProcessCredit(int salesid, decimal amt, bool capture)
        {

            logger.Info("CREDIT SALE => ticket=" + salesid + ", amount=" + amt);
            decimal tip = 0;

            if (capture)
            {
                //request tip amount from customer
                tip = GetTip(amt);
            }
   
            //connect to pinpad to authorize card
            CCSaleResponse resp = authCard(salesid, amt + tip, capture);



            if (resp != null)
            {

                if (resp.resptext != null)
                {
                    string code = resp.resptext.ToUpper().Substring(0, 4);
                    switch (code)
                    {

                        case "APPR":
                            bool result = AddCreditPayment(capture ? "SALE" : "AUTH", salesid, amt, tip, resp, DateTime.Now,"");
                            if (result == false)
                            {
                                TouchMessageBox.Show("Payment record insert failed.");
                                return false;
                            }
                           
                            if (resp.retref != "")
                            {

                                Payment payment = GetPayment(resp.retref);

                                //print credit slip



                                if (GlobalSettings.Instance.PrintMerchantCopy || !capture) ReceiptPrinterModel.PrintCreditSlip(payment, "**Merchant Copy**");

                                //if settings is true ,then always print customer receipt
                                if (GlobalSettings.Instance.PrintCustomerCopy || (!capture && GlobalSettings.Instance.PrintCustomerAuthCopy )) ReceiptPrinterModel.PrintCreditSlip(payment, "**Customer Copy**");

                               
                            
                      
                                
                            }
                            return true;


                        default:

                            TouchMessageBox.Show("Response:" + resp.resptext);
                            logger.Debug("Response Message:" + resp.resptext);
                            logger.Debug("Response Reference:" + resp.retref);
                            break;

                    }
                }

                else
                    if (resp.errorcode == "8")
                {
                    TouchMessageBox.Show("Transaction Cancelled.");
                }



                return false;


            }
            else
            {
                logger.Error("No Response from Credit Card Pin Pad  ");
                TouchMessageBox.Show("No Response from Credit Card Pin Pad");
                return false;

            }
        }

 

        public static Payment GetPayment(string transactionNo)
        {
            DBSales m_dbsales = new DBSales();
            DataTable rtn = m_dbsales.GetPayment(transactionNo);
            if (rtn.Rows.Count > 0)
            {
                return new Payment(rtn.Rows[0]);
            }
            else return null;
        }

        public static string ProcessRefund(string retref)
        {
            logger.Info("COMMAND:Credit Refund ");

            CCRefundResponse resp = CardConnectModel.Refund(retref);
            logger.Info("CREDIT REFUND => reference number:" + retref );

            logger.Info("Credit Refund:RECEIVED:" + resp.ToString());




            if (resp.respcode == "000") //resptext = 'Approval'
            {


                return resp.resptext;

            }
            else
            {
                logger.Error(resp.resptext);
                TouchMessageBox.Show("REFUND TRANSACTION FAILED !!!! ERROR:  " + resp.resptext);
                return "";

            }
        }



        public static bool AddCreditPayment(string transtype, int salesid, decimal requested_amount, decimal tip, CCSaleResponse response, DateTime paymentdate,string reason)
        {

            var result = SalesModel.InsertCreditPayment(transtype, salesid, requested_amount, tip, response, paymentdate,reason);


            decimal balance = 0.00m;
            (new VFD(GlobalSettings.Instance.DisplayComPort)).WriteDisplay("Credit:", requested_amount, "Balance:", balance);

            return result;


        }
    }



}
