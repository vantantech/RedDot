using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class SalesTicket
    {
        public int salesid { get; set; }
        public DateTime salesdate { get; set; }
        public decimal discount { get; set; }
        public decimal total { get; set; }
        public decimal subtotal { get; set; }


    }
}
