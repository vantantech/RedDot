using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.Models.CardConnect
{
    public class CCReceiptData
    {

        public string dba { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string phone { get; set; }
        public string header { get; set; }
        public string orderNote { get; set; }
        public string dateTime { get; set; }
        public string items { get; set; }
        public string nameOnCard { get; set; }
        public string footer { get; set; }
    }
}
