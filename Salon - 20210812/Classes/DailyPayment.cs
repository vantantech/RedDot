using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DailyPayment
    {
        public DateTime PaymentDate { get; set; }
        public string Cashier { get; set; }
        public int EmployeeId { get; set; }
       // public int SalesId { get; set; }
        public decimal Cash { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal GiftCard { get; set; }
        public decimal GiftCertificate { get; set; }

 
        public decimal Reward { get; set; }
        public decimal StampCard { get; set; }  
       // public decimal NetPayment { get; set; }
        public decimal Tips { get; set; }
        public decimal AllPayments { get; set; }

        public int TotalTicket {
            get {
                if (PaymentDetails != null)
                    return PaymentDetails.Rows.Count;
                else return 0;
            }
        }

        public DataTable PaymentDetails { get; set; }
        public DailyPayment(DataRow row)
        {
            PaymentDate = (DateTime)row["paymentdate"];
            Cashier = row["cashier"].ToString();
            if (row["employeeid"].ToString() != "") EmployeeId = int.Parse(row["employeeid"].ToString()); else EmployeeId = 0;

            // SalesId = (int)row["salesid"];
            if (row["cash"].ToString() != "") Cash = decimal.Parse(row["cash"].ToString()); else Cash = 0m;
            if (row["credit"].ToString() != "") Credit = decimal.Parse(row["credit"].ToString()); else Credit = 0m;
            if (row["debit"].ToString() != "") Debit = decimal.Parse(row["debit"].ToString()); else Debit = 0m;
            if (row["giftcard"].ToString() != "") GiftCard = decimal.Parse(row["giftcard"].ToString()); else GiftCard = 0m;
            if (row["giftcertificate"].ToString() != "") GiftCertificate = decimal.Parse(row["giftcertificate"].ToString()); else GiftCertificate = 0m;
            if (row["reward"].ToString() != "") Reward = decimal.Parse(row["reward"].ToString()); else Reward = 0m;
            if (row["stampcard"].ToString() != "") StampCard = decimal.Parse(row["stampcard"].ToString()); else StampCard = 0m;
            if (row["tips"].ToString() != "") Tips = decimal.Parse(row["tips"].ToString()); else Tips = 0m;
            if (row["allpayments"].ToString() != "") AllPayments = decimal.Parse(row["allpayments"].ToString()); else AllPayments = 0m;


          
        }

    }
}
