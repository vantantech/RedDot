using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace RedDot
{
    public class CreditCardManagerVM:INPCBase
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();



        public ICommand CreditSaleClicked { get; set; }
        public ICommand DebitSaleClicked { get; set; }
        public ICommand LineItemClicked { get; set; }
        public ICommand CreditRefundClicked { get; set; }
        public ICommand DebitRefundClicked { get; set; }





        public ICommand CreditDebitClicked { get; set; }
            public ICommand CardClicked { get; set; }


            public ICommand GiftCardClicked { get; set; }

            public ICommand CommandClicked { get; set; }
     

            public ICommand VoidClicked { get; set; }

 

            public ICommand ExitClicked { get; set; }

        public ICommand POSMessageClicked { get; set; }

        public ICommand POSXMLClicked { get; set; }

        public ICommand AddGiftCardClicked { get; set; }

        public ICommand GiftCardBalanceClicked { get; set; }

      
        public string SIPPassword { get; set; }

        private VFD vfd;

            private decimal m_giftcardbalance;
            private decimal m_quickamount;
            private string m_creditcardname = "Credit,Debit";



            private List<string> m_creditcardchoices;

            private HeartPOS m_ccp;



            //private bool canExecute = true;
            private Window m_parent;

        int m_requestid = 0;

        IDeviceInterface _device;  //PAX 300

        private Employee m_currentemployee;

     
        bool UsingHeartSIP { get; set; }
  
        public CreditCardManagerVM(Window parent, HeartPOS ccp, Employee currentemployee)
        {
                CCP = ccp;
                m_parent = parent;
                m_currentemployee = currentemployee;
                CurrentTime = DateTime.Now.ToLongTimeString();


                Random rand = new Random();
                m_requestid = rand.Next(10000, 99999999);

             
            UsingHeartSIP = true;


                Init();  // initialize button handlers


                CreditCardChoices = GlobalSettings.Instance.CreditCardChoices.Split(',').ToList();


                if (CreditCardChoices.Count == 1)
                {
                        m_creditcardname = GlobalSettings.Instance.CreditCardChoices;
                        CreditCardChoices.Clear();
                }

                DateTime latestDateTime = DateTime.Now;

                SIPPassword = "SIP Password:" + CalculateAdminPassword(latestDateTime.Date, new DateTime(latestDateTime.Date.Year, 1, 1));


        }




   



        private void Init()
        {
            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);

            CardClicked = new RelayCommand(ExecuteCardClicked, param => true);
            CreditSaleClicked = new RelayCommand(ExecuteCreditSaleClicked, param => true);
            DebitSaleClicked = new RelayCommand(ExecuteDebitSaleClicked, param => true);
            CreditRefundClicked = new RelayCommand(ExecuteCreditRefundClicked, param => true);
            DebitRefundClicked = new RelayCommand(ExecuteDebitRefundClicked, param => true);

            CommandClicked = new RelayCommand(ExecuteCommandClicked, param => CanExecuteCommand(param));

            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => true);
    

            ExitClicked = new RelayCommand(ExecuteExitClicked, param => true);
            AddGiftCardClicked = new RelayCommand(ExecuteAddGiftCardClicked, param => UsingHeartSIP);
            GiftCardBalanceClicked = new RelayCommand(ExecuteGiftCardBalanceClicked, param => UsingHeartSIP);

            POSMessageClicked = new RelayCommand(ExecutePOSMessageClicked, param => true);

            POSXMLClicked = new RelayCommand(ExecutePOSXMLClicked, param => UsingHeartSIP);
        }

            public Visibility NumPadVisibility { get; set; }

        public string Request_ID { get { m_requestid++;  return m_requestid.ToString(); } }

        public string CalculateAdminPassword(DateTime currentDate, DateTime startDate)
        {
            TimeSpan span = currentDate.Subtract(startDate);
            return ((span.Days + 1) * 2).ToString();
        }

        private string m_currenttime;
            public string CurrentTime
            {
                get { return m_currenttime; }
                set
                {
                    m_currenttime = value;
                    NotifyPropertyChanged("CurrentTime");
                }
            }

            public List<string> CreditCardChoices
            {
                get { return m_creditcardchoices; }
                set
                {
                    m_creditcardchoices = value;
                    NotifyPropertyChanged("CreditCardChoices");
                }
            }

  



        public decimal GiftCardBalance
            {
                get
                {
                    return m_giftcardbalance;
                }

                set
                {
                    m_giftcardbalance = value;
                    NotifyPropertyChanged("GiftCardBalance");
                }
            }
            public decimal QuickAmount
            {
                get
                {
                    return m_quickamount;
                }

                set
                {
                    m_quickamount = value;
                    NotifyPropertyChanged("QuickAmount");
                }
            }


        public HeartPOS CCP
        {
            get { return m_ccp; }
            set
            {
                m_ccp = value;
                NotifyPropertyChanged("CCP");
            }
        }




        //------------------------------------------------------------------------------------------
        //  ____        _   _                ____  _           _     _      
        // | __ ) _   _| |_| |_ ___  _ __   |  _ \(_)___  __ _| |__ | | ___ 
        // |  _ \| | | | __| __/ _ \| '_ \  | | | | / __|/ _` | '_ \| |/ _ \
        // | |_) | |_| | |_| || (_) | | | | | |_| | \__ \ (_| | |_) | |  __/
        // |____/ \__,_|\__|\__\___/|_| |_| |____/|_|___/\__,_|_.__/|_|\___|                                                                 
        //
        //------------------------------------------------------------------------------------------ 





        public bool CanExecuteCommand(object command)
        {

            string cmd = command.ToString();

            if(UsingHeartSIP)
            {
                return true;
            }else
            {
                switch(cmd)
                {
                    case "EOD":
                    case "Reset":
                    case "Reboot":
                    case "CardVerify":
                        return true;

                    default:
                        return false;
                }
            }
           
        }




        //-------------------Methods ------------------------------

        public String XMLGetTagValue(String SIPResponseMessage, String Tag)
            {

                var XMLDoc = XDocument.Parse(SIPResponseMessage);

                XElement element = XMLDoc.Root.Element(Tag);
                if (element != null)
                    return element.Value;

                return "";
            }


   

            public void ExecuteCardClicked(object cardname)
            {
                string temp = cardname as string;
                m_creditcardname = temp;

            }





        public void ExecuteCommandClicked(object obj)
        {
            string command = "";
         

            if (obj != null) command = obj.ToString();


      
                //calls credit card processor command
                m_ccp.ExecuteCommand(command);
       
     


        }



        public void ExecuteCreditSaleClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;
       

            try
            {


                if (temp != "")
                {
                    amt = decimal.Parse(temp);


                   
                        HeartSIP_Charge(amt, "Credit");
                 
                      


                }
                else TouchMessageBox.Show("Please Enter $ Amt to Process.");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
            }


        }

        public void ExecuteDebitSaleClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;
         

            try
            {



                if (temp != "")
                {
                    amt = decimal.Parse(temp);


                  
                        HeartSIP_Charge(amt, "Debit");
              

                      


                }
                else TouchMessageBox.Show("Please Enter $ Amt to Process.");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
            }


        }

        public void HeartSIP_Charge(decimal amt, string creditcardname)
        {
         

            try
            {

                if (amt == 0)
                {
                   

                    m_ccp.ExecuteSaleCommand(amt, Request_ID, creditcardname);

                }
                else TouchMessageBox.Show("Please Enter Amount");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
            }


        }

        public void ExecuteVoidClicked(object obj)
        {
           

            if (obj.ToString() == "")
            {
                TouchMessageBox.Show("No Authorization Code to Void Transaction with !!!");
                return;
            }

       
                m_ccp.ExecuteVoidCommand(obj.ToString(), obj.ToString());


        }


        public void ExecuteCreditRefundClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;
         

            try
            {



                if (temp != "")
                {
                    amt = decimal.Parse(temp);


                  
                        HeartSIP_Refund(amt, "Credit");
                  
                     

                }
                else TouchMessageBox.Show("Please Enter Refund Amount");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Refund Clicked: " + e.Message);
            }
        }

        public void ExecuteDebitRefundClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;
      

            try
            {



                if (temp != "")
                {
                    amt = decimal.Parse(temp);


           
                        HeartSIP_Refund(amt, "Debit");
             

                }
                else TouchMessageBox.Show("Please Enter Refund Amount");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Refund Clicked: " + e.Message);
            }
        }



        public void HeartSIP_Refund(decimal amt, string creditcardname)
        {
         
            try
            {

                if (amt == 0)
                {
                 
                     m_ccp.ExecuteRefundCommand(amt,Request_ID, creditcardname);

                }
                else TouchMessageBox.Show("Please Enter Refund Amount");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
            }
        }

        public void ExecuteAddGiftCardClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;

            try
            {



                if (temp != "")
                {
                    amt = decimal.Parse(temp);






                    m_ccp.ExecuteAddGiftCardCommand(amt, Request_ID);




                }
                else TouchMessageBox.Show("Please Enter Amount to Add to Gift Card");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Refund Clicked: " + e.Message);
            }
        }

        public void ExecuteGiftCardBalanceClicked(object obj)
        {
            m_ccp.ExecuteGiftCardBalanceCommand(Request_ID);
        }



        public void ExecutePOSMessageClicked(object obj)
        {
          if (CCP.POSMessage != "") ReceiptPrinterModel.PrintResponse(CCP.LastCommand, CCP.POSMessage);
        }


        public void ExecutePOSXMLClicked(object obj)
        {
            if(UsingHeartSIP)
            if (CCP.POSXML != "") ReceiptPrinterModel.PrintResponse(CCP.LastCommand, CCP.POSXML);
        }


        public void ExecuteExitClicked(object obj)
            {

                m_parent.Close();
            }


  

        }
    }
