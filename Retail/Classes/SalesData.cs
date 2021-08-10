using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{


    //Class for each sales ticket
    public class SalesData
    {

        public int SalesID { get; set; }

        public string TicketNo { get { return GlobalSettings.Instance.Shop.StorePrefix + SalesID; } }
        public DateTime CloseDate { get; set; }
          public string PaymentType { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalAdjustments { get; set; }
        public decimal TotalMargin { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalCommissionAdjustment { get; set; }
        public decimal TotalNetCommission { get { return TotalCommission - TotalCommissionAdjustment; } }
        public ObservableCollection<RetailLineItem> SalesItem { get; set; }


    }
}
