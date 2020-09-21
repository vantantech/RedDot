using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class Difference
    {
        public decimal NetRevenue { get; set; }
        public decimal NetPayment { get; set; }
        public decimal NetDifference { get { return NetPayment - NetRevenue; } }
    }
}
