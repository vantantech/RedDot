using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows.Forms;

namespace RedDot.DataManager
{
    public class DBSecurity
    {
        private DBConnect m_dbconnect;

        public DBSecurity()
        {

            m_dbconnect = new DBConnect();
         
        }

        public DataTable GetACL()
        {

            string query;
            query = "select  id, windowname, description, level, case when level=10 then 'Employee' when level = 50 then 'Manager' when level =100 then 'Admin/Owner' else 'Free Access' end as leveltype from security order by description";
            return m_dbconnect.GetData(query);

        }

        public void UpdateAccessLevel(int id, int level)
        {
            string query;
            query = "update security set level =" + level + " where id=" + id;
            m_dbconnect.Command(query);


        }

            public int GetWindowLevel(string windowname)
        {

            string sql;
            DataTable dt;
            int windowlevel;


            sql = "select level from security where windowname ='" + windowname + "'";
            dt = m_dbconnect.GetData(sql);
            if (dt.Rows.Count > 0)
            {

                windowlevel = int.Parse(dt.Rows[0]["level"].ToString());
                return windowlevel;
            }
            else
            {
                sql = "insert into security (windowname,description,level) value ('" + windowname + "','" + windowname + "',100)";
                m_dbconnect.Command(sql);
                return 100;

            }


        }


        public int UserAuthenticate(string MSRCard, string pin, int len)
        {
            DataTable table;
            try
            {
                if(MSRCard.Length > 0)    table = m_dbconnect.GetData("select id, securitylevel from employee where msrcard ='" + MSRCard + "'");
                else   table = m_dbconnect.GetData("select id, securitylevel from employee where substring(pin,1," + len.ToString() + ")='" + pin.ToString().Substring(0, len) + "'");

                if (table.Rows.Count >= 1)
                {

                    if (table.Rows[0]["securitylevel"].ToString() != "")
                    {
                        if ((int)table.Rows[0]["securitylevel"] > 0)
                        {
                            return (int)table.Rows[0]["id"];
                        }
                        else return 0;

                    }
                    else return 0;


                }
                else return 0;
            }
            catch (Exception e)
            {

                MessageBox.Show("DBSecurity:UserAuthenticate" + e.Message);
                return 0;
            }


        }
    }
}
