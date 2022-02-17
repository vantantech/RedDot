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

        private SecurityModel m_security;
        private DataTable m_accesslist;
        private bool CanExecute = true;
        Window m_parent;



        public ICommand EditAccessClicked { get; set; }
        public SecurityVM(Window parent)
        {

 
            EditAccessClicked = new RelayCommand(ExecuteEditAccessClicked, param => this.CanExecute);

            m_parent = parent;
            m_security = new SecurityModel();
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


            SecurityLevel dlg = new SecurityLevel() { Topmost = true };
            dlg.ShowDialog();

            if(dlg.Level > -1)
            {

                m_security.UpdateAccessLevel((int)accessid, dlg.Level);
                AccessControlList = m_security.GetACL();

            }

               


        }

    }
}
