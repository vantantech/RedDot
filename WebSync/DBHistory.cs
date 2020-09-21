using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSync
{
    public class DBHistory
    {

        private DBConnect dbConnect;
        public DBHistory()
        {

            dbConnect = new DBConnect();

 
        }


        //this function currently only gets all the employee link to services for salon, no products 

        public DataTable GetOrdersSalon(DateTime startdate, DateTime enddate)
        {


            string query = "Select 0 as selected,  sales.id , sales.websyncdate, sales.saledate,sales.status, sales.total  from sales" +
              
                   " where DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and (sales.status = 'Closed' or sales.status='Voided') " +
                     " group by sales.id " +
                    " order by saledate ";




            return dbConnect.GetData(query, "Table");
        }

        public DataTable GetReversedTicketsSalon()
        {


            string query = "Select  sales.id, sales.websyncdate, sales.saledate, sales.status,  sales.total from sales" +
          
         
                     " where  sales.status = 'Reversed' group by sales.id order by saledate ";




            return dbConnect.GetData(query, "Table");
        }

        public DataTable GetSyncList(DateTime startdate, DateTime enddate)
        {
            string query = "Select * from sales where websyncdate is null and ( DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "') and (sales.status = 'Closed' or sales.status='Voided') " +
            " order by saledate ";

            return dbConnect.GetData(query, "Table");
        }


        public DataTable GetAutoSyncList()
        {
            string query = "Select * from sales where websyncdate is null and (sales.status = 'Closed' or sales.status='Voided') order by saledate limit 100 ";

            return dbConnect.GetData(query, "Table");
        }


        public void UpdateSyncDate(int salesid)
        {
            string query = "Update sales set websyncdate = NOW() where id=" + salesid;
            dbConnect.Command(query);
        }

    }
}
