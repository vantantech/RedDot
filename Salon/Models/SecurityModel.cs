using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;
using System.Data;
using RedDot.DataManager;
using NLog;

namespace RedDot
{
    public class SecurityModel
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private DBSecurity m_dbsecurity;
        public Employee CurrentEmployee { get; set; }
    
     

        public SecurityModel()
        {
         
            m_dbsecurity = new DBSecurity();
        }

        public bool HasAccess(string windowname)
        {

            int windowlevel;
            if (CurrentEmployee == null) return false;


            windowlevel = m_dbsecurity.GetWindowLevel(windowname);
            if (windowlevel == 0) return true; //access not required so return true;


            if (CurrentEmployee.SecurityLevel <= 0) return false;

            if (CurrentEmployee.SecurityLevel >= windowlevel) return true;
            else return false;

        }



        public int GetWindowLevel(string windowname)
        {
            return m_dbsecurity.GetWindowLevel(windowname);
        }

        public bool WindowAccess(string windowname)
        {

            int windowlevel;
            if (CurrentEmployee == null) return WindowManagerAccess(windowname);


            windowlevel = m_dbsecurity.GetWindowLevel(windowname);
            if (windowlevel == 0) return true; //access not required so return true;


            if (CurrentEmployee.SecurityLevel <= 0) return false;

            if (CurrentEmployee.SecurityLevel >= windowlevel) return true;
            else
            {
                return WindowManagerAccess(windowname);
            }
        }

        private bool WindowManagerAccess(string windowname)
        {

            int windowlevel;
            Employee tempemployee;

            try
            {
                windowlevel = m_dbsecurity.GetWindowLevel(windowname);

                if (windowlevel == 9999)
                {
                    MessageBox.Show("Access not setup for:" + windowname);
                    return false;
                }

                if (windowlevel == 0) return true; //access not required so return true;


                tempemployee = GetEmployee();

                if (tempemployee == null) return false;

                if (tempemployee.SecurityLevel >= windowlevel)
                {

                    return true;
                }
                else
                {
                    AuditModel.WriteLog(CurrentEmployee.DisplayName, "Access Denied.", windowname, "security", 0);
                    TouchMessageBox.ShowSmall("Access Denied");
                    return false;
                }

            }
            catch (Exception e)
            {

                TouchMessageBox.Show(e.Message);
                return false;
            }

        }

        public bool WindowNewAccess(string windowname)
        {

            int windowlevel;
    

            try
            {
                 windowlevel = m_dbsecurity.GetWindowLevel(windowname);

                if(windowlevel == 9999)
                {
                    TouchMessageBox.Show("Access not setup for:" + windowname);
                    return false;
                }


                //if window access is set to 0, it means use guess account
                 if (windowlevel == 0)
                 {
                    CurrentEmployee = new Employee(0);
                     return true;
                 }
                 else CurrentEmployee = GetEmployee(); //calls login window to get employee

                 if (CurrentEmployee == null) return false;

                 if (CurrentEmployee.SecurityLevel >= windowlevel)
                 {
                    logger.Info("Logged in successfully:" + windowname + ":" + CurrentEmployee.DisplayName);
                     return true;
                 }
                 else
                 {
                     TouchMessageBox.Show("Access Denied");
                    logger.Info("Access Denied:" + windowname);
                     return false;
                 }

            }catch( Exception e)
            {

                TouchMessageBox.Show(e.Message);
                return false;
            }

        }

        public Employee GetEmployee()
        {
            try
            {
                Login lgn = new Login();
                lgn.ShowInTaskbar = false;
                lgn.Topmost = true;
                lgn.ShowDialog();

                if (lgn.ID > 0)
                {
                    
                    return new Employee(lgn.ID);

                }
                else
                {
                    if(lgn.ID == 0)
                    {
                        TouchMessageBox.Show("User Not Found!");
                    }
                    return CurrentEmployee;
                }

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Retrieve employee:" + e.Message);
                return null;
            }

        }


        public DataTable GetACL()
        {
            return m_dbsecurity.GetACL();

        }

        public void UpdateAccessLevel(int id, int level)
        {
            m_dbsecurity.UpdateAccessLevel(id, level);
        }


    }
}
