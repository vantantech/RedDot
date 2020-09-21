using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class Payment
    {

        public string Description {get; set;}
        public decimal Amount {get; set;}
        public decimal NetAmount { get; set; }
        public int ID { get; set; }
        public string AuthorCode { get; set; }

        public DateTime PaymentDate { get; set; }
        public DateTime VoidDate { get; set; }

        public bool Voided { get; set; }

        public string AmountStr {
          get {
                if (Voided)
                {
                    return "**";
                }
                else

                    return String.Format("{0:0.00}", Amount);
          } 

      }

      public string ReceiptStr
      {
          get
          {
              return Utility.FormatPrintRow(Description + " " + AuthorCode, AmountStr, 37);
          }

      }

      public string ReceiptDateStr
      {
          get
          {
                if (Voided)
                {
                    return Utility.FormatPrintRow(VoidDate.ToShortDateString() + "(VOIDED) ", "**", 31);
                }
                else
                    return Utility.FormatPrintRow(PaymentDate.ToShortDateString() + " " +  Description + " " + AuthorCode, AmountStr, 37);
          }

      }
        public Payment(int id,string description, decimal amount, decimal netamount, string authorcode, bool voided)
        {
            Description = description;
            Amount = amount;
            ID = id;
            AuthorCode = authorcode;
            NetAmount = netamount;
            Voided = voided;
          
        }
    }


}
