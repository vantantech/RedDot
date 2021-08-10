using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAccess.ViewModels
{
    public class PaymentVM
    {
        public Nullable<System.DateTime> saledate { get; set; }
        public string Cashier { get; set; }
        public string description { get; set; }
        public decimal netamount { get; set; }
    }

    public class PaymentPivot
    {
        public Nullable<System.DateTime> saledate { get; set; }
        public string Cashier { get; set; }
        public decimal Cash { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal Check{ get; set; }
        public decimal GiftCard { get; set; }
        public decimal GiftCertificate { get; set; }
        public decimal Reward { get; set; }
        public decimal StampCard { get; set; }
        public decimal Tips { get; set; }

        public decimal AllPayments { get; set; }

    }
}