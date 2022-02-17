using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class Gratuity
    {
        public Employee CurrentEmployee { get; set; }
        public decimal Amount { get; set; }

        public decimal TicketShareAmount { get; set; }

        public int ID { get; set; }

        private bool m_empty;

     public string AmountStr 
     {
       get {
           if (m_empty)
               return "[empty]";
           else
               return String.Format("{0:0.00}", Amount);
          } 
      }

      public string ReceiptStr
      {
          get
          {
              return Utility.FormatPrintRow(CurrentEmployee.DisplayName, AmountStr, 35);
          }

      }

       public string IDStr
      {
          get { return ID + "," + CurrentEmployee.ID; }
      }


      public Gratuity(DataRow row)
        {

            int employeeid;


            if (row["id"].ToString() != "") ID = int.Parse(row["id"].ToString()); else ID = 0;

            if (row["amount"].ToString() != "[N/A]" && row["amount"].ToString() != "") Amount = decimal.Parse(row["amount"].ToString()); else Amount = 0;
            if (row["techamount"].ToString() != "") TicketShareAmount = decimal.Parse(row["techamount"].ToString()); else TicketShareAmount = 0;
            if (row["employeeid"].ToString() != "") employeeid = int.Parse(row["employeeid"].ToString()); else employeeid = 0;
            if (row["amountstr"].ToString() != "[empty]") m_empty = false; else m_empty = true;

            CurrentEmployee = new Employee(employeeid);
    
          
        }

  

    }
}
