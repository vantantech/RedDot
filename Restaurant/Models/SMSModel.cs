using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models
{
    public class SMSModel
    {




        string m_smsplatform;
        string m_from;
        string m_apikey;
        string m_password;

        public SMSModel()
        {

            m_smsplatform = GlobalSettings.Instance.SMSTextPlatform;

            m_from = GlobalSettings.Instance.SMSFromPhone;
            m_apikey = GlobalSettings.Instance.APIKey;
            m_password = GlobalSettings.Instance.SMSPassword;
        }



        public string SendSMS(string MessageText, string[] ToMSISDN)
        {
            switch (m_smsplatform)
            {
                // case "Clickatell API":
                // ClickatellAPI.SendSMS(m_apikey, m_from, MessageText, ToMSISDN);
                // break;
                case "Clickatell REST":

                    //creating a dictionary to store all the parameters that needs to be sent
                    Dictionary<string, string> Params = new Dictionary<string, string>();


                    Params.Add("content", MessageText);
                    if (m_from != "") { Params.Add("from", m_from); }
                    string to = string.Join(",", ToMSISDN);
                    Params.Add("to", to);


                    var res = ClickatellRest.SendSMS(m_apikey, Params);

                    return res;

                // break;


                case "Nexmo API":
                    string res2 = "";
                    var client = new Client(creds: new Nexmo.Api.Request.Credentials
                    {
                        ApiKey = m_apikey,
                        ApiSecret = m_password
                    });

                    foreach (var phone in ToMSISDN)
                    {
                        var results = client.SMS.Send(request: new SMS.SMSRequest
                        {
                            from = m_from,
                            to = phone,
                            text = MessageText
                        });

                        res2 = results.messages.ToString();
                    }

                    return res2;

                    // break;

            }
            return "Error";
        }

        public void SendSMS(string MessageText, string phonenumber)
        {
            string[] phone = new string[] { phonenumber };

            SendSMS(MessageText, phone);
        }

        public string GetBalance()
        {

            switch (m_smsplatform)
            {
                // case "Clickatell API":
                //  return ClickatellAPI.GetBalance(m_apikey);

                case "Clickatell REST":

                    return ClickatellRest.GetBalance(m_apikey);


                case "Nexmo API":

                    var client = new Client(creds: new Nexmo.Api.Request.Credentials
                    {
                        ApiKey = m_apikey,
                        ApiSecret = m_password
                    });


                    return client.Account.GetBalance().value.ToString();

            }
            return "0";
        }
    }
}

