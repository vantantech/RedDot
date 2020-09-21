using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.DataManager
{
    public class DBHistory
    {

        private DBConnect dbConnect;
        public DBHistory()
        {

            dbConnect = new DBConnect();

 
        }


        //exclude payments that are VOID
        public DataTable GetOrdersByID(int salesid)
        {
            string query = "Select  sales.id ,sales.websyncdate, sales.saledate, sales.status, group_concat(if(iscredit,concat(displayname,'=',coalesce(amount,'[empty]')), displayname)) as tip, sales.total, (sales.total - coalesce(pay.netamount,0)) as balance, pay.paymenttype, customer.phone1 from sales" +
                     " left outer join (select distinct employeeid , salesid from salesitem ) as si on sales.id = si.salesid" +
                     " left outer join employee on si.employeeid = employee.id " +
                     " left outer join customer on sales.customerid = customer.id " +
                     " left outer join (select salesid,sum(if(void=0,netamount,0)) as netamount, group_concat(cardgroup) as paymenttype,sum(if(cardgroup ='Credit' or cardgroup ='CREDIT'  ,1, 0)) as iscredit  from payment group by salesid ) as pay on sales.id = pay.salesid " +
                     " left outer join  gratuity  on employee.id = gratuity.employeeid and sales.id = gratuity.salesid ";
                   
          query += " where sales.id=" + salesid + " and sales.status != 'Open'  order by saledate  ";




            return dbConnect.GetData(query);
        }
        //this function currently only gets all the employee link to services for salon, no products 

        public DataTable GetOrdersSalon(DateTime startdate, DateTime enddate, int employeeid, bool simple)
        {

            string query = "";


            query += "Select 0 as selected, if(sales.status='Voided',0,sales.total) as total , sales.id ,if(pay.auth > 0,'AUTH','OTHER') as transtype, sales.websyncdate, sales.saledate,sales.status, group_concat(if(credittotal > 0 ,concat(displayname,'=',coalesce(amount,'[empty]')), displayname)) as tip, (if(sales.status='Voided',0,sales.total) - coalesce(pay.netamount,0)) as balance , pay.paymenttype, pay.cashtotal, pay.credittotal,pay.tipamount, customer.phone1 from sales" +
                     " left outer join (select distinct employeeid , salesid from salesitem ) as si on sales.id = si.salesid" +
                     " left outer join employee on si.employeeid = employee.id " +
                     " left outer join customer on sales.customerid = customer.id " +
                     " left outer join (select salesid,sum(tipamount) as tipamount,  sum(if(UPPER(cardgroup) = 'CASH',netamount, 0)) as cashtotal,sum(if(void=0,netamount,0)) as netamount, group_concat(cardgroup) as paymenttype,sum(if(transtype='AUTH',1,0)) as auth, sum(if(UPPER(cardgroup) ='CREDIT'  ,netamount, 0)) as credittotal from payment group by salesid ) as pay on sales.id = pay.salesid " +
                     " left outer join  gratuity  on employee.id = gratuity.employeeid and sales.id = gratuity.salesid ";
            if(simple)
            {
                query = "Select 0 as selected, if(sales.status='Voided',0,sales.total) as total , sales.id ,'none' as transtype, sales.websyncdate, sales.saledate,sales.status, 0 as tip, 0 as balance , 'none' as paymenttype, 0 as cashtotal, 0 as credittotal,0 tipamount, '' as phone1 from sales";

            }

            //filter
            query += " where DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and (sales.status = 'Closed' or sales.status='Voided')";



            if (employeeid > 0) query += " and sales.employeeid = " + employeeid;

            //group by        
            query += " group by sales.id order by saledate desc ";




            return dbConnect.GetData(query);
        }

        public DataTable GetAUTHPayments(DateTime startdate, DateTime enddate)
        {

            string query = "";


            query += "Select payment.* from sales" +
                     " inner join payment on sales.id = payment.salesid ";

            //filter
            query += " where DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and sales.status = 'Closed' and payment.transtype = 'AUTH' ";






            return dbConnect.GetData(query);
        }

        public DataTable GetReversedTicketsSalon()
        {


            string query = "Select 0 as selected, sales.id, sales.websyncdate, sales.saledate, sales.status, group_concat(if(iscredit ,concat(displayname,'=',coalesce(amount,'[empty]')), displayname)) as tip, sales.total,(sales.total - coalesce(pay.netamount,0)) as balance, pay.paymenttype, customer.phone1 from sales" +
                      " left outer join (select distinct employeeid , salesid from salesitem ) as si on sales.id = si.salesid" +
                      " left outer join employee on si.employeeid = employee.id " +
                      " left outer join customer on sales.customerid = customer.id " +
                      " left outer join (select salesid,sum(if(void=0,netamount,0)) as netamount, group_concat(cardgroup) as paymenttype,sum(if(cardgroup ='Credit' or cardgroup ='Visa' or cardgroup ='Mastercard' or cardgroup ='Discover' or cardgroup ='American Express' ,1, 0)) as iscredit from payment group by salesid ) as pay on sales.id = pay.salesid " +
                      " left outer join  gratuity  on employee.id = gratuity.employeeid and sales.id = gratuity.salesid ";

         
                     query = query + " where  sales.status = 'Reversed' group by sales.id order by saledate ";




            return dbConnect.GetData(query);
        }

        public DataTable GetSyncList(DateTime startdate, DateTime enddate)
        {
            string query = "Select * from sales where websyncdate is null and ( DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "') and (sales.status = 'Closed' or sales.status='Voided') " +
            " order by saledate ";

            return dbConnect.GetData(query);
        }

        public void UpdateSyncDate(int salesid)
        {
            string query = "Update sales set websyncdate = NOW() where id=" + salesid;
            dbConnect.Command(query);
        }

    }
}
