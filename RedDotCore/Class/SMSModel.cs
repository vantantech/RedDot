using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS_COMAPILib;
using DataManager;

namespace RedDot
{
    public class SMSModel:INPCBase
    {
        DBCustomer dbcustomer;
        private SMS_COMAPILib.SMS oSMS;


        public string Email { get { return GlobalSettings.Instance.SMSEmail; } set { GlobalSettings.Instance.SMSEmail = value; NotifyPropertyChanged("Email"); } }
        public string AccountID { get { return GlobalSettings.Instance.SMSAccountID; } set { GlobalSettings.Instance.SMSAccountID = value; NotifyPropertyChanged("AccountID"); } }

        public string Password { get { return GlobalSettings.Instance.SMSPassword; } set { GlobalSettings.Instance.SMSPassword = value; NotifyPropertyChanged("Password"); } }

        public string UserName { get { return GlobalSettings.Instance.SMSUserName; } set { GlobalSettings.Instance.SMSUserName = value; NotifyPropertyChanged("UserName"); } }
        public string FromPhone { get { return GlobalSettings.Instance.SMSFromPhone; } set { GlobalSettings.Instance.SMSFromPhone = value; NotifyPropertyChanged("FromPhone"); } }
        public SMSModel()
        {
             dbcustomer = new DBCustomer();
             oSMS = new SMS_COMAPILib.SMS();
             oSMS.Authenticate(AccountID, UserName, Password);
             oSMS.smsFROM = FromPhone;
             oSMS.smsMO = true;
        }


        public void SendSMSMessage(string phone, string message)
        {

            if (phone.Length == 10) phone = "1" + phone;
            oSMS.SendMsg(true, message, phone);

        }

    }
}
