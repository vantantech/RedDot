using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RedDot
{
    public class DBSecurity
    {
        private DBConnect dbconnect;

        public DBSecurity()
        {

            dbconnect = new DBConnect();
         
        }
        public int GetWindowLevel(string windowname)
        {

            string sql;
            MySqlDataReader reader;
            int windowlevel;


            sql = "select level from windows where windowname ='" + windowname + "'";
            reader = dbconnect.GetDataReader(sql);
            if (reader != null)
            {
                reader.Read();
                windowlevel = reader.GetInt32(0);
                reader.Close();
                return windowlevel;
            }
            else
            {
               
                return 0;

            }


        }
    }
}
