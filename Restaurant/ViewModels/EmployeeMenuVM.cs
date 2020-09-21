using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class EmployeeMenuVM:INPCBase
    {
        public ICommand ClockinClicked { get; set; }

        public ICommand ClockoutClicked { get; set; }

        public ICommand CashierInOutClicked { get; set; }

        public ICommand ProfileClicked { get; set; }

        public ICommand TimeCardClicked { get; set; }

        public ICommand SalesClicked { get; set; }

        private SecurityModel m_security;
        private Window m_parent;

        public EmployeeMenuVM(Window parent,SecurityModel security)
        {
            m_parent = parent;
            m_security = security;
            ClockinClicked = new RelayCommand(ExecuteClockinClicked, param => this.CanClockIn);
            ClockoutClicked = new RelayCommand(ExecuteClockoutClicked, param => this.CanClockOut);
            CashierInOutClicked = new RelayCommand(ExecuteCashierInOutClicked, param => true);
            ProfileClicked = new RelayCommand(ExecuteProfileClicked, param => true);
            TimeCardClicked = new RelayCommand(ExecuteTimeCardClicked, param => true);
            SalesClicked = new RelayCommand(ExecuteSalesClicked, param => true);
        }


        public bool CanClockIn
        {
            get
            {
                if (m_security.CurrentEmployee == null) return false;
                if (m_security.CurrentEmployee.ClockedIn) return false;
                else return true;
            }
        }

        public bool CanClockOut
        {
            get
            {
                if (m_security.CurrentEmployee == null) return false;
                return m_security.CurrentEmployee.ClockedIn;

            }
        }

        public void ExecuteClockinClicked(object salesid)
        {
            //if merchant setup so that there is security at the portal level , we can use it and past it in

            if(m_security.CurrentEmployee.SecurityLevel > 1)
            {
                if (m_security.WindowAccess("ClockInOut"))
                {
                    if (m_security.CurrentEmployee.ClockedIn)
                    {
                        TouchMessageBox.Show("Already Clocked In !!");
                    }
                    else
                    {
                        bool succeed = m_security.CurrentEmployee.ClockIn(DateTime.Now, false);
                        if (succeed)
                        {
                            TouchMessageBox.Show(m_security.CurrentEmployee.FullName + "\n\rClock In Time:" + m_security.CurrentEmployee.ClockinTime);
                            m_parent.Close();
                        }

                    }


                }
            }else
            {
                //new security model so it doesn't save access
                 SecurityModel m_security = new SecurityModel();
                if (m_security.WindowNewAccess("ClockInOut","", GlobalSettings.Instance.FingerPrintRequiredForTimeCard))
                {
                    if (m_security.CurrentEmployee.ClockedIn)
                    {
                        TouchMessageBox.Show("Already Clocked In !!");
                    }
                    else
                    {
                        bool succeed = m_security.CurrentEmployee.ClockIn(DateTime.Now, false);
                        if (succeed)
                        {
                            TouchMessageBox.Show(m_security.CurrentEmployee.FullName + "\n\rClock In Time:" + m_security.CurrentEmployee.ClockinTime);
                            m_parent.Close();
                        }
                    }

                }
            }

        

        }


        public void ExecuteClockoutClicked(object salesid)
        {
            
            //if merchant setup so that there is security at the portal level , we can use it and past it in

            if (m_security.CurrentEmployee.SecurityLevel > 1)
            {
                if (m_security.WindowAccess("ClockInOut"))
                {
                    if (m_security.CurrentEmployee.ClockedIn)
                    {
                        m_security.CurrentEmployee.ClockOut(DateTime.Now);
                        m_parent.Close();
                    }
                    else
                        TouchMessageBox.Show("Already Clocked Out !!");
                 

                }
            }
            else
            {
                //new security model so it doesn't save access
                SecurityModel m_security = new SecurityModel();
                if (m_security.WindowNewAccess("ClockInOut","", GlobalSettings.Instance.FingerPrintRequiredForTimeCard))
                {
                    if (m_security.CurrentEmployee.ClockedIn)
                    {
                        m_security.CurrentEmployee.ClockOut(DateTime.Now);

                        m_parent.Close();
                    }
                    else
                        TouchMessageBox.Show("Already Clocked Out !!");

                }
            }

        
           
            
        }

        private void ExecuteCashierInOutClicked(object sender)
        {

            //if merchant setup so that there is security at the portal level , we can use it and past it in

            if (m_security.CurrentEmployee.SecurityLevel > 1)
            {
                if (m_security.WindowAccess("CashierInOut"))
                {
                    CashierInOut rpt = new CashierInOut(m_security,false);
                    Utility.OpenModal(m_parent, rpt);

                }
            }
            else
            {
                //new security model so it doesn't save access
                SecurityModel m_security = new SecurityModel();
                if (m_security.WindowNewAccess("CashierInOut"))
                {
                    CashierInOut rpt = new CashierInOut(m_security,false);
                    Utility.OpenModal(m_parent, rpt);

                }
            }


       
        }

        private void ExecuteProfileClicked(object obj)
        {
            if (m_security.CurrentEmployee.SecurityLevel > 1)
            {
                if (m_security.WindowAccess("EmployeeView"))
                {
                    EmployeeView rpt = new EmployeeView(m_security, m_security.CurrentEmployee, false, false);
                    Utility.OpenModal(m_parent, rpt);

                }

            }
            else
            {
                SecurityModel m_security = new SecurityModel();
                if (m_security.WindowNewAccess("EmployeeView"))
                {
                    EmployeeView rpt = new EmployeeView(m_security, m_security.CurrentEmployee, false, false);
                    Utility.OpenModal(m_parent, rpt);

                }
            }
    
        }

        private void ExecuteTimeCardClicked(object obj)
        {
            if (m_security.CurrentEmployee.SecurityLevel > 1)
            {

                if (m_security.WindowAccess("EmployeeView"))
                {
                    EmployeeHours rpt = new EmployeeHours(m_security, false, false);
                    Utility.OpenModal(m_parent, rpt);

                }
            }
            else
            {
                SecurityModel m_security = new SecurityModel();
                if (m_security.WindowNewAccess("EmployeeView","", GlobalSettings.Instance.FingerPrintRequiredForTimeCard))
                {
                    EmployeeHours rpt = new EmployeeHours(m_security, false, false);
                    Utility.OpenModal(m_parent, rpt);

                }
            }

        }


        private void ExecuteSalesClicked(object obj)
        {
            if (m_security.CurrentEmployee.SecurityLevel > 1)
            {
                if (m_security.WindowAccess("EmployeeView"))
                {
                    EmployeeSales rpt = new EmployeeSales(m_security);
                    Utility.OpenModal(m_parent, rpt);

                }
            }
            else
            {
                SecurityModel m_security = new SecurityModel();
                if (m_security.WindowNewAccess("EmployeeView"))
                {
                    EmployeeSales rpt = new EmployeeSales(m_security);
                    Utility.OpenModal(m_parent, rpt);

                }
            }

        }
    }
}
