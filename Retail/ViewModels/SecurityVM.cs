using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class SecurityVM:INPCBase
    {

        private Security m_security;
        private DataTable m_accesslist;
        private bool CanExecute = true;
        Window m_parent;
    


        public ICommand EditAccessClicked { get; set; }
        public SecurityVM(Window parent, Security security)
        {

 
            EditAccessClicked = new RelayCommand(ExecuteEditAccessClicked, param => this.CanExecute);

            m_parent = parent;
            m_security = security;
            m_accesslist = m_security.GetACL();
         
        }


        public DataTable AccessControlList
        {
            get { return m_accesslist; }
            set
            {
                m_accesslist = value;
                NotifyPropertyChanged("AccessControlList");
            }

        }



        public void ExecuteEditAccessClicked(object accessid)
        {
            NumPad dlg = new NumPad("Enter New Access level:",true);
            Utility.OpenModal(m_parent, dlg);

            if(dlg.Amount != "")
            {
                AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Security:" + accessid.ToString() + "=>" + dlg.Amount , "Level Updated", "security", (int)accessid);
                m_security.UpdateAccessLevel((int)accessid, int.Parse(dlg.Amount));
                AccessControlList = m_security.GetACL();

            }

               


        }

    }
}
