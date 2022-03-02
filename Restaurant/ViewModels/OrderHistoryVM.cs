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
    public class OrderHistoryVM:INPCBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool _showcancelledtickets = GlobalSettings.Instance.ShowCancelledTickets;

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

        private bool showvoids;
        public bool ShowVoids
        {
            get { return showvoids; }
            set
            {
                showvoids = value;
                NotifyPropertyChanged("ShowVoids");
            }
        }
        private QSHistory m_history;

        private bool CanExecute = true;

        Window m_parent;
        SecurityModel m_security;
        Employee m_currentemployee;

     
        DataTable m_historydata;
      //  DataTable m_reversedtickets;
        private DateTime m_startdate;
        private DateTime m_enddate;
        private int m_subid;


   

       // private bool integrated = (GlobalSettings.Instance.CreditCardProcessor != "External");
  

        public Visibility VisibleBarTabCustomer { get; set; }
        public Visibility VisibleCustomer { get; set; }



        public ICommand CustomerClicked { get; set; }
        public ICommand BarTabCustomerClicked { get; set; }


        public ICommand OpenOrderClicked { get; set; }
        public ICommand ViewOrderClicked { get; set; }


     //   public ICommand EditClicked { get; set; }
        public ICommand SelectClicked { get; set; }
        public ICommand PrintOrderClicked { get; set; }

        public ICommand TodayClicked { get; set; }
    
        public ICommand Past7DaysClicked { get; set; }
        public ICommand CustomClicked { get; set; }

        public ICommand PreviousClicked { get; set; }

        public ICommand NextClicked { get; set; }

        public ICommand ByTicketIDClicked { get; set; }

        public ICommand PrintReceiptClicked { get; set; }
        public ICommand PrintPackagingReceiptClicked { get; set; }
        public ICommand ReverseOrderClicked { get; set; }
        public ICommand PrintCreditSlipClicked { get; set; }

        public ICommand MarkPaidClicked { get; set; }

 
        public ICommand RefundClicked { get; set; }
  

        private SalesModel m_salesmodel;
   


        public OrderHistoryVM(Window parent, SecurityModel security)
        {

            m_security        = security;
            m_parent          = parent;


            m_salesmodel = new SalesModel(m_security);
            m_history = new QSHistory();

            VisibleTicket = Visibility.Collapsed;
            VisibleMessage = Visibility.Collapsed;

   
           CustomerClicked     = new RelayCommand(ExecuteCustomerClicked, param => this.CanExecuteNotNullTicket);
            BarTabCustomerClicked = new RelayCommand(ExecuteBarTabCustomerClicked, param => this.CanExecuteNotNullTicket);
            PrintCreditSlipClicked = new RelayCommand(ExecutePrintCreditSlipClicked, param => HasCreditDebitPayment);

            // ViewOrderClicked    = new RelayCommand(ExecuteViewOrderClicked, param => this.CanExecute);
            // VoidClicked         = new RelayCommand(ExecuteVoidClicked, param => this.IsOpenTicket(param));


          //  EditClicked = new RelayCommand(ExecuteEditClicked, param => this.IsReversedTicket(param));
      
            SelectClicked = new RelayCommand(ExecuteSelectClicked, param => true);

            PreviousClicked = new RelayCommand(ExecutePreviousClicked, param => this.CanExecute);
            NextClicked = new RelayCommand(ExecuteNextClicked, param => this.CanExecute);
            TodayClicked = new RelayCommand(ExecuteTodayClicked, param => this.CanExecute);
            
            Past7DaysClicked    = new RelayCommand(ExecutePast7DaysClicked, param => this.CanExecute);
            CustomClicked       = new RelayCommand(ExecuteCustomClicked, param => this.CanExecute);

            ByTicketIDClicked = new RelayCommand(ExecuteByTicketIDClicked, param => this.CanExecute);
            PrintReceiptClicked = new RelayCommand(ExecutePrintReceiptClicked, param => this.CanExecutePrintReceipt);
            PrintPackagingReceiptClicked = new RelayCommand(ExecutePrintPackagingReceiptClicked, param => this.CanExecutePrintReceipt);
            PrintCreditSlipClicked = new RelayCommand(ExecutePrintCreditSlipClicked, param => HasCreditDebitPayment);
            ReverseOrderClicked = new RelayCommand(ExecuteReverseOrderClicked, param => this.CanExecuteClosedTicket);
            MarkPaidClicked = new RelayCommand(ExecuteMarkPaidClicked, param => this.CanMarkPaid);

       
            RefundClicked = new RelayCommand(ExecuteRefundClicked, param => this.CanExecuteRefundClicked);
           



            m_currentemployee = new Employee(0);
            StartDate        = DateTime.Today;
            EndDate          = DateTime.Today;

            VisibleCustomer = Visibility.Collapsed;
            VisibleBarTabCustomer = Visibility.Collapsed;

         

            LoadHistory();

  
        }



        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        ____                            _         
        // |  _ \ _   _| |__ | (_) ___  |  _ \ _ __ ___  _ __   ___ _ __| |_ _   _ 
        // | |_) | | | | '_ \| | |/ __| | |_) | '__/ _ \| '_ \ / _ \ '__| __| | | |
        // |  __/| |_| | |_) | | | (__  |  __/| | | (_) | |_) |  __/ |  | |_| |_| |
        // |_|    \__,_|_.__/|_|_|\___| |_|   |_|  \___/| .__/ \___|_|   \__|\__, |
        //                                              |_|                  |___/ 
        //------------------------------------------------------------------------------------------
        #region PublicProperty

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
        { get { return m_subid; }
            set
            {
                m_subid = value;
                NotifyPropertyChanged("SubID");
            }
        }

        public DateTime StartDate
        {
            get { return m_startdate; }
            set { m_startdate = value; NotifyPropertyChanged("StartDate"); }
        }

        public DateTime EndDate
        {
            get { return m_enddate; }
            set { m_enddate = value; NotifyPropertyChanged("EndDate"); }
        }

        public DataTable HistoryData
        {

            get
            {

                return m_historydata;

            }

            set
            {
                m_historydata = value;
                NotifyPropertyChanged("HistoryData");
            }

        }


     

        /*
        public DataTable ReversedTickets
        {
            get { return m_reversedtickets; }
            set
            {
                m_reversedtickets = value;
                NotifyPropertyChanged("ReversedTickets");
            }
        }
        */

        #endregion

        //------------------------------------------------------------------------------------------
        //  ____        _   _                ____  _           _     _      
        // | __ ) _   _| |_| |_ ___  _ __   |  _ \(_)___  __ _| |__ | | ___ 
        // |  _ \| | | | __| __/ _ \| '_ \  | | | | / __|/ _` | '_ \| |/ _ \
        // | |_) | |_| | |_| || (_) | | | | | |_| | \__ \ (_| | |_) | |  __/
        // |____/ \__,_|\__|\__\___/|_| |_| |____/|_|___/\__,_|_.__/|_|\___|                                                                 
        //
        //------------------------------------------------------------------------------------------ 
        #region ButtonDisable

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



        public bool CanExecuteRefundClicked
        {
            get
            {
                if (CurrentTicket == null) return false;
                if (CurrentTicket.Status == "Closed")
                {
                    return true;
                }
                else return false;
            }

        }

        /*
        public bool CanExecuteReversedTicket
        {
            get
            {
                if (CurrentTicket == null) return false;
                if (CurrentTicket.Status == "Reversed")
                {
                    return true;
                }
                else return false;
            }
        }*/
   
        public bool CanMarkPaid
        {
            get
            {
                if (CanExecuteNotClosed && CurrentTicket.Status != "Voided") return true; else return false;
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
              //  if (!integrated) return false;  //no credit slips if not integrated


                if (CanExecutePrintReceipt)
                {
                    return (CurrentTicket.HasVoidedDebitPayment || CurrentTicket.HasDebitPayment || (CurrentTicket.HasVoidedCreditPayment) || (CurrentTicket.HasCreditPayment ) );
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
                    return true;

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

 
        /*
        public bool IsReversedTicket(object salesid = null)
        {
            if (CurrentTicket == null) return false;

            if (CurrentTicket.Status == "Reversed") return true;
            else return false;


        }*/

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



        public bool CanExecuteNotNullTicket
        {
            get
            {
                if (CurrentTicket != null) return true;
                else return false;
            }
        }

        #endregion


        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        __  __      _   _               _     
        // |  _ \ _   _| |__ | (_) ___  |  \/  | ___| |_| |__   ___   __| |___ 
        // | |_) | | | | '_ \| | |/ __| | |\/| |/ _ \ __| '_ \ / _ \ / _` / __|
        // |  __/| |_| | |_) | | | (__  | |  | |  __/ |_| | | | (_) | (_| \__ \
        // |_|    \__,_|_.__/|_|_|\___| |_|  |_|\___|\__|_| |_|\___/ \__,_|___/
        //    
        //------------------------------------------------------------------------------------------  

        #region PublicMethod



        public void LoadHistory()
        {
           

            //if user has access to all sales
            if (m_security.HasAccess("AllSales"))
                HistoryData = m_history.GetClosedVoidedOrders(StartDate, EndDate, 0,"",_showcancelledtickets);
            else
                HistoryData = m_history.GetClosedVoidedOrders(StartDate, EndDate, m_security.CurrentEmployee.ID,"",_showcancelledtickets);

           // ReversedTickets = m_history.GetReversedTickets();

            VisibleTicket = Visibility.Collapsed;
            decimal tickettotal = 0;
            int numberticket = 0;
            int numbervoid = 0;
            int id = 0;

   
            foreach (DataRow row in HistoryData.Rows)
            {
                id = (int)row["id"];

                if (CurrentTicket != null)
                    if (id == CurrentTicket.SalesID)
                    {
                        row["selected"] = true;
                        VisibleTicket = Visibility.Visible;
                        CurrentTicket.Reload();
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


                   // if (row["tip"].ToString().Contains("empty")) HasEmptyTip = true;
                }

            }

            /*
            foreach(DataRow row in ReversedTickets.Rows)
            {
                id = (int)row["id"];
                if (CurrentTicket != null)
                    if (id == CurrentTicket.SalesID)
                    {
                        row["selected"] = true;
                        VisibleTicket = Visibility.Visible;
                        CurrentTicket.Reload();
                    }
            }
            */


            NumberTicket = numberticket;
            NumberVoid = numbervoid;

            TicketTotal = tickettotal;
        }


        public void ExecutePrintReceiptClicked(object button)
        {
            ReceiptPrinterModel.PrintReceipt(CurrentTicket, GlobalSettings.Instance.ReceiptPrinter);
        }

        public void ExecutePrintPackagingReceiptClicked(object button)
        {
            ReceiptPrinterModel.PrintPackagingReceipt(CurrentTicket,true);
        }


        public void ExecuteCustomerClicked(object button)
        {
            CustomerModel.AddEditCustomer(CurrentTicket, m_security, m_parent);


        }
        public void ExecuteBarTabCustomerClicked(object button)
        {

            TextPad text = new TextPad("Enter Customer Name", "");
            Utility.OpenModal(m_parent, text);

            if (text.ReturnText != "")
            {
                CurrentTicket.UpdateBarTabCustomer(text.ReturnText);
                //CurrentTicket.Reload();
            }


        }


        public void ExecuteReverseOrderClicked(object salesid)
        {
            if (!m_security.WindowAccess("ReverseTicket")) return;
        

            try
            {
                ConfirmAudit win = new ConfirmAudit() { Topmost = true };
                Utility.OpenModal(m_parent, win);
                if (win.Reason != "")
                {
                    CurrentTicket.ReverseTicket();
                    AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Reverse Ticket", win.Reason, "sales", CurrentTicket.SalesID);

               

                    QuickSales sales = new QuickSales(m_security, CurrentTicket, CurrentTicket.TableNumber, CurrentTicket.CustomerCount, CurrentTicket.TicketOrderType, CurrentTicket.TicketSubOrderType);
                    Utility.OpenModal(m_parent, sales);


                    m_parent.Close();

                }


            }
            catch (Exception e)
            {
                MessageBox.Show("Error Reversing ticket: " + e.Message);
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
                CurrentTicket = SalesModel.GetTicket(id);

                VisibleTicket = Visibility.Visible;

                if (CurrentTicket.TicketOrderType == OrderType.Bar)
                {
                    VisibleCustomer = Visibility.Collapsed;
                    VisibleBarTabCustomer = Visibility.Visible;
                }
                else
                {
                    VisibleCustomer = Visibility.Visible;
                    VisibleBarTabCustomer = Visibility.Collapsed;
                }

                NotifyPropertyChanged("VisibleCustomer");
                NotifyPropertyChanged("VisibleBarTabCustomer");
            }
            catch (Exception e)
            {
                MessageBox.Show("Execute Select Clicked: " + e.Message);
            }
        }

        /*
        public void ExecuteEditClicked(object salesid)
        {
            if (!m_security.WindowAccess("ReverseTicket")) return;
   
          
           QuickSales sales = new QuickSales(m_security, CurrentTicket,CurrentTicket.TableNumber, CurrentTicket.CustomerCount, CurrentTicket.TicketOrderType, CurrentTicket.TicketSubOrderType);
           Utility.OpenModal(m_parent, sales);

        
                LoadHistory();
          
          
          //  m_parent.Close();
        }
        */

        public void ExecuteTodayClicked(object salesid)
        {
          
            try
            {
                StartDate = DateTime.Today;
                EndDate = DateTime.Today;

                LoadHistory();

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

                LoadHistory();

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

                LoadHistory();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecutePast7DaysClicked(object salesid)
        {

            try
            {

                StartDate = DateTime.Today.AddDays(-7);
                EndDate = DateTime.Today;

                LoadHistory();

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


                LoadHistory();

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }
        }


        public void ExecuteByTicketIDClicked(object salesid)
        {
            try
            {

                NumPad np = new NumPad("Enter Ticket #",true, false);
                Utility.OpenModal(m_parent, np);

                int id = 0;
                if (np.Amount != "") id = int.Parse(np.Amount);

                VisibleTicket = Visibility.Collapsed;
                CurrentTicket = null;

                HistoryData = m_history.GetOrdersByID(id);
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteTodayClicked: " + e.Message);
            }


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


                ListPad lp2 = new ListPad("Print Choice", new List<CustomList> { new CustomList { returnstring = "Merchant", description = "Merchant Copy" }, new CustomList { returnstring = "Customer", description = "Customer Copy" }, new CustomList { returnstring = "Both", description = "Both Copy" } });
                Utility.OpenModal(m_parent, lp2);

                switch (lp2.ReturnString)
                {
                    case "Customer":
                        ReceiptPrinterModel.PrintCreditSlip(result.First(), "**Customer Copy**");
                        break;
                    case "Merchant":
                        ReceiptPrinterModel.PrintCreditSlip(result.First(), "**Merchant Copy**");
                        break;

                    case "Both":
                        ReceiptPrinterModel.PrintCreditSlip(result.First(), "**Merchant Copy**");
                        ReceiptPrinterModel.PrintCreditSlip(result.First(), "**Customer Copy**");
                        break;
                }
                    





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
                CurrentTicket.ForceClose();
          

                LoadHistory();
            }


        }



        public void ExecuteRefundClicked(object obj)
        {
            if (!m_security.WindowNewAccess("Refund")) return;
     



            try
            {

                switch (GlobalSettings.Instance.CreditCardProcessor.ToUpper())
                {
                    case "HEARTSIP":
                        // Refund ccp = new Refund(m_parent, CurrentTicket, m_ccp, "REFUND", "");
                        //   Utility.OpenModal(m_parent, ccp);

                        break;

                    case "PAX_S300":
                    case "HSIP_ISC250":
                    case "VANTIV":
                    case "VIRTUAL":
                    case "CARDCONNECT":

                        CCPRefund ccp = new CCPRefund(CurrentTicket,m_security);
                        Utility.OpenModal(m_parent, ccp);
                        break;



                    case "CLOVER":
                        CloverRefund clover = new CloverRefund(CurrentTicket, m_security);
                        Utility.OpenModal(m_parent, clover);
                        break;


                    case "EXTERNAL":

                        CreditCardView ccv = new CreditCardView(m_parent, CurrentTicket,m_security, 674);
                        Utility.OpenModal(m_parent, ccv);
                        break;

                }



                CurrentTicket.LoadPayment();

                 NotifyPropertyChanged("Payments");
            




            }
            catch (Exception e)
            {

                TouchMessageBox.Show(" ExecuteRefundClicked:" + e.Message);
            }
        }



        #endregion



    }
}
