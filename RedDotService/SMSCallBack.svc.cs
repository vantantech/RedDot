using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace RedDotService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SMSCallBack" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SMSCallBack.svc or SMSCallBack.svc.cs at the Solution Explorer and start debugging.
    public class SMSCallBack : ISMSCallBack
    {
        DBTicket m_dbticket;
        public SMSCallBack()
        {
           
        }
  

   


        public string  SMSReply(string api_id, string from , string to , string timestamp, string text )
        {
            //Do stuff
            m_dbticket.SMSReply(api_id, from, to, timestamp, text);
            return "<html><body>Success</body></html>";
        }
    }
}
