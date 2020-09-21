using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RedDot
{
    public class TimeCardVM:INPCBase
    {
        public ICommand ClockinClicked { get; set; }

        public ICommand ClockoutClicked { get; set; }

        private Security m_security;



        public Employee CurrentEmployee
        {
            get { return m_security.CurrentEmployee; }
        }
        
        public TimeCardVM(Security security)
        {
            m_security = security;
            ClockinClicked = new RelayCommand(ExecuteClockinClicked, param => this.CanClockIn);
            ClockoutClicked = new RelayCommand(ExecuteClockoutClicked, param => this.CanClockOut);

            CurrentEmployee.GetLastTimeRecord();


        }


  
 



        public bool CanClockIn
        {
            get {
                if (CurrentEmployee == null) return false;
                if ((CurrentEmployee.ClockinTime == "" && CurrentEmployee.ClockoutTime == "") || (CurrentEmployee.ClockinTime != "" && CurrentEmployee.ClockoutTime != "")) return true;
                else return false; }
        }

        public bool CanClockOut
        {
            get {
                if (CurrentEmployee == null) return false; 
                if (CurrentEmployee.ClockinTime != "" && CurrentEmployee.ClockoutTime == "") return true;
                else return false; }
        }
        public void ExecuteClockinClicked(object salesid)
        {
            
            CurrentEmployee.ClockIn(DateTime.Now);
            
        }


        public void ExecuteClockoutClicked(object salesid)
        {
           
            CurrentEmployee.ClockOut(DateTime.Now);
            
        }
    }
}
