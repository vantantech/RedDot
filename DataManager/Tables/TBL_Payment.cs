using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.DataManager
{
    public class TBL_Payment
    {
        public int Id { get; set; }
        public int Salesid { get; set; }
        public string Cardgroup { get; set; }
        public decimal Amount { get; set; }
        public string Authorcode { get; set; }
        public decimal Netamount { get; set; }
        public string Cardtype { get; set; }
        public string Maskedpan { get; set; }
        public decimal Tipamount { get; set; }
        public DateTime Paymentdate { get; set; }
        public DateTime Voiddate { get; set; }
        public decimal Cashbackamount { get; set; }
        public string Cardacquisition { get; set; }
        public string Responseid { get; set; }
        public bool Void {get;set;}
        public string Transtype { get; set; }
        public int PinVerified { get; set; }


    }
}
