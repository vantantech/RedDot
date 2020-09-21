using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.DataManager
{
    public class DBAudit
    {

        DBConnect _dbconnect;
        public DBAudit()
        {

            _dbconnect = new DBConnect();

        }

        public void WriteLog(string UserName, string Action, string Reason, string Tablename, int RecordID)
        {
            string sql;

            sql = "Insert into audit (username,action, reason, tablename,recordid) values ('" + UserName + "','" + Action + "','" + Reason + "','" + Tablename + "'," + RecordID + " )";

            _dbconnect.Command(sql);

        }
    }
}
