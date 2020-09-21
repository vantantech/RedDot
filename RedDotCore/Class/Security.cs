using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataManager;
using System.Data;


namespace RedDot
{
    public class Security
    {
        private DBSecurity m_dbsecurity;
        public Employee CurrentEmployee { get; set; }
     

        public Security()
        {
         
            m_dbsecurity = new DBSecurity();
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
                    MessageBox.Show("Access Denied");
                    return false;
                }

            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
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
                    MessageBox.Show("Access not setup for:" + windowname);
                    return false;
                }


                //if window access is set to 0, it means use guess account
                 if (windowlevel == 0)
                 {
                     CurrentEmployee = new Employee(0);
                     return true;
                 }
                 else CurrentEmployee = GetEmployee();

                 if (CurrentEmployee == null) return false;

                 if (CurrentEmployee.SecurityLevel >= windowlevel)
                 {
                    
                     return true;
                 }
                 else
                 {
                     MessageBox.Show("Access Denied");
                     return false;
                 }

            }catch( Exception e)
            {

                MessageBox.Show(e.Message);
                return false;
            }

        }

        public Employee GetEmployee()
        {
            try
            {
                Login lgn = new Login();
                lgn.ShowInTaskbar = false;
                lgn.ShowDialog();
               
                if (lgn.ID > 0)
                {
                    return new Employee(lgn.ID);
                   
                }
                else return null;

            }
            catch (Exception e)
            {
                MessageBox.Show("Retrieve employee:" + e.Message);
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
