using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBEmployee
    {

        DBConnect m_dbconnect;

        public DBEmployee()
        {
            m_dbconnect = new DBConnect();
        }

        public DataTable GetEmployeeAll()
        {
            DataTable tbl = new DataTable();
            tbl = m_dbconnect.GetData("Select employee.*, id as employeeid from employee order by displayname", "employee");
            return tbl;

        }

        public DataTable GetEmployeeActive(string role="")
        {
            DataTable tbl = new DataTable();

            string filter = "";

            if (role != "") filter = " and role='" + role + "' ";

            string query = "Select employee.*, id as employeeid from employee where active=1 " + filter + " order by displayname";


            tbl = m_dbconnect.GetData(query);
            return tbl;

        }

        public DataTable GetEmployeeInactive()
        {
            DataTable tbl = new DataTable();
            tbl = m_dbconnect.GetData("Select employee.*, id as employeeid from employee where active=0", "employee");
            return tbl;

        }

        public DataTable GetEmployee(int id)
        {

            return m_dbconnect.GetData("select employee.*, id as employeeid from employee where id=" + id.ToString(), "Employee");
        }

        public DataTable GetCashDrawer(int stationno, int employeeid)
        {
            string sqlstr;
            sqlstr = "select * from cashdrawer where cashoutdate is null and  stationno=" + stationno + " and employeeid=" + employeeid ;
            return m_dbconnect.GetData(sqlstr);
        }

        public DataTable GetEmployeeMeals(int employeeid, DateTime startdate, DateTime enddate)
        {


            string query = " select * from sales inner join salesitem on sales.id = salesitem.salesid   where status != 'Voided' and   (sales.adjustmenttype = 'EMPLOYEE MEAL' or salesitem.discounttype = 'EMPLOYEE MEAL') and  employeeid = " + employeeid + " and   saledate between '" + startdate.ToString("yyyy-MM-dd H:mm:ss") + "' and '" + enddate.ToString("yyyy-MM-dd H:mm:ss") + "'";
            return m_dbconnect.GetData(query);
        }

        public DataTable GetEmployeeHours(int employeeid, DateTime startdate , DateTime enddate )
        {


            string query = " select firstname, lastname, timeinout.*, ROUND(TIMESTAMPDIFF(MINUTE,timein,timeout)/60,2) as hours from timeinout inner join employee on timeinout.employeeid = employee.id   where employeeid = " + employeeid + " and   DATE(timein) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' order by timein";
            return m_dbconnect.GetData(query);
        }

        public DataTable GetEmployeeSales(int employeeid, DateTime startdate, DateTime enddate, bool summary=false)
        {


            string query = " select 1 as totalticket,sales.id, saledate, sales.total, sales.autotip,   sum(payment.tipamount) as tipamount," +
                            " sum(if (cardgroup = 'CREDIT',netamount,0)) as credit ," +
                            " sum(if (cardgroup = 'DEBIT',netamount,0)) as debit ," +
                            " sum(if (cardgroup = 'CASH',netamount,0)) as cash," +
                            " sum(if (cardgroup != 'CASH' && cardgroup != 'CREDIT' && cardgroup != 'DEBIT',netamount,0)) as other  from sales " + 
                            " inner join payment on sales.id = payment.salesid " + 
                            " where employeeid = " + employeeid + " and   DATE(saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'" +
                            " group by sales.id order by saledate";

            string finalquery = query;

            if(summary)  finalquery = "select sum(totalticket) as totalticket, DATE(saledate) as saledate,sum(total) as total,sum(autotip) as autotip,sum(tipamount) as tipamount," +
                   " sum(credit) as credit, sum(debit) as debit, sum(cash) as cash, sum(other) as other from (" + query + ") as report group by DATE(saledate)";

            return m_dbconnect.GetData(finalquery);
        }


        /*
        public DataTable GetEmployeeSalesSummary(int employeeid, DateTime startdate, DateTime enddate)
        {


           string query = " select count(sales.id) as totalticket, DATE(saledate) as saledate, sum(sales.total) as total, sum(tipamount) as tipamount," +
                            " sum(if (cardgroup = 'CREDIT',netamount,0)) as credit ," +
                            " sum(if (cardgroup = 'CASH',netamount,0)) as cash," +
                            " sum(if (cardgroup != 'CASH' && cardgroup != 'CREDIT',netamount,0)) as other  from sales " +
                            " inner join payment on sales.id = payment.salesid " +
                            " where employeeid = " + employeeid + " and   DATE(saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'" +
                            " group by  DATE(saledate) order by saledate";
                            

            return m_dbconnect.GetData(query);
        }


        */




        public decimal GetTotalHours(int employeeid, DateTime startdate, DateTime enddate)
        {

            DataTable dt;
            string query = " select sum( TIMESTAMPDIFF(MINUTE,timein,timeout)/60) as totalhours from timeinout   where employeeid = " + employeeid + " and   DATE(timein) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
            dt = m_dbconnect.GetData(query);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["totalhours"].ToString() != "") return decimal.Parse(dt.Rows[0]["totalhours"].ToString());
                else return 0;

            }
            else return 0;
        }


        public decimal GetTotalShiftHours(int employeeid, DateTime startdate, DateTime enddate)
        {

            DataTable dt;
            string query = " select sum( TIMESTAMPDIFF(MINUTE,timein,timeout)/60) as totalhours from timeinout   where employeeid = " + employeeid + " and  timein between '" + startdate.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + enddate.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            dt = m_dbconnect.GetData(query);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["totalhours"].ToString() != "") return decimal.Parse(dt.Rows[0]["totalhours"].ToString());
                else return 0;

            }
            else return 0;
        }


        //looks at how many people was working during that shift.  It should only look at people that are in the tip pool
        public decimal GetTotalWorkers(DateTime startdate, DateTime enddate)
        {
            DataTable dt;

            string query = " select employeeid, sum(TIMESTAMPDIFF(MINUTE,timein,timeout)/60) as totalhours from timeinout " +
                           " inner join employee on timeinout.employeeid = employee.id   where employee.intippool = 1 and   timein between '" + startdate.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + enddate.ToString("yyyy-MM-dd HH:mm:ss") + "' group by employeeid";

            string query2 = "select count(*) as cnt from (" + query + ") as result where totalhours >=1";
            dt = m_dbconnect.GetData(query2);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["cnt"].ToString() != "") return decimal.Parse(dt.Rows[0]["cnt"].ToString());
                else return 0;

            }
            else return 0;
        }

        public decimal GetShiftTips(DateTime startdate, DateTime enddate)
        {
            DataTable dt;

            string query = " select  (sum(pay.tipamount) + sales.autotip) as tipamount from sales " +
                 " inner join (select salesid,sum(payment.tipamount) as tipamount from  payment group by salesid ) as pay on sales.id = pay.salesid " +
               " where saledate between '" + startdate.ToString("yyyy-MM-dd HH:mm:ss") + "' and '" + enddate.ToString("yyyy-MM-dd HH:mm:ss") + "' ";

            dt = m_dbconnect.GetData(query);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["tipamount"].ToString() != "") return decimal.Parse(dt.Rows[0]["tipamount"].ToString());
                else return 0;

            }
            else return 0;
        }

        public DataTable GetLastTimeRecord(int id)
        {
            return m_dbconnect.GetData("Select  * from timeinout where employeeid = " + id + " order by id desc limit 1 ");
        }

        public void InsertTimeIn(int employeeid, DateTime time)
        {
            string query = "INSERT INTO TimeInOut (employeeid, timein) VALUES('" + employeeid + "', '" + time.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            m_dbconnect.Command(query);

        }

        public void InsertTimeRecord(int employeeid, string timein)
        {

            DateTime lcTimeIn = Convert.ToDateTime(timein);

            string query = "INSERT INTO TimeInOut (employeeid, timein,timeout,note) VALUES('" + employeeid + "', '" + lcTimeIn.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + lcTimeIn.ToString("yyyy-MM-dd HH:mm:ss") + "', 'added manually')";
            m_dbconnect.Command(query);
        }

        public void DeleteTimeRecord(int id)
        {
            string query = "Delete from timeinout where id = " + id;
            m_dbconnect.Command(query);
        }


        public DataRow GetTimeRecord(int recid)
        {
            string query = "select * from timeinout where id=" + recid;
            var table = m_dbconnect.GetData(query);

            if (table != null)
            {
                return table.Rows[0];
            }
            else return null;
        }

        public void SaveTimeRecord(int recid, string timein, string timeout, string note)
        {

            DateTime lcTimeIn = Convert.ToDateTime(timein);

            DateTime lcTimeOut = Convert.ToDateTime(timeout);


            string query = "update timeinout set timein='" + lcTimeIn.ToString("yyyy-MM-dd HH:mm:ss") + "', timeout='" + lcTimeOut.ToString("yyyy-MM-dd HH:mm:ss") + "' , note='" + note + "' where id=" + recid;
            m_dbconnect.Command(query);
        }

        public void UpdateTimeOut(int timeid, DateTime time)
        {
            string query = "UPDATE TimeInOut SET TimeOut ='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id=" + timeid;
            m_dbconnect.Command(query);

        }
        public int AddEmployee(int pin)
        {
            string query;
            query = "Insert into employee ( active, securitylevel,displayname , pin,imagesrc) values (1,10,'new employee'," + pin.ToString() + ",'images/employee.jpg')";
            m_dbconnect.Command(query);


            query = "select max(id) as id from employee where displayname ='new employee' ";
            var tab = m_dbconnect.GetData(query);
            if (tab.Rows.Count > 0)
                return int.Parse(tab.Rows[0]["id"].ToString());
            else return 0;

        }
        //only allow delete if employee has no history/records
        public bool DeleteEmployee(int employeeid)
        {
            string query;
         
         

                query = "delete from employee where id=" + employeeid;
                return m_dbconnect.Command(query);
         


        }

        public bool CashierIn(int stationno, int employeeid, int cashin100,int cashin50, int cashin20, int cashin10, int cashin5, int cashin2, int cashin1, int cashin50cent, int cashin25cent, int cashin10cent, int cashin5cent, int cashin1cent, decimal cashintotal)
        {
            string query = "insert into cashdrawer (stationno,employeeid,cashin100,cashin50,cashin20,cashin10,cashin5,cashin2,cashin1,cashin50cent,cashin25cent,cashin10cent,cashin5cent,cashin1cent,cashintotal) values ";
            query += " (" + stationno + "," + employeeid + "," + cashin100 + "," + cashin50 + "," + cashin20 + "," + cashin10 + "," + cashin5 + "," + cashin2 + "," + cashin1 + "," + cashin50cent + "," + cashin25cent + "," + cashin10cent + "," + cashin5cent + "," + cashin1cent + "," + cashintotal + ")";
            return m_dbconnect.Command(query);
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


            return m_dbconnect.Command(query);
        }

        public bool UpdateBoolean(int id,string field, bool setting)
        {
            string query;
        
            query = "update employee set " + field + " = " + (setting?"1":"0") + " where id=" + id;

         

            return m_dbconnect.Command(query);

        }

        public bool UpdateNumeric (int id, string field, int fieldvalue)
        {
            string query;

            query = "update employee set " + field + " =" + fieldvalue + " where id=" + id;
           return  m_dbconnect.Command(query);
        }

        public bool UpdateNumeric(int id, string field, decimal fieldvalue)
        {
            string query;

            query = "update employee set " + field + " =" + fieldvalue + " where id=" + id;
           return  m_dbconnect.Command(query);
        }
        public bool UpdateString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update employee set " + field + " ='" + fieldvalue + "' where id=" + id;
           return m_dbconnect.Command(query);

        }

    }
}
