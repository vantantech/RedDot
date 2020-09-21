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
            tbl = m_dbconnect.GetData("Select * from employee order by displayname", "employee");
            return tbl;

        }

        public DataTable GetEmployeeActive()
        {
            DataTable tbl = new DataTable();
            tbl = m_dbconnect.GetData("Select * from employee where active=1 order by displayname", "employee");
            return tbl;

        }

        public DataTable GetEmployeeInactive()
        {
            DataTable tbl = new DataTable();
            tbl = m_dbconnect.GetData("Select * from employee where active=0", "employee");
            return tbl;

        }

        public DataTable GetEmployee(int id)
        {

            return m_dbconnect.GetData("select * from employee where id=" + id.ToString(), "Employee");
        }

        public DataTable GetEmployeeHours(int employeeid, DateTime startdate , DateTime enddate )
        {


            string query = " select *, TIMESTAMPDIFF(MINUTE,timein,timeout)/60 as hours from timeinout   where employeeid = " + employeeid + " and   DATE(timein) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' order by timein";
            return m_dbconnect.GetData(query);
        }

        public decimal GetTotalHours(int employeeid, DateTime startdate, DateTime enddate)
        {

            DataTable dt;
            string query = " select sum( TIMESTAMPDIFF(MINUTE,timein,timeout)/60) as totalhours from timeinout   where employeeid = " + employeeid + " and   DATE(timein) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
             dt =m_dbconnect.GetData(query);
             if (dt.Rows.Count > 0)
             {
                 if (dt.Rows[0]["totalhours"].ToString() != "") return decimal.Parse(dt.Rows[0]["totalhours"].ToString());
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

        public void InsertCheckIn(int employeeid, DateTime time)
        {
            int max=0;
            max = GetMaxCheckIn(time) + 1;

            string query = "INSERT INTO Checkin(employeeid, checkindate,turn) VALUES (" + employeeid + ", '" + time.ToString("yyyy-MM-dd HH:mm:ss") + "'," + max + ")";
            m_dbconnect.Command(query);

        }

        public void MoveCheckInToBottom(int employeeid, DateTime time)
        {
            int max = 0;
            max = GetMaxCheckIn(time) + 1;

            UpdateCheckIn(employeeid, time, max);

        }

        public void MoveCheckInToTop(int employeeid, DateTime time)
        {
            int min = 0;
            min = GetMinCheckIn(time) - 1;

            UpdateCheckIn(employeeid, time, min);

        }

        public void UpdateCheckIn(int employeeid, DateTime time, int turn)
        {


            string query = "update  Checkin set turn= " + turn + " where employeeid = " + employeeid + " and DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "'";
            m_dbconnect.Command(query);

        }

        public void IncrementTurnCount(int employeeid, DateTime time)
        {


            string query = "update  Checkin set turncount = coalesce(turncount,0) + 1 where employeeid = " + employeeid + " and DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "'";
            m_dbconnect.Command(query);

        }
        public void UpdatePartialTurn(int employeeid, DateTime time, decimal partialturn)
        {


            string query = "update  Checkin set partialturn  = " + partialturn + " where employeeid = " + employeeid + " and DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "'";
            m_dbconnect.Command(query);

        }
        public void DeleteCheckIn(int employeeid, DateTime time)
        {


            string query = "Delete from Checkin  where employeeid = " + employeeid + " and DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "'";
            m_dbconnect.Command(query);

        }
        public DataTable GetCheckIn(int employeeid, DateTime time)
        {
            return m_dbconnect.GetData("Select  * from checkin where employeeid= " + employeeid + " and DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "'");
        }

        public DataTable GetCheckInList( DateTime today,bool alphaorder=false)
        {
           // string querystring = "Select  checkin.*, employee.displayname, 0 as selected from checkin inner join employee on checkin.employeeid = employee.id where  DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "' order by turn";

            string querystring = "Select  checkin.ID, checkin.employeeid, checkin.checkindate,coalesce(checkin.partialturn ,0) as partialturn, checkin.turn,coalesce(checkin.partialturn ,0) + coalesce(checkin.turncount ,0) as turncount, employee.displayname, concat('pack://siteoforigin:,,,/',employee.imagesrc) as imagesrc, 0 as selected , earning.totalsales from checkin " + 
                                    " inner join employee on checkin.employeeid = employee.id " +
                                    " left outer join (select  sum(salesitem.price) as totalsales, salesitem.employeeid  from salesitem  inner join sales on sales.id = salesitem.salesid where DATE(sales.saledate) = '" + today.ToString("yyyy-MM-dd") + "' and sales.status='Closed' group by salesitem.employeeid) as earning " +
                                    " on checkin.employeeid = earning.employeeid " +
                                    " where  DATE(checkindate) = '" + today.ToString("yyyy-MM-dd") + "' " + ( alphaorder? " order by employee.displayname   " : " order by turn asc");
            
            
            return m_dbconnect.GetData(querystring);
        }

        public DataTable GetCheckOutList(DateTime today)
        {
            // string querystring = "Select  checkin.*, employee.displayname, 0 as selected from checkin inner join employee on checkin.employeeid = employee.id where  DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "' order by turn";

            string querystring = "select concat('pack://siteoforigin:,,,/', imagesrc) as imageurl ,employee.* from employee where id not in (  Select checkin.employeeid from checkin where  DATE(checkindate) = '" + today.ToString("yyyy-MM-dd") + "') and employee.active = 1";


            return m_dbconnect.GetData(querystring);
        }

        public int GetMaxCheckIn(DateTime time)
        {
            string query = "Select  coalesce(max(turn),0) as max from checkin where  DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "'";
           DataTable dt = m_dbconnect.GetData(query);
           if (dt.Rows.Count > 0)
           {
               if (dt.Rows[0]["max"] != null) return int.Parse(dt.Rows[0]["max"].ToString());
               else return 0;
           }
           else return 0;

        }

        public int GetMinCheckIn(DateTime time)
        {
            string query = "Select  coalesce(min(turn),0) as min from checkin where  DATE(checkindate) = '" + time.ToString("yyyy-MM-dd") + "'";
            DataTable dt = m_dbconnect.GetData(query);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["min"] != null) return int.Parse(dt.Rows[0]["min"].ToString());
                else return 0;
            }
            else return 0;

        }
        public void UpdateTimeOut(int timeid, DateTime time)
        {
            string query = "UPDATE TimeInOut SET TimeOut ='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id=" + timeid;
            m_dbconnect.Command(query);

        }
        public void AddEmployee(int pin)
        {
            string query;
            query = "Insert into employee ( active, securitylevel,displayname , pin,imagesrc) values (1,0,'new employee'," + pin.ToString() + ",'images/employee.jpg')";
            m_dbconnect.Command(query);

        }
        //only allow delete if employee has no history/records
        public bool DeleteEmployee(int employeeid)
        {
            string query;
         
         

                query = "delete from employee where id=" + employeeid;
                return m_dbconnect.Command(query);
         


        }


        public bool UpdateActive(int id, bool setting)
        {
            string query;
            if (setting == true)
            {
                query = "update employee set active =1 where id=" + id;

            }
            else
            {
                query = "update employee set active =0 where id=" + id;

            }

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
