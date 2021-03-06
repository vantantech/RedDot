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
using RedDotBase;

namespace RedDot
{
    public class PAXManagerVM : INPCBase
    {


        public ICommand CreditAuthClicked { get; set; }
        public ICommand CreditSaleClicked { get; set; }
        public ICommand DebitSaleClicked { get; set; }
        public ICommand LineItemClicked { get; set; }
        public ICommand CreditRefundClicked { get; set; }
        public ICommand DebitRefundClicked { get; set; }





        public ICommand CreditDebitClicked { get; set; }
  


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
       



        private List<string> m_creditcardchoices;

        private HeartPOS m_ccp;



        //private bool canExecute = true;
        private Window m_parent;

        int m_requestid = 0;

        IDeviceInterface _device;  //PAX 300

        private Employee m_currentemployee;

        bool UsingPAX { get; set; }
        bool UsingHeartSIP { get; set; }

   


        public PAXManagerVM(Window parent, Employee currentemployee)
        {
            m_parent = parent;
            m_currentemployee = currentemployee;
            Init();



            UsingPAX = true;
            UsingHeartSIP = false;


            try
            {

                if (GlobalSettings.Instance.CreditCardProcessor == "PAX_S300")
                {
                    _device = DeviceService.Create(new ConnectionConfig
                    {
                        DeviceType = DeviceType.PAX_S300,
                        ConnectionMode = ConnectionModes.TCP_IP,
                        IpAddress = GlobalSettings.Instance.IPAddress,
                        Port = GlobalSettings.Instance.Port,
                        Timeout = 30000
                    });

                    //initialize logging event
                    _device.OnMessageSent += (message) =>
                    {
                        logger.Info("SENT:" + message);
                    };



                    _device.Initialize();

                    _device.Reset();
                }

            }catch(Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
                m_parent.Close();
            }



        }



        private void Init()
        {
            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);

         
            CreditSaleClicked = new RelayCommand(ExecuteCreditSaleClicked, param => true);
            CreditAuthClicked = new RelayCommand(ExecuteCreditAuthClicked, param => true);
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

        public string Request_ID { get { m_requestid++; return m_requestid.ToString(); } }

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

        string m_posmessage;
        public string POSMessage
        {
            get { return m_posmessage; }
            set
            {
                m_posmessage = value;
                NotifyPropertyChanged("POSMessage");

            }
        }

        string m_posxml;
        public string POSXML
        {
            get { return m_posxml; }
            set
            {
                m_posxml = value;
                NotifyPropertyChanged("POSXML");

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

            if (UsingHeartSIP)
            {
                return true;
            }
            else
            {
                switch (cmd)
                {
                    case "EOD":
                    case "Reset":
                    case "Reboot":
                    case "GetInfo":
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




 





        public void ExecuteCommandClicked(object obj)
        {
            string command = "";
            POSMessage = "";
            POSXML = "";

            if (obj != null) command = obj.ToString();


            if (UsingPAX)
            {
                logger.Info("COMMAND:" + command);

                switch (command)
                {
                    case "EOD":

                        try
                        {
                            IBatchCloseResponse resp = _device.BatchClose();


                            POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                            if (resp.DeviceResponseText == "OK")
                            {
                                var count = resp.TotalCount.Split('=');
                                var amount = resp.TotalAmount.Split('=');

                                POSMessage += "Credit Count:" + count[0] + (char)13 + (char)10;
                                POSMessage += "Credit Amount:" + Math.Round(int.Parse(amount[0]) / 100m, 2) + (char)13 + (char)10;
                                POSMessage += "Debit Count:" + count[1] + (char)13 + (char)10;
                                POSMessage += "Debit Amount:" + Math.Round(int.Parse(amount[1]) / 100m, 2) + (char)13 + (char)10;


                            }


                            POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;

                            logger.Info(command + ":RECEIVED:" + resp.ToString());
                            logger.Info("Status:" + resp.DeviceResponseText + "  BatchNumber:" + resp.SequenceNumber);
                            logger.Info("Total Count:" + resp.TotalCount + "  Total Amount:" + resp.TotalAmount);

                            ReceiptPrinterModel.PrintResponse("Batch Close", POSMessage);

                            break;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error Closing Batch:" + ex.Message);
                            break;
                        }



                    case "Reset":
                        var resp1 = _device.Reset();

                        POSMessage = "Response:" + resp1.DeviceResponseText + (char)13 + (char)10;
                        POSXML = "RAW:" + resp1.ToString() + (char)13 + (char)10;
                        logger.Info(command + ":RECEIVED:" + resp1.ToString());
                        break;

                    case "Reboot":
                        var resp2 = _device.Reboot();

                        POSMessage = "Response:" + resp2.DeviceResponseText + (char)13 + (char)10;
                        POSXML = "RAW:" + resp2.ToString() + (char)13 + (char)10;
                        logger.Info(command + ":RECEIVED:" + resp2.ToString());

                        break;


                    case "CardVerify":
                        var resp3 = _device.CreditVerify(1).Execute();

                        POSMessage = "Response:" + resp3.DeviceResponseText + (char)13 + (char)10;

                        POSMessage += resp3.ResponseText + (char)13 + (char)10;



                        POSXML = "RAW:" + resp3.ToString() + (char)13 + (char)10;

                        logger.Info(command + ":HRef=" + resp3.TransactionId);
                        logger.Info(command + ":RECEIVED:" + resp3.ToString());

                        break;

                    case "GetInfo":
                        var resp4 = _device.Initialize();

                        POSMessage = "Response:" + resp4.DeviceResponseText + (char)13 + (char)10;

                        POSMessage += "Serial Number:" +  resp4.SerialNumber + (char)13 + (char)10;



                        POSXML = "RAW:" + resp4.ToString() + (char)13 + (char)10;

                        logger.Info(command + ":RECEIVED:" + resp4.ToString());


                        break;
                }

            }
            else
            {
                //calls credit card processor command
                m_ccp.ExecuteCommand(command);
            }



        }


        public void ExecuteCreditAuthClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;
            POSMessage = "";
            POSXML = "";

            try
            {


                if (temp != "")
                {
                    amt = decimal.Parse(temp);


                    if (GlobalSettings.Instance.CreditCardProcessor == "PAX_S300")
                    {
                        logger.Info("COMMAND:Credit Authorize , Amount=" + amt);
                        TerminalResponse resp = _device.CreditAuth(m_requestid, amt).Execute();
                        logger.Info("Credit Authorize:RECEIVED:" + resp.ToString());
                        logger.Info("Credit Authorize:HRef=" + resp.TransactionId);


                        POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                        POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;

                    }
                    else
                    {
                        HeartSIP_Charge(amt, "Credit");
                    }



                }
                else TouchMessageBox.Show("Please Enter $ Amt to Process.");

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Charge Clicked: " + e.Message);
            }


        }
        public void ExecuteCreditSaleClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;
            POSMessage = "";
            POSXML = "";

            try
            {


                if (temp != "")
                {
                    amt = decimal.Parse(temp);


                    if (GlobalSettings.Instance.CreditCardProcessor == "PAX_S300")
                    {
                        logger.Info("COMMAND:Credit Authorize , Amount=" + amt);
                        TerminalResponse resp = _device.CreditSale(m_requestid, amt).WithRequestTip(true).Execute();
                        logger.Info("Credit Authorize:RECEIVED:" + resp.ToString());
                        logger.Info("Credit Authorize:HRef=" + resp.TransactionId);


                        POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                        POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;

                    }
                    else
                    {
                        HeartSIP_Charge(amt, "Credit");
                    }



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
            POSMessage = "";
            POSXML = "";

            try
            {



                if (temp != "")
                {
                    amt = decimal.Parse(temp);


                    if (GlobalSettings.Instance.CreditCardProcessor == "PAX_S300")
                    {
                        logger.Info("COMMAND:Debit Sale , Amount=" + amt);
                        TerminalResponse resp = _device.DebitSale(m_requestid, amt).Execute();
                        logger.Info("Debit Sale:RECEIVED:" + resp.ToString());
                        logger.Info("Debit Sale:HRef=" + resp.TransactionId);

                        POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                        POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;
                    }
                    else
                    {
                        HeartSIP_Charge(amt, "Debit");
                    }




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
            POSMessage = "";
            POSXML = "";

            if (obj.ToString() == "")
            {
                TouchMessageBox.Show("No Authorization Code to Void Transaction with !!!");
                return;
            }

            if (GlobalSettings.Instance.CreditCardProcessor == "PAX_S300")
            {
                logger.Info("COMMAND:Credit Void , Transaction ID=" + obj.ToString());
                TerminalResponse resp = _device.CreditVoid(1).WithTransactionId(obj.ToString()).Execute();
                logger.Info("Credit Void:RECEIVED:" + resp.ToString());
                logger.Info("Credit Void:HRef=" + resp.TransactionId);

                POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;
            }
            else
                m_ccp.ExecuteVoidCommand(obj.ToString(), obj.ToString());


        }


        public void ExecuteCreditRefundClicked(object amount)
        {
            string temp = amount as string;
            decimal amt;
            POSMessage = "";
            POSXML = "";

            try
            {



                if (temp != "")
                {
                    amt = decimal.Parse(temp);


                    if (GlobalSettings.Instance.CreditCardProcessor == "PAX_S300")
                    {
                        logger.Info("COMMAND:Credit Refund , Amount=" + amt);
                        TerminalResponse resp = _device.CreditRefund(m_requestid, amt).Execute();
                        logger.Info("Credit Refund:RECEIVED:" + resp.ToString());
                        logger.Info("Credit Refund:HRef=" + resp.TransactionId);

                        POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                        POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;
                    }
                    else
                    {
                        HeartSIP_Refund(amt, "Credit");
                    }


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
            POSMessage = "";
            POSXML = "";

            try
            {



                if (temp != "")
                {
                    amt = decimal.Parse(temp);


                    if (GlobalSettings.Instance.CreditCardProcessor == "PAX_S300")
                    {

                        logger.Info("COMMAND:Debit Refund , Amount=" + amt);
                        TerminalResponse resp = _device.DebitRefund(m_requestid, amt).Execute();
                        logger.Info("Debit Refund:RECEIVED:" + resp.ToString());
                        logger.Info("Debit Refund:HRef=" + resp.TransactionId);


                        POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                        POSXML = "RAW:" + resp.ToString() + (char)13 + (char)10;
                    }
                    else
                    {
                        HeartSIP_Refund(amt, "Debit");
                    }

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

                    m_ccp.ExecuteRefundCommand(amt, Request_ID, creditcardname);

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
            if (UsingPAX) ReceiptPrinterModel.PrintResponse("", POSMessage);
            else if (CCP.POSMessage != "") ReceiptPrinterModel.PrintResponse(CCP.LastCommand, CCP.POSMessage);
        }


        public void ExecutePOSXMLClicked(object obj)
        {
            if (UsingHeartSIP)
                if (CCP.POSXML != "") ReceiptPrinterModel.PrintResponse(CCP.LastCommand, CCP.POSXML);
        }


        public void ExecuteExitClicked(object obj)
        {

            m_parent.Close();
        }




    }
}
