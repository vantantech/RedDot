using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WebSync
{
    public class History
    {

        private DBHistory _dbhistory;

        public History()
        {
            _dbhistory = new DBHistory();
        }

        public DataTable GetOrders(DateTime startdate, DateTime enddate)
        {

                    return _dbhistory.GetOrdersSalon(startdate, enddate);


        }

        public DataTable GetSyncList(DateTime startdate, DateTime enddate)
        {

            return _dbhistory.GetSyncList(startdate, enddate);


        }

        public DataTable GetAutoSyncList()
        {

            return _dbhistory.GetAutoSyncList();


        }
        public DataTable GetReversedTickets()
        {

            return _dbhistory.GetReversedTicketsSalon();

        }


        public void UpdateSyncDate(int salesid)
        {
            _dbhistory.UpdateSyncDate(salesid);
        }


    }
}
