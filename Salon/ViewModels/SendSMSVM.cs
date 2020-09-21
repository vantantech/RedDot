using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;
using System.Windows.Input;
using System.Data;
using RedDot.DataManager;
using RedDot.Models;



namespace RedDot
{
    public class SendSMSVM:INPCBase
    {
        IDataInterface _dbsettings;

        private DataTable _smslist;
        private CustomerModel _customermodel;

        private bool CanExecute = true;

     

        private Window m_parent;


     

        public ICommand SendSMSClicked { get; set; }
        public ICommand SendSMSRewardClicked { get; set; }
        public ICommand SendTestClicked { get; set; }

        public SendSMSVM(Window parent)
        {
            m_parent = parent;
            SendSMSClicked = new RelayCommand(ExecuteSendSMSClicked, param => this.CanExecute);
            SendSMSRewardClicked = new RelayCommand(ExecuteSendSMSRewardClicked, param => this.CanExecute);

            SendTestClicked = new RelayCommand(ExecuteSendTestClicked, param => this.CanExecute);


            _dbsettings = GlobalSettings.Instance.RedDotData;

            _customermodel = new CustomerModel();
         


        

            SMSList = _customermodel.GetSMSList();

            Credits = SMSModel.GetBalance();
           
        }


        public string Email { get { return GlobalSettings.Instance.SMSEmail; } set { GlobalSettings.Instance.SMSEmail = value; NotifyPropertyChanged("Email"); } }
        public string AccountID { get { return GlobalSettings.Instance.SMSAccountID; } set { GlobalSettings.Instance.SMSAccountID = value; NotifyPropertyChanged("AccountID"); } }

        public string Password { get { return GlobalSettings.Instance.SMSPassword; } set { GlobalSettings.Instance.SMSPassword = value; NotifyPropertyChanged("Password"); } }

        public string APIKey { get { return GlobalSettings.Instance.APIKey; } set { GlobalSettings.Instance.APIKey = value; NotifyPropertyChanged("APIKey"); } }


        public string UserName { get { return GlobalSettings.Instance.SMSUserName; } set { GlobalSettings.Instance.SMSUserName = value; NotifyPropertyChanged("UserName"); } }
        public string FromPhone { get { return GlobalSettings.Instance.SMSFromPhone; } set { GlobalSettings.Instance.SMSFromPhone = value; NotifyPropertyChanged("FromPhone"); } }

        public string Credits { get { return _credits; } set { _credits = value; NotifyPropertyChanged("Credits"); } }

        public string TestNumber { get; set; }
        public string TestMessage { get; set; }

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
            
            string result =SMSModel.SendSMS(TestMessage, TestNumber.Split(','));

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

                if(conf.Action == "Yes")
                {
                    int count = 0;
                    foreach (DataRow row in SMSList.Rows)
                    {
                         string phone = row["phone1"].ToString();
                          if (phone.Length == 10) phone = "1" + phone;
                          count++;

                         if(count > 1)
                          phonelist = phonelist + "," + phone;
                         else
                             phonelist = phone;
                    }

                     SMSModel.SendSMS(Message, phonelist.Split(','));
                    logger.Info("Sent message to " + SMSList.Rows.Count + " customers");


                    // row["sent"] = result;
                    NotifyPropertyChanged("SMSList");

                     Credits = SMSModel.GetBalance();
                    TouchMessageBox.Show("Credits Remaining:" + Credits);
                    logger.Info("SMS credits left:" + Credits);

     

                }

            }


        }

        private void ExecuteSendSMSRewardClicked(object sender)
        {

            string phone = "";
         
            string message = "";
            int customerid=0;
          
            DBCustomer dbcustomer = new DBCustomer();

          



                Confirm conf = new Confirm("Are your sure?" + "  Sending to " + SMSList.Rows.Count + " customers if they have rewards.");
                Utility.OpenModal(m_parent, conf);
                if (conf.Action == "Yes")
                {
                    string storename = GlobalSettings.Instance.Shop.Name;

                int count = 0;
                   
                    foreach (DataRow row in SMSList.Rows)
                    {
                        customerid = (int)row["id"];

                        decimal rewardamount = dbcustomer.GetRewardBalance(customerid);
                        decimal rewardcredit = dbcustomer.GetRewardCredit(customerid);

                        if(rewardamount + rewardcredit > 0)
                        {
                        //has to be sent one at a time because each customer has different message.
                            message = "Hello from " +  storename + ".  Your Current Reward/Credit Balance is: " + rewardamount + rewardcredit;

                            phone = row["phone1"].ToString();

                            if (phone.Length == 10) phone = "1" + phone;


                            SMSModel.SendSMS(message, phone.Split(','));
                        count++;

                        }
                      


                    }
                    

                    Credits = SMSModel.GetBalance();
                logger.Info("SMS credits left:" + Credits);
                logger.Info("Sent rewards to " + count + " customers");

                TouchMessageBox.Show("Credits Remaining:" + Credits);



            }

 


        }
    }
}
