using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;
using System.Data;
using System.Windows.Media;

namespace RedDot
{
    public class SecurityModel
    {
        private DBSecurity m_dbsecurity;
        public Employee CurrentEmployee { get; set; }
    

        public SecurityModel()
        {
         
            m_dbsecurity = new DBSecurity();
        }

        public static bool ManagerOverride(string windowname,string message="")
        {
   
            SecurityModel model = new SecurityModel();
           return  model.ManagerOverrideAccess(windowname,message);

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

            //this means now access was given at previous level so need new access
            if (CurrentEmployee == null) return false;
       

            windowlevel = m_dbsecurity.GetWindowLevel(windowname);
            if (windowlevel == 0) return true; //access not required so return true;


            if (CurrentEmployee.SecurityLevel <= 0) return false;


            //employee level most likely so need override
            if (CurrentEmployee.SecurityLevel >= windowlevel) return true;
            else
            {
                return ManagerOverrideAccess(windowname);
            }
        }

        public  bool ManagerOverrideAccess(string windowname, string message="")
        {

            int windowlevel;
            Employee tempemployee;

            try
            {
                windowlevel = m_dbsecurity.GetWindowLevel(windowname);

                if (windowlevel == 0) return true; //access not required so return true;


                tempemployee = GetEmployee(false,message,true);

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

        public void LogOff()
        {
            CurrentEmployee = null;
        }
        public bool WindowNewAccess(string windowname,string message="", bool fingerprintonly=false)
        {

            int windowlevel;
    

            try
            {
                 windowlevel = m_dbsecurity.GetWindowLevel(windowname);

                //if window access is set to 0, it means use guess account
                 if (windowlevel == 0)
                 {
                     CurrentEmployee = new Employee(0);
                     return true;
                 }
                 else CurrentEmployee = GetEmployee(fingerprintonly,message);

                if (CurrentEmployee == null)
                {
                   // TouchMessageBox.ShowSmall("Employee PIN not found!!");
                    return false;
                }

                 if (CurrentEmployee.SecurityLevel >= windowlevel)
                 {
                    
                     return true;
                 }
                 else
                 {
                     TouchMessageBox.ShowSmall("Access Denied");
                     return false;
                 }

            }catch( Exception e)
            {

                TouchMessageBox.Show(e.Message);
                return false;
            }

        }

        public Employee GetEmployee(bool fingerprintonly,string message,bool manageroverride=false)
        {
            try
            {
                Login lgn = new Login(fingerprintonly,message)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };

                if (manageroverride) lgn.Background = Brushes.Red;
                lgn.ShowDialog();
               
                if (lgn.ID > 0)
                {
                    return new Employee(lgn.ID);
                   
                }
                else return CurrentEmployee;

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
