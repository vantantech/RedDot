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

           return _dbconnect.GetData("Select  sales.*, concat(firstname,' ',lastname) as displayname, 0 as selected from sales left outer join customer on sales.customerid = customer.id where status='Open' order by  sales.id asc  ", "sales");

       }



       public DataTable GetClosedSalesbyDates(DateTime startdate, DateTime enddate)
       {

           return _dbconnect.GetData("Select  * from sales where status='Closed' and  ( DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "')", "sales");

       }

       public DataTable GetSalonSalesCount(int employeeid)
       {

           return _dbconnect.GetData("Select  count(*) as count from salesitem where salesitem.employeeid=" + employeeid, "sales");

       }

    }
}
