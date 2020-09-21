using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;
using System.Windows.Input;
using System.Data;




namespace RedDot
{
    public class SendSMSVM:INPCBase
    {
        DBSettings _dbsettings;

        private DataTable _smslist;
        private CustomerModelCore _customermodel;

        private bool CanExecute = true;

       // private SMS _sms;

        private Window m_parent;
   

        public ICommand SendSMSClicked { get; set; }
        public ICommand SendSMSRewardClicked { get; set; }


        public SendSMSVM(Window parent)
        {
        
        }


        public string Email { get { return GlobalSettings.Instance.SMSEmail; } set { GlobalSettings.Instance.SMSEmail = value; NotifyPropertyChanged("Email"); } }
        public string AccountID { get { return GlobalSettings.Instance.SMSAccountID; } set { GlobalSettings.Instance.SMSAccountID = value; NotifyPropertyChanged("AccountID"); } }

        public string Password { get { return GlobalSettings.Instance.SMSPassword; } set { GlobalSettings.Instance.SMSPassword = value; NotifyPropertyChanged("Password"); } }

        public string UserName { get { return GlobalSettings.Instance.SMSUserName; } set { GlobalSettings.Instance.SMSUserName = value; NotifyPropertyChanged("UserName"); } }
        public string FromPhone { get { return GlobalSettings.Instance.SMSFromPhone; } set { GlobalSettings.Instance.SMSFromPhone = value; NotifyPropertyChanged("FromPhone"); } }
        public string Credits { get { return _credits; } set { _credits = value; NotifyPropertyChanged("Credits"); } }
        private string _credits;
        public string Message
        {
            get { return GlobalSettings.Instance.SMSMessage; }
            set
            {
                GlobalSettings.Instance.SMSMessage = value;  
                NotifyPropertyChanged("Message");
           
            }
        }



        public DataTable SMSList { get { return _smslist; } set { _smslist = value; NotifyPropertyChanged("SMSList"); } }

        private void ExecuteSendSMSClicked(object sender)
        {


            TouchMessageBox.Show("not implemented");
        }

        private void ExecuteSendSMSRewardClicked(object sender)
        {



            TouchMessageBox.Show("not implemented");


        }
    }
}
