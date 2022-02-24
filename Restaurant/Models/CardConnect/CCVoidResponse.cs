using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models.CardConnect
{
    public class CCVoidResponse
    {
        public string merchid { get; set; }
        public string orderId { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string retref { get; set; }
        public string authcode { get; set; }
        public string respcode { get; set; }
        public string respproc { get; set; }
        public string respstat { get; set; }
        public string resptext { get; set; }

// public CCReceiptData receipt { get; set; }
    }
}
