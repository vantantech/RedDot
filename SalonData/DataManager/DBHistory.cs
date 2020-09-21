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


            string query = "Select  sales.id , sales.saledate, sales.status, group_concat(if(iscredit,concat(displayname,'=',coalesce(amount,'[empty]')), displayname)) as tip, sales.total, (sales.total - coalesce(pay.netamount,0)) as balance, pay.paymenttype, customer.phone1 from sales" +
                     " left outer join (select distinct employeeid , salesid from salesitem ) as si on sales.id = si.salesid" +
                     " left outer join employee on si.employeeid = employee.id " +
                     " left outer join customer on sales.customerid = customer.id " +
                     " left outer join (select salesid,sum(netamount) as netamount, group_concat(description) as paymenttype,sum(if(description ='Credit' or description ='Visa' or description ='Mastercard' or description ='Discover' or description ='American Express' ,1, 0)) as iscredit  from payment group by salesid ) as pay on sales.id = pay.salesid " +
                     " left outer join  gratuity  on employee.id = gratuity.employeeid and sales.id = gratuity.salesid ";
                   
          query = query +  " where sales.id=" + salesid + " and sales.status != 'Open'  order by saledate  ";




            return dbConnect.GetData(query, "Table");
        }
        //this function currently only gets all the employee link to services for salon, no products 

        public DataTable GetOrdersSalon(DateTime startdate, DateTime enddate)
        {


            string query = "Select  sales.id ,sales.saledate,sales.status, group_concat(if(iscredit ,concat(displayname,'=',coalesce(amount,'[empty]')), displayname)) as tip, sales.total, (sales.total - coalesce(pay.netamount,0)) as balance , pay.paymenttype, customer.phone1 from sales" +
                     " left outer join (select distinct employeeid , salesid from salesitem ) as si on sales.id = si.salesid" +
                     " left outer join employee on si.employeeid = employee.id " +
                     " left outer join customer on sales.customerid = customer.id " +
                     " left outer join (select salesid,sum(netamount) as netamount, group_concat(description) as paymenttype,sum(if(description ='Credit' or description ='Visa' or description ='Mastercard' or description ='Discover' or description ='American Express' ,1, 0)) as iscredit from payment group by salesid ) as pay on sales.id = pay.salesid " +
                     " left outer join  gratuity  on employee.id = gratuity.employeeid and sales.id = gratuity.salesid ";

                    query = query + " where DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and (sales.status = 'Closed' or sales.status='Voided') " +
                     " group by sales.id " +
                    " order by saledate ";




            return dbConnect.GetData(query, "Table");
        }

        public DataTable GetReversedTicketsSalon()
        {


            string query = "Select  sales.id ,sales.saledate, sales.status, group_concat(if(iscredit ,concat(displayname,'=',coalesce(amount,'[empty]')), displayname)) as tip, sales.total, pay.paymenttype, customer.phone1 from sales" +
                      " left outer join (select distinct employeeid , salesid from salesitem ) as si on sales.id = si.salesid" +
                      " left outer join employee on si.employeeid = employee.id " +
                      " left outer join customer on sales.customerid = customer.id " +
                      " left outer join (select salesid, group_concat(description) as paymenttype,sum(if(description ='Credit' or description ='Visa' or description ='Mastercard' or description ='Discover' or description ='American Express' ,1, 0)) as iscredit from payment group by salesid ) as pay on sales.id = pay.salesid " +
                      " left outer join  gratuity  on employee.id = gratuity.employeeid and sales.id = gratuity.salesid ";

         
                     query = query + " where  sales.status = 'Reversed' group by sales.id order by saledate ";




            return dbConnect.GetData(query, "Table");
        }

      

    }
}
