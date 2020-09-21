using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBHistory
    {

        private DBConnect dbConnect;
        public DBHistory()
        {

            dbConnect = new DBConnect();

 
        }

        public DataTable GetOrdersByID(int salesid)
        {


            string query = "Select  sales.* , TIME(sales.closedate) as time,(sales.total - coalesce(pay.netamount,0)) as balance, pay.paymenttype, customer.phone1 from sales" +
                         " left outer join customer on sales.customerid = customer.id " +
                     " left outer join (select salesid,sum(netamount) as netamount,group_concat(description) as paymenttype from payment group by salesid ) as pay on sales.id = pay.salesid " +
                     " where sales.id=" + salesid + " and sales.status != 'Open'   " +
                    " order by closedate desc ";




            return dbConnect.GetData(query, "Table");
        }


   

 

        /// <summary>
        ///                                                                         RETAIL
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public DataTable GetClosedTicketsRetail(DateTime startdate, DateTime enddate)
        {


            string query = "Select  sales.*, workorder.id as workordernumber, TIME(sales.closedate) as time,(sales.total - coalesce(pay.netamount,0)) as balance , pay.paymenttype, customer.phone1, concat(customer.firstname,' ' ,customer.lastname) as customername, displayname from sales" +
                      " left outer join employee on sales.employeeid = employee.id " +
                     " left outer join customer on sales.customerid = customer.id " +
                    " left outer join workorder on sales.id = workorder.salesid " +
                     " left outer join (select salesid,sum(netamount) as netamount, group_concat(description) as paymenttype from payment where payment.void =0 group by salesid ) as pay on sales.id = pay.salesid " +
                     " where  DATE(sales.closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and  sales.status = 'Closed'  or DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and  sales.status = 'Voided'  " +
                    " order by saledate desc ";




            return dbConnect.GetData(query, "Table");
        }



        public DataTable GetReversedTicketsRetail()
        {


            string query = "Select  sales.*, workorder.id as workordernumber,TIME(sales.closedate) as time,(sales.total - coalesce(pay.netamount,0)) as balance , pay.paymenttype, customer.phone1, concat(customer.firstname,' ' ,customer.lastname) as customername, displayname from sales" +
                      " left outer join employee on sales.employeeid = employee.id " +
                     " left outer join customer on sales.customerid = customer.id " +
                      " left outer join workorder on sales.id = workorder.salesid " +
                     " left outer join (select salesid,sum(netamount) as netamount, group_concat(description) as paymenttype from payment where payment.void =0 group by salesid ) as pay on sales.id = pay.salesid " +
                    " where sales.status = 'Reversed' " +
                   " order by saledate desc ";




            return dbConnect.GetData(query, "Table");
        }


        public DataTable GetOpenTicketsRetail()
        {


            string query = "Select  sales.*, workorder.id as workordernumber,TIME(sales.closedate) as time,sum(salesitem.price * salesitem.quantity) as salestotal,(sum(salesitem.price * salesitem.quantity) - coalesce(pay.netamount,0)) as balance , pay.paymenttype, customer.phone1, concat(customer.firstname,' ' ,customer.lastname) as customername, displayname from sales" +
                      " left outer join employee on sales.employeeid = employee.id " +
                     " left outer join customer on sales.customerid = customer.id " +
                      " left outer join workorder on sales.id = workorder.salesid " +
                     " left outer join (select salesid,sum(netamount) as netamount, group_concat(description) as paymenttype from payment where payment.void =0 group by salesid ) as pay on sales.id = pay.salesid " +
                    " inner join salesitem on sales.id = salesitem.salesid " +
                     " where sales.status = 'Open' and pay.netamount > 0 group by .sales.id " +
                   " order by saledate desc ";




            return dbConnect.GetData(query, "Table");
        }


        public DataTable GetQuotes()
        {


            string query = "Select  sales.*, workorder.id as workordernumber,TIME(sales.closedate) as time,sum(salesitem.price * salesitem.quantity) as salestotal,(sum(salesitem.price * salesitem.quantity) - coalesce(pay.netamount,0)) as balance , pay.paymenttype, customer.phone1, concat(customer.firstname,' ' ,customer.lastname) as customername, displayname from sales" +
                      " left outer join employee on sales.employeeid = employee.id " +
                     " left outer join customer on sales.customerid = customer.id " +
                      " left outer join workorder on sales.id = workorder.salesid " +
                     " left outer join (select salesid,sum(netamount) as netamount, group_concat(description) as paymenttype from payment where payment.void =0 group by salesid ) as pay on sales.id = pay.salesid " +
                    " inner join salesitem on sales.id = salesitem.salesid " +
                     " where sales.status = 'Open' and pay.netamount is null group by .sales.id " +
                   " order by saledate desc ";




            return dbConnect.GetData(query, "Table");
        }
    }
}
