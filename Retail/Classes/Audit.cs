using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;

namespace RedDot
{
    public class AuditModel
    {
        public static void WriteLog(string username, string Action, string Reason, string Tablename, int RecordID)
        {

            try
            {
                DBAudit _dbaudit;
                _dbaudit = new DBAudit();
                if (username == null) username = "No User";


                Reason = Reason.Replace("'", "^");

                if (Reason.Length > 999) Reason = Reason.Substring(0, 999);
                _dbaudit.WriteLog(username, Action, Reason, Tablename, RecordID);

            }
            catch (Exception e)
            {
                MessageBox.Show("Audit: Writelog : " + e.Message);
            }


        }
    }
}
