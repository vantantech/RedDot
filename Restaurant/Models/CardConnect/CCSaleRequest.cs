using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models.CardConnect
{
    public class CCSaleRequest
    {
        public CCSaleRequest()
        {
            amount = "0";
            includeSignature = "true";
            includeAmountDisplay = "false";
            beep = "false";
            aid = "credit";
            includeAVS = "false";
            capture = "true";
            clearDisplayDelay = "500";

        }
        public string merchantId { get; set; }
        public string hsn { get; set; }
        public string amount { get; set; }

        public string includeSignature { get; set; }
        public string includeAmountDisplay { get; set; }
        public string beep { get; set; }
        public string aid { get; set; }
        public string includeAVS { get; set; }
        public string capture { get; set; }
        public string orderId { get; set; }

        public string clearDisplayDelay { get; set; }
    }
}
