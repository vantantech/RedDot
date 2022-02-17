using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DailySales
    {
        public string Cashier { get; set; }
        public int EmployeeId { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal ItemDiscount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal Balance { get; set; }
        public decimal Tips { get; set; }

        public int TotalTicket
        {
            get
            {
                if (SalesDetails != null)
                    return SalesDetails.Rows.Count;
                else return 0;
            }
        }

        public DailySales(DataRow row)
        {
            Cashier = row["cashier"].ToString();
            EmployeeId = (int)row["employeeid"];
            ItemPrice = (decimal) row["itemprice"];
            ItemDiscount = (decimal)row["itemdiscount"];
            Tax = (decimal)row["tax"];
            Total = (decimal)row["total"];
            Balance = (decimal)row["balance"];
            Tips = (decimal)row["tips"];
        }

        public DataTable SalesDetails { get; set; }
    }
}
