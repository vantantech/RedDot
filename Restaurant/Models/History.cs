using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using System.Collections.ObjectModel;

namespace RedDot
{
    public class QSHistory
    {

        private DBHistory _dbhistory;

        public QSHistory()
        {
            _dbhistory = new DBHistory();
        }

        public DataTable GetClosedVoidedOrders(DateTime startdate, DateTime enddate, int employeeid, string transtype, bool showcancelledtickets)
        {
          return _dbhistory.GetClosedVoidedOrders(startdate, enddate,employeeid,transtype,showcancelledtickets);
        }


        public DataTable GetAUTHPayments(DateTime startdate, DateTime enddate)
        {

            return _dbhistory.GetAUTHPayments(startdate, enddate);


        }

        public DataTable GetCreditDebitPayments(DateTime startdate, DateTime enddate)
        {

            return _dbhistory.GetCreditDebitPayments(startdate, enddate);


        }

        public ObservableCollection<Ticket> GetOpenOrders(DateTime startdate, DateTime enddate)
        {
            ObservableCollection<Ticket> rtn = new ObservableCollection<Ticket>();

            DataTable dat = _dbhistory.QSGetOpenOrdersID(startdate, enddate);

            foreach(DataRow row in dat.Rows)
            {
                Ticket newrec = new Ticket(int.Parse(row["salesid"].ToString()));
                rtn.Add(newrec);
            }

            return rtn;
        }

        /*
        public DataTable GetReversedTickets()
        {

            return _dbhistory.GetReversedTickets();

        }
        */

        public DataTable GetOrdersByID(int salesid)
        {


            return _dbhistory.QSGetOrdersByID(salesid);

        }


    }
}
