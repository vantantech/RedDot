using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBSales
    {
        DBConnect _dbconnect;

        public DBSales()
        {

            _dbconnect = new DBConnect();
        }

       public  DataTable GetSalesbyID(int salesid)
        {

            return _dbconnect.GetData("Select * from sales where  id=" + salesid, "sales");

        }

       public DataTable GetOpenSalesbyEmployee(int employeeid)
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as server from sales left outer join employee on sales.employeeid = employee.id where status='Open' and employeeid=" + employeeid, "sales");

       }

       public DataTable GetOpenSalesbyTable(int tablenumber, int employeeid)
       {
            string filter = "";

            if (employeeid > 0) filter = " and employeeid =" + employeeid;


            string query = "Select  sales.*, sales.id as salesid from sales where status='Open' and tablenumber=" + tablenumber + filter;

            return _dbconnect.GetData(query, "sales");

       }

        public DataTable GetOpenSalesbyOrderNumber(int ordernumber, int employeeid)
        {
            string filter = "";

            if (employeeid > 0) filter = " and employeeid =" + employeeid;


            string query = "Select  sales.*, sales.id as salesid from sales where status='Open' and ordernumber=" + ordernumber + filter;

            return _dbconnect.GetData(query, "sales");

        }

        public DataTable GetSplitTickets(string tracker)
        {

            string query = "Select  sales.*, sales.id as salesid from sales where tracker='" + tracker + "'";

            return _dbconnect.GetData(query, "sales");

        }


        public DataTable GetCombineTickets(int employeeid, int currentsalesid)
        {
        


            string query = "Select sales.*, sales.id as salesid from sales " +
                     " where sales.status = 'Open'  and employeeid = " + employeeid + " and  id != " + currentsalesid +
                     " order by saledate desc ";




            return _dbconnect.GetData(query, "Table");
        }


        public DataTable GetOpenOrders(int employeeid,string ordertype)
        {
            string filter = "";

            if (employeeid > 0) filter = " and employeeid =" + employeeid;
            if (ordertype != "") filter = filter + " and ordertype='" + ordertype + "' ";

            string query = "Select sales.*, sales.id as salesid from sales " +
                     " where sales.status = 'Open' " + filter + 
                     " order by saledate desc ";

            return _dbconnect.GetData(query, "Table");
        }

        public DataTable GetOpenOrdersByOthers(int employeeid, string ordertype)
        {
            string filter = "";

            if (employeeid > 0) filter = " and employeeid !=" + employeeid;
            if (ordertype != "") filter = filter + " and ordertype='" + ordertype + "' ";

            string query = "Select sales.*, sales.id as salesid from sales " +
                     " where sales.status = 'Open' " + filter +
                     " order by saledate desc ";

            return _dbconnect.GetData(query, "Table");
        }

        public DataTable GetHoldOrders(int station)
        {


            string query = "select sales.* from sales " +
                            " inner join salesitem on sales.id = salesitem.salesid " +
                            " where stationno = " + station + " and status = 'Open' and sent = 0 and holddate is not null and holddate<NOW() " +
                            " group by sales.id ";

            return _dbconnect.GetData(query, "Table");
        }

        public int GetTicketCountbyTable(int tablenumber)
        {

            DataTable dt = _dbconnect.GetData("Select  count(sales.id) as cnt from sales where status='Open' and tablenumber=" + tablenumber, "sales");
            if (dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["cnt"].ToString());
            }
            else
                return 0;
        }

        public int GetTicketElapsedTimeTable(int tablenumber)
        {

            DataTable dt = _dbconnect.GetData("Select  coalesce(max(TIMESTAMPDIFF(MINUTE,saledate,NOW())),0) as max from sales where status='Open' and tablenumber=" + tablenumber, "sales");
            if (dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["max"].ToString());
            }
            else
                return 0;
        }

        public string GetTicketServerByTable(int tablenumber)
        {

            DataTable dt = _dbconnect.GetData("Select  displayname from sales inner join employee on sales.employeeid = employee.id where status='Open' and tablenumber=" + tablenumber, "sales");
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["displayname"].ToString();
            }
            else
                return "";
        }
        public DataTable GetOpenSalesbyCustomer(int customerid)
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where status='Open' and customerid=" + customerid, "sales");

       }

       public DataTable GetOpenSalesbyTicket(int id)
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where status='Open' and sales.id=" + id, "sales");

       }

       public DataTable GetOpenSalesbyDates(DateTime startdate , DateTime enddate)
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where status='Open' and   DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'", "sales");

       }
       public DataTable GetAllOpenSales()
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where status='Open' order by  sales.id asc  ", "sales");

       }

       public DataTable GetAllOpenSalesQS()
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where status='Open' order by sales.tablenumber asc, sales.id asc  ", "sales");

       }

       public DataTable GetClosedSalesbyDates(DateTime startdate, DateTime enddate)
       {

           return _dbconnect.GetData("Select  * from sales where status='Closed' and  ( DATE(closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "')", "sales");

       }

       public DataTable GetSalesCount(int employeeid)
       {

           return _dbconnect.GetData("Select  count(*) as count from sales where sales.employeeid=" + employeeid, "sales");

       }



        public DataTable GetPayment(string txn)
        {
            return _dbconnect.GetData("Select * from payment where responseid=" + txn);
        }


        public DataTable GetPayment(int id)
        {
            return _dbconnect.GetData("Select * from payment where id=" + id);
        }

        public bool DBUpdateTipAmount(int paymentid, decimal tipamount, decimal totalamount)
        {
            if (paymentid == 0) return false;

            string querystring = "";
            querystring = "Update  payment set tipamount = " + tipamount + ", amount =" + totalamount + "  where id =" + paymentid;
            return _dbconnect.Command(querystring);
        }

        public bool UpdatePaymentCapture(int paymentid, decimal tipamount, decimal amount, decimal netamount, string transactionid)
        {
            if (paymentid == 0) return false;

            string querystring = "";
            querystring = "Update  payment set transtype='POSTAUTH', tipamount = " + tipamount + ", amount =" + amount + ", netamount=" + netamount + ", responseid=" + transactionid + "  where id =" + paymentid;
            return _dbconnect.Command(querystring);
        }

    }
}
