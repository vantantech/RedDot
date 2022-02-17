
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models
{
    public  class SMSModel
    {





        public static string SendSMS(string MessageText, string phonenumber)
        {
            if (phonenumber.Length == 10) phonenumber = "1" + phonenumber;


            return SendSMS(MessageText, phonenumber.Split(','));
        }

        public static string SendSMS(string MessageText, string[] ToPhoneNumbers)
        {

            string m_smsplatform;
            string m_from;
            string m_apikey;
            string m_password;

            m_smsplatform = GlobalSettings.Instance.SMSTextPlatform;

            m_from = GlobalSettings.Instance.SMSFromPhone;
            if (m_from.Length == 10) m_from = "1" + m_from;
            m_apikey = GlobalSettings.Instance.APIKey;
            m_password = GlobalSettings.Instance.SMSPassword;
            SMSModel sms = new SMSModel();


            switch (m_smsplatform)
            {
               // case "Clickatell API":
                   // ClickatellAPI.SendSMS(m_apikey, m_from, MessageText, ToMSISDN);
                   // break;
                case "Clickatell REST":

                    

                   
                        for (int i = 0; i < ToPhoneNumbers.Length; ++i)
                        {
                        if (ToPhoneNumbers[i].Length == 10)  ToPhoneNumbers[i] = "1" + ToPhoneNumbers[i];
                        }
                    

                    if(ToPhoneNumbers.Length > 100)
                    {
                        string result = "";
                        int numberofloop = ToPhoneNumbers.Length / 100;
                        for(int i=0;i <= numberofloop; i++)
                        {
                            var groupof100 = ToPhoneNumbers.Skip(i * 100).Take(100);
                            result = ClickatellRest.sendSMS(m_apikey, MessageText, m_from,groupof100.ToArray());
                        }

                        return result;
                    }else
                    {
                        var res = ClickatellRest.sendSMS(m_apikey, MessageText,m_from, ToPhoneNumbers);

                        return res;
                    }

                
                       
                   // break;

/*
                case "Nexmo API":
                    string res2 = "";
                    var client = new Client(creds: new Nexmo.Api.Request.Credentials
                    {
                        ApiKey = m_apikey,
                        ApiSecret = m_password
                    });

                    foreach(var phone in ToPhoneNumbers)
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
*/
                   // break;
                
            }
            return "Error";
        }

  

        public static string GetBalance()
        {

            string m_smsplatform;
            string m_from;
            string m_apikey;
            string m_password;

            m_smsplatform = GlobalSettings.Instance.SMSTextPlatform;

            m_from = GlobalSettings.Instance.SMSFromPhone;
            if (m_from.Length == 10) m_from = "1" + m_from;

            m_apikey = GlobalSettings.Instance.APIKey;
            m_password = GlobalSettings.Instance.SMSPassword;
            SMSModel sms = new SMSModel();

            switch (m_smsplatform)
            {
               // case "Clickatell API":
                  //  return ClickatellAPI.GetBalance(m_apikey);
                   
                case "Clickatell REST":

                    return ClickatellRest.GetBalance(m_apikey);

/*
                case "Nexmo API":

                    var client = new Client(creds: new Nexmo.Api.Request.Credentials
                    {
                        ApiKey = m_apikey,
                        ApiSecret = m_password
                    });

                  
                    return client.Account.GetBalance().value.ToString();
               */
            }
            return "0";
        }
    }
}
