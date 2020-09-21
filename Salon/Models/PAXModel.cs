using NLog;
using POSLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RedDot.Models
{
    public class PAXModel
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private int m_laneid;
        PosLink pg = new PosLink();
        CommSetting commSetting = new POSLink.CommSetting();

        public PAXModel(int laneid)
        {
            commSetting.DestIP = "192.168.1.248";
            commSetting.DestPort = "10009";
            commSetting.CommType = "TCP";
            commSetting.TimeOut = "-1";
            pg.CommSetting = commSetting;
        }


        public PaymentResponse ExecuteCreditSale(int salesid, decimal amount, string referencenumber)
        {

            PaymentRequest paymentRequest = new PaymentRequest();
            paymentRequest.TenderType = paymentRequest.ParseTenderType("CREDIT");
            paymentRequest.TransType = paymentRequest.ParseTransType("SALE");
            paymentRequest.Amount = ((int)(amount * 100)).ToString();
            paymentRequest.ECRRefNum = referencenumber;
            paymentRequest.ExtData = "<TipRequest>1</TipRequest>";
           // paymentRequest.OrigRefNum = referencenumber;
           // paymentRequest.ECRTransID = salesid.ToString();
           // paymentRequest.SigSavePath = "C:\\reddot\\logs";

            pg.PaymentRequest = paymentRequest;
            
            ProcessTransResult result = pg.ProcessTrans();

            if (result.Code == POSLink.ProcessTransResultCode.OK)
                return pg.PaymentResponse;
            else return null;

        }
    }
}
