using Clover;
using com.clover.remotepay.sdk;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;
using GlobalPayments.Api.Terminals.Abstractions;
using NLog;
using RedDot.Models;
using RedDot.Models.CardConnect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TriPOS.ResponseModels;

namespace RedDot
{
    public class OrderHistoryVM : SalonSalesBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
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
        private History _history;

        private bool CanExecute = true;

        Window m_parent;
      
        Employee m_currentemployee;

        DataTable _historydata;
        DataTable _reversedtickets;

       

        private DateTime _startdate;
        private DateTime _enddate;
        private int _subid;


    
   

     
        public ICommand VoidClicked { get; set; }

       public ICommand EditClicked { get; set; }
        public ICommand SelectClicked { get; set; }
        public ICommand PrintOrderClicked { get; set; }

        public ICommand TodayClicked { get; set; }
        public ICommand PreviousClicked { get; set; }

        public ICommand NextClicked { get; set; }

        public ICommand Past7DaysClicked { get; set; }
        public ICommand CustomClicked { get; set; }

        public ICommand ByTicketIDClicked { get; set; }


        public ICommand PrintReceiptClicked { get; set; }

      

        public ICommand ReverseOrderClicked { get; set; }
        public ICommand GratuityClicked { get; set; }

       // public ICommand SettleClicked { get; set; }

      
        public ICommand CustomerClicked { get; set; }

        public ICommand CaptureClicked { get; set; }

        public ICommand QueryBatchClicked { get; set; }

        public ICommand CloseBatchClicked { get; set; }


        public ICommand RefundClicked { get; set; }

        public ICommand MarkPaidClicked { get; set; }
        public ICommand BackClicked { get; set; }
        private bool External = false;
   


        private HeartPOS m_ccp;
        private SalesModel m_salesmodel;

        IDeviceInterface _device;  //PAX 300
        ICloverConnector cloverConnector;
        CloverListener ccl;

        private TriPOSModel triposmodel = new TriPOSModel(GlobalSettings.Instance.LaneId);  //Vantiv - WorldPay
   



        BackgroundWorker worker = new BackgroundWorker();
    

        public OrderHistoryVM(Window parent, SecurityModel security, HeartPOS ccp):base(parent)
        {

            _history            = new History();
            m_salesmodel = new SalesModel(null);
            m_security          = security;
            m_parent            = parent;
            m_ccp = ccp;
            VisibleTicket = Visibility.Collapsed;
      

            EditClicked         = new RelayCommand(ExecuteEditClicked, param => this.IsReversedTicket(param));
            SelectClicked = new RelayCommand(ExecuteSelectClicked, param => true);

            TodayClicked        = new RelayCommand(ExecuteTodayClicked, param => this.CanExecute);
            PreviousClicked     = new RelayCommand(ExecutePreviousClicked, param => this.CanExecute);
            NextClicked         = new RelayCommand(ExecuteNextClicked, param => this.CanExecute);
            Past7DaysClicked    = new RelayCommand(ExecutePast7DaysClicked, param => this.CanExecute);
            CustomClicked       = new RelayCommand(ExecuteCustomClicked, param => this.CanExecute);

            ByTicketIDClicked   = new RelayCommand(ExecuteByTicketIDClicked, param => this.CanExecute);



    
            GratuityClicked     = new RelayCommand(ExecuteEditGratuity, param => this.CanExecuteGratuityClicked);
            ReverseOrderClicked = new RelayCommand(ExecuteReverseTicket, param => this.CanExecuteClosedTicket);
        
            PrintReceiptClicked = new RelayCommand(ExecutePrintReceiptClicked, param => CanExecutePrintReceipt);


           // SettleClicked = new RelayCommand(ExecuteSettleClicked, param => this.CanExecuteNotClosed);


            CaptureClicked = new RelayCommand(ExecuteCaptureClicked, param => this.CanExecuteCapture);
            CustomerClicked = new RelayCommand(ExecuteCustomerClicked, param => this.CanExecuteCustomer);

            CloseBatchClicked = new RelayCommand(ExecuteCloseBatchClicked, param => this.CanExecuteCloseBatch);

            QueryBatchClicked = new RelayCommand(ExecuteQueryBatchClicked, param => true);

            RefundClicked = new RelayCommand(ExecuteRefundClicked, param => this.CanExecuteRefundClicked);

            MarkPaidClicked = new RelayCommand(ExecuteMarkPaidClicked, param => this.CanExecuteNotClosed);
            BackClicked = new RelayCommand(ExecuteBackClicked, param => true);


            m_currentemployee   = new Employee(0);
            StartDate           = DateTime.Today;
            EndDate             = DateTime.Today;

            worker.DoWork += RunCapture;


            LoadHistory();

    

            switch (GlobalSettings.Instance.CreditCardProcessor)
            {
                case "PAX_S300":
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

                    //  _device.Initialize();
                    // _device.Reset();
                    break;

                case "Clover":
                    //initialize device
                    cloverConnector = GlobalSettings.Instance.cloverConnector;
                    ccl = GlobalSettings.Instance.ccl;

                    break;
                case "External":
                    External = true;
                    break;

            }


        }




        public bool CanExecuteCapture
        {
            get;set;
        }

        public bool CanExecuteCloseBatch
        {
            get
            {
                return !External ;
            }
        }


        public bool CanExecuteRefundClicked
        {
            get
            {
                return CanExecuteReversedTicket;
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
        private decimal _cashtotal;
        public decimal CashTotal
        {
            get { return _cashtotal; }
            set
            {
                _cashtotal = value;
                NotifyPropertyChanged("CashTotal");
            }
        }

        private decimal _credittotal;
        public decimal CreditTotal
        {
            get { return _credittotal; }
            set
            {
                _credittotal = value;
                NotifyPropertyChanged("CreditTotal");
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


        public int SubID
        { get { return _subid; }
            set
            {
                _subid = value;
                NotifyPropertyChanged("SubID");
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

        public DataTable HistoryData
        {
            get
            {
                return _historydata;
            }

            set
            {
                _historydata = value;
                NotifyPropertyChanged("HistoryData");
            }

        }


        public DataTable ReversedTickets
        {
            get { return _reversedtickets; }
            set
            {
                _reversedtickets = value;
                NotifyPropertyChanged("ReversedTickets");
            }
        }






        public bool IsReversedTicket(object salesid = null)
        {
            if (CurrentTicket == null) return false;

            if (CurrentTicket.Status == "Reversed") return true;
            else return false;


        }

        public bool IsClosedVoidedTicket(object salesid = null)
        {
            int id;

            if (salesid == null) return false;

            if (salesid.ToString() != "") id = int.Parse(salesid.ToString());
            else id = 0;

            if (Ticket.GetTicketStatus(id) == "Closed" || Ticket.GetTicketStatus(id) == "Voided") return true;
            else return false;



        }

  

        private bool m_ischangedue;
        public bool IsChangeDue
        {
            get { return m_ischangedue; }
            set { m_ischangedue = value; NotifyPropertyChanged("IsChangeDue"); }
        }

        private Visibility m_paidinfull;
        public Visibility PaidInFull
        {
            get
            {
                return m_paidinfull;
            }
            set
            {
                m_paidinfull = value;
                NotifyPropertyChanged("PaidInFull");
            }
        }

        private Visibility m_visibleclosed;
        public Visibility VisibleClosed
        {
            get
            {
                return m_visibleclosed;
            }
            set
            {
                m_visibleclosed = value;
                NotifyPropertyChanged("VisibleClosed");
            }
        }


        private Visibility m_visiblevoided;
        public Visibility VisibleVoided
        {
            get
            {
                return m_visiblevoided;
            }
            set
            {
                m_visiblevoided = value;
                NotifyPropertyChanged("VisibleVoided");
            }
        }


        private Visibility m_creditcardsurchargevisibility;
        public Visibility CreditCardSurchargeVisibility
        {
            get
            {
                return m_creditcardsurchargevisibility;
            }
            set
            {
                m_creditcardsurchargevisibility = value;
                NotifyPropertyChanged("CreditCardSurchargeVisibility");
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

        /*

        public void ExecuteGratuityClicked(object salesid)
        {

            try
            {

                //If Nail Salon, then assign to multiple techs
                if (GlobalSettings.Instance.Shop.Type == "Salon")
                {
                    int newsalesid = 0;
                    if (salesid != null) newsalesid = (int)salesid;

                    GratuityView gv = new GratuityView(null, newsalesid);
                    Utility.OpenModal(_parent, gv);
                    //need to refresh the history list since it's disconnected
                    HistoryData = _history.GetOrders(StartDate, EndDate);

                }
                else
                {
                    //restaurant , assign to ticket or single item
                    NumPad pad = new NumPad("Enter Tip Amount:");
                    pad.Amount = "";
                    Utility.OpenModal(_parent, pad);
                    if (pad.Amount != "")
                    {
                       // _currentticket.AddGratuity(_currentemployee.ID, decimal.Parse(pad.Amount));

                    }
                }

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("SalesVM:ExecuteGratuityClicked: " + ex.Message);
            }

        }



        */
        public void LoadHistory()
        {
         
            if(m_security.HasAccess("AllSales"))
                HistoryData = _history.GetOrders(StartDate, EndDate,0,GlobalSettings.Instance.HistorySimpleView);
            else
                HistoryData = _history.GetOrders(StartDate, EndDate, m_security.CurrentEmployee.ID,GlobalSettings.Instance.HistorySimpleView);

            ReversedTickets = _history.GetReversedTickets();



            VisibleTicket = Visibility.Collapsed;
            decimal tickettotal = 0;
            int numberticket = 0;
            int numbervoid = 0;
            decimal credittotal = 0;
            decimal cashtotal = 0;
            int id = 0;
            CanExecuteCapture = false;
    
            //empty tips are only for Auth tickets ..  does not mean tickets that has tips not assigned to 
            bool HasEmptyTip = false;

            foreach(DataRow row in ReversedTickets.Rows)
            {
                id = (int)row["id"];
                if (CurrentTicket != null)
                    if (id == CurrentTicket.SalesID)
                    {
                        row["selected"] = true;
                        VisibleTicket = Visibility.Visible;
                        CurrentTicket = m_salesmodel.LoadTicket(id);
                    }
            }

           
           foreach (DataRow row in HistoryData.Rows)
           {
                id = (int)row["id"];
                if (CurrentTicket != null)
                    if (id == CurrentTicket.SalesID)
                    {
                        row["selected"] = true;
                       VisibleTicket = Visibility.Visible;
                        CurrentTicket = m_salesmodel.LoadTicket(id);
                    }

              

                if (row["status"].ToString() == "Voided") 
                {
                    numbervoid++;
                    row["transtype"] = "void";
                }
                else //not VOIDED
                {
                    string total = row["total"].ToString();
                    if (total != "")
                        tickettotal += decimal.Parse(total);

                    string cashtotalstr = row["cashtotal"].ToString();
                    if (cashtotalstr != "")
                        cashtotal += decimal.Parse(cashtotalstr);

                    string credittotalstr = row["credittotal"].ToString();
                    if (credittotalstr != "")
                        credittotal += decimal.Parse(credittotalstr);

                    string tiptotalstr = row["tipamount"].ToString();
                    if (tiptotalstr != "")
                        credittotal += decimal.Parse(tiptotalstr);


                    numberticket++;
                  
                    if (row["transtype"].ToString() == "AUTH")
                    {
                        if (row["tip"].ToString().Contains("empty")) HasEmptyTip = true;
                        CanExecuteCapture = true;  //if atleast one item is not settled
                   
                    }
                }


                  
                  
                     

           }


            if(GlobalSettings.Instance.MustEnterAllTipBeforeSettlement)
                if (HasEmptyTip) CanExecuteCapture = false;
      

            NumberTicket = numberticket;
            NumberVoid = numbervoid;
            CashTotal = cashtotal;
            CreditTotal = credittotal;

            TicketTotal = tickettotal;
        }

        public void ExecuteRefundClicked(object obj)
        {
            if (!m_security.WindowNewAccess("Refund"))
            {
                // Message("Access Denied.");
                return;
            }



            try
            {

                switch (GlobalSettings.Instance.CreditCardProcessor)
                {
                    case "HeartSIP":
                        HeartSIPRefund ccp = new HeartSIPRefund(m_parent, CurrentTicket, m_ccp, "REFUND", "");
                        Utility.OpenModal(m_parent, ccp);

                        break;

                    case "PAX_S300":
                    case "HSIP_ISC250":
                    case "VANTIV":
                    case "CardConnect":
                        PAXRefund pax = new PAXRefund(CurrentTicket);
                        Utility.OpenModal(m_parent, pax);
                        break;


                    case "Clover":
                        CloverRefund clover = new CloverRefund(CurrentTicket, m_security);
                        Utility.OpenModal(m_parent, clover);
                        break;



                    case "External":

                        CreditCardView ccv = new CreditCardView(m_parent, CurrentTicket, "REFUND");
                        Utility.OpenModal(m_parent, ccv);
                        break;

                   
              

                   

                }



                CurrentTicket.LoadPayment();
               // CurrentTicket.CloseTicket(); //need to load payment  to refresh object first before trying to close ticket
                NotifyPropertyChanged("Payments");
                //test for close - display change given and such
                LoadHistory();



            }
            catch (Exception e)
            {

                TouchMessageBox.Show(" ExecuteRefundClicked:" + e.Message);
            }
        }



        public void ExecuteMarkPaidClicked(object salesid)
        {
            if (m_security.WindowNewAccess("MarkPaid") == false)
            {

                return;
            }


            Confirm conf = new Confirm("Are you sure??");
            Utility.OpenModal(m_parent, conf);
            if (conf.Action == "Yes")
            {
                CurrentTicket.CloseTicket(true);
                //test for close
                if (CurrentTicket.Status == "Closed")
                {


                    Decimal dec = 0m;

                    SetVisibility();


                    if (CurrentTicket.Balance > 0)  //still has a balance
                    {
                        TouchMessageBox.Show(String.Format("Ticket has a Balance: {0:c}", CurrentTicket.Balance));

                    }
                    else
                    {
                        dec = CurrentTicket.Balance * (-1m);
                        if (dec > 0) //change is due
                            TouchMessageBox.Show(String.Format("Change Due: {0:c}", dec));
                    }


                    LoadHistory();

                    if (GlobalSettings.Instance.AutoHideClosedTicket) CurrentTicket = null;

                }
            }


        }




        public void ExecuteCaptureClicked(object obj)
        {
            if (m_security.WindowNewAccess("SettleTicket") == false)
            {
                return;
            }

            worker.RunWorkerAsync();

        }

        private void RunCapture(object sender, DoWorkEventArgs e)
        {
            //pax s300

            bool allresult = true;
     
            var payments = _history.GetAUTHPayments(StartDate, EndDate);

            //call invoke so error message can interact with gui
            m_parent.Dispatcher.Invoke(new Action(() =>
            {
                foreach (DataRow payment in payments.Rows)
                {
                    switch (GlobalSettings.Instance.CreditCardProcessor)
                    {
                        case "PAX_S300":
                            if (CapturePAX(payment) == false) allresult = false;
                            break;
                        case "Clover":
                            if (CaptureClover(payment) == false) allresult = false;
                            break;

                        case "VANTIV":
                            if (CaptureVantiv(payment) == false) allresult = false;
                            break;
                        case "CardConnect":
                            if (CaptureCardConnect(payment) == false) allresult = false;
                            break;
                    }


                }

     
                LoadHistory();
     

                if (allresult) TouchMessageBox.ShowSmall("Tickets Settled Successfully.");
                else TouchMessageBox.ShowSmall("NOT all ticket was settled successfuly.");

            }));


        }

        private bool CapturePAX(DataRow payment)
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
                        m_salesmodel.UpdatePaymentCapture(ID, TipAmount, (decimal)resp.TransactionAmount, NetAmount, resp.TransactionId);
                        return true;
                    }
                    else
                    {
                        logger.Error(resp.DeviceResponseText);
                        TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + resp.DeviceResponseText);
                        return false;
                    }


            
        }


        private bool CaptureClover(DataRow payment)
        {
         

            decimal NetAmount = decimal.Parse(payment["netamount"].ToString());
            decimal TipAmount =decimal.Parse(payment["tipamount"].ToString()) ;
            string CloverPaymentId = payment["custom1"].ToString();
            string CloverOrderId = payment["custom2"].ToString();
        
            int ID = int.Parse(payment["id"].ToString());


            logger.Info("COMMAND:Tip Adjust / Capture , Amount=" + NetAmount + ",Tip=" + TipAmount);


            TipAdjustAuthRequest taRequest = new TipAdjustAuthRequest();


            taRequest.OrderID = CloverOrderId;
            taRequest.PaymentID = CloverPaymentId;
            taRequest.TipAmount =(long) TipAmount * 100;  //need to pass as an integer

            ccl.tipDone = false;
            cloverConnector.TipAdjustAuth(taRequest);


            while (!ccl.tipDone)
            {
                Thread.Sleep(1000);
            }

            logger.Info("Credit Capture:Result=" + ccl.result);
            logger.Info("Credit Capture:RECEIVED=" + ccl.message);
            logger.Info("Credit Capture:PaymentID=" + taRequest.PaymentID);
            logger.Info("Credit Capture:OrderID=" + taRequest.OrderID);

            if (ccl.result == ResponseCode.SUCCESS)
            {
                m_salesmodel.UpdatePaymentCapture(ID, TipAmount, TipAmount + NetAmount);
                return true;
            }
            else
            {
                logger.Error(ccl.reason);
                TouchMessageBox.Show("TIP TRANSACTION FAILED !!!! ERROR:  " + ccl.reason);
                return false;
            }


        }

        private bool  CaptureVantiv(DataRow payment)
        {



            decimal NetAmount = decimal.Parse(payment["netamount"].ToString());
            decimal TipAmount = decimal.Parse(payment["tipamount"].ToString());
            string ResponseId = payment["responseid"].ToString();
            int ID = int.Parse(payment["id"].ToString());


            logger.Info("COMMAND:Credit Capture , Amount=" + NetAmount + ",Tip=" + TipAmount);

            string referencenumber = Utility.GenerateReferenceNumber();
            AuthorizationCompletionResponse resp = triposmodel.ExecuteCreditAuthorizeCompletion(ResponseId, NetAmount + TipAmount, referencenumber);
            logger.Info("CREDIT COMPLETE=> reference number:" + referencenumber + ", ticket=" + payment["salesid"] + ",responseid=" + ResponseId + ", amount=" + NetAmount + TipAmount);



            logger.Info("Credit Capture:RECEIVED:" + resp.ToString());
            logger.Info("Credit Capture:HRef=" + resp.TransactionId);
            if (resp.StatusCode == "Approved")
            {
        
    
                m_salesmodel.UpdatePaymentCapture(ID, TipAmount, (decimal)resp.TotalAmount, NetAmount, resp.TransactionId);

                TouchMessageBox.ShowSmall("Settled:" + ResponseId + " Amount:" + (NetAmount + TipAmount).ToString(), 1);
                return true;
            }
            else
            {
                logger.Error(resp.StatusCode);
                TouchMessageBox.Show("CAPTURE FAILED !!!! ERROR:  " + resp.StatusCode);
                return false;
            }



        }


        private bool CaptureCardConnect(DataRow payment)
        {



            decimal NetAmount = decimal.Parse(payment["netamount"].ToString());
            decimal TipAmount = decimal.Parse(payment["tipamount"].ToString());
            string ResponseId = payment["responseid"].ToString();
            string AuthCode = payment["authorcode"].ToString();
            int ID = int.Parse(payment["id"].ToString());


            logger.Info("COMMAND:Credit Capture , Amount=" + NetAmount + ",Tip=" + TipAmount);

 
            CCCaptureResponse resp = CardConnectModel.Capture(ResponseId,AuthCode, NetAmount + TipAmount);
            logger.Info("CREDIT COMPLETE=> reference number:" + resp.retref + ", ticket=" + payment["salesid"] + ",responseid=" + ResponseId + ", amount=" + NetAmount + TipAmount);



            logger.Info("Credit Capture:RECEIVED:" + resp.ToString());
            logger.Info("Credit Capture:HRef=" + resp.retref);
            if (resp.resptext == "Approval")
            {


                m_salesmodel.UpdatePaymentCapture(ID, TipAmount,decimal.Parse(resp.amount), NetAmount, resp.retref);

                TouchMessageBox.ShowSmall("Settled:" + ResponseId + " Amount:" + (NetAmount + TipAmount).ToString(), 1);
                return true;
            }
            else
            {
                logger.Error(resp.resptext);
                TouchMessageBox.Show("CAPTURE FAILED !!!! ERROR:  " + resp.resptext);
                return false;
            }



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


        public void ExecuteCloseBatchClicked(object obj)
        {
            if (m_security.WindowNewAccess("SettleTicket") == false)
            {

                return;
            }

            //pax s300
            string POSMessage = "";
            string POSXML = "";

         
                switch (GlobalSettings.Instance.CreditCardProcessor)
                {
                    case "PAX_S300":
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

                            logger.Info("BatchClose:RECEIVED:" + resp.ToString());
                            logger.Info("Status:" + resp.DeviceResponseText + "  BatchNumber:" + resp.SequenceNumber);
                            logger.Info("Total Count:" + resp.TotalCount + "  Total Amount:" + resp.TotalAmount);

                            ReceiptPrinterModel.PrintResponse("Batch Close", POSMessage);

                        TouchMessageBox.Show(POSMessage);


                        }
                        catch (Exception ex)
                        {
                            TouchMessageBox.Show("Error Closing Batch:" + ex.Message);
                           
                        }
                        break;


                    case "Clover":

                    ccl.closeoutDone = false;
                    cloverConnector.Closeout(new CloseoutRequest());
                    while (!ccl.closeoutDone)
                    {
                        Thread.Sleep(1000);
                    }

                    logger.Info("BatchClose:RECEIVED:" + ccl.message);
                    logger.Info("Result:" + ccl.result);

                    if (ccl.result == ResponseCode.SUCCESS)
                    {
                        logger.Info( "  BatchNumber:" + ccl.Batch.id);
                        logger.Info("Total Count:" + ccl.Batch.txCount + "  Total Amount:" + ccl.Batch.totalBatchAmount/100);
                        POSMessage = "Batch Number:" + ccl.Batch.id + (char)13 + (char)10;
                        POSMessage += "Total Count:" + ccl.Batch.txCount + (char)13 + (char)10;
                        POSMessage += "Total Amount:" + ccl.Batch.totalBatchAmount/100 + (char)13 + (char)10;

                      
                        ReceiptPrinterModel.PrintResponse("Batch Close", POSMessage);
                        TouchMessageBox.Show(POSMessage);

                    }
                    else
                    {
                        logger.Error("Result: Batch Close Failed :" + ccl.reason);
                        ReceiptPrinterModel.PrintResponse("Batch Close Failed", ccl.reason);
                        TouchMessageBox.ShowSmall("Batch Close Failed : " + ccl.reason );

                    }

                    break;


                case "VANTIV":
                    TriPOSModel tripos = new TriPOSModel(GlobalSettings.Instance.LaneId);

                    BatchCloseResponse result = tripos.ExecuteBatch();
                    if (result == null)
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
          

            LoadHistory();

        }






        /* ExecuteSettledClicked
         * 
         * 
        public void ExecuteSettleClicked(object salesid)
        {
            if (m_security.WindowNewAccess("SettleTicket") == false)
            {

                return;
            }


            Confirm conf = new Confirm("Are you sure??");
            Utility.OpenModal(m_parent, conf);
            if (conf.Action == "Yes")
            {
                CurrentTicket.CloseTicket(true);
                //test for close
                if (CurrentTicket.Status == "Closed")
                {


                    Decimal dec = 0m;




                    if (CurrentTicket.Balance > 0)  //still has a balance
                    {
                        new TouchMessageBox(String.Format("Ticket has a Balance: {0:c}", CurrentTicket.Balance)).ShowDialog();

                    }
                    else
                    {
                        dec = CurrentTicket.Balance * (-1m);
                        if (dec > 0) //change is due
                            new TouchMessageBox(String.Format("Change Due: {0:c}", dec)).ShowDialog();
                    }


                    LoadHistory();

                    if (GlobalSettings.Instance.AutoHideClosedTicket) CurrentTicket = null;

                }
            }


        }


        */

        public void ExecuteEditGratuity(object obj)
        {
            // ExecuteGratuityClicked(obj);
            try
            {
                // CurrentTicket.LoadGratuity();  //refresh gratuity list                   
                GratuityView gv = new GratuityView(CurrentTicket, m_ccp);
                Utility.OpenModal(m_parent, gv);
            

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("SalesVM:ExecuteGratuityClicked: " + ex.Message);
            }
            LoadHistory();
        }


        public void ExecuteReverseTicket(object obj)
        {
            if (!m_security.WindowNewAccess("ReverseTicket")) return;

            if(CurrentTicket.SaleDate < DateTime.Today)
            {
                if (!m_security.HasAccess("EditOldTicket"))
                {
                    TouchMessageBox.Show("Access Denied, You can not edit old tickets");
                    return;
                }
            }

            // confirm reason for reversing ticket
            ConfirmAudit win;
            win = new ConfirmAudit() { Topmost = true };
            Utility.OpenModal(m_parent, win);

            if (win.Reason != "")
            {
                CurrentTicket.ReverseTicket();
                AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Reverse Ticket", win.Reason, "sales", CurrentTicket.SalesID);


                //open for editing
                SalonSales sales = new SalonSales(CurrentTicket.SalesID);
                Utility.OpenModal(m_parent, sales);

                //refresh history list
                LoadHistory();
            }

          
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
                CurrentTicket = m_salesmodel.LoadTicket(id);

                VisibleTicket = Visibility.Visible;

                SetVisibility();

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteEditClicked: " + e.Message);
            }
        }


        public void ExecuteEditClicked(object salesid)
        {
            if (!m_security.WindowNewAccess("ReverseTicket")) return;


            SalonSales sales = new SalonSales( CurrentTicket.SalesID);
            Utility.OpenModal(m_parent, sales);

      
            m_parent.Close();
        }



        public void SetVisibility()
        {

            PaidInFull = Visibility.Hidden;
            VisibleClosed = Visibility.Hidden;
            VisibleVoided = Visibility.Hidden;
       


            if (CurrentTicket != null)
            {

                if (CurrentTicket.Balance < 0)
                {
                    IsChangeDue = true;


                }
                else
                {
                    IsChangeDue = false;
                    PaidInFull = Visibility.Visible;


                }






                if (CurrentTicket.Status == "Closed") //closed ticket - paid in full and allow create new
                {
                    VisibleClosed = Visibility.Visible;

      

                }
                else if (CurrentTicket.Status == "Voided")
                {
                    //voided ticket
                    VisibleVoided = Visibility.Visible;

                }
                else //open ticket
                {

                    VisibleClosed = Visibility.Hidden;

                    PaidInFull = Visibility.Hidden;
                }

                if (CurrentTicket.CreditCardSurcharge > 0) CreditCardSurchargeVisibility = Visibility.Visible;
                else CreditCardSurchargeVisibility = Visibility.Collapsed;

            }
            else
            {
                //null ticket so allow to create new

                CreditCardSurchargeVisibility = Visibility.Collapsed;

            }

        }
        public void ExecuteTodayClicked(object salesid)
        {
          
            try
            {
                StartDate = DateTime.Today;
                EndDate = DateTime.Today;
                CurrentTicket = null;
                LoadHistory();

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecutePreviousClicked(object obj)
        {

            try
            {
                StartDate = StartDate.AddDays(-1);
                EndDate = StartDate;
                CurrentTicket = null;
                LoadHistory();

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecuteNextClicked(object obj)
        {

            try
            {
                StartDate = StartDate.AddDays(1);
                EndDate = StartDate;
                CurrentTicket = null;
                LoadHistory();

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }

        public void ExecutePast7DaysClicked(object salesid)
        {

            try
            {

                StartDate = DateTime.Today.AddDays(-7);
                EndDate = DateTime.Today;
                CurrentTicket = null;
                LoadHistory();

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteTodayClicked: " + e.Message);
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
                CurrentTicket = null;

                LoadHistory();

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecuteByTicketIDClicked(object salesid)
        {
            try
            {

                NumPad np = new NumPad("Enter Ticket #",true);
                Utility.OpenModal(m_parent, np);
          

                int id = 0;
                if (np.Amount != "")
                {
                    string resultString = Regex.Match(np.Amount, @"\d+").Value;
                    id = int.Parse(resultString);

                    VisibleTicket = Visibility.Collapsed;
                    CurrentTicket = null;
                    HistoryData = _history.GetOrdersByID(id);
                }

               
          
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }


        }



        public void ExecuteBackClicked(object obj)
        {
          
                m_parent.Close();
        }


    }
}
