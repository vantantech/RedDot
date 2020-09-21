using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;

namespace RedDot
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

            return _dbhistory.GetClosedTicketsRetail(startdate, enddate);

        }

        public DataTable GetReversedTickets()
        {

            return _dbhistory.GetReversedTicketsRetail();

        }

        public DataTable GetOpenTickets()
        {

            return _dbhistory.GetOpenTicketsRetail();

        }


        public DataTable GetQuotes()
        {

            return _dbhistory.GetQuotes();

        }


        public DataTable GetOrdersByID(int salesid)
        {


          return _dbhistory.GetOrdersByID(salesid);



        }


    }
}
