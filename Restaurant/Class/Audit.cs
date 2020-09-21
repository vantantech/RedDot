using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;

namespace RedDot
{
    public static class Audit
    {
       

   
        public static void WriteLog(string username, string Action, string Reason, string Tablename, int RecordID)
        {
            DBAudit _dbaudit = new DBAudit();

            try
            {
                if (username == null) username = "No User";
                _dbaudit.WriteLog(username, Action, Reason, Tablename, RecordID);

            }catch(Exception e)
            {
                MessageBox.Show("Audit: Writelog : " + e.Message);
            }
            

        }
    }
}
