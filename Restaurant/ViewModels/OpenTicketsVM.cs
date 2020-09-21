using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;

//using POSLink;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;

namespace RedDot
{
   public  class OpenTicketsVM:SalesBaseVM
    {

      

 //POS LINK
      ///  ProcessTransResult result2 = new ProcessTransResult();


        public ICommand PaymentDeleteClicked { get; set; }
    
        public ICommand ExternalClicked { get; set; }
        public ICommand GiftCardClicked { get; set; }
        public ICommand StampCardClicked { get; set; }
        public ICommand CheckClicked { get; set; }
        public ICommand HouseCreditClicked { get; set; }
        public ICommand GiftCertificateClicked { get; set; }
        public ICommand RewardClicked { get; set; }
        public ICommand SplitClicked { get; set; }
        public ICommand CombineClicked { get; set; }
        public ICommand VerifyClicked { get; set; }
        public ICommand TicketClicked { get; set; }
        public ICommand CashTenderClicked { get; set; }
        public ICommand CCPClicked { get; set; }
        public ICommand ExitClicked { get; set; }
        public ICommand EditClicked { get; set; }
        public ICommand DiscountClicked { get; set; }
        public ICommand CashClicked { get; set; }
        public ICommand PrintReceiptClicked { get; set; }

        public ICommand EmployeeClicked { get; set; }
        public ICommand CustomerClicked { get; set; }
        public ICommand ChangeTableClicked { get; set; }

        private SalesModel m_salesmodel;
        
     
        private int m_tablenumber = 0;

        public Visibility CreditCardVisible { get; set; }
        public Visibility CCPVisible { get; set; }


        private PaymentModel m_paymentmodel;
        private int m_clickcount;
        private int m_lastticketid;

        DispatcherTimer dispatchTimer = new DispatcherTimer();
        int counter = GlobalSettings.Instance.OpenOrdersAutoCloseSeconds;

        public OpenTicketsVM(Window parent, SecurityModel security, int id, int tablenumber) : base(parent, security)
        {

            Title = "Open Orders";

            CurrentEmployee = security.CurrentEmployee;
            dispatchTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatchTimer.Interval = new TimeSpan(0, 0, 1);
            dispatchTimer.Start();



            m_paymentmodel = new PaymentModel(m_security);
            TableNumber = tablenumber;

            PaidInFull = Visibility.Hidden;


            IsChangeDue = false;
            IsBalance = true;
            m_clickcount = 0;
            m_lastticketid = 0;
            m_mode = "edit"; /// need to do this so the BASE viewmodel knows that we are not in 'creating' mode 

            PaymentDeleteClicked = new RelayCommand(ExecutePaymentDeleteClicked, param => this.CanExecutePaymentClicked);
            GiftCardClicked = new RelayCommand(ExecuteGiftCardClicked, param => this.CanExecutePaymentClicked);
            StampCardClicked = new RelayCommand(ExecuteStampCardClicked, param => this.CanExecutePaymentClicked);
            CheckClicked = new RelayCommand(ExecuteCheckClicked, param => this.CanExecutePaymentClicked);
            HouseCreditClicked = new RelayCommand(ExecuteHouseCreditClicked, param => false);
            GiftCertificateClicked = new RelayCommand(ExecuteGiftCertificateClicked, param => this.CanExecutePaymentClicked);
            RewardClicked = new RelayCommand(ExecuteRewardClicked, param => this.CanExecuteReward);
            VerifyClicked = new RelayCommand(ExecuteVerifyClicked, param => this.CanExecute);

            ExternalClicked = new RelayCommand(ExecuteCreditCardClicked, param => this.CanExecuteCreditDebitClicked);
            SplitClicked = new RelayCommand(ExecuteSplitClicked, param => this.CanExecuteSplitTicket);
            CombineClicked = new RelayCommand(ExecuteCombineClicked, param => this.CanExecuteCombineTicket);
            TicketClicked = new RelayCommand(ExecuteTicketClicked, param => this.CanExecute);
            CashTenderClicked = new RelayCommand(ExecuteCashTenderClicked, param => this.CanExecuteCashTenderClicked);
            CCPClicked = new RelayCommand(ExecuteCCPClicked, param => this.CanExecuteCreditDebitClicked);
            ExitClicked = new RelayCommand(ExecuteExitClicked, param => this.CanExecute);
            EditClicked = new RelayCommand(ExecuteEditClicked, param => this.CanEdit);
            PrintReceiptClicked = new RelayCommand(ExecutePrintReceiptClicked, param => CanPrintReceiptClickedExecute);
            DiscountClicked = new RelayCommand(ExecuteAdjustTicketClicked, param => this.CanExecutePaymentClicked);

            CustomerClicked = new RelayCommand(ExecuteCustomerClicked, param => this.CanExecuteNotNullTicket);
            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => this.CanExecuteOpenTicket);
            CashClicked = new RelayCommand(ExecuteCashClicked, param => this.CanExecuteCashTenderClicked);
            ChangeTableClicked = new RelayCommand(ExecuteChangeTableClicked, param => this.CanExecuteOpenTicket);

            m_salesmodel = new SalesModel(m_security);

            if (GlobalSettings.Instance.CreditCardProcessor == "External")
            {
                CreditCardVisible = Visibility.Visible;
                CCPVisible = Visibility.Collapsed;
              
            }
            else
            {
                CreditCardVisible = Visibility.Collapsed;
                CCPVisible = Visibility.Visible;
            }




            if (id > 0)
            {
                LoadTicket(id);  

            }
            else
            {
 
                LoadTickets();
            }



        
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            counter--;
            Title = "Open Orders -- " + counter.ToString() + " seconds  before auto closing";


            if (counter <= 0)
            {
                dispatchTimer.Stop();
                GlobalSettings.Instance.CurrentTicket = null;
                GlobalSettings.Instance.RemoteScreen.remotescreenvm.RefreshTicket();
                ExecuteExitClicked(null);
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









        public void LoadTicket(int salesid)
        {
            CurrentTicket = m_salesmodel.LoadTicket(salesid);
            GlobalSettings.Instance.CurrentTicket = CurrentTicket;
            GlobalSettings.Instance.RemoteScreen.remotescreenvm.RefreshTicket();

            if (CurrentTicket != null)
            {
                GlobalSettings.CustomerDisplay.WriteDisplay("Ticket : ", CurrentTicket.SalesID.ToString(), "Total", CurrentTicket.Total);

            }

            CalculateCashButtons();


        }

        public void LoadTickets()
        {
            try
            {
                int employeeid;
                CurrentTicket = null; //close current ticket
                m_clickcount = 0;
                m_lastticketid = 0;



                if (m_tablenumber >= 0)
                {

                    if (m_security.HasAccess("AllSales"))
                         DineInTickets = m_salesmodel.LoadOpenTicketsByTable(m_tablenumber, 0);
                    else
                        DineInTickets = m_salesmodel.LoadOpenTicketsByTable(m_tablenumber, CurrentEmployee.ID);
                    //select the table tab only
                    SelectedIndex = 1;
                }
                else
                {
                    //if have all sales access then load all ticket
                    if (m_security.HasAccess("AllSales"))
                    {
                        if (GlobalSettings.Instance.SeparateStaffOrders)
                        {
                            Tickets = m_salesmodel.LoadOpenTickets(CurrentEmployee.ID, "");
                            DineInTickets = m_salesmodel.LoadOpenTickets(CurrentEmployee.ID, "DineIn");
                            ToGoTickets = m_salesmodel.LoadOpenTickets(CurrentEmployee.ID, "Togo");
                            ByOthersTickets = m_salesmodel.LoadOpenTicketsByOthers(CurrentEmployee.ID, "");
                        }
                        else
                        {
                            //load all orders into Tickets tab
                           Tickets = m_salesmodel.LoadOpenTickets(0, "");
                           // DBTickets = m_salesmodel.GetOpenOrders(0, "");
                            DineInTickets = m_salesmodel.LoadOpenTickets(0, "DineIn");
                            ToGoTickets = m_salesmodel.LoadOpenTickets(0, "Togo");
                            ByOthersTickets = null;
                        }

                    }
                    else
                    {

                        //Does not have access to other orders
                        Tickets = m_salesmodel.LoadOpenTickets(CurrentEmployee.ID, "");
                        DineInTickets = m_salesmodel.LoadOpenTickets(CurrentEmployee.ID, "DineIn");
                        ToGoTickets = m_salesmodel.LoadOpenTickets(CurrentEmployee.ID, "Togo");
                        ByOthersTickets = null;

                    }
                }




             

            }
            catch(Exception ex)
            {
                TouchMessageBox.Show("Load Tickets:" + ex.Message);
            }
         
               



        }


        public int SelectedIndex { get; set; }

   


        public bool AddCreditPayment(decimal requested_amount, ResponseEventArgs response, DateTime paymentdate,string reason)
        {

            try
            {



                if (CurrentTicket == null)
                    return SalesModel.InsertCreditPayment(0, requested_amount, response, paymentdate, reason);
                else
                {
                  //  GlobalSettings.CustomerDisplay.WriteDisplay(response.CardType + ":", requested_amount, "Balance:", CurrentTicket.Balance);
                    return SalesModel.InsertCreditPayment(CurrentTicket.SalesID, requested_amount, response, paymentdate,reason);
                }



            }
            catch (Exception e)
            {
                TouchMessageBox.Show("AddPayment:" + e.Message);
                return false;
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


        private string m_title;
        public string Title
        {
            get { return m_title; }
            set
            {
                m_title = value;
                NotifyPropertyChanged("Title");
            }
        }



        private bool m_isbalance;
    
        public bool IsBalance
        {
            get { return m_isbalance; }
            set { m_isbalance = value; NotifyPropertyChanged("IsBalance"); }
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



        public int TableNumber
        {
            get { return m_tablenumber; }
            set
            {
                m_tablenumber = value;
                NotifyPropertyChanged("TableNumber");
            }
        }

        DataTable m_dbtickets;
        public DataTable DBTickets
        {
            get
            {
                return m_dbtickets;
            }
            set
            {
                m_dbtickets = value;
                NotifyPropertyChanged("DBTickets");
            }
        }

        private ObservableCollection<Ticket> m_tickets;
        public ObservableCollection<Ticket> Tickets
        {

            get
            {
                return m_tickets;
            }

            set
            {
                if (value != m_tickets)
                {
                    m_tickets = value;
                    NotifyPropertyChanged("Tickets");
                }
            }
        }


        private ObservableCollection<Ticket> m_togotickets;
        public ObservableCollection<Ticket> ToGoTickets
        {

            get
            {
                return m_togotickets;
            }

            set
            {
                if (value != m_togotickets)
                {
                    m_togotickets = value;
                    NotifyPropertyChanged("ToGoTickets");
                }
            }
        }


        private ObservableCollection<Ticket> m_dineintickets;
        public ObservableCollection<Ticket> DineInTickets
        {

            get
            {
                return m_dineintickets;
            }

            set
            {
                if (value != m_dineintickets)
                {
                    m_dineintickets = value;
                    NotifyPropertyChanged("DineInTickets");
                }
            }
        }


        private ObservableCollection<Ticket> m_byotherstickets;
        public ObservableCollection<Ticket> ByOthersTickets
        {

            get
            {
                return m_byotherstickets;
            }

            set
            {
                if (value != m_byotherstickets)
                {
                    m_byotherstickets = value;
                    NotifyPropertyChanged("ByOthersTickets");
                }
            }
        }







        //---------------------------------------------- BUTTON DISABLE ---------------------------------



        public bool CanPrintReceiptClickedExecute
        {
            get
            {
                if (m_currentticket == null)
                {
                    return false;
                }
                else
                {
                    if (m_currentticket.Seats.Count > 0) return true;
                    else return false;

                }

            }
        }


        public bool CanEdit
        {
            get
            {
                if (m_currentticket == null) return false;
                if (CurrentTicket.Status == "Closed") return false;
                return true;
            }
        }

        public bool CanExecuteCashTenderClicked
        {
            get
            {

                if (m_currentticket == null) return false;
                //  if (m_currentticket.HasBeenPaid("Cash")) return false;
                return CanExecutePaymentClicked;
            }

        }

        public bool CanExecuteOpenTicket
        {
            get
            {
                if (m_currentticket == null) return false;
                if (m_currentticket.Status == "Open")
                {
                    return true;
                }
                else return false;
            }
        }

        public bool CanExecuteNotNullTicket
        {
            get
            {
                if (m_currentticket != null) return true;
                else return false;
            }
        }

        public bool CanExecuteSplitTicket
        {
            get
            {
                if (m_currentticket == null) return false;
                else
                    if (CurrentTicket.HasPayment) return false;
                else
                   if (CurrentTicket.ActiveItemCount > 1) return true;
                else return false;
              
            }
        }

        public bool CanExecuteCombineTicket
        {
            get
            {
                if (m_currentticket == null) return false;
                else
                    if (CurrentTicket.HasPayment) return false;
                else
                    return true;
             

            }
        }

        public bool CanExecuteCreditDebitClicked
        {
            get
            {

                if (CurrentTicket == null) return false;

                return CanExecutePaymentClicked;
            }

        }



        public bool CanExecuteReward
        {
            get
            {
                if (CanExecuteOpenTicket && m_currentticket.CurrentCustomer != null)
                {
                    if (m_currentticket.CurrentCustomer.UsableBalance > 0)
                    {
                        if (m_currentticket.HasBeenPaid("Reward")) return false; //aslo need to check if has any reward
                        else return true;

                    }
                    else return false;


                }
                else return false;
            }
        }


        public bool CanExecutePaymentClicked
        {
            get
            {

                if (CurrentTicket == null) return false;

                if (CurrentTicket.Status != "Voided" && CurrentTicket.Status != "Closed" && m_currentticket.ActiveItemCount >= 1 && m_currentticket.Balance >= 0)
                {
                    return true;
                }
                else return false;
            }

        }





        //------------------------------------------------------------------------------------------
        //  ____        _   _                 ____                                          _     
        // | __ ) _   _| |_| |_ ___  _ __    / ___|___  _ __ ___  _ __ ___   __ _ _ __   __| |___ 
        // |  _ \| | | | __| __/ _ \| '_ \  | |   / _ \| '_ ` _ \| '_ ` _ \ / _` | '_ \ / _` / __|
        // | |_) | |_| | |_| || (_) | | | | | |__| (_) | | | | | | | | | | | (_| | | | | (_| \__ \
        // |____/ \__,_|\__|\__\___/|_| |_|  \____\___/|_| |_| |_|_| |_| |_|\__,_|_| |_|\__,_|___/
        //
        //------------------------------------------------------------------------------------------ 

       public void ExecuteExitClicked(object obj)
        {

            GlobalSettings.Instance.CurrentTicket = null;
            GlobalSettings.Instance.RemoteScreen.remotescreenvm.RefreshTicket();

            m_parent.Close();
        }



        public void ExecuteCCPClicked(object button)
        {


            try
            {
                m_clickcount = 0;
                dispatchTimer.Stop();

                bool paid = m_salesmodel.ProcessCCP(m_parent, CurrentTicket);

                if (paid)
                {
                    GlobalSettings.Instance.RemoteScreen.remotescreenvm.CurrentTicket = null;
                    CurrentTicket = null;
                }

      

                LoadTickets();
                dispatchTimer.Start();

                if (Tickets.Count == 0) m_parent.Close();


            }
            catch (Exception e)
            {

                TouchMessageBox.Show(" ExecuteCCPClicked:" + e.Message);
            }
        }

        public void ExecuteTicketClicked(object salesid)
        {
            int selectedid = (int)salesid;

            if (m_security.CurrentEmployee == null) return;

 
            //temporary load of ticket to test
            Ticket selected = new Ticket(selectedid);
            if(m_security.GetWindowLevel("OpenOrders")>0)
            if (!m_security.HasAccess("AllSales"))
                if (selected.CurrentEmployee.ID != m_security.CurrentEmployee.ID)
                {
                    TouchMessageBox.Show("This ticket does not belong to you.");
                    return;
                }

            CurrentEmployee = m_security.CurrentEmployee;

            //these ticket status must be OPEN , closed ticket can not be operated on
            LoadTicket((int)salesid);

            if (m_clickcount >= 1 && selectedid == m_lastticketid)
            {
                ExecuteEditClicked(null);
                m_clickcount = 0;
                m_lastticketid = selectedid;
                return;
            }

            m_clickcount++;
            m_lastticketid = selectedid;
        }


        public void ExecuteSplitClicked(object obj)
        {

            try
            {
                m_clickcount = 0;
                counter = GlobalSettings.Instance.OpenOrdersAutoCloseSeconds;
                dispatchTimer.Stop();

                SplitTicket dlg = new SplitTicket(m_security, CurrentTicket);
                Utility.OpenModal(m_parent, dlg);
                LoadTickets();
                dispatchTimer.Start();

            }
            catch (Exception e)
            {
                MessageBox.Show("Split:" + e.Message);

            }
        }

        public void ExecuteCombineClicked(object obj)
        {

            try
            {
                m_clickcount = 0;
                counter = GlobalSettings.Instance.OpenOrdersAutoCloseSeconds;
                dispatchTimer.Stop();
                CombineTicket dlg = new CombineTicket(m_security, CurrentTicket);
                Utility.OpenModal(m_parent, dlg);
                LoadTickets();
                dispatchTimer.Start();

            }
            catch (Exception e)
            {
                MessageBox.Show("Split:" + e.Message);

            }
        }

        public override void ExecuteVoidClicked(object salesid)
        {
            base.ExecuteVoidClicked(salesid);
            LoadTickets();
        }

        private void CleanUPOrRefresh(bool paid)
        {
            try
            {
                if (paid)
                {
                    if (GlobalSettings.Instance.RemoteScreen != null)
                        if (GlobalSettings.Instance.RemoteScreen.remotescreenvm != null)
                            GlobalSettings.Instance.RemoteScreen.remotescreenvm.CurrentTicket = null;

                    LoadTickets();

                }

                CalculateCashButtons();

                dispatchTimer.Start();
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Clean Up/ Refresh:" + ex.Message);
                logger.Error("Clean Up/Refresh Error:" + ex.Message);
            }
        }

        public void ExecuteCashClicked(object amount)
        {

            string temp = amount.ToString();
            decimal amt;

            try
            {
                if (temp != "")
                {

                    amt = decimal.Parse(temp);
                    m_clickcount = 0;
                    dispatchTimer.Stop();
                    bool paid = m_salesmodel.ProcessQuickCash(m_parent, m_currentticket, amt);

                    CleanUPOrRefresh(paid);
               

                }
                else MessageBox.Show("Error Processing Quick Cash buttons");

            }
            catch (Exception e)
            {
                logger.Error("ExecuteCashClicked:" + e.Message);
                MessageBox.Show("Error Cash Clicked: " + e.Message);
            }
        }



        public void ExecuteCashTenderClicked(object button)
        {
            try
            {
                m_clickcount = 0;
                dispatchTimer.Stop();

                bool paid = m_salesmodel.ProcessCashTender(m_parent, m_currentticket);

                CleanUPOrRefresh(paid);

            }
            catch (Exception e)
            {
                MessageBox.Show(" ExecuteCashTenderClicked:" + e.Message);
            }
        }

        /*
        public void ExecuteSettleOrderClicked(object obj)
        {
            try
            {
                m_salesmodel.ProcessSettleTicket(m_parent, m_security, m_currentticket);
                m_currentticket.CheckforBalance();

                if (m_currentticket.IsClosed())
                    GlobalSettings.Instance.RemoteScreen.remotescreenvm.CurrentTicket = null;

                LoadTickets();

                if (Tickets.Count == 0) m_parent.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show(" ExecuteSettleOrderClicked:" + e.Message);
            }


        }

        */


        public void ExecuteCreditCardClicked(object button)
        {


            try
            {
                m_clickcount = 0;
                dispatchTimer.Stop();

                bool paid = m_salesmodel.ProcessExternalPay(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);

            }
            catch (Exception e)
            {

                MessageBox.Show("Credit Card Clicked:" + e.Message);
            }
        }

        public void ExecuteVerifyClicked(object button)
        {

            GCVerify gcv = new GCVerify();
            Utility.OpenModal(m_parent, gcv);
        }

        public void ExecuteGiftCardClicked(object button)
        {
            try
            {
                m_clickcount = 0;
                dispatchTimer.Stop();

                bool paid = m_salesmodel.ProcessGiftCard(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {

                MessageBox.Show("Gift Card Clicked:" + e.Message);
            }

        }

        public void ExecuteStampCardClicked(object button)
        {
            try
            {
                m_clickcount = 0;
                dispatchTimer.Stop();
                bool paid = m_salesmodel.ProcessStampCard(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);

            }
            catch (Exception e)
            {
                TouchMessageBox.Show(" Execute Stamp Clicked:" + e.Message);
            }
        }


        public void ExecuteCheckClicked(object button)
        {
            try
            {
                m_clickcount = 0;
                dispatchTimer.Stop();

                bool paid =  m_salesmodel.ProcessCheck(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);

            }
            catch (Exception e)
            {
                MessageBox.Show(" ExecuteCheckClicked:" + e.Message);
            }
        }

        public void ExecuteHouseCreditClicked(object button)
        {
            try
            {
                m_clickcount = 0;
                dispatchTimer.Stop();

                bool paid = m_salesmodel.ProcessHouseCredit(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {
                MessageBox.Show(" ExecuteHouseCreditClicked:" + e.Message);
            }
        }

        public void ExecuteGiftCertificateClicked(object button)
        {
            try
            {
                m_clickcount = 0;
                dispatchTimer.Stop();

                bool paid = m_salesmodel.ProcessGiftCertificate(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);

            }
            catch (Exception e)
            {
                MessageBox.Show(" ExecuteGiftCertificateClicked:" + e.Message);
            }
        }
        public void ExecuteRewardClicked(object button)
        {
            try
            {
                m_clickcount = 0;
                dispatchTimer.Stop();
                bool paid = m_salesmodel.ProcessRewardCredit(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {
                MessageBox.Show(" Execute Reward Clicked:" + e.Message);
            }
        }

    

        public void ExecutePaymentDeleteClicked(object paymentid)
        {
            m_paymentmodel.PaymentDelete(CurrentTicket, (int)paymentid, m_parent);
        }

        public void ExecuteEditClicked(object salesid)
        {
            if (CurrentTicket == null) return;


            m_clickcount = 0;
            counter = GlobalSettings.Instance.OpenOrdersAutoCloseSeconds;
            dispatchTimer.Stop();

            QuickSales sales = new QuickSales(m_security, CurrentTicket,CurrentTicket.TableNumber,CurrentTicket.CustomerCount,CurrentTicket.TicketOrderType,CurrentTicket.TicketSubOrderType) { Topmost = true };
            sales.ShowDialog();

            //check to see if ticket has been deleted...
            if(CurrentTicket != null)
            {
                LoadTicket(CurrentTicket.SalesID);
                if(CurrentTicket != null)
                {
                    if (CurrentTicket.Status == "Voided" || CurrentTicket.Seats == null) CurrentTicket = null;

                }
              
            }
      

            LoadTickets();

            dispatchTimer.Start();

            //if ticket list is empty, then close
            if (Tickets == null || Tickets.Count == 0) m_parent.Close();

        }

     


        public void ExecutePrintReceiptClicked(object button)
        {
            ReceiptPrinterModel.PrintReceipt(CurrentTicket, GlobalSettings.Instance.ReceiptPrinter);
        }



        public void ExecuteAdjustTicketClicked(object button)
        {

            if (m_security.WindowAccess("Discount") == false) return;

         
            m_salesmodel.AdjustTicket(CurrentTicket);
    
            CalculateCashButtons();



        }



        public void ExecuteCustomerClicked(object button)
        {

            CustomerModel.AddEditCustomer(CurrentTicket, m_security, m_parent);

        }


        public void ExecuteEmployeeClicked(object button)
        {

            int ticketid = m_currentticket.SalesID;

            if (m_security.WindowAccess("EmployeeList") == false) return;

            PickEmployee empl = new PickEmployee(m_parent, m_security);
            Utility.OpenModal(m_parent, empl);
            if (empl.EmployeeID > 0)
            {
                m_currentticket.ChangeServer(empl.EmployeeID);
                LoadTickets();
                LoadTicket(ticketid);
            }
        }

        public void ExecuteChangeTableClicked(object obj)
        {
            try
            {
                NumPad np = new NumPad("Enter New Table Number", true, false, m_currentticket.TableNumberStr);
                Utility.OpenModal(m_parent, np);
                if (np.Amount != "")
                {
                    CurrentTicket.UpdateTable(int.Parse(np.Amount));
                    CurrentTicket.Reload();
                    LoadTickets();
                }
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Table Change Error:" + ex.Message);
            }

        }


        void Run1()
        {

            /**
            PosLink pg = new PosLink();
            // myDelegate1 md1 = new myDelegate1(threadTrans);
            result2 = pg.ProcessTrans();

            // this.Invoke(md1);

            // waitform_sample1.Hide();

            // 5. Show the result
            #region Show the PayLink Result

            if (result2.Code == POSLink.ProcessTransResultCode.OK)
            {
                POSLink.ManageResponse manageResponse = pg.ManageResponse;
                if (manageResponse != null && manageResponse.ResultCode != null)
                {

                    POSLink.ManageRequest manageRequest = new POSLink.ManageRequest();


                    manageRequest.ConvertSigToPic("c:\\temp\\sig\\" + manageResponse.SigFileName, "BMP", "c:\\temp\\sig\\" + manageResponse.SigFileName + ".BMP");
                }
                else
                {
                    MessageBox.Show("Signature download - Unknown error: mg.manageResponse is null.");
                }
            }
            else if (result2.Code == POSLink.ProcessTransResultCode.TimeOut)
            {
                MessageBox.Show("Signature download - Action Timeout.");
            }
            else
            {
                MessageBox.Show("Signature download:" + result2.Msg, "Error Retrieving Signature");
            }
            //  this.Focus();
            #endregion


    **/

        }

    }
}
