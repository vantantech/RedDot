using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class TicketTotals
    {

        public decimal TipTotal { get; set; }
        public decimal SelectedTotal { get; set; }

        public decimal TaxTotal { get; set; }
        public decimal SubTotal
        {
            get
            {
                return Math.Round(SelectedTotal + TaxTotal, 2);
            }
        }
        public decimal TotalWithTip
        {
            get
            {
                return Math.Round(SubTotal + TipTotal, 2);
            }
        }
    }
}
