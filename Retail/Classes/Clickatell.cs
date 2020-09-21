using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedDot
{
    public class Clickatell
    {
        static public string SendSms(String strUsername,  String strPassword, String strApi_ID, String To, String strMessage)
        {
            WebClient wc = new WebClient();
            String sRequestURL;

        sRequestURL = "http://api.clickatell.com/http/sendmsg?" + 
     
                "user=" + strUsername
                + "&password=" + strPassword
                + "&api_id=" + strApi_ID
                + "&to=" + To
                + "&text=" + strMessage;

            byte[] response = wc.DownloadData(sRequestURL);


            String sResult = Encoding.ASCII.GetString(response); // sResult contains the error text 
            return sResult;
          

           // int nResult = System.Convert.ToInt32(sResult.Substring(0, 4));
            //if (nResult > 0) MessageBox.Show("Error SMS:" + sResult);
            //return nResult;
            
        }

        static public string SendSms2(String strUsername, String strPassword, String strApi_ID, String To, String strMessage)
        {
            WebClient wc = new WebClient();
            String sRequestURL, sRequestData;

            sRequestURL = "http://api.clickatell.com/http/sendmsg?";

            sRequestData = "user=" + strUsername
                + "&password=" + strPassword
                + "&api_id=" + strApi_ID
                + "&to=" + To
                + "&text=" + strMessage;


            byte[] postData = Encoding.ASCII.GetBytes(sRequestData);
            byte[] response = wc.UploadData(sRequestURL, postData);

            String sResult = Encoding.ASCII.GetString(response); // sResult contains the error text 
            return sResult;
        }

    }
}


    