using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;

namespace RedDotService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISMSCallBack" in both code and config file together.
    [ServiceContract]
    public interface ISMSCallBack
    {
      

        [OperationContract]
        [WebGet]
        string SMSReply(string api_id, string from , string to , string timestamp, string text );

 
    }



 
}
