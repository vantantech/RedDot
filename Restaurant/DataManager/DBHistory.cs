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

  
    

        /// <summary>
        ///                                                                       QUICK SERVICE
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>

        public DataTable GetClosedVoidedOrders(DateTime startdate, DateTime enddate, int employeeid, string transtype, bool showcancelledtickets)
        {
            string paymentquery = "select salesid,sum(if(void=0,netamount,0)) as netamount,sum(if(void=0,tipamount,0)) as tip, group_concat(concat(cardgroup ,'-', transtype) ) as paymenttype,sum(if((transtype='AUTH' && void=0),1,0)) as auth from payment " +
                " inner join sales on sales.id = payment.salesid " +
                " where DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  and(sales.status = 'Closed' or sales.status = 'Voided')  group by salesid";


            string query = "Select 0 as selected, sales.id, sales.ordernumber ,if(pay.auth > 0,'AUTH','OTHER') as transtype, customer.phone1,  sales.saledate, TIME(sales.saledate) as time,sales.status, pay.tip, sales.total, (sales.total - coalesce(pay.netamount,0)) as balance, pay.paymenttype, employee.displayname from sales" +
                        " left outer join employee on sales.employeeid = employee.id " +
                        " left outer join customer on sales.customerid = customer.id " +
                        " left outer join (" + paymentquery +  ") as pay on sales.id = pay.salesid " +
                        " where DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and (sales.status = 'Closed' or sales.status = 'Voided') ";
                  

            if (employeeid > 0) query += " and sales.employeeid = " + employeeid;
            if (transtype == "AUTH") query += " and pay.auth > 0 ";
            if (transtype == "SETTLED") query += " and pay.auth =0 ";
            if (!showcancelledtickets) query += " and ordernumber > 0 ";


            query += " order by saledate desc ";

            return dbConnect.GetData(query, "Table");
        }




        public DataTable QSGetOpenOrdersDetail(DateTime startdate, DateTime enddate)
        {


            string query = "Select  sales.id , sales.ordernumber, sales.saledate, sales.saledate, TIME(sales.saledate) as time,sales.status, pay.tip, sales.total,  pay.paymenttype, employee.displayname from sales" +
                      " left outer join employee on sales.employeeid = employee.id " +
                     " left outer join (select salesid,sum(tipamount) as tip, group_concat(cardgroup) as paymenttype from payment group by salesid ) as pay on sales.id = pay.salesid " +
                     " where DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and sales.status = 'Open' " +
                     " order by saledate desc ";




            return dbConnect.GetData(query, "Table");
        }


        public DataTable QSGetOpenOrdersID(DateTime startdate, DateTime enddate)
        {


            string query = "Select  sales.id as salesid from sales " +
                     " where DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and sales.status = 'Open' " +
                     " order by saledate desc ";




            return dbConnect.GetData(query, "Table");
        }



        public DataTable QSGetOrdersByID(int salesid)
        {


            string query = "Select  sales.id ,sales.ordernumber, sales.saledate, sales.saledate, TIME(sales.saledate) as time,sales.status,pay.tip, sales.total, pay.paymenttype, employee.displayname from sales" +
                     " left outer join employee on sales.employeeid = employee.id " +
                     " left outer join (select salesid,sum(tipamount) as tip, group_concat(cardgroup) as paymenttype from payment group by salesid ) as pay on sales.id = pay.salesid " +
                     " where sales.id=" + salesid + " and sales.status != 'Open'  group by sales.id " +
                    " order by saledate desc ";




            return dbConnect.GetData(query, "Table");
        }

        public DataTable GetAUTHPayments(DateTime startdate, DateTime enddate)
        {

            string query = "";


            query += "Select payment.* from sales" +
                     " inner join payment on sales.id = payment.salesid ";

            //filter
            query += " where DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and sales.status = 'Closed' and payment.transtype = 'AUTH' and payment.void =0 ";

            return dbConnect.GetData(query);
        }

        public DataTable GetCreditDebitPayments(DateTime startdate, DateTime enddate)
        {

            string query = "";


            query += "Select sum(payment.netamount + coalesce(payment.tipamount,0)) as netamount from sales" +
                     " inner join payment on sales.id = payment.salesid ";

            //filter
            query += " where DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and sales.status = 'Closed' and ( cardgroup = 'CREDIT' or cardgroup = 'DEBIT') and payment.void =0 ";

            return dbConnect.GetData(query);
        }

//no longer used
/*
        public DataTable GetReversedTickets()
        {


            string query = "Select 0 as selected,  sales.id, sales.ordernumber ,customer.phone1, sales.saledate,sales.saledate, TIME(sales.saledate) as time,sales.status,pay.tip, sales.total , pay.paymenttype, employee.displayname from sales" +
                      " left outer join employee on sales.employeeid = employee.id " +
                        " left outer join customer on sales.customerid = customer.id " +
                     " left outer join (select salesid, sum(tipamount) as tip, group_concat(concat(cardgroup ,'-', transtype) ) as paymenttype from payment group by salesid ) as pay on sales.id = pay.salesid " +
                     " where sales.status = 'Reversed' " +
                    " order by saledate desc ";




            return dbConnect.GetData(query, "Table");
        }

    */


    }
}
