using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{


    //Class for each sales ticket
    //SalesData represent a ticket and the salesitem is each ticket service item
    public class SalesData
    {
        private decimal m_creditcardfeepercent=0;
        public SalesData(decimal creditcardfeepercent)
        {
            m_creditcardfeepercent = creditcardfeepercent;
        }
        public int SalesID { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Gratuity { get; set; }
        public decimal NetGratuity { get { return Math.Round(Gratuity * (100 - m_creditcardfeepercent) / 100,2); } }
        public string PaymentType { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalSupplyFee { get; set; }
        public decimal TotalCommission { get; set; }

        public ObservableCollection<SalonLineItem> SalesItem { get; set; }

        public int ServiceCount { get { return SalesItem.Count(); } }


    }
}
