using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RedDot
{
    public class Promotion
    {
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProductID { get; set; }
        public string[] DOW { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountAmount { get; set; }

      

    }
}
