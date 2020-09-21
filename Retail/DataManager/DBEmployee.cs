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

        public DataTable GetEmployeeOnSchedule()
        {
            DataTable tbl = new DataTable();
            tbl = m_dbconnect.GetData("Select * from employee where active=1 and onschedule = 1 order by displayname", "employee");
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

        public void SaveTimeRecord(int recid, string timein, string timeout , string note)
        {

            DateTime lcTimeIn = Convert.ToDateTime(timein);

            DateTime lcTimeOut = Convert.ToDateTime(timeout);


            string query = "update timeinout set timein='" + lcTimeIn.ToString("yyyy-MM-dd HH:mm:ss") + "', timeout='" + lcTimeOut.ToString("yyyy-MM-dd HH:mm:ss") + "' , note='" + note + "' where id=" + recid;
            m_dbconnect.Command(query);
        }


        public void InsertTimeRecord(int employeeid, string timein)
        {

            DateTime lcTimeIn = Convert.ToDateTime(timein);

            string query = "INSERT INTO timeinout (employeeid, timein,timeout,note) VALUES('" + employeeid + "', '" + lcTimeIn.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + lcTimeIn.ToString("yyyy-MM-dd HH:mm:ss") + "', 'added manually')";
            m_dbconnect.Command(query);
        }

        public void InsertTimeIn(int employeeid, DateTime time)
        {
            string query = "INSERT INTO timeinout (employeeid, timein) VALUES('" + employeeid + "', '" + time.ToString("yyyy-MM-dd HH:mm:ss") + "')";
            m_dbconnect.Command(query);

        }
        public void DeleteTimeRecord(int id)
        {
            string query = "Delete from timeinout where id = " + id;
            m_dbconnect.Command(query);
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

   



        public void UpdateTimeOut(int timeid, DateTime time)
        {
            string query = "UPDATE timeinout SET TimeOut ='" + time.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE id=" + timeid;
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

        public bool UpdateBoolean(int id, string field, bool setting)
        {
            string query;

            query = "update employee set " + field + " = " + (setting ? "1" : "0") + " where id=" + id;



            return m_dbconnect.Command(query);

        }
        public bool UpdateOnSchedule(int id, bool setting)
        {
            string query;
            if (setting == true)
            {
                query = "update employee set onschedule =1 where id=" + id;

            }
            else
            {
                query = "update employee set onschedule =0 where id=" + id;

            }

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
