using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RedDot;

namespace RedDot
{
    public class EODSettleVM:INPCBase
    {
        public ICommand PrintAllReceiptClicked { get; set; }
        public ICommand PrintAllLargeReceiptClicked { get; set; }
        private DBSales m_dbsales;


        public EODSettleVM()
        {
            PrintAllReceiptClicked = new RelayCommand(ExecutePrintAllReceiptClicked, param => true);
            PrintAllLargeReceiptClicked = new RelayCommand(ExecutePrintAllLargeReceiptClicked, param => true);
            m_dbsales = new DBSales();
        }




        public void ExecutePrintAllReceiptClicked(object button)
        {
            DataTable _tickets = m_dbsales.GetClosedSalesbyDates(DateTime.Today, DateTime.Today);
            Ticket ticket;
            foreach(DataRow row in _tickets.Rows)
            {
                ticket = new Ticket((int)row["id"]);
                ticket.PrintReceipt();
            }


        }



        public void ExecutePrintAllLargeReceiptClicked(object button)
        {
            DataTable _tickets = m_dbsales.GetClosedSalesbyDates(DateTime.Today, DateTime.Today);
            Ticket ticket;
            foreach (DataRow row in _tickets.Rows)
            {
                ticket = new Ticket((int)row["id"]);
                ticket.PrintLargeReceipt();
            }


        }
    }
}
