using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;
using System.Windows.Input;
using System.Data;

using RedDot.Models;



namespace RedDot
{
    public class SendSMSVM : INPCBase
    {
     

        private DataTable _smslist;
        private CustomerModel _customermodel;

        private bool CanExecute = true;



        private Window m_parent;

        private SMSModel m_smsmodel;


        public ICommand SendSMSClicked { get; set; }
        public ICommand SendSMSRewardClicked { get; set; }
        public ICommand SendTestClicked { get; set; }

        public SendSMSVM(Window parent)
        {
            m_parent = parent;
            SendSMSClicked = new RelayCommand(ExecuteSendSMSClicked, param => this.CanExecute);
            SendSMSRewardClicked = new RelayCommand(ExecuteSendSMSRewardClicked, param => this.CanExecute);

            SendTestClicked = new RelayCommand(ExecuteSendTestClicked, param => this.CanExecute);


        

            _customermodel = new CustomerModel();
            m_smsmodel = new SMSModel();




            SMSList = _customermodel.GetSMSList();

            Credits = m_smsmodel.GetBalance();

        }


        public string Email { get { return GlobalSettings.Instance.SMSEmail; } set { GlobalSettings.Instance.SMSEmail = value; NotifyPropertyChanged("Email"); } }
        public string AccountID { get { return GlobalSettings.Instance.SMSAccountID; } set { GlobalSettings.Instance.SMSAccountID = value; NotifyPropertyChanged("AccountID"); } }

        public string Password { get { return GlobalSettings.Instance.SMSPassword; } set { GlobalSettings.Instance.SMSPassword = value; NotifyPropertyChanged("Password"); } }

        public string APIKey { get { return GlobalSettings.Instance.APIKey; } set { GlobalSettings.Instance.APIKey = value; NotifyPropertyChanged("APIKey"); } }


        public string UserName { get { return GlobalSettings.Instance.SMSUserName; } set { GlobalSettings.Instance.SMSUserName = value; NotifyPropertyChanged("UserName"); } }
        public string FromPhone { get { return GlobalSettings.Instance.SMSFromPhone; } set { GlobalSettings.Instance.SMSFromPhone = value; NotifyPropertyChanged("FromPhone"); } }
        public string Credits { get { return _credits; } set { _credits = value; NotifyPropertyChanged("Credits"); } }

        public string TestNumber { get; set; }

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



        private void ExecuteSendTestClicked(object sender)
        {
            if (TestNumber.Length == 10) TestNumber = "1" + TestNumber;
            string result = m_smsmodel.SendSMS("This is a TEST from your Red Dot POS", TestNumber.Split(','));

            TouchMessageBox.Show("Message Sent:" + result);
        }
        private void ExecuteSendSMSClicked(object sender)
        {

            string phonelist = "";

            if (Message.Length > 160)
            {
                TouchMessageBox.Show("Message Length too long");
            }
            else
            {
                Confirm conf = new Confirm("Are your sure?" + "  Sending to " + SMSList.Rows.Count + " customers.") { Topmost = true };
                conf.ShowDialog();

                if (conf.Action == "Yes")
                {
                    int count = 0;
                    foreach (DataRow row in SMSList.Rows)
                    {
                        string phone = row["phone1"].ToString();
                        if (phone.Length == 10) phone = "1" + phone;
                        count++;

                        if (count > 1)
                            phonelist = phonelist + "," + phone;
                        else
                            phonelist = phone;
                    }

                    m_smsmodel.SendSMS(Message, phonelist.Split(','));

                    logger.Info("Sent message to " + SMSList.Rows.Count + " customers");

                    // row["sent"] = result;
                    NotifyPropertyChanged("SMSList");

                    Credits = m_smsmodel.GetBalance();
                    TouchMessageBox.Show("Credits Remaining:" + Credits);



                }

            }


        }

        private void ExecuteSendSMSRewardClicked(object sender)
        {

            string phone = "";

            string message = "";
            int customerid = 0;

            DBCustomer dbcustomer = new DBCustomer();





            Confirm conf = new Confirm("Are your sure?" + "  Sending to " + SMSList.Rows.Count + " customers.");
            Utility.OpenModal(m_parent, conf);
            if (conf.Action == "Yes")
            {
                string storename = GlobalSettings.Instance.Shop.Name;

                foreach (DataRow row in SMSList.Rows)
                {
                    customerid = (int)row["id"];

                    decimal rewardamount = dbcustomer.GetRewardBalance(customerid);

                    if (rewardamount > 0)
                    {
                        message = "Hello from " + storename + ".  Your Current Reward Balance is: " + rewardamount;

                        phone = row["phone1"].ToString();

                        if (phone.Length == 10) phone = "1" + phone;


                        m_smsmodel.SendSMS(message, phone.Split(','));

                    }



                }


                Credits = m_smsmodel.GetBalance();
                //  TouchMessageBox.Show("Credits Remaining:" + Credits);



            }




        }
    }
}
