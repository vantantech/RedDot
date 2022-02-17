using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;
using RedDot.DataManager;
using NLog;

namespace RedDot
{
    public class AuditModel
    {
       

     
        public static void WriteLog(string username, string Action, string Reason, string Tablename, int RecordID)
        {
            

            try
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                DBAudit _dbaudit;
                _dbaudit = new DBAudit();
                if (username == null) username = "No User";


                Reason = Reason.Replace("'", "^");

                if (Reason.Length > 1999) Reason = Reason.Substring(0, 1999);
                _dbaudit.WriteLog(username, Action, Reason, Tablename, RecordID);
                logger.Info("user:" + username + " Action:" + Action + " Reason:" + Reason + " TableName:" + Tablename + " RecordID:" + RecordID);
            }catch
            {
               
            }
            

        }
    }
}
