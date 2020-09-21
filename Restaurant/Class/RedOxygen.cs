using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedDot
{
    public class RedOxygen
    {
        static public int SendSms(String strAccountId, String strEmail, String strPassword, String strMSISDN, String strMessage) 
        { 
            WebClient wc = new WebClient(); 
            String sRequestURL; 
          sRequestURL = "http://www.redoxygen.net/sms.dll?Action=SendSMS&" +
              "AccountId=" + strAccountId
              + "&Email=" +strEmail
              + "&Password=" + strPassword 
              + "&Recipient=" + strMSISDN 
              + "&Message="+ strMessage; 

          byte[] response = wc.DownloadData(sRequestURL); 


          String sResult = Encoding.ASCII.GetString(response); // sResult contains the error text 
          
          int nResult = System.Convert.ToInt32(sResult.Substring(0,4));
          if (nResult > 0) MessageBox.Show("Error SMS:" + sResult);
          return nResult; 
        }


        static public int SendSms2(String strAccountId, String strEmail, String strPassword, String strMSISDN, String strMessage)
        {
            WebClient wc = new WebClient();
            String sRequestURL, sRequestData;

            sRequestURL = "http://www.redoxygen.net/sms.dll?Action=SendSMS";

            sRequestData = "AccountId=" + strAccountId
                + "&Email=" + System.Web.HttpUtility.UrlEncode(strEmail)
                + "&Password=" + System.Web.HttpUtility.UrlEncode(strPassword)
                + "&Recipient=" + System.Web.HttpUtility.UrlEncode(strMSISDN)
                + "&Message=" + System.Web.HttpUtility.UrlEncode(strMessage);


            byte[] postData = Encoding.ASCII.GetBytes(sRequestData);
            byte[] response = wc.UploadData(sRequestURL, postData);

            String sResult = Encoding.ASCII.GetString(response);  // sResult contains the error text
            int nResult = System.Convert.ToInt32(sResult.Substring(0, 4));
            return nResult;
        }
    }
}
