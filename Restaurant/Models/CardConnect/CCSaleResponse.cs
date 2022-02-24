using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models.CardConnect
{
    public class CCSaleResponse
    {
        public string errorcode { get; set; }
        public string errormessage { get; set; }
        public string token { get; set; }
        public string expiry { get; set; }
        public string name { get; set; }
        public string signature { get; set; }
        public int batchid { get; set; }
        public string retref { get; set; }
        public string avsresp { get; set; }
        public string respproc { get; set; }
        public decimal amount { get; set; }
        public string resptext { get; set; }
        public string authcode { get; set; }
        public string respcode { get; set; }
        public string cvvresp { get; set; }
        public string respstat { get; set; }

        public string emvTagData { get; set; }

        public EMV EMV_Data { get; set; }
        public string orderid { get; set; }
        public string entrymode { get; set; }
        public string bintype { get; set; }
        public CCReceiptData receiptData { get; set; }
    }
}
