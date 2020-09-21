using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager;
using System.Windows;

namespace RedDot
{
    public class TicketCore:INPCBase
    {
        

        public static bool HasBeenPaid(int salesid , string paymenttype)
        {
            DBTicket dbticket = new DBTicket();

            return dbticket.HasBeenPaid(salesid, paymenttype);
            
        }

        public static bool GiftCardOnPayment(int salesid,string giftcardnumber)
        {
            DBTicket dbticket = new DBTicket();
            return dbticket.GiftCardOnPayment(salesid, giftcardnumber);


        }

        public static bool InsertPayment(int salesid, string paytype, decimal amount, decimal netamount, string authorizecode)
        {
            DBTicket dbticket = new DBTicket();
  
           return dbticket.DBInsertPayment(salesid, paytype, amount, netamount, authorizecode);
        }
  
    }
}
