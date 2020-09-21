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

       public DataTable GetOpenSalesbyEmployee(int employeeid, bool haspayment)
       {
           string query = "Select * from (Select  sales.*, concat(firstname,' ',lastname) as displayname ,sum(coalesce(payment.netamount,0)) as totalpayment from sales" 
               + " left outer join payment on sales.id = payment.salesid "
              + " left outer join customer on sales.customerid = customer.id where (status='Open' or status='Pending') and employeeid=" + employeeid + " group by sales.id ) as result where totalpayment > 0";
           
           if(haspayment)

               return _dbconnect.GetData(query, "sales");
           else
               return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where  (status='Open' or status='Pending') and employeeid=" + employeeid, "sales");

       }
       public DataTable GetOpenSalesbyCustomer(int customerid, bool haspayment)
       {
           string query = "Select * from (Select  sales.*, concat(firstname,' ',lastname) as displayname ,sum(coalesce(payment.netamount,0)) as totalpayment from sales"
              + " left outer join payment on sales.id = payment.salesid "
                 + " left outer join customer on sales.customerid = customer.id where (status='Open' or status='Pending') and customerid=" + customerid + "group by sales.id) as result where totalpayment > 0";
           if (haspayment)

               return _dbconnect.GetData(query, "sales");
           else
               return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where (status='Open' or status='Pending') and customerid=" + customerid, "sales");

       }

       public DataTable GetOpenSalesbyTicket(int id)
       {

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where (status='Open' or status='Pending') and sales.id=" + id, "sales");

       }

       public DataTable GetOpenSalesbyDates(DateTime startdate , DateTime enddate, bool haspayment)
       {
           string query = "Select * from (Select  sales.*, concat(firstname,' ',lastname) as displayname ,sum(coalesce(payment.netamount,0)) as totalpayment from sales"
             + " left outer join payment on sales.id = payment.salesid "
              + "  left outer join customer on sales.customerid = customer.id where (status='Open' or status='Pending') and   DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by sales.id) as result where totalpayment > 0";

           if (haspayment)

               return _dbconnect.GetData(query, "sales");
           else
               return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where (status='Open' or status='Pending') and   DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'", "sales");

       }
       public DataTable GetAllOpenSales(bool haspayment)
       {
           string query = "Select * from (Select  sales.*, concat(firstname,' ',lastname) as displayname ,sum(coalesce(payment.netamount,0)) as totalpayment from sales"
          + " left outer join payment on sales.id = payment.salesid "
           + "  left outer join customer on sales.customerid = customer.id where (status='Open' or status='Pending')   group by sales.id  order by  sales.id asc) as result where totalpayment > 0";

           if (haspayment)

               return _dbconnect.GetData(query, "sales");
           else
               return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname from sales left outer join customer on sales.customerid = customer.id where (status='Open' or status='Pending') order by  sales.id asc  ", "sales");

       }

   

       public DataTable GetClosedSalesbyDates(DateTime startdate, DateTime enddate)
       {

           return _dbconnect.GetData("Select  * from sales where status='Closed' and  ( DATE(closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "')", "sales");

       }

        public DataTable GetSalesCount(int employeeid)
        {

            return _dbconnect.GetData("Select  count(*) as count from sales where employeeid=" + employeeid);

        }

    }
}
