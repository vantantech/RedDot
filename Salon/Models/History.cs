using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using RedDot.DataManager;

namespace RedDot
{
    public class History
    {

        private DBHistory _dbhistory;

        public History()
        {
            _dbhistory = new DBHistory();
        }

        public DataTable GetOrders(DateTime startdate, DateTime enddate, int employeeid, bool simple)
        {

                    return _dbhistory.GetOrdersSalon(startdate, enddate, employeeid,simple);


        }

        public DataTable GetAUTHPayments(DateTime startdate, DateTime enddate)
        {

            return _dbhistory.GetAUTHPayments(startdate, enddate);


        }

        public DataTable GetSyncList(DateTime startdate, DateTime enddate)
        {

            return _dbhistory.GetSyncList(startdate, enddate);


        }
        public DataTable GetReversedTickets()
        {

            return _dbhistory.GetReversedTicketsSalon();

        }
        public DataTable GetOrdersByID(int salesid)
        {
                    return _dbhistory.GetOrdersByID(salesid);

        }

        public void UpdateSyncDate(int salesid)
        {
            _dbhistory.UpdateSyncDate(salesid);
        }


    }
}
