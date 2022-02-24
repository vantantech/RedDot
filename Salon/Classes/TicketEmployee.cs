using RedDot.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class TicketEmployee:INPCBase
    {

        string m_employeename;
        int m_employeeid;

        private ObservableCollection<LineItem> m_lineitems;

    
        public string EmployeeName
        {
            get { return m_employeename; }
            set
            {
                m_employeename = value;
                NotifyPropertyChanged("EmployeeName");
            }
        }


        public int EmployeeId
        {
            get { return m_employeeid; }
            set
            {
                m_employeeid = value;
                NotifyPropertyChanged("EmployeeId");
            }
        }

        public ObservableCollection<LineItem> LineItems
        {
            get { return m_lineitems; }
            set { m_lineitems = value; NotifyPropertyChanged("LineItems"); }
        }

    


    }
}
