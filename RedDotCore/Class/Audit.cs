using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManager;
using System.Windows;

namespace RedDot
{
    public class Audit
    {
        DBAudit _dbaudit;

        public Audit()
        {
            _dbaudit = new DBAudit();
        }
        public void WriteLog(string username, string Action, string Reason, string Tablename, int RecordID)
        {

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
