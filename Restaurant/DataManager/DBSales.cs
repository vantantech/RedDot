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

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where status='Open' and employeeid=" + employeeid, "sales");

       }

        public DataTable GetOpenTabletSalesbyEmployee(int employeeid)
        {

            return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where (status='Open' or status='OpenTablet') and employeeid=" + employeeid, "sales");

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
                "left outer join payment on sales.id = payment.salesid " +
                     " where sales.status = 'Open' and payment.id is null and employeeid = " + employeeid + " and  sales.id != " + currentsalesid +
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
                            " where stationno = " + station + "  and sent = 0 and holddate is not null and holddate < NOW() " +
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

        public DataTable GetCashDrawer(int stationno)
        {
            string sqlstr;
            sqlstr = "select * from cashdrawer where cashoutdate is null and  stationno=" + stationno ;
            return _dbconnect.GetData(sqlstr);
        }

        public DataTable GetLastCashDrawer(int stationno)
        {
            string sqlstr;
            sqlstr = "select  * from cashdrawer where   stationno=" + stationno + " and  cashoutdate is not null order by cashoutdate desc limit 1 ";
            return _dbconnect.GetData(sqlstr);
        }

        public bool CashierIn(int stationno, int employeeid, int cashin100, int cashin50, int cashin20, int cashin10, int cashin5, int cashin2, int cashin1, int cashin50cent, int cashin25cent, int cashin10cent, int cashin5cent, int cashin1cent, decimal cashintotal)
        {
            string query = "insert into cashdrawer (stationno,employeeid,cashin100,cashin50,cashin20,cashin10,cashin5,cashin2,cashin1,cashin50cent,cashin25cent,cashin10cent,cashin5cent,cashin1cent,cashintotal) values ";
            query += " (" + stationno + "," + employeeid + "," + cashin100 + "," + cashin50 + "," + cashin20 + "," + cashin10 + "," + cashin5 + "," + cashin2 + "," + cashin1 + "," + cashin50cent + "," + cashin25cent + "," + cashin10cent + "," + cashin5cent + "," + cashin1cent + "," + cashintotal + ")";
            return _dbconnect.Command(query);
        }

        public bool CashierOut(int id, int cashout100, int cashout50, int cashout20, int cashout10, int cashout5, int cashout2, int cashout1, int cashout50cent, int cashout25cent, int cashout10cent, int cashout5cent, int cashout1cent, decimal cashouttotal, string note)
        {
            string query = "update cashdrawer set cashout100=" + cashout100;
            query += ",cashout50=" + cashout50;
            query += ",cashout20=" + cashout20;
            query += ",cashout10=" + cashout10;
            query += ",cashout5=" + cashout5;
            query += ",cashout2=" + cashout2;
            query += ",cashout1=" + cashout1;
            query += ",cashout50cent=" + cashout50cent;
            query += ",cashout25cent=" + cashout25cent;
            query += ",cashout10cent=" + cashout10cent;
            query += ",cashout5cent=" + cashout5cent;
            query += ",cashout1cent=" + cashout1cent;
            query += ",cashouttotal=" + cashouttotal;
            query += ",cashoutdate='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            query += ",note='" + note + "'";
            query += " where id=" + id;


            return _dbconnect.Command(query);
        }


    }
}
