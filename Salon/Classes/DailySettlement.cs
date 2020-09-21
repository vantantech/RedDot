using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DailySettlement
    {
        public string DOW { get; set; }
        public string ReportDate { get; set; }
        public ObservableCollection<ReportCat> PaymentCat { get; set; }

        public decimal TotalPayment { get; set; }
  

    }
}
