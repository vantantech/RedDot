using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBAppointment
    {
        DBConnect m_dbconnect;
        public DBAppointment()
        {

            m_dbconnect = new DBConnect();
        }
        public bool DeleteAppt( int appointmentid)
        {
            string query;

            query = "Delete from  appointment   where id=" + appointmentid;
            return m_dbconnect.Command(query);

        }
        public bool UpdateApptDate(DateTime apptdate, int appointmentid)
        {
            string query;

            query = "update appointment set apptdate = '" + apptdate.ToString("yyyy-MM-dd HH:mm") + "'  where id=" + appointmentid;
            return m_dbconnect.Command(query);

        }
       public bool UpdateEmployee(int employeeid, int appointmentid)
        {
            string query;

            query = "update appointment set employeeid = " + employeeid + "  where id=" + appointmentid;
            return m_dbconnect.Command(query);

        }


       public bool UpdateCustomer(int customerid, int appointmentid)
       {
           string query;

           query = "update appointment set customerid = " + customerid + "  where id=" + appointmentid;
           return m_dbconnect.Command(query);

       }

       public bool UpdateString(int id, string field, string fieldvalue)
       {
           string query;

           query = "update appointment set " + field + " ='" + fieldvalue + "' where id=" + id;
           return m_dbconnect.Command(query);

       }

       public bool UpdateNumeric(int id, string field, int fieldvalue)
       {
           string query;

           query = "update appointment set " + field + " =" + fieldvalue + " where id=" + id;
           return m_dbconnect.Command(query);
       }


        public bool CheckLength( DateTime apptdate, int employeeid, int length)
       {
           string query;
            DateTime newtime;
            newtime = apptdate.AddMinutes(length  -1);
           

            query = "select * from appointment where apptdate between '" + apptdate.AddMinutes(1).ToString("yyyy-MM-dd HH:mm") + "' and '" + newtime.ToString("yyyy-MM-dd HH:mm") + "' and employeeid=" + employeeid;
            DataTable dt = m_dbconnect.GetData(query);
            if(dt.Rows.Count == 0 )
            {
                return true;
            } return false;

       }


        /// <summary>
        /// Function: SpaceAvailable 
        /// 1) Need to check if requested space is occuppied by appointment that starts in that space
        /// 2) Also need to check if requested space is occuppied by appointment before that space that extends into that space
        /// </summary>
        /// <param name="currentapptid"></param>
        /// <param name="apptdate"></param>
        /// <param name="employeeid"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool SpaceAvailable(int currentapptid, DateTime apptdate, int employeeid, int length)
        {

            string query;
            DateTime newtime;
            DataTable dt;

            newtime = apptdate.AddMinutes(length - 1);

            //case 1
            query = "select * from appointment where apptdate between '" + apptdate.ToString("yyyy-MM-dd HH:mm") + "' and '" + newtime.ToString("yyyy-MM-dd HH:mm") + "' and employeeid=" + employeeid + " and id != " + currentapptid;
             dt = m_dbconnect.GetData(query);
             if (dt.Rows.Count == 0)
             {
                 //case 2
                 query = "select * from appointment where  '" + apptdate.ToString("yyyy-MM-dd HH:mm") + "' between apptdate and DATE_ADD(apptdate,INTERVAL length-1 MINUTE)   and employeeid=" + employeeid + " and id != " + currentapptid;
                 dt = m_dbconnect.GetData(query);
                 if (dt.Rows.Count == 0)
                 {
                     return true;
                 }
                 else return false;
             }
             else return false;
            
            


        }

         public int CreateNewAppointment(DateTime apptdate, int employeeid, int customerid, int length, string note, int catid,string createdby)
        {
            string query;
            query = "Insert into appointment (apptdate,employeeid,customerid,length,note,catid,createdby) values ('" + apptdate.ToString("yyyy-MM-dd HH:mm") + "'," + employeeid + "," + customerid + "," + length + ",'" + note +  "'," + catid + ",'" + createdby + "')";
            m_dbconnect.Command(query);

            query = "select max(id) as maxid from appointment";
            DataTable dt = m_dbconnect.GetData(query);

            return int.Parse(dt.Rows[0]["maxid"].ToString());
        }


        public DataTable GetAppointmentsByEmployee(int employeeid, DateTime appointmentdate)
       {
           string query;
           query = "select appointment.*, category.colorcode as catcolorcode from appointment left outer join category on appointment.catid = category.id where employeeid=" + employeeid + " and DATE(apptdate) ='" + appointmentdate.ToString("yyyy-MM-dd") + "' order by apptdate";
           return m_dbconnect.GetData(query);
       }

        public DataTable GetAppointmentsByDate( DateTime appointmentdate)
        {
            string query;
            query = "select employee.displayname, customer.firstname as customerfirstname, customer.lastname as customerlastname, customer.phone1 as customerphonenumber, appointment.*, category.colorcode as catcolorcode from appointment left outer join category on appointment.catid = category.id " +
                " left outer join customer on appointment.customerid = customer.id inner join employee on appointment.employeeid = employee.id where  DATE(apptdate) ='" + appointmentdate.ToString("yyyy-MM-dd") + "' order by apptdate";
            return m_dbconnect.GetData(query);
        }

        public DataRow GetAppointmentByID(int id)
        {

            DataTable dt;
            string query;
          
            query = "select employee.displayname, customer.firstname as customerfirstname, customer.lastname as customerlastname, customer.phone1 as customerphonenumber, appointment.*, category.colorcode as catcolorcode from appointment left outer join category on appointment.catid = category.id " +
                " left outer join customer on appointment.customerid = customer.id inner join employee on appointment.employeeid = employee.id  where appointment.id=" + id;

            dt = m_dbconnect.GetData(query);
            if (dt.Rows.Count > 0) return dt.Rows[0];
            else return null;
        }
    }
}
