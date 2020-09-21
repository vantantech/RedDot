using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.DataManager
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

            return _dbconnect.GetData("Select * from sales where  id=" + salesid);

        }

       public DataTable GetOpenSalesbyEmployee(int employeeid)
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where status='Open' and employeeid=" + employeeid);

       }
       public DataTable GetOpenSalesbyCustomer(int customerid)
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where status='Open' and customerid=" + customerid);

       }

       public DataTable GetOpenSalesbyTicket(int id)
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where status='Open' and sales.id=" + id);

       }

       public DataTable GetOpenSalesbyDates(DateTime startdate , DateTime enddate)
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where status='Open' and   DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'");

       }
       public DataTable GetAllOpenSales()
       {

           return _dbconnect.GetData("Select  sales.*, concat(coalesce(customer.firstname,''),' ',coalesce(customer.lastname,'')) as customername, employee.displayname, 0 as selected from sales left outer join customer on sales.customerid = customer.id left outer join employee on sales.employeeid = employee.id    where status='Open' or status='Reversed' order by  sales.id asc  ");

       }



       public DataTable GetClosedSalesbyDates(DateTime startdate, DateTime enddate)
       {

           return _dbconnect.GetData("Select  * from sales where status='Closed' and  ( DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "')");

       }

       public DataTable GetSalonSalesCount(int employeeid)
       {

           return _dbconnect.GetData("Select  count(*) as count from salesitem where salesitem.employeeid=" + employeeid);

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

            ResetWebSyncDate(paymentid);

            string querystring = "";
            querystring = "Update  payment set tipamount = " + tipamount + ", amount =" + totalamount + "  where id =" + paymentid;
            return _dbconnect.Command(querystring);
        }

        public bool UpdatePaymentCapture(int paymentid, decimal tipamount, decimal amount, decimal netamount, string transactionid)
        {
            if (paymentid == 0) return false;

            ResetWebSyncDate(paymentid);

            string querystring = "";
            querystring = "Update  payment set transtype='POSTAUTH', tipamount = " + tipamount + ", amount =" + amount + ", netamount=" + netamount + ", responseid=" + transactionid + "  where id =" + paymentid;
            return _dbconnect.Command(querystring);
        }

        public bool UpdatePaymentCapture(int paymentid, decimal tipamount, decimal amount)
        {
            if (paymentid == 0) return false;

            ResetWebSyncDate(paymentid);

            string querystring = "";
            querystring = "Update  payment set transtype='POSTAUTH', tipamount = " + tipamount + ", amount =" + amount + "  where id =" + paymentid;
            return _dbconnect.Command(querystring);
        }


        public bool ResetWebSyncDate(int paymentid)
        {
            string querystring = "update sales inner join payment on sales.id = payment.salesid set websyncdate = null where payment.id = " + paymentid;
            return _dbconnect.Command(querystring);
        }
    }
}
