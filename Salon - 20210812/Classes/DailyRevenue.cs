using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DailyRevenue
    {
        public string DOW { get; set; }
        public string ReportDate { get; set; }
        public ObservableCollection<ReportCat> SalesCat { get; set; }

        public decimal SalesTax { get; set; }
        public decimal TipsWitheld { get; set; }
        public decimal TotalRevenue { get; set; }
      
        public decimal TotalDiscount { get; set; }
        public decimal TotalAdjustment { get; set; }

        public decimal NetRevenue { get { return TotalRevenue  + TipsWitheld + SalesTax + TotalDiscount + TotalAdjustment; } } // discount and adjustment should be negative

    }
}
