using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace RedDot.Models
{
   public class ClickatellRest
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //This takes the API Key and JSON array of data and posts it to the Message URL to send the SMS's
        public static string Post(string Token, string json)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://platform.clickatell.com/messages");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Headers.Add("Authorization", Token);

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }


         
        }

        public static string sendSMS(string Token, string message,string m_from, string[] phonenumbers)
        {
            //creating a dictionary to store all the parameters that needs to be sent
            Dictionary<string, string> Params = new Dictionary<string, string>();
            Params.Add("content", message);
            if (m_from != "") { Params.Add("from", m_from); }
            string to = string.Join(",", phonenumbers);
            Params.Add("to", to);
            logger.Info("SMS Message:" + message);
            logger.Info("SMS Sent to:" + to);
             var res = ClickatellRest.SendSMS(Token, Params);
            return res;

        }


        public static string SendSMS(string Token, Dictionary<string, string> Params)
        {
            Params["to"] = CreateRecipientList(Params["to"]);
            string JsonArray = JsonConvert.SerializeObject(Params, Formatting.None);
            JsonArray = JsonArray.Replace("\\\"", "\"").Replace("\"[", "[").Replace("]\"", "]");
            return Post(Token, JsonArray);
        }

        //This function converts the recipients list into an array string so it can be parsed correctly by the json array.
        public static string CreateRecipientList(string to)
        {
            string[] tmp = to.Split(',');
            to = "[\"";
            to = to + string.Join("\",\"", tmp);
            to = to + "\"]";
            return to;
        }

     

        public static string GetBalance(string Token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://platform.clickatell.com/public-client/balance");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Headers.Add("Authorization", Token);


            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    //deserialize
                    var resultarray = JsonConvert.DeserializeObject<Balance>(result);
                    return resultarray.balance;
                }
            }catch(Exception ex)
            {
                return "Error:" + ex.Message;
            }
      

      

        }
    }
}

public class Balance
{
    public string balance { get; set; }
    public string currency { get; set; }
}
