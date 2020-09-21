using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using TriPOS.ResponseModels;

namespace RedDot
{
    public class SettleTicketsVM : INPCBase
    {
      
        public ICommand GratuityClicked { get; set; }

        public ICommand TipClicked { get; set; }

        public ICommand SelectClicked { get; set; }
        public ICommand PrintOrderClicked { get; set; }

        public ICommand PrintReceiptClicked { get; set; }

        public ICommand PrintCreditSlipClicked { get; set; }

        public ICommand CaptureClicked { get; set; }

        public ICommand PaymentDeleteClicked { get; set; }

        public ICommand CloseBatchClicked { get; set; }
        public ICommand QueryBatchClicked { get; set; }
        public ICommand QueryItemBatchClicked { get; set; }
        public ICommand BackClicked { get; set; }

        public ICommand CashierInOutClicked { get; set; }

        public ICommand TodayClicked { get; set; }
        public ICommand CustomClicked { get; set; }

        public ICommand PreviousClicked { get; set; }

        public ICommand NextClicked { get; set; }
   

        private QSHistory _history;


        Window m_parent;
        SecurityModel m_security;
        Employee m_currentemployee;


        DataTable _authdata;
        DataTable _settledtickets;


        private DateTime _startdate;
        private DateTime _enddate;
        private int _subid;

        public Visibility VisibleBarTabCustomer { get; set; }
        public Visibility VisibleCustomer { get; set; }


        private SalesModel m_salesmodel;
        private PaymentModel m_paymentmodel;

        IDeviceInterface _device;  //Heartland - GlobalPayments - PAX 300
        private TriPOSModel triposmodel = new TriPOSModel(GlobalSettings.Instance.LaneId);  //Vantiv - WorldPay
        private VirtualPaymentModel virtualpaymentmodel = new VirtualPaymentModel();


        private bool integrated = (GlobalSettings.Instance.CreditCardProcessor != "External");  // external - standalone


        private string CreditCardProcessor = GlobalSettings.Instance.CreditCardProcessor;

        BackgroundWorker worker = new BackgroundWorker();


        private int totalcount = 0;
        private int currentcount = 0;

        public SettleTicketsVM(Window parent, SecurityModel security)
        {
            m_security = security;
            m_parent = parent;


            _history = new QSHistory();
            m_salesmodel = new SalesModel(m_security);
            m_paymentmodel = new PaymentModel(m_security);


            VisibleTicket = Visibility.Collapsed;
            VisibleMessage = Visibility.Collapsed;
            VisibleBarTabCustomer = Visibility.Visible;
            VisibleCustomer = Visibility.Visible;

            GratuityClicked = new RelayCommand(ExecuteGratuityClicked, param => this.CanExecuteGratuity);
            TipClicked = new RelayCommand(ExecuteTipClicked, param => true);

            PrintCreditSlipClicked = new RelayCommand(ExecutePrintCreditSlipClicked, param => HasCreditDebitPayment);

            SelectClicked = new RelayCommand(ExecuteSelectClicked, param => true);

            PrintReceiptClicked = new RelayCommand(ExecutePrintReceiptClicked, param => this.CanExecutePrintReceipt);
  
            CaptureClicked = new RelayCommand(ExecuteCaptureClicked, param => this.CanExecuteCapture);
            PaymentDeleteClicked = new RelayCommand(ExecutePaymentDeleteClicked, param => this.CanExecutePaymentClicked);
            CloseBatchClicked = new RelayCommand(ExecuteCloseBatchClicked, param => this.CanExecuteCloseBatch);
            QueryBatchClicked = new RelayCommand(ExecuteQueryBatchClicked, param => true);
            QueryItemBatchClicked = new RelayCommand(ExecuteQueryItemBatchClicked, param => true);
            BackClicked = new RelayCommand(ExecuteBackClicked, param => true);
            CashierInOutClicked = new RelayCommand(ExecuteCashierInOutClicked, param => this.CanClockOut);

            PreviousClicked = new RelayCommand(ExecutePreviousClicked, param => true);
            NextClicked = new RelayCommand(ExecuteNextClicked, param => true);
            TodayClicked = new RelayCommand(ExecuteTodayClicked, param => true);

            CustomClicked = new RelayCommand(ExecuteCustomClicked, param => true);

  

            m_currentemployee = new Employee(0);
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;


            VisibleCustomer = Visibility.Collapsed;
            VisibleBarTabCustomer = Visibility.Collapsed;

            worker.DoWork += RunCapture;

            LoadOrders();

            if (CreditCardProcessor == "PAX_S300")
            {
                _device = DeviceService.Create(new ConnectionConfig
                {
                    DeviceType = DeviceType.PAX_S300,
                    ConnectionMode = ConnectionModes.TCP_IP,
                    IpAddress = GlobalSettings.Instance.SIPDefaultIPAddress,
                    Port = "10009",
                    Timeout = 30000
                });

                //initialize logging event
                _device.OnMessageSent += (message) =>
                {
                    logger.Info("SENT:" + message);
                };

           
            }
        }



        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        ____                            _         
        // |  _ \ _   _| |__ | (_) ___  |  _ \ _ __ ___  _ __   ___ _ __| |_ _   _ 
        // | |_) | | | | '_ \| | |/ __| | |_) | '__/ _ \| '_ \ / _ \ '__| __| | | |
        // |  __/| |_| | |_) | | | (__  |  __/| | | (_) | |_) |  __/ |  | |_| |_| |
        // |_|    \__,_|_.__/|_|_|\___| |_|   |_|  \___/| .__/ \___|_|   \__|\__, |
        //                                              |_|                  |___/ 
        //------------------------------------------------------------------------------------------

        private string m_message;
        public string Message
        {
            get { return m_message; }
            set
            {
                m_message = value;
                NotifyPropertyChanged("Message");
            }
        }

        private decimal _tickettotal;
        public decimal TicketTotal
        {
            get { return _tickettotal; }
            set
            {
                _tickettotal = value;
                NotifyPropertyChanged("TicketTotal");
            }
        }


        private decimal _batchtotal;
        public decimal BatchTotal
        {
            get { return _batchtotal; }
            set
            {
                _batchtotal = value;
                NotifyPropertyChanged("BatchTotal");
            }
        }

        private int _numberticket;
        public int NumberTicket
        {
            get { return _numberticket; }
            set
            {
                _numberticket = value;
                NotifyPropertyChanged("NumberTicket");
            }
        }

        private int _numbervoid;
        public int NumberVoid
        {
            get { return _numbervoid; }
            set
            {
                _numbervoid = value;
                NotifyPropertyChanged("NumberVoid");
            }
        }



        private Ticket m_currentticket;
        public Ticket CurrentTicket
        {
            get { return m_currentticket; }

            set
            {
                m_currentticket = value;
                NotifyPropertyChanged("CurrentTicket");
            }
        }


        public int SubID
        {
            get { return _subid; }
            set
            {
                _subid = value;
                NotifyPropertyChanged("SubID");
            }
        }

        public DateTime StartDate
        {
            get { return _startdate; }
            set { _startdate = value; NotifyPropertyChanged("StartDate"); }
        }

        public DateTime EndDate
        {
            get { return _enddate; }
            set { _enddate = value; NotifyPropertyChanged("EndDate"); }
        }



        public DataTable AuthTickets
        {
            get{ return _authdata; }
            set
            {
                _authdata = value;
                NotifyPropertyChanged("AuthTickets");
            }
        }


        public DataTable SettledTickets
        {
            get { return _settledtickets; }
            set
            {
                _settledtickets = value;
                NotifyPropertyChanged("SettledTickets");
            }
        }

        private Visibility m_visibleticket;
        public Visibility VisibleTicket
        {
            get
            {
                return m_visibleticket;
            }
            set
            {
                m_visibleticket = value;
                NotifyPropertyChanged("VisibleTicket");
            }
        }

        private Visibility m_visiblemessage;
        public Visibility VisibleMessage
        {
            get { return m_visiblemessage; }
            set
            {
                m_visiblemessage = value;
                NotifyPropertyChanged("VisibleMessage");
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

        public bool CanClockOut
        {
            get
            {
                if (m_security.CurrentEmployee == null) return false;
                return m_security.CurrentEmployee.ClockedIn;

            }
        }

        public bool CanExecutePaymentClicked
        {
            get
            {

                if (CurrentTicket == null) return false;

                if (CurrentTicket.Status != "Voided" && CurrentTicket.Status != "Closed")
                {
                    return true;
                }
                else return false;
            }

        }



    


        public bool CanExecuteCapture
        {
            get; set;
        }

        private bool m_canexecuteclosebatch;
        public bool CanExecuteCloseBatch
        {get { return m_canexecuteclosebatch; }
            set
            {
                m_canexecuteclosebatch = value;
                NotifyPropertyChanged("CanExecuteCloseBatch");
            }
        }


        public bool CanExecuteNotClosed
        {
            get
            {
                if (CurrentTicket == null) return false;

                if (CurrentTicket.Status != "Closed")
                {
                    return true;
                }
                else return false;
            }

        }
        public bool HasCreditDebitPayment
        {
            get
            {
                if (!integrated) return false;  //no credit slips if not integrated


                if (CanExecutePrintReceipt)
                {
                    return (CurrentTicket.HasDebitPayment || (CurrentTicket.HasCreditPayment && CurrentTicket.HasAuthorCode) || CurrentTicket.HasExternalGiftPayment);
                }
                else return false;




            }
        }

        public bool CanExecutePrintReceipt
        {
            get
            {
                if (CurrentTicket == null)
                {
                    return false;
                }
                else
                {
                    if (CurrentTicket.ActiveItemCount > 0) return true;
                    else return false;

                }

            }
        }



        public bool IsClosedVoidedTicket(object salesid = null)
        {
            if (CurrentTicket == null) return false;

            if (CurrentTicket.Status == "Closed" || CurrentTicket.Status == "Voided") return true;
            else return false;



        }



        public bool CanExecuteClosedTicket
        {
            get
            {
                if (m_currentticket == null) return false;
                if (m_currentticket.Status == "Closed")
                {
                    return true;
                }
                else return false;
            }
        }



   

        public bool IsOpenTicket(object salesid = null)
        {
            if (CurrentTicket == null) return false;

            if (CurrentTicket.Status == "Open") return true;
            else return false;


        }
        public bool IsClosedTicket(object salesid = null)
        {
            if (CurrentTicket == null) return false;

            if (CurrentTicket.Status == "Closed") return true;
            else return false;



        }

        public bool CanExecuteGratuity
        {

            get
            {
               


                if (CanExecuteNotNullTicket && CurrentTicket.Status == "Closed" && (CurrentTicket.HasAUTH)) return true;
                else return false;

            }

        }

        public bool CanExecuteNotNullTicket
        {
            get
            {
                if (CurrentTicket != null) return true;
                else return false;
            }
        }


        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        __  __      _   _               _     
        // |  _ \ _   _| |__ | (_) ___  |  \/  | ___| |_| |__   ___   __| |___ 
        // | |_) | | | | '_ \| | |/ __| | |\/| |/ _ \ __| '_ \ / _ \ / _` / __|
        // |  __/| |_| | |_) | | | (__  | |  | |  __/ |_| | | | (_) | (_| \__ \
        // |_|    \__,_|_.__/|_|_|\___| |_|  |_|\___|\__|_| |_|\___/ \__,_|___/
        //    
        //------------------------------------------------------------------------------------------  


  
        public void LoadOrders()
        {


            //if user has access to all sales
            if (m_security.HasAccess("AllSales"))
            {
                AuthTickets = _history.GetClosedVoidedOrders(StartDate, EndDate, 0,"AUTH",false);
                SettledTickets = _history.GetClosedVoidedOrders(StartDate, EndDate, 0,"SETTLED",false);
            }
            else
            {
                AuthTickets = _history.GetClosedVoidedOrders(StartDate, EndDate, m_security.CurrentEmployee.ID,"AUTH",false);
                SettledTickets = _history.GetClosedVoidedOrders(StartDate, EndDate, m_security.CurrentEmployee.ID,"SETTLED",false);
            }


            DataTable batchrecord = _history.GetCreditDebitPayments(StartDate, EndDate);
            if (batchrecord.Rows.Count > 0)
            {
                string str = batchrecord.Rows[0]["netamount"].ToString();
                if (str != "")
                    BatchTotal = decimal.Parse(str);
                else
                    BatchTotal = 0;
            }
            else BatchTotal = 0;




            VisibleTicket = Visibility.Collapsed;
            decimal tickettotal = 0;
            int numberticket = 0;
            int numbervoid = 0;
            int id = 0;
            CanExecuteCapture = false;
            CanExecuteCloseBatch = true;
            bool HasEmptyTip = false;


            foreach (DataRow row in AuthTickets.Rows)
            {
                id = (int)row["id"];

                if (CurrentTicket != null)
                    if (id == CurrentTicket.SalesID)
                    {
                        row["selected"] = true;
                        VisibleTicket = Visibility.Visible;
                        CurrentTicket = SalesModel.GetTicket(id);
                    }



                if (row["status"].ToString() == "Voided")
                {
                    numbervoid++;
                    row["transtype"] = "void";
                }
                else
                {
                    numberticket++;
                    string total = row["total"].ToString();

                    if (total != "")
                        tickettotal += decimal.Parse(total);

                    if (row["transtype"].ToString() == "AUTH")
                    {
                        CanExecuteCapture = true;  //if atleast one item is not settled
                        CanExecuteCloseBatch = false;  // if one is not settled , can not close batch
                    }

                    if (row["tip"].ToString().Contains("empty")) HasEmptyTip = true;
                }



            }


            if (GlobalSettings.Instance.MustEnterAllTipBeforeSettlement)
                if (HasEmptyTip) CanExecuteCapture = false;



            NumberTicket = numberticket;
            NumberVoid = numbervoid;

            TicketTotal = tickettotal;
        }


        public void ExecuteSelectClicked(object salesid)
        {

            try
            {
                int id;

                if (salesid == null) return;

                if (salesid.ToString() != "") id = int.Parse(salesid.ToString());
                else id = 0;

                //BaseLoadTicket((int)salesid);
                CurrentTicket = SalesModel.GetTicket(id);

                VisibleTicket = Visibility.Visible;

        

                NotifyPropertyChanged("VisibleCustomer");
                NotifyPropertyChanged("VisibleBarTabCustomer");
            }
            catch (Exception e)
            {
                MessageBox.Show("Execute Select Clicked: " + e.Message);
            }
        }


        public void ExecuteGratuityClicked(object salesid)
        {

            try
            {
         

                List<Payment> payment = CurrentTicket.Payments.Where(x => x.CardGroup.ToUpper() == "CREDIT").Where(x => x.Voided == false).ToList();

                if (payment.Count() >0)
                {
                    foreach(Payment pay in payment)
                    {
                        NumPad np = new NumPad("Enter Tip Amount for " + pay.AuthorCode + ":", false, false, pay.TipAmount.ToString());
                        Utility.OpenModal(m_parent, np);


                        if (np.Amount == "") continue;

                        if (np.Amount != CurrentTicket.TipAmount.ToString())
                        {
                            decimal amt = decimal.Parse(np.Amount);

                            switch (GlobalSettings.Instance.CreditCardProcessor)
                            {

                                case "HeartSIP":
                                    Random rand = new Random();

                                    //  m_ccp.TipPaymentId = payment.ID;
                                    //  m_ccp.TipRequestId = rand.Next(1000, 9999999).ToString();
                                    //  m_ccp.ExecuteAdjustTipCommand(amt, payment.ResponseId, m_ccp.TipRequestId);
                                    break;


                                default:
                                    CurrentTicket.UpdateGratuity(pay.ID, amt);
                                    break;
                            }


                        }
                    }
                   


                    LoadOrders();
                    m_parent.Focus();

                }
                else
                {
                    TouchMessageBox.Show("No Credit transaction available!!");
                }







            }
            catch (Exception ex)
            {
                MessageBox.Show("SalesVM:ExecuteGratuityClicked: " + ex.Message);
            }

        }

        public void ExecuteTipClicked(object salesid)
        {

            try
            {
                int id = int.Parse(salesid.ToString());
                Ticket thisticket = new Ticket(id);

               List<Payment> payment = thisticket.Payments.Where(x => x.CardGroup.ToUpper() == "CREDIT").Where(x=> x.Voided == false).ToList();
         


                if (payment.Count() > 0)
                {
                    foreach (Payment pay in payment)
                    {
                        NumPad np = new NumPad("Enter Tip Amount for " + pay.AuthorCode + ":", false, false, pay.TipAmount.ToString());
                        Utility.OpenModal(m_parent, np);


                        if (np.Amount == "") continue;

                        if (np.Amount != thisticket.TipAmount.ToString())
                        {
                            decimal amt = decimal.Parse(np.Amount);

                            switch (GlobalSettings.Instance.CreditCardProcessor)
                            {

                                case "HeartSIP":
                                    Random rand = new Random();

                                    //  m_ccp.TipPaymentId = payment.ID;
                                    //  m_ccp.TipRequestId = rand.Next(1000, 9999999).ToString();
                                    //  m_ccp.ExecuteAdjustTipCommand(amt, payment.ResponseId, m_ccp.TipRequestId);
                                    break;


                                default:
                                    thisticket.UpdateGratuity(pay.ID, amt);
                                    break;
                            }


                        }
                    }


                    LoadOrders();

                }
                else
                {
                    TouchMessageBox.Show("No Credit transaction available!!");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("SalesVM:ExecuteGratuityClicked: " + ex.Message);
            }

        }


        public void ExecutePrintReceiptClicked(object button)
        {
            ReceiptPrinterModel.PrintReceipt(CurrentTicket, GlobalSettings.Instance.ReceiptPrinter);
        }

        public void ExecutePrintCreditSlipClicked(object param)
        {
            string location = param.ToString();

            var result = CurrentTicket.Payments.Where(x => x.CardGroup == "DEBIT" || x.CardGroup == "CREDIT" || (x.CardGroup == "GIFT" && x.CardType == "GIFT/REWARDS"));
            if (result != null)
            {
                if (result.Count() == 0) return;

                if (result.Count() > 1)
                {
                    List<CustomList> list = new List<CustomList>();



                    foreach (var item in result)
                    {
                        list.Add(new CustomList { returnstring = item.ID.ToString(), description = item.CardType + " " + item.AmountStr });
                    }
                    ListPad lp = new ListPad("Pick slip to print:", list);
                    Utility.OpenModal(m_parent, lp);

                    if (lp.ReturnString != "")
                    {
                        result = CurrentTicket.Payments.Where(x => x.ID.ToString() == lp.ReturnString);

                    }
                }


                ReceiptPrinterModel.PrintCreditSlip(result.First(), "**---COPY---**");

            }


        }


        public void ExecuteCaptureClicked(object obj)
        {
            if (m_security.WindowNewAccess("SettleTicket") == false)
            {
                return;
            }

            TouchMessageBox.Show("*** Settlement Started ***  This may take a few minutes.." , 3);

            worker.RunWorkerAsync();
        }

        private void RunCapture(object sender, DoWorkEventArgs e)
        {
         

            var payments = _history.GetAUTHPayments(StartDate, EndDate);
            var processor = GlobalSettings.Instance.CreditCardProcessor.ToUpper();

            //call invoke so error message can interact with gui
            m_parent.Dispatcher.Invoke(new Action(() =>
            {
                totalcount = payments.Rows.Count;
                currentcount = 0;
                foreach (DataRow payment in payments.Rows)
                {
                    currentcount++;
                    switch (processor)
                    {
                        case "PAX_S300":
                            CapturePAX(payment);
                            break;
                        case "CLOVER":
                            CaptureClover(payment);
                            break;
                        case "VANTIV":
                            CaptureVantiv(payment);
                            break;

                        case "VIRTUAL":
                            CaptureVirtual(payment);
                            break;
                    }
                }
            }));

            LoadOrders();
       
        }

        private void CapturePAX(DataRow payment)
        {

            _device.Reset();

            decimal NetAmount = decimal.Parse(payment["netamount"].ToString());
            decimal TipAmount = decimal.Parse(payment["tipamount"].ToString());
            string ResponseId = payment["responseid"].ToString();
            int ID = int.Parse(payment["id"].ToString());


            logger.Info("COMMAND:Credit Capture , Amount=" + NetAmount + ",Tip=" + TipAmount);


            TerminalResponse resp = _device.CreditCapture(ID, NetAmount + TipAmount)
                                        .WithTransactionId(ResponseId)
                                        .Execute();

            logger.Info("Credit Capture:RECEIVED:" + resp.ToString());
            logger.Info("Credit Capture:HRef=" + resp.TransactionId);
            if (resp.DeviceResponseText.ToUpper() == "OK")
            {
                Message = "Settled:" + ResponseId + " Amount:" + (NetAmount + TipAmount).ToString();
                m_salesmodel.UpdatePaymentCapture(ID, TipAmount, (decimal)resp.TransactionAmount, NetAmount, resp.TransactionId);

                TouchMessageBox.ShowSmall("Settle ID: " + ResponseId + " Amount: " + (NetAmount + TipAmount).ToString() + (char)13 + (char)10 + currentcount.ToString() + " out of  " + totalcount.ToString(), 1);
            }
            else
            {
                logger.Error(resp.DeviceResponseText);
                TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);
            }



        }

        private void CaptureClover(DataRow payment)
        {





        }



        private void CaptureVantiv(DataRow payment)
        {



            decimal NetAmount = decimal.Parse(payment["netamount"].ToString());
            decimal TipAmount = decimal.Parse(payment["tipamount"].ToString());
            string ResponseId = payment["responseid"].ToString();
            int ID = int.Parse(payment["id"].ToString());


            logger.Info("COMMAND:Credit Capture , Amount=" + NetAmount + ",Tip=" + TipAmount);

            string referencenumber = Utility.GenerateReferenceNumber();
            AuthorizationCompletionResponse resp = triposmodel.ExecuteCreditAuthorizeCompletion(ResponseId, NetAmount + TipAmount,referencenumber);
            logger.Info("CREDIT COMPLETE=> reference number:" + referencenumber + ", ticket=" + payment["salesid"] + ",responseid=" + ResponseId +  ", amount=" + NetAmount + TipAmount);



            logger.Info("Credit Capture:RECEIVED:" + resp.ToString());
            logger.Info("Credit Capture:HRef=" + resp.TransactionId);
            if (resp.StatusCode == "Approved")
            {
                
                m_salesmodel.UpdatePaymentCapture(ID, TipAmount, (decimal)resp.TotalAmount, NetAmount, resp.TransactionId);

                TouchMessageBox.ShowSmall("Settle ID: " + ResponseId + " Amount: " + (NetAmount + TipAmount).ToString() + (char)13 + (char)10 + currentcount.ToString() + " out of  " + totalcount.ToString(), 1);

            }
            else
            {
                logger.Error(resp.StatusCode);
                TouchMessageBox.Show("CAPTURE FAILED !!!! ERROR:  " + resp.StatusCode);
            }



        }


        private void CaptureVirtual(DataRow payment)
        {



            decimal NetAmount = decimal.Parse(payment["netamount"].ToString());
            decimal TipAmount = decimal.Parse(payment["tipamount"].ToString());
            string ResponseId = payment["responseid"].ToString();
            int ID = int.Parse(payment["id"].ToString());


            logger.Info("COMMAND:Credit Capture , Amount=" + NetAmount + ",Tip=" + TipAmount);


            AuthorizationCompletionResponse resp = virtualpaymentmodel.ExecuteCreditAuthorizeCompletion(ResponseId, NetAmount + TipAmount);

            logger.Info("Credit Capture:RECEIVED:" + resp.ToString());
            logger.Info("Credit Capture:HRef=" + resp.TransactionId);
            if (resp.StatusCode.ToUpper() == "APPROVED")
            {
                Message = "Settled:" + ResponseId + " Amount:" + (NetAmount + TipAmount).ToString();
                m_salesmodel.UpdatePaymentCapture(ID, TipAmount, (decimal)resp.TotalAmount, NetAmount, resp.TransactionId);

                TouchMessageBox.ShowSmall("Settle ID: " + ResponseId + " Amount: " + (NetAmount + TipAmount).ToString() + (char)13 + (char)10 + currentcount.ToString() + " out of  " + totalcount.ToString(), 1);

            }
            else
            {
                logger.Error(resp.StatusCode);
                
                TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + resp.StatusCode);
            }



        }

 


        public void ExecutePaymentDeleteClicked(object paymentid)
        {
            m_paymentmodel.PaymentDelete(CurrentTicket, (int)paymentid, m_parent);
        }


        public void ExecuteCloseBatchClicked(object obj)
        {
            if (m_security.WindowNewAccess("CloseBatch") == false)
            {

                return;
            }

   


            switch (GlobalSettings.Instance.CreditCardProcessor.ToUpper())
            {
                case "PAX_S300":
                    try
                    {

                        string POSMessage = "";
                   


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


                        logger.Info("BatchClose:RECEIVED:" + resp.ToString());
                        logger.Info("Status:" + resp.DeviceResponseText + "  BatchNumber:" + resp.SequenceNumber);
                        logger.Info("Total Count:" + resp.TotalCount + "  Total Amount:" + resp.TotalAmount);

                        logger.Info(POSMessage);

                        ReceiptPrinterModel.PrintResponse("Batch Close", POSMessage);

                        TouchMessageBox.Show(POSMessage);


                    }
                    catch (Exception ex)
                    {
                        TouchMessageBox.Show("Error Closing Batch:" + ex.Message);

                    }
                    break;


                case "CLOVER":



                    break;

                case "VANTIV":
                    TriPOSModel tripos = new TriPOSModel(GlobalSettings.Instance.LaneId);

                    BatchCloseResponse result = tripos.ExecuteBatch();
                    if(result == null)
                    {
                        TouchMessageBox.Show("Error Sending Batch Command.");
                        return;
                    }
                    if (result.ExpResponse.ExpressResponseCode == 0)
                    {
                        string message = "Credit Count:" + result.ExpResponse.ExpBatch.HostBatchCount + " Credit Amount:" + result.ExpResponse.ExpBatch.HostBatchAmount + "\r\n" +
                            "Refund Count:" + result.ExpResponse.ExpBatch.HostCreditReturnCount + " Refund Amount:" + result.ExpResponse.ExpBatch.HostCreditReturnAmount + "\r\n" +
                            "Total Count:" + result.ExpResponse.ExpBatch.HostBatchCount + " Total Amount:" + result.ExpResponse.ExpBatch.HostBatchAmount;
                        ReceiptPrinterModel.PrintResponse("Batch Close", message);
                        TouchMessageBox.Show("Batch Close:\r\n" + message);
                    }
                    else
                    {
                        TouchMessageBox.Show("Batch Close:\r\n" + result.ExpResponse.ExpressResponseMessage);
                    }

                    break;
            }


            LoadOrders();

        }


        public void ExecuteQueryBatchClicked(object obj)
        {


            switch (GlobalSettings.Instance.CreditCardProcessor.ToUpper())
            {
                case "PAX_S300":
                    break;


                case "CLOVER":

                    break;

                case "VANTIV":
                    TriPOSModel tripos = new TriPOSModel(GlobalSettings.Instance.LaneId);

                    BatchTotalsQueryResponse result = tripos.ExecuteBatchTotalsQuery();


                    if (result.ExpResponse.ExpressResponseCode == 0)
                    {
                        string message = "Credit Count:" + result.ExpResponse.ExpBatch.HostBatchCount + " Credit Amount:" + result.ExpResponse.ExpBatch.HostBatchAmount + "\r\n" +
                            "Refund Count:" + result.ExpResponse.ExpBatch.HostCreditReturnCount + " Refund Amount:" + result.ExpResponse.ExpBatch.HostCreditReturnAmount + "\r\n" +
                            "Total Count:" + result.ExpResponse.ExpBatch.HostBatchCount + " Total Amount:" + result.ExpResponse.ExpBatch.HostBatchAmount;
                        TouchMessageBox.Show("Batch Query:\r\n" + message);
                    }
                    else
                    {
                        TouchMessageBox.Show("Batch Query:\r\n" + result.ExpResponse.ExpressResponseMessage);
                    }

                    break;
            }




        }


        public void ExecuteQueryItemBatchClicked(object obj)
        {


            switch (GlobalSettings.Instance.CreditCardProcessor.ToUpper())
            {
                case "PAX_S300":
                    break;


                case "CLOVER":

                    break;

                case "VANTIV":
                    TriPOSModel tripos = new TriPOSModel(GlobalSettings.Instance.LaneId);

                    BatchItemQueryResponse result = tripos.ExecuteBatchItemQuery(1);


                    if (result.ExpResponse.ExpressResponseCode == 0)
                    {
                        string message = "Credit Count:" + result.ExpResponse.ExpBatch.HostBatchCount + " Credit Amount:" + result.ExpResponse.ExpBatch.HostBatchAmount + "\r\n" +
                            "Refund Count:" + result.ExpResponse.ExpBatch.HostCreditReturnCount + " Refund Amount:" + result.ExpResponse.ExpBatch.HostCreditReturnAmount + "\r\n" +
                            "Total Count:" + result.ExpResponse.ExpBatch.HostBatchCount + " Total Amount:" + result.ExpResponse.ExpBatch.HostBatchAmount;
                        TouchMessageBox.Show("Batch Query:\r\n" + message);
                    }
                    else
                    {
                        TouchMessageBox.Show("Batch Query:\r\n" + result.ExpResponse.ExpressResponseMessage);
                    }

                    break;
            }




        }


        public void ExecuteBackClicked(object obj)
        {
       
                m_parent.Close();
        }

        private void ExecuteCashierInOutClicked(object sender)
        {
            if (m_security.WindowNewAccess("CashierInOut","",GlobalSettings.Instance.FingerPrintRequiredForTimeCard))
            {
                CashierInOut rpt = new CashierInOut(m_security,false);
                Utility.OpenModal(m_parent, rpt);

            }
        }


        public void ExecuteTodayClicked(object salesid)
        {

            try
            {
                StartDate = DateTime.Today;
                EndDate = DateTime.Today;

                LoadOrders();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecutePreviousClicked(object obj)
        {

            try
            {
                StartDate = StartDate.AddDays(-1);
                EndDate = StartDate;
                CurrentTicket = null;

                LoadOrders();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecuteNextClicked(object obj)
        {

            try
            {
                StartDate = StartDate.AddDays(1);
                EndDate = StartDate;
                CurrentTicket = null;

                LoadOrders();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecuteCustomClicked(object salesid)
        {

            try
            {
                CustomDate cd = new CustomDate(Visibility.Visible);
                Utility.OpenModal(m_parent, cd);


                StartDate = cd.StartDate;
                EndDate = cd.EndDate;


                LoadOrders();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }

  
    }
}
