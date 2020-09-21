using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Diagnostics;
using System.Xml.Linq;
using NLog;
using RedDot.DataManager;
using System.Globalization;
using System.Threading;
using WpfLocalization;
using RedDot.Models;

namespace RedDot
{
    public class SalonSalesVM : SalonSalesBase
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();
        BackgroundWorker worker = new BackgroundWorker();

        public string SelectedLanguage
        {
            get { return GlobalSettings.Instance.LanguageCode; }
            set
            {
                GlobalSettings.Instance.LanguageCode = value;
                NotifyPropertyChanged("SelectedLanguage");


                //Thread.CurrentThread.CurrentCulture = new CultureInfo(value);
                // Thread.CurrentThread.CurrentUICulture = new CultureInfo(value);
                // FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));


                var culture = CultureInfo.GetCultureInfo(value);
                // Dispatcher.Thread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                LocalizationManager.UpdateValues();
            }
        }

        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        System.Windows.Threading.DispatcherTimer dispatcherTimer2;
        System.Windows.Threading.DispatcherTimer dispatcherTimer3;

        private ObservableCollection<MenuItem> m_quickproducts;


        private DataTable m_products;
        private DataTable m_services;
        private DataTable m_favorites;
        private DataTable m_searchresults;
        private DataTable m_categories;
        private DataTable m_menuprefixes;
        private DataTable m_menuprefixes2;

        private DataTable m_tickets;

        private Visibility m_visible_category;
        private Visibility m_visible_product;
        private Visibility m_visible_quicksale;
        private Visibility m_visible_newticketbutton;
        private Visibility m_paidinfull;
        private Visibility m_visibleclosed;
        private Visibility m_visiblevoided;
        private Visibility m_gratuityvisibility;
        private Visibility m_creditcardsurchargevisibility;
        private Visibility m_visibleemployee;
        private Visibility m_visiblewarning;

        public Visibility CreditCardVisible { get; set; }
        public Visibility CCPVisible { get; set; }
        public bool m_editmode = false;
        public bool m_loadedtickets = false;
        public bool m_usecheckinlist = false;

        public SalesModel m_salesmodel { get; set; }

        private Employee m_currentemployee;

        //private MenuSetupModel                          m_inventorymodel;


        private bool m_autocloseproduct;


        private int m_selectedindex;
        private int m_techid;

        private string m_menuprefix1;
        private string m_rewardrestriction;
        private string m_salesviewmode;



        private Window m_parent;



        private DataTable m_employees;
        private DBEmployee m_dbemployee;
        private HeartPOS m_ccp;

   

        public ICommand CategoryProductClicked { get; set; }

        public ICommand ProductClicked { get; set; }
        //  public ICommand ServiceClicked { get; set; }

        public ICommand LineItemClicked { get; set; }
        public ICommand PaymentDeleteClicked { get; set; }
        public ICommand CashTenderClicked { get; set; }
        public ICommand CreditCardClicked { get; set; }
        public ICommand GiftCardClicked { get; set; }
        public ICommand StampCardClicked { get; set; }
        public ICommand CheckClicked { get; set; }
        public ICommand GiftCertificateClicked { get; set; }

        public ICommand NewTicketClicked { get; set; }

        public ICommand NoSaleClicked { get; set; }
        public ICommand VoidClicked { get; set; }

        public ICommand CustomerClicked { get; set; }
        public ICommand CustomerSearchClicked { get; set; }

        //public ICommand SettingsClicked { get; set; }
        public ICommand DiscountClicked { get; set; }
        public ICommand RewardClicked { get; set; }

        //  public ICommand HoldClicked { get; set; }
        public ICommand ExitClicked { get; set; }

        public ICommand MoreClicked { get; set; }

        public ICommand VerifyClicked { get; set; }

        public ICommand SettingsClicked { get; set; }


        public ICommand MarkPaidClicked { get; set; }

        public ICommand DateClicked { get; set; }

        public ICommand BackClicked { get; set; }

        public ICommand CategoryBackClicked { get; set; }
        // public ICommand OpenOrderClicked { get; set; }


        public ICommand MenuPrefixClicked { get; set; }
        public ICommand MenuPrefix2Clicked { get; set; }

        public ICommand TicketClicked { get; set; }


        public ICommand EmployeeClicked { get; set; }

        public ICommand TurnsCheckInClicked { get; set; }
        public ICommand PrintReceiptClicked { get; set; }
  
        public ICommand GratuityClicked { get; set; }
        public ICommand WebSyncClicked { get; set; }

        public ICommand CreditCardManagerClicked { get; set; }
        public ICommand CheckInClicked { get; set; }

        public ICommand RefundClicked { get; set; }

        public ICommand TicketHistoryClicked { get; set; }
        public ICommand CCPClicked { get; set; }
        public ICommand HoldClicked { get; set; }

        private bool ProcessCreditCard = false;
        private bool CanExecute = true;
        private bool EditMode = false;

        bool AutoSendReceiptToCustomer = GlobalSettings.Instance.AutoSendReceiptToCustomer;
        bool AutoSendMessageToCustomer = GlobalSettings.Instance.AutoSendMessageToCustomer;
        bool AutoHideClosedTicket = GlobalSettings.Instance.AutoHideClosedTicket;
        string MessageText = GlobalSettings.Instance.SMSCustomerMessage;

        public SalonSalesVM(Window parent,  int salesid) : base(parent)
        {
            //------- create credit card processor object -----------------------------------
            try
            {

                ProcessCreditCard = (GlobalSettings.Instance.CreditCardProcessor != "External");
                CanExecuteHeartSIP = (GlobalSettings.Instance.CreditCardProcessor == "HeartSIP" || GlobalSettings.Instance.CreditCardProcessor == "PAX_S300" || GlobalSettings.Instance.CreditCardProcessor == "HSIP_ISC250" || GlobalSettings.Instance.CreditCardProcessor == "VANTIV");

                if (GlobalSettings.Instance.CreditCardProcessor == "HeartSIP")
                {
                    m_ccp = new HeartPOS(Response_Message, Response_Sales, Response_Void, Response_Refund, Response_Gift, Response_TipAdjust, Response_GetCardData, Response_ALL);

                    // Random rand = new Random();
                    //  m_ccp.RequestId = rand.Next(10000, 99999999).ToString();
                    m_ccp.RequestId = Utility.RandomPin(7).ToString();

                    //send reset after 3 seconds
                    dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
                    dispatcherTimer2.Tick += new EventHandler(dispatcherTimer2_Tick);
                    dispatcherTimer2.Interval = new TimeSpan(0, 0, 3);
                    dispatcherTimer2.Start();

                    //send lane open after 6 seconds ( 3 seconds after the reset)
                    dispatcherTimer3 = new System.Windows.Threading.DispatcherTimer();
                    dispatcherTimer3.Tick += new EventHandler(dispatcherTimer3_Tick);
                    dispatcherTimer3.Interval = new TimeSpan(0, 0, 3);


                }






             




            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Error:" + ex.Message);
            }

            m_salesmodel = new SalesModel(m_ccp);

            SelectedLanguage = GlobalSettings.Instance.LanguageCode;

            worker.DoWork += RunSMSAsync;


            m_parent = parent;
            CurrentEmployee = new Employee(0);
            SelectedIndex = 0;
            m_techid = -1;


            m_dbemployee = new DBEmployee();


            if (GlobalSettings.Instance.Demo) Demo = Visibility.Visible;
            else Demo = Visibility.Hidden;

            LoadSettings();
            Initialize();



            VisibleNewticketbutton = Visibility.Visible;


            if(salesid > 0)
            {
                EditMode = true;
                LoadTicket(salesid);
            }else
                LoadTickets();
       





            if (m_usecheckinlist) LoadCheckInList(0);
            else LoadAllSalesTech();






            //start this last since it sends data to the GUI
            if (GlobalSettings.Instance.CreditCardProcessor == "HeartSIP") CCP.StartThread();
        }

        public void LoadSettings()
        {


            //get settings from Database

            m_rewardrestriction = GlobalSettings.Instance.RewardUsageRestriction;

            CategoryWidth = GlobalSettings.Instance.CategoryWidth - 30;  //minus 50 for the Prefix box width which is always 50 constant
            CategoryHeight = GlobalSettings.Instance.CategoryHeight;
            CategoryFontSize = GlobalSettings.Instance.CategoryFontSize;

            ProductWidth = GlobalSettings.Instance.ProductWidth;  //minus 50 for the Prefix box width which is always 50 constant
            ProductWidth2 = ProductWidth - 50;
            ProductHeight = GlobalSettings.Instance.ProductHeight - 25;  //minus 30 for the fixed product price height
            ProductFontSize = GlobalSettings.Instance.ProductFontSize;





            //AutoCloseProduct = GlobalSettings.Instance.AutoCloseProduct;

            m_usecheckinlist = GlobalSettings.Instance.UseCheckInList;

            m_salesviewmode = GlobalSettings.Instance.SalesViewMode;

        }

        public void Initialize()
        {



            //Assign command relays
            CategoryProductClicked = new RelayCommand(ExecuteCategoryProductClicked, param => this.CanExecute);
            ProductClicked = new RelayCommand(ExecuteItemClicked, param => this.CanExecute);



            LineItemClicked = new RelayCommand(ExecuteLineItemClicked, param => this.IsOpenOrReversed);
            PaymentDeleteClicked = new RelayCommand(ExecutePaymentDeleteClicked, param => this.CanExecutePayment);

            CashTenderClicked = new RelayCommand(ExecuteCashTenderClicked, param => this.CanExecutePaymentClicked);
            CreditCardClicked = new RelayCommand(ExecuteCreditCardClicked, param => this.CanExecuteCreditDebitClicked);
            CCPClicked = new RelayCommand(ExecuteCCPClicked, param => this.CanExecuteCreditDebitClicked);
            GiftCardClicked = new RelayCommand(ExecuteGiftCardClicked, param => this.CanExecutePaymentClicked);
            StampCardClicked = new RelayCommand(ExecuteStampCardClicked, param => this.CanExecutePaymentClicked);
            CheckClicked = new RelayCommand(ExecuteCheckClicked, param => this.CanExecutePaymentClicked);
            GiftCertificateClicked = new RelayCommand(ExecuteGiftCertificateClicked, param => this.CanExecutePaymentClicked);


            DiscountClicked = new RelayCommand(ExecuteAdjustTicketClicked, param => this.CanExecutePaymentClicked);
            RewardClicked = new RelayCommand(ExecuteRewardClicked, param => this.CanExecuteReward);


            NoSaleClicked = new RelayCommand(ExecuteNoSaleClicked, param => this.CanExecute);
            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => this.CanExecuteVoidClicked);


            CustomerClicked = new RelayCommand(ExecuteCustomerClicked2, param => this.CanExecuteCustomer);
            CustomerSearchClicked = new RelayCommand(ExecuteCustomerSearchClicked, param => this.CanExecuteCustomer);


            // HoldClicked                 = new RelayCommand(ExecuteHoldClicked, param => this.CanExecuteOpenTicket);
            NewTicketClicked = new RelayCommand(ExecuteNewTicketClicked, param => this.CanExecuteNewTicket);
            TicketClicked = new RelayCommand(ExecuteTicketClicked, param => this.CanExecute);

            MoreClicked = new RelayCommand(ExecuteMoreClicked, param => this.CanExecute);
            VerifyClicked = new RelayCommand(ExecuteVerifyClicked, param => this.CanExecute);

            MarkPaidClicked = new RelayCommand(ExecuteMarkPaidClicked, param => this.CanExecuteMarkPaid);
            DateClicked = new RelayCommand(ExecuteDateClicked, param => this.CanExecuteReversedTicket);

            BackClicked = new RelayCommand(ExecuteBackClicked, param => this.CanExecute);
            CategoryBackClicked = new RelayCommand(ExecuteCategoryBackClicked, param => this.CanExecute);
            SettingsClicked = new RelayCommand(ExecuteSettingsClicked, param => this.CanExecute);
            MenuPrefixClicked = new RelayCommand(ExecuteMenuPrefixClicked, param => this.CanExecute);
            MenuPrefix2Clicked = new RelayCommand(ExecuteMenuPrefix2Clicked, param => this.CanExecuteMenuPrefix);


            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => this.CanExecute);

            TurnsCheckInClicked = new RelayCommand(ExecuteTurnsCheckInClicked, param => this.CanExecute);

            PrintReceiptClicked = new RelayCommand(ExecutePrintReceiptClicked, param => CanExecutePrintReceipt);

            GratuityClicked = new RelayCommand(ExecuteGratuityClicked, param => this.CanExecuteGratuityClicked);
        
            CreditCardManagerClicked = new RelayCommand(ExecuteCreditCardManagerClicked, param => this.CanExecuteHeartSIP);
            CheckInClicked = new RelayCommand(ExecuteCheckInClicked, param => true);

            RefundClicked = new RelayCommand(ExecuteRefundClicked, param => this.CanExecuteRefundClicked);

            TicketHistoryClicked = new RelayCommand(ExecuteTicketHistoryClicked, param => true);

            HoldClicked = new RelayCommand(ExecuteHoldClicked, param => this.CanExecuteOpenTicket);


            // m_inventorymodel           = new MenuSetupModel();







            //set startup defaults
            VisibleQuickSale = Visibility.Hidden;
            VisibleCategory = Visibility.Hidden;  //Collapsed, Hidden , Visible
            VisibleProduct = Visibility.Hidden;



            VisibleClosed = Visibility.Hidden;
            VisibleVoided = Visibility.Hidden;
            VisibleEmployee = Visibility.Hidden;
            VisibleWarning = Visibility.Hidden;


   




            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            dispatcherTimer.Interval = new TimeSpan(0, 5, 5);


            if (GlobalSettings.Instance.CreditCardProcessor == "External")
            {
                CreditCardVisible = Visibility.Visible;
                CCPVisible = Visibility.Collapsed;

            } else
            {
              
                CreditCardVisible = Visibility.Collapsed;
            
                CCPVisible = Visibility.Visible;
            }




        }


        private void LoadAllSalesTech()
        {
            m_techid = -1;




            VisibleWarning = Visibility.Hidden;

            Employees = m_dbemployee.GetSalesTechList(true, GlobalSettings.Instance.Demo);
            if(Employees != null)
            {
                //Add empty row on top so user can select "none"
                DataRow newrow = Employees.NewRow();
                newrow["employeeid"] = 0;
                newrow["displayname"] = "None";
                newrow["imageurl"] = "/media/noemployee.png";
                Employees.Rows.InsertAt(newrow, 0);
            }
 

            VisibleCategory = Visibility.Hidden;  //Collapsed, Hidden , Visible
            VisibleProduct = Visibility.Hidden;

        }

        private void LoadCheckInList(int selectedemployeeid)
        {
            m_techid = -1;






            var employees = m_dbemployee.GetCheckInList(DateTime.Now, true);

            foreach (DataRow row in employees.Rows)
            {
                if ((int)row["employeeid"] == selectedemployeeid)
                {
                    row["selected"] = true;
                    m_techid = selectedemployeeid;
                    break;
                }
            }

            //if tech id is not reassigned a higher number .. which means employee not found
            //  if (m_techid == 0)
            // {
            VisibleCategory = Visibility.Hidden;  //Collapsed, Hidden , Visible
            VisibleProduct = Visibility.Hidden;
            // }
            Employees = employees;

            if (Employees.Rows.Count > 0)
            {
                VisibleWarning = Visibility.Hidden;

            } else
            {
                VisibleWarning = Visibility.Visible;

            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            VisibleCategory = Visibility.Visible;
            VisibleProduct = Visibility.Hidden;
        }

        private void dispatcherTimer2_Tick(object sender, EventArgs e)
        {


            //send reset command
            m_ccp.ExecuteCommand("Reset");

            dispatcherTimer2.Stop();
            dispatcherTimer3.Start();
        }

        private void dispatcherTimer3_Tick(object sender, EventArgs e)
        {


            m_ccp.ExecuteCommand("OpenLane");

            dispatcherTimer3.Stop();
        }




        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        ____                            _         
        // |  _ \ _   _| |__ | (_) ___  |  _ \ _ __ ___  _ __   ___ _ __| |_ _   _ 
        // | |_) | | | | '_ \| | |/ __| | |_) | '__/ _ \| '_ \ / _ \ '__| __| | | |
        // |  __/| |_| | |_) | | | (__  |  __/| | | (_) | |_) |  __/ |  | |_| |_| |
        // |_|    \__,_|_.__/|_|_|\___| |_|   |_|  \___/| .__/ \___|_|   \__|\__, |
        //                                              |_|                  |___/ 
        //------------------------------------------------------------------------------------------


        #region public_property

        public string StoreLogo
        {
            get { return "pack://siteoforigin:,,,/" + GlobalSettings.Instance.StoreLogo; }
        }

        public string MainBackgroundImage
        {
            get { return "pack://siteoforigin:,,,/" + GlobalSettings.Instance.MainBackgroundImage; }
        }

        public bool ShowStampCardButton
        {
            get
            {
                return GlobalSettings.Instance.ShowStampCardButton;
            }
        }

        public bool ShowCheckButton
        {
            get
            {
                return GlobalSettings.Instance.ShowCheckButton;
            }
        }

        public bool ShowRewardButton
        {
            get
            {
                return GlobalSettings.Instance.ShowRewardButton;
            }
        }

        public string MessageColor
        {
            get { return GlobalSettings.Instance.MessageColor; }
        }

        public Visibility Demo { get; set; }

     
           
        public DataTable Employees
        {
            get { return m_employees; }
            set { m_employees = value; NotifyPropertyChanged("Employees"); }
        }

        /*
        public bool AutoCloseProduct
        {
            get { return m_autocloseproduct; }
            set
            {
                m_autocloseproduct = value;
                GlobalSettings.Instance.AutoCloseProduct = value;
                NotifyPropertyChanged("AutoCloseProduct");
            }
        }
        */

        public int SelectedIndex
        {
            get { return m_selectedindex; }
            set
            {
                m_selectedindex = value;
                if (value == 4 && m_loadedtickets == false)
                {
                    m_loadedtickets = true;
                    LoadTickets();
                }
                NotifyPropertyChanged("SelectedIndex");
            }
        }



   
        //current logged in employee / cashier
        public Employee CurrentEmployee
        {
            get { return m_currentemployee; }
            set { m_currentemployee = value; NotifyPropertyChanged("CurrentEmployee"); }
        }

        private Employee m_currenttech;
        public Employee CurrentTech
        {
            get { return m_currenttech; }
            set { m_currenttech = value; NotifyPropertyChanged("CurrentTech"); }
        }



        public DataTable Tickets
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
        public DataTable Categories
        {

            get
            {

                return m_categories;
            }

            set
            {
                if (value != m_categories)
                {
                    m_categories = value;
                    NotifyPropertyChanged("Categories");
                }
            }
        }



        public ObservableCollection<MenuItem> QuickProducts
        {

            get
            {

                return this.m_quickproducts;
            }

            set
            {
              
                    this.m_quickproducts = value;
                    NotifyPropertyChanged("QuickProducts");
               
            }
        }



        public DataTable MenuPrefixes
        {

            get
            {
                return this.m_menuprefixes;
            }

            set
            {
                if (value != this.m_menuprefixes)
                {
                    this.m_menuprefixes = value;
                    NotifyPropertyChanged("MenuPrefixes");
                }
            }
        }


        public DataTable MenuPrefixes2
        {

            get
            {
                return this.m_menuprefixes2;
            }

            set
            {
                if (value != this.m_menuprefixes2)
                {
                    this.m_menuprefixes2 = value;
                    NotifyPropertyChanged("MenuPrefixes2");
                }
            }
        }

        public DataTable Products
        {

            get
            {

                return this.m_products;
            }

            set
            {
                if (value != this.m_products)
                {
                    this.m_products = value;
                    NotifyPropertyChanged("Products");
                }
            }
        }


        public DataTable Services
        {

            get
            {

                return this.m_services;
            }

            set
            {
                if (value != this.m_services)
                {
                    this.m_services = value;
                    NotifyPropertyChanged("Services");
                }
            }
        }

        public DataTable Favorites
        {

            get
            {
                return this.m_favorites;
            }

            set
            {
                if (value != this.m_favorites)
                {
                    this.m_favorites = value;
                    NotifyPropertyChanged("Favorites");
                }
            }
        }
        public DataTable SearchResults
        {

            get
            {

                return this.m_searchresults;
            }

            set
            {
                if (value != this.m_searchresults)
                {
                    this.m_searchresults = value;
                    NotifyPropertyChanged("SearchResults");
                }
            }
        }


        public Visibility VisibleWarning
        {
            get
            {
                return m_visiblewarning;
            }
            set
            {
                m_visiblewarning = value;
                NotifyPropertyChanged("VisibleWarning");
            }
        }


        public Visibility VisibleEmployee
        {
            get
            {
                return m_visibleemployee;
            }
            set
            {
                m_visibleemployee = value;
                NotifyPropertyChanged("VisibleEmployee");
            }
        }




        public Visibility VisibleQuickSale
        {
            get
            {
                return m_visible_quicksale;
            }
            set
            {
                m_visible_quicksale = value;
                NotifyPropertyChanged("VisibleQuickSale");
            }
        }
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
        public Visibility GratuityVisibility
        {
            get
            {
                return m_gratuityvisibility;
            }
            set
            {
                m_gratuityvisibility = value;
                NotifyPropertyChanged("GratuityVisibility");
            }
        }


        Visibility m_receiptvisibility;
        public Visibility ReceiptVisibility
        {
            get { return m_receiptvisibility; }
            set
            {
                m_receiptvisibility = value;
                NotifyPropertyChanged("ReceiptVisibility");
            }
        }

        public Visibility VisibleCategory
        {
            get
            {
                return m_visible_category;
            }
            set
            {
                m_visible_category = value;
                NotifyPropertyChanged("VisibleCategory");
            }
        }


        public Visibility VisibleProduct
        {
            get
            {
                return m_visible_product;
            }
            set
            {
                m_visible_product = value;
                NotifyPropertyChanged("VisibleProduct");
            }
        }



        public Visibility VisibleNewticketbutton
        {
            get
            {
                return m_visible_newticketbutton;
            }
            set
            {
                m_visible_newticketbutton = value;
                NotifyPropertyChanged("VisibleNewticketbutton");
            }
        }


  




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


        public int CategoryWidth { get; set; }


        public int CategoryHeight { get; set; }
    

        public int ProductWidth { get; set; }
        public int ProductWidth2 { get; set; }

        public int ProductHeight { get; set; }
    

        public int ProductFontSize { get; set; }

        public int CategoryFontSize { get; set; }


        private DataTable m_employeelist;
        public DataTable EmployeeList
        {
            get { return m_employeelist; }
            set
            {
                m_employeelist = value;
                NotifyPropertyChanged("EmployeeList");
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

        #region public_methods




      

        public void LoadTicket(int salesid)
        {
            CurrentTicket = m_salesmodel.LoadTicket(salesid);
            if (CurrentTicket != null)
            {
                vfd.WriteDisplay("Ticket : ", CurrentTicket.SalesID.ToString(), "Total", CurrentTicket.Total);

            }

            if (Categories == null)
            {
                Categories  = m_salesmodel.GetCategoryList();
           
                MenuPrefixes = m_salesmodel.GetMenuPrefix();

            }

            if (m_usecheckinlist) LoadCheckInList(0);
            else LoadAllSalesTech();

            VisibleCategory = Visibility.Hidden;
            VisibleProduct = Visibility.Hidden;

            SetVisibility();

        }

        public void LoadTickets()
        {
          
          Tickets = m_salesmodel.LoadAllOpenTickets();


          if(CurrentTicket != null)
          foreach (DataRow row in Tickets.Rows)
          {
              if ((int)row["id"] == CurrentTicket.SalesID)
              {
                  row["selected"] = true;
               
                  break;
              }
          }




            SetVisibility();

        }

        public void SetVisibility()
        {
          


            if (CurrentTicket != null)
            {

                ReceiptVisibility =  Visibility.Visible;



                if (CurrentTicket.Status == "Closed") //closed ticket - paid in full and allow create new
                {
                    VisibleClosed = Visibility.Visible;

                     VisibleCategory = Visibility.Hidden;
                     VisibleProduct = Visibility.Hidden;

                    VisibleEmployee = Visibility.Hidden;


                }else if(CurrentTicket.Status == "Voided")
                {
                    //voided ticket
                    VisibleVoided = Visibility.Visible;

                }
                else //open ticket
                {
                    
                    VisibleClosed = Visibility.Hidden;
                    VisibleEmployee = Visibility.Visible;
                 
                 
                }

                if (CurrentTicket.CreditCardSurcharge > 0) CreditCardSurchargeVisibility = Visibility.Visible;
                else CreditCardSurchargeVisibility = Visibility.Collapsed;

            }
            else
            {
                //null ticket so allow to create new
                VisibleCategory = Visibility.Hidden;
                VisibleProduct = Visibility.Hidden;
                VisibleEmployee = Visibility.Hidden;
                ReceiptVisibility = Visibility.Hidden;

                CreditCardSurchargeVisibility = Visibility.Collapsed;
                VisibleClosed = Visibility.Hidden;
            }

          

        }

        #endregion












        //------------------------------------------------------------------------------------------
        //  ____        _   _                ____  _           _     _      
        // | __ ) _   _| |_| |_ ___  _ __   |  _ \(_)___  __ _| |__ | | ___ 
        // |  _ \| | | | __| __/ _ \| '_ \  | | | | / __|/ _` | '_ \| |/ _ \
        // | |_) | |_| | |_| || (_) | | | | | |_| | \__ \ (_| | |_) | |  __/
        // |____/ \__,_|\__|\__\___/|_| |_| |____/|_|___/\__,_|_.__/|_|\___|                                                                 
        //
        //------------------------------------------------------------------------------------------ 
        #region button_disable



        public bool CanExecuteNewTicket
        {
            get
            {
                return !EditMode;

            }

        }
        public bool CanExecuteReward
        {
            get
            {
                if (CanExecutePaymentClicked &&  CurrentTicket.CurrentCustomer != null)
                {
                    if (CurrentTicket.CurrentCustomer.UsableBalance > 0)
                    {
                        //check for restrictions
                        if(m_rewardrestriction.ToUpper() == "DISCOUNT")
                        {
                            if (CurrentTicket.HasDiscount) return false;
                        }

                        if (CurrentTicket.HasPaymentType("Reward")) return false; //aslo need to check if has any reward
                        else return true;

                    }
                    else return false;


                }
                else return false;
            }
        }


        public bool CanExecuteVoidClicked
        {
            get
            {
                //Can only void ticket if ticket is open/reverse and no payments
                if ((CanExecuteOpenTicket || CanExecuteReversedTicket) && CurrentTicket.Payments.Where(x=>!x.Voided).Count() == 0) return true;
                else return false;
              
                
            }
        }

        public bool CanExecuteMarkPaid
        {
            get
            {
                if (CanExecuteNotClosed && CurrentTicket.Status.ToUpper() == "REVERSED") return true;
                else return false;
            }
        }

        public bool CanExecuteHeartSIP { get; set; }
      

        public bool CanExecuteRefundClicked
        {
            get
            {
                return CanExecuteReversedTicket;
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



 


        public bool CanExecuteNullCloseTicket
        {
            get
            {
                if (CurrentTicket == null) return true;
                else
                {
                    if (CurrentTicket.Status == "Closed") return true;
                    else return false;
                }
            }
        }

    


 

    


        public bool CanExecuteMenuPrefix
        {
            get
            {
                if (m_menuprefix1 == "") return false; else return true;
            }
        }



        #endregion



        //------------------------------------------------------------------------------------------
        //  ____        _   _                 ____                                          _     
        // | __ ) _   _| |_| |_ ___  _ __    / ___|___  _ __ ___  _ __ ___   __ _ _ __   __| |___ 
        // |  _ \| | | | __| __/ _ \| '_ \  | |   / _ \| '_ ` _ \| '_ ` _ \ / _` | '_ \ / _` / __|
        // | |_) | |_| | |_| || (_) | | | | | |__| (_) | | | | | | | | | | | (_| | | | | (_| \__ \
        // |____/ \__,_|\__|\__\___/|_| |_|  \____\___/|_| |_| |_|_| |_| |_|\__,_|_| |_|\__,_|___/
        //
        //------------------------------------------------------------------------------------------ 
        #region button_commands



        public void ExecuteMarkPaidClicked(object salesid)
        {
            if (m_security.WindowNewAccess("MarkPaid") == false)
            {

                return;
            }

            if(CurrentTicket.Balance > 0)
            {
                Confirm conf = new Confirm(" Are you sure??  Ticket is NOT PAID!!");
                Utility.OpenModal(m_parent, conf);
                if (conf.Action != "Yes") return;
            }
          
         
                CurrentTicket.CloseTicket(true);
                //test for close
                if (CurrentTicket.Status == "Closed")
                {


                    Decimal dec = 0m;

                    SetVisibility();


                    if (CurrentTicket.Balance > 0)  //still has a balance
                    {
                       
                        TouchMessageBox.Show(String.Format("ATTENTION !!!   Ticket has a Balance: {0:c}", CurrentTicket.Balance));

                    }
                    else
                    {
                        dec = CurrentTicket.Balance * (-1m);
                        if (dec > 0) //change is due
                         TouchMessageBox.Show(String.Format("Change Due: {0:c}", dec));
                    }


                    // 

                    // if (GlobalSettings.Instance.AutoHideClosedTicket) CurrentTicket = null;

                    //code was changed so that this feature is only available for a Reversed Ticket so we can close form afterwards
                    if (m_parent.Name != "SalesWindowLarge")
                    {
                        m_parent.Close();
                    }else
                    {
                        LoadTickets();

                    }

                }
          


        }



        public void ExecuteDateClicked(object button)
        {
            CustomDate cd = new CustomDate(Visibility.Hidden);
        
            Utility.OpenModal(m_parent, cd);

            DateTime ticketdate = cd.StartDate;

            CurrentTicket.UpdateSalesDate(ticketdate);
           
        }
        public void ExecuteCategoryProductClicked(object objid)
        {

            int id = 0;


            try
            {
                if (objid != null) id = (int)objid;

                if (id != 0)
                {
                   

                  
                        QuickProducts = m_salesmodel.FillProductbyCatID(id);

                        if (m_salesviewmode == "Small")
                        {
                            VisibleCategory = Visibility.Hidden;
                            
                        }

                        VisibleProduct = Visibility.Visible;

                        dispatcherTimer.Start(); //starts timer to auto close the Product Result page and make category visible again
                       
                       

                }
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("ExecuteCategoryProductClicked:" + e.Message);

            }
        }


        public void ExecuteEmployeeClicked(object obj_id)
        {
            VisibleEmployee = Visibility.Visible;
            if (obj_id != null)
            {
                //current tech
                if (obj_id.ToString() != "") m_techid = int.Parse(obj_id.ToString());
                CurrentTech = new Employee(m_techid);

                VisibleCategory = Visibility.Visible;
                VisibleProduct = Visibility.Hidden;

            }
        }

        public void ExecuteItemClicked(object obj_id)
        {


            int productid = 0;
  

            try
            {

                if(CurrentTicket == null)
                {
                    CurrentEmployee = m_security.CurrentEmployee;

                    CurrentTicket = new Ticket(CurrentEmployee);

                    //creates a sales record
                    CurrentTicket.CreateTicket();
                    AuditModel.WriteLog(CurrentEmployee.DisplayName, "new ticket", "", "sales", CurrentTicket.SalesID);
                   // LoadTicket(CurrentTicket.SalesID); //load ticket will call SetVisibility
                    LoadTickets();

                    //Ask cashier to enter customer phone number
                    if (GlobalSettings.Instance.AskCustomerPhone)
                    {

                        ExecuteCustomerClicked(null);
                    }
                }
               


                if (obj_id != null)
                {
                    if (obj_id.ToString() != "") productid = int.Parse(obj_id.ToString());

                }

                AddItemToTicket(productid);


                   dispatcherTimer.Stop();

                if(m_salesviewmode == "Small")
                {
                    VisibleCategory = Visibility.Visible;

                   VisibleProduct = Visibility.Hidden;

                }else
                {
                 

                    if (m_autocloseproduct) VisibleProduct = Visibility.Hidden;
                }
                   
                   


            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Product Clicked: " + e.Message);
            }
        }


      




        public void AddItemByBarCode(string searchstr)
        {
           int productid = m_salesmodel.FindProductByBarCode(searchstr);

           if (productid > 0) AddItemToTicket(productid);
        }







        public void AddItemToTicket(int prodid)
        {
            try
            {
                if (m_techid == -1) return;

                if (CurrentTicket.Status == "Closed")
                {
                    TouchMessageBox.Show("Cannot add item to Closed ticket");
                    return;
                }

                if (CurrentTicket.Status == "Reversed")
                {
                    MenuItem product = new MenuItem(prodid, false);
                    if (product.Type.ToUpper().Substring(0, 4) == "GIFT")
                    {
                        TouchMessageBox.Show("Cannot add Gift Card or Certificate to Reversed Ticket.  Please create new ticket.");
                        return;
                    }
                }

           

   
                m_salesmodel.AddItemToTicket(m_techid, prodid, CurrentTicket);

            }
            catch(Exception e)
            {
                TouchMessageBox.Show("Add Item to Ticket: " + e.Message);
            }
        }



        /// -------------------------------------------------------------------------Execute Line Item Clicked -----------------------------
        public void ExecuteLineItemClicked(object iditemtype)
        {

            string temp = iditemtype as string;
            string[] portion = temp.Split(',');

            int id = 0;
            string itemtype = portion[1]; // type = service, product , giftcard .. etc..
            LineItemActionView linevw;
            ConfirmDelete dlg;

            try
            {

                id = int.Parse(portion[0]);

                if (id == 0) return;

                LineItem line = CurrentTicket.GetLineItemLine(id);

                if (CurrentTicket.Status == "Closed") linevw = new LineItemActionView(m_parent, "Closed", line);
                else
                {
                    if(CurrentTicket.Status == "Reversed" && line.ItemType.Substring(0,4).ToUpper() == "GIFT")
                        linevw = new LineItemActionView(m_parent, "REVERSEDGIFTCARD", line);
                    else
                    linevw = new LineItemActionView(m_parent, itemtype, line);
                }

                Utility.OpenModal(m_parent,linevw);

        


                switch (linevw.Action)
                {


                    case "Delete":


                        if (CurrentTicket.Status == "Closed") TouchMessageBox.Show("Ticket is Closed!");
                        else
                        {
                            /*   06/19/2018 .. allow user to delete item  without removing payment since it might be a partial refund
                            if (CurrentTicket.Status == "Reversed" && CurrentTicket.Payments.Where(x=> !x.Voided).Count() > 0)
                            {
                                TouchMessageBox.Show("First remove all payments.");
                                break;
                            }*/

                            dlg = new ConfirmDelete("Delete " + line.ReceiptStr + "?");
                            Utility.OpenModal(m_parent, dlg);

                            if (dlg.Action == "Delete")
                            {
                                CurrentTicket.DeleteLineItem(id);
                                //if tip is already assigned to current employee, then need to remove tip first for current employee
                                //passing 0 for employee ID will delete all gratuity for that ticket
                                //this only affects nail salons
                                CurrentTicket.DeleteGratuity();
                            }
                              

                        }


                        break;

                    case "Employee":

                 


                        PickEmployee empl = new PickEmployee(m_parent,m_security);
                        Utility.OpenModal(m_parent, empl);
                        if (empl.EmployeeID >-1)
                        {
                            //if tip is already assigned to current employee, then need to remove tip first for current employee
                            //passing 0 for employee ID will delete all gratuity for that ticket
                            //this only affects nail salons
                            CurrentTicket.DeleteGratuity();

                            //add new selected employee to salesitem
                            CurrentTicket.UpdateSalesItemEmployeeID(id, empl.EmployeeID);

                        }

                        break;


                    case "PriceOverride":

                        if (CurrentTicket.Status == "Closed") TouchMessageBox.Show("Ticket is Closed!");
                        else
                        {
                            if (m_security.WindowNewAccess("PriceOverride") == false)
                            {
                             
                                return; //jump out of routine
                            }
                            NumPad np2 = new NumPad("Enter NEW Price:",false);
                            Utility.OpenModal(m_parent, np2);
                            if (np2.Amount != "")
                            {
                                if (decimal.Parse(np2.Amount) >= 0)
                                {
                                    CurrentTicket.OverrideLineItemPrice(id, decimal.Parse(np2.Amount));
                                    //if (portion[1] == "Service") CurrentTicket.OverrideServicePrice(id, decimal.Parse(np.Amount));
                                }
                            }

                        }
                        break;



                    case "Discount1":
                        if (CurrentTicket.Status == "Closed")
                        {
                            TouchMessageBox.Show("Ticket is Closed!");
                            return;
                        }

                        if (m_security.WindowAccess("Discount") == false) { return; }

                         CurrentTicket.DiscountLineItem(line.ID,1, "Manual Discount", "Discount");

                        break;
                    case "Discount2":
                        if (CurrentTicket.Status == "Closed")
                        {
                            TouchMessageBox.Show("Ticket is Closed!");
                            return;
                        }

                        if (m_security.WindowAccess("Discount") == false) { return; }

                        CurrentTicket.DiscountLineItem(line.ID, 2, "Manual Discount", "Discount");

                        break;
                    case "Discount3":
                        if (CurrentTicket.Status == "Closed")
                        {
                            TouchMessageBox.Show("Ticket is Closed!");
                            return;
                        }

                        if (m_security.WindowAccess("Discount") == false) { return; }

                        CurrentTicket.DiscountLineItem(line.ID, 3, "Manual Discount", "Discount");

                        break;

                    case "Discount4":
                        if (CurrentTicket.Status == "Closed")
                        {
                            TouchMessageBox.Show("Ticket is Closed!");
                            return;
                        }

                        if (m_security.WindowAccess("Discount") == false) { return; }

                        CurrentTicket.DiscountLineItem(line.ID, 4, "Manual Discount", "Discount");

                        break;
                    case "Discount5":
                        if (CurrentTicket.Status == "Closed")
                        {
                            TouchMessageBox.Show("Ticket is Closed!");
                            return;
                        }

                        if (m_security.WindowAccess("Discount") == false) { return; }

                        CurrentTicket.DiscountLineItem(line.ID, 5, "Manual Discount", "Discount");

                        break;
                    case "Discount":

                        if (CurrentTicket.Status == "Closed")
                        {
                            TouchMessageBox.Show("Ticket is Closed!");
                            return;
                        }


                        DiscountPad np = new DiscountPad(line.ProductID, line.Price);
                        Utility.OpenModal(m_parent, np);
                        if (np.ReturnPromotion == null) return;  /// user cancelled

                        switch (np.ReturnPromotion.Description)
                        {

                            case "Manual":
                                //need manual discount access
                                if (m_security.WindowAccess("Discount") == false) { return; }

                                if (np.ReturnPromotion.DiscountAmount >= 0)
                                {
                                    string reason = "";
                                    TextPad tp1 = new TextPad("Enter Discount Description", "");
                                    Utility.OpenModal(m_parent, tp1);
                                    reason = tp1.ReturnText;

                                    CurrentTicket.DiscountLineItem(line.ID, np.ReturnPromotion.DiscountAmount, "Manual Discount", reason);

                                }
                                break;

                            case "Remove":

                                CurrentTicket.DiscountLineItem(line.ID, 0, "", "");
                                break;

                            default:

                                //normal predefined discounts
                                //check for security override requirements
                                if (CurrentTicket.CurrentEmployee.SecurityLevel < np.ReturnPromotion.SecurityLevel)
                                {
                                    var manager = m_security.GetEmployee();
                                    if (manager.SecurityLevel < np.ReturnPromotion.SecurityLevel)
                                    {
                                        TouchMessageBox.Show("Need Approval Override!!");
                                        return;
                                    }
                                }

                      

                                //limited usage
                                if (np.ReturnPromotion.LimitedUseOnly && np.ReturnPromotion.UsageNumber == 0)
                                {
                                    TouchMessageBox.Show("Limted Number of Usage Reached!!");
                                    return;
                                }

                                if (line.AdjustedPrice < np.ReturnPromotion.MinimumAmount)
                                {
                                    TouchMessageBox.Show("Amount must be atleast:" + np.ReturnPromotion.MinimumAmount);
                                    return;
                                }

                             

                                decimal discount = 0;
                                switch (np.ReturnPromotion.DiscountMethod)
                                {
                                    case "PERCENT":
                                        discount = line.Price * np.ReturnPromotion.DiscountAmount / 100;
                                        break;

                                    case "AMOUNT":
                                        discount = np.ReturnPromotion.DiscountAmount;
                                        break;

                                }


                                CurrentTicket.DiscountLineItem(line.ID, discount, np.ReturnPromotion.DiscountType, np.ReturnPromotion.Description);
                                if (np.ReturnPromotion.LimitedUseOnly)
                                {
                                    //reduce usage by one
                                    np.ReturnPromotion.DeductUsage();
                                }
                                break;

                        }
                        np = null;

                        break;

                    case "NoDiscount":

                        if (CurrentTicket.Status == "Closed") TouchMessageBox.Show("Ticket is Closed!");
                        else
                        {
                            if (m_security.WindowNewAccess("Discount") == false) return; //jump out of routine

                            CurrentTicket.DiscountLineItem(line.ID, 0, "", "");
                        }
                        break;

                    case "Upgrade":

                        if (CurrentTicket.Status == "Closed") TouchMessageBox.Show("Ticket is Closed!");
                        else
                        {
                            if (m_security.WindowNewAccess("Upgrade") == false) return; //jump out of routine

                            NumPad np2 = new NumPad("Enter Upgrade Amount",false,line.Surcharge.ToString());
                            Utility.OpenModal(m_parent, np2);
                            if (np2.Amount != "")
                            {
                                if (decimal.Parse(np2.Amount) >= 0)
                                {
                                    CurrentTicket.UpgradeLineItem(id, decimal.Parse(np2.Amount));
                                    // if (portion[1] == "Service") CurrentTicket.DiscountService(id, decimal.Parse(np.Amount));
                                }
                            }
                            np = null;
                        }
                        break;


                    case "NoUpgrade":

                        if (CurrentTicket.Status == "Closed") TouchMessageBox.Show("Ticket is Closed!");
                        else
                        {
                            if (m_security.WindowNewAccess("Upgrade") == false) return; //jump out of routine

                            CurrentTicket.UpgradeLineItem(id, 0);
                        
                        }
                        break;

                    case "Notes":


                        TextPad tp = new TextPad("Item Notes:", line.Note);
                        Utility.OpenModal(m_parent, tp);
                        CurrentTicket.UpdateNote(id, tp.ReturnText);
                        tp = null;



                        break;

    
                }


            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Execute Line Item Clicked: " + e.Message);
            }
        }


        public void ExecutePaymentDeleteClicked(object paymentid)
        {
            ConfirmDelete dlg;

            try
            {
                if (CurrentTicket.Status == "Closed")
                {
                    TouchMessageBox.Show("Ticket is closed.  Can not modify");
                    return;
                }

                Payment pay = CurrentTicket.GetPaymentLine((int)paymentid);


                // can't delete voided payments...
                if (pay.Voided) return;

                if (!m_security.WindowNewAccess("VoidPayment")) return;

                TextPad tp = new TextPad("Enter Void Reason", "");
                Utility.OpenModal(m_parent, tp);

                if (tp.ReturnText == "") return;


                switch (pay.CardGroup.ToUpper())
                {
                    case "GIFT CERTIFICATE":
                    case "GIFT CARD":
                        dlg = new ConfirmDelete("Deleting Gift Card from payment will refund charged amount back to gift card. Proceed?");
                        Utility.OpenModal(m_parent, dlg);

                        if (dlg.Action == "Delete")
                        {
                            CurrentTicket.VoidPayment((int)paymentid);
                            CurrentTicket.VoidGiftCardPayment(pay.AuthorCode);
                        }
             

                        break;
                    case "REWARD":
                        dlg = new ConfirmDelete("Deleting Reward payment will put reward back to Customer records. Proceed?");
                        Utility.OpenModal(m_parent, dlg);

                        if (dlg.Action == "Delete")
                        {
                            CurrentTicket.VoidPayment((int)paymentid);
                            CurrentTicket.VoidRewardREDEEM();
                        }
                            

                        break;

                    case "CREDIT":


                        dlg = new ConfirmDelete("Void " + pay.CardType + " Payment of " + pay.Amount + "?");
                        Utility.OpenModal(m_parent, dlg);

                        if (dlg.Action == "Delete")
                        {

                            switch (GlobalSettings.Instance.CreditCardProcessor)
                            {
                                case "HeartSIP":
                                    CreditPayment ccp = new CreditPayment(m_parent, CurrentTicket, m_ccp, "VOID", pay.ResponseId);
                                    Utility.OpenModal(m_parent, ccp);
                                    break;

                                case "External":

                                    CurrentTicket.VoidPayment((int)paymentid);
                                    //if tip is already assigned to current employee, then need to remove tip first for current employee
                                    //passing 0 for employee ID will delete all gratuity for that ticket
                                    //this only affects nail salons
                                    CurrentTicket.DeleteGratuity();
                                    break;

                                case "PAX_S300":
                                case "HSIP_ISC250":
                                case "VANTIV":

                                    CCPPayment pax = new CCPPayment(CurrentTicket, "VOID", pay); //pax300 and isc250 on global payments
                                    Utility.OpenModal(m_parent, pax);
                                    break;


                                case "Clover":

                                    CloverPayment clover = new CloverPayment(CurrentTicket,m_security, "VOID", pay,tp.ReturnText);
                                    Utility.OpenModal(m_parent, clover);

                                    break;
                            }


                        }


                        break;

                  

                    default:

                        dlg = new ConfirmDelete("Delete " + pay.CardGroup + " Payment?");
                        Utility.OpenModal(m_parent, dlg);

                        if (dlg.Action == "Delete")
                            CurrentTicket.VoidPayment((int)paymentid);

                        break;

                }
              


                SetVisibility();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Error deleting line item: " + e.Message);
            }
        }

        public void ExecuteVoidClicked(object salesid)
        {
            


            try
            {
                //if ticket is reversed .. meaning it has been closed before...
                if (CurrentTicket.Status == "Reversed")
                {
                    if (m_security.WindowNewAccess("VoidClosedTicket") == false) return;
                }else
                {
                    //if not reversed .. then it's an open ticket .. so need to test for Open Void
                    if (m_security.WindowNewAccess("VoidOpenTicket") == false) return;
                }
                   

               


                // confirm reason for VOIDING ticket
                ConfirmAudit win;
                win = new ConfirmAudit();
                Utility.OpenModal(m_parent, win);
                if (win.Reason != "")
                {


    

                    //06/23/2018  -- delete has been removed .. only VOIDs are allowed
                    CurrentTicket.VoidTicket(win.Reason);
                    CurrentTicket.VoidRewardADD();
                    AuditModel.WriteLog(CurrentEmployee.DisplayName, "Void Ticket", win.Reason, "sales", CurrentTicket.SalesID);
                    
                        
                        CurrentTicket = null;


                        SetVisibility();
                        LoadTickets();

                }


            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Error deleting line item: " + e.Message);
            }
        }
        public void ExecuteCashTenderClicked(object button)
        {


            try
            {


                m_salesmodel.ProcessCash(m_parent, m_currentticket);

                PostProcessPayment();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show(" ExecuteCashTenderClicked:" + e.Message);
            }
        }


        public void ExecuteCreditCardClicked(object button)
        {


            try
            {
             
                m_salesmodel.ProcessCreditCard(m_parent, m_currentticket);
                PostProcessPayment();

            }
            catch (Exception e)
            {

                TouchMessageBox.Show(" ExecuteCreditCardClicked:" + e.Message);
            }
        }


        public void ExecuteCCPClicked(object button)
        {


            try
            {
                switch(GlobalSettings.Instance.CreditCardProcessor)
                {
                    case "HeartSIP":
                        CreditPayment ccp = new CreditPayment(m_parent, CurrentTicket, m_ccp, "SALE", "");
                        Utility.OpenModal(m_parent, ccp);
                        break;


                    case "PAX_S300":
                    case "WORLDPAY":
                    case "PAX":
                    case "VANTIV":
                        CCPPayment ccp2 = new CCPPayment(CurrentTicket, "SALE", null); //pax300 and isc250 on global payments
                        Utility.OpenModal(m_parent, ccp2);
                        break;

                 


                    case "Clover":
                        CloverPayment pay = new CloverPayment(CurrentTicket,m_security,"SALE",null,"");
                        Utility.OpenModal(m_parent, pay);
                        break;


                }
          
              

                CurrentTicket.LoadPayment();
                CurrentTicket.CloseTicket(); //need to load payment  to refresh object first before trying to close ticket
               

                PostProcessPayment();
            }
            catch (Exception e)
            {

                TouchMessageBox.Show(" ExecuteCCPClicked:" + e.Message);
            }
        }

        public void ExecuteVerifyClicked(object button)
        {
            if (!m_security.WindowNewAccess("Verify"))
            {
                // Message("Access Denied.");
                return;
            }

            GCVerify gcv = new GCVerify();
            Utility.OpenModal(m_parent, gcv);

          
        }


        public void ExecuteNewTicketClicked(object button)
        {

    

            if (m_security.WindowNewAccess("SalonSales"))
            {
                CurrentEmployee = m_security.CurrentEmployee;

                if(GlobalSettings.Instance.DelayTicketCreation)
                {

                    CurrentTicket = null;
                }else
                {
             

                    CurrentTicket = new Ticket(CurrentEmployee);

                    //creates a sales record
                    CurrentTicket.CreateTicket();
                    AuditModel.WriteLog(CurrentEmployee.DisplayName, "new ticket", "", "sales", CurrentTicket.SalesID);
                    LoadTicket(CurrentTicket.SalesID); //load ticket will call SetVisibility
                

                    //Ask cashier to enter customer phone number
                    if (GlobalSettings.Instance.AskCustomerPhone)
                    {

                        ExecuteCustomerClicked(null);
                    }
                }

           

             


                LoadTickets();


                if (Categories == null)
                {
                    Categories = m_salesmodel.GetCategoryList();

                    MenuPrefixes = m_salesmodel.GetMenuPrefix();

                }

                if (m_usecheckinlist) LoadCheckInList(0);
                else LoadAllSalesTech();

                VisibleCategory = Visibility.Hidden;
                VisibleProduct = Visibility.Hidden;
                VisibleEmployee = Visibility.Visible;
                ReceiptVisibility = Visibility.Visible;
                VisibleClosed = Visibility.Hidden;


            }


        }



        public void ExecuteTicketClicked(object salesid)
        {
            if (m_security.WindowNewAccess("SalonSales"))
            {
                //test to see if selected ticket belong to employee.

                Ticket selected = new Ticket((int)salesid);

                if(m_security.GetWindowLevel("SalonSales") > 0)  //only check if security is turned on for tickets
                if(!m_security.HasAccess("AllSales"))  //if employee does not have access to all sales
                if(selected.CurrentEmployee.ID != m_security.CurrentEmployee.ID)
                {
                    TouchMessageBox.Show("This ticket does not belong to you.");
                    return;
                }

                CurrentEmployee = m_security.CurrentEmployee;

                //these ticket status must be OPEN , closed ticket can not be operated on
                LoadTicket((int)salesid);
                LoadTickets();
                SelectedIndex = 0;
            }

        }

        public void ExecuteCustomerClicked2(object obj)
        {
            ExecuteCustomerClicked(obj);
            LoadTickets();
        }

        public void ExecuteCustomerSearchClicked(object button)
        {

            //if ticket already has customer number, then bring up edit screen
            if (CurrentTicket.CurrentCustomer != null)
            {
                CustomerViewEditDelete();
                LoadTickets();
                return;

            }


            //if ticket is Closed , you can not add customer
            if (CurrentTicket.Status.ToUpper() == "CLOSED")
            {
                TouchMessageBox.Show("Ticket is Closed, Need to reverse first");
                return;
            }

            //if no customers then code continues below
            CustomerSearch cs = new CustomerSearch(m_security);
            Utility.OpenModal(m_parent, cs);

            //need to check existence of customerid
            if (cs.customerid > 0)
            {
                CurrentTicket.UpdateCustomerID(cs.customerid);

                if(CurrentTicket.CurrentCustomer.Custom2 != "" && CurrentTicket.CurrentCustomer.Custom2.ToUpper() != "NONE")
                {
                    TouchMessageBox.Show("Special Status:  ***   " + CurrentTicket.CurrentCustomer.Custom2 + "   ***");

                }
                if (GlobalSettings.Instance.DisplayRewardAlert)
                {
                    //check to see if customer has usable rewards
                    if (CurrentTicket.CurrentCustomer.UsableBalance > 0)
                    {
                        string message;
                        message = "Customer has Reward: " + CurrentTicket.CurrentCustomer.UsableBalance.ToString("c");
                        TouchMessageBox.Show(message);
                    }

                }

                LoadTickets();
            }

        }




        public void ExecuteGratuityClicked(object salesid)
        {

            try
            {
                // CurrentTicket.LoadGratuity();  //refresh gratuity list                   
                GratuityView gv = new GratuityView(CurrentTicket,m_ccp);
                Utility.OpenModal(m_parent, gv);
              

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("SalesVM:ExecuteGratuityClicked: " + ex.Message);
            }

        }


        public void ExecuteNoSaleClicked(object button)
        {
            //Open cash drawer
            if (m_security.WindowNewAccess("NoSale") == true)
            {
                ReceiptPrinter.CashDrawer(GlobalSettings.Instance.ReceiptPrinter);
                
            }
           

        }


        public void ExecuteAdjustTicketClicked(object button)
        {
            DiscountPad np = new DiscountPad(0, CurrentTicket.SubTotal);
            Utility.OpenModal(m_parent, np);

            if (np.ReturnPromotion == null) return;  /// user cancelled


            switch (np.ReturnPromotion.Description)
            {

                case "Manual":
                    //need manual discount access
                    if (m_security.WindowAccess("Discount") == false) { return; }

                    if (np.ReturnPromotion.DiscountAmount >= 0)
                    {
                        string reason = "";
                        TextPad tp1 = new TextPad("Enter Discount Description", "");
                        Utility.OpenModal(m_parent, tp1);
                        reason = tp1.ReturnText;

                        // currentticket.DiscountLineItem(line.ID, np.ReturnPromotion.DiscountAmount, "Manual Discount", reason);

                        CurrentTicket.AdjustTicket((-1) * np.ReturnPromotion.DiscountAmount, "Manual Discount", reason);
                        AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, np.ReturnPromotion.Description + "=" + np.ReturnPromotion.DiscountAmount.ToString(), reason, "ticket Adjustment", CurrentTicket.SalesID);

                    }
                    break;

                case "Remove":

                    // CurrentTicket.DiscountLineItem(line.ID, 0, "", "");
                    CurrentTicket.AdjustTicket(0, "", "");


                    break;

                default:

                    //normal predefined discounts
                    //check for security override requirements
                    if (CurrentTicket.CurrentEmployee.SecurityLevel < np.ReturnPromotion.SecurityLevel)
                    {
                        var manager = m_security.GetEmployee();
                        if (manager.SecurityLevel < np.ReturnPromotion.SecurityLevel)
                        {
                            TouchMessageBox.Show("Need Approval Override!!");
                            return;
                        }
                    }



                    //limited usage
                    if (np.ReturnPromotion.LimitedUseOnly && np.ReturnPromotion.UsageNumber == 0)
                    {
                        TouchMessageBox.Show("Limted Number of Usage Reached!!");
                        return;
                    }




                    //IS THIS DISCOUNT for Ticket??  Or individual Items , check for restrictions
                    DataTable productlist = np.ReturnPromotion.GetProductIDs();
                    string[] array = productlist.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
                    if (array.Count() > 0)
                    {
                        foreach (TicketEmployee emp in CurrentTicket.TicketEmployees)
                            foreach (LineItem line in emp.LineItems)
                            {
                                //check to see if item is even on list of qualified items
                                if (!array.Contains(line.ProductID.ToString()))
                                {
                                    continue;
                                }
                       
                                if (line.AdjustedPrice < np.ReturnPromotion.MinimumAmount)
                                {
                                    //marke line as bad
                                    CurrentTicket.DiscountLineItem(line.ID, 0, "", "Not Discountable");
                                    continue;  // go to next line
                                }


                                decimal discount = 0;
                                switch (np.ReturnPromotion.DiscountMethod)
                                {
                                    case "PERCENT":
                                        discount = line.Price * np.ReturnPromotion.DiscountAmount / 100;
                                        break;

                                    case "AMOUNT":
                                        discount = np.ReturnPromotion.DiscountAmount;
                                        break;

                                }


                                CurrentTicket.DiscountLineItem(line.ID, discount, np.ReturnPromotion.DiscountType, np.ReturnPromotion.Description);

                                if (np.ReturnPromotion.LimitedUseOnly)
                                {
                                    //reduce usage by one
                                    np.ReturnPromotion.DeductUsage();
                                }



                            }


                    }
                    else
                    {
                        decimal discount = 0;
                        switch (np.ReturnPromotion.DiscountMethod)
                        {
                            case "PERCENT":
                                discount = CurrentTicket.SubTotal * np.ReturnPromotion.DiscountAmount / 100;
                                break;

                            case "AMOUNT":
                                discount = np.ReturnPromotion.DiscountAmount;
                                break;

                        }

                        //apply to whole ticket instead
                        CurrentTicket.AdjustTicket((-1) * discount, np.ReturnPromotion.DiscountType, np.ReturnPromotion.Description);
                        AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, np.ReturnPromotion.DiscountType + "=" + np.ReturnPromotion.DiscountAmount.ToString() + " " +  np.ReturnPromotion.DiscountMethod.ToString(), np.ReturnPromotion.Description, "ticket Adjustment", CurrentTicket.SalesID);


                    }


                    break;

            }
            np = null;


        }


        public void ExecuteGiftCardClicked(object button)
        {
            try
            {
                GiftCardView ccv = new GiftCardView(CurrentTicket,m_ccp);
                Utility.OpenModal(m_parent, ccv);
                CurrentTicket.LoadPayment();
                CurrentTicket.CloseTicket(); //need to load payment  to refresh object first before trying to close ticket
                NotifyPropertyChanged("Payments");

                PostProcessPayment();
            }
            catch (Exception e)
            {

                TouchMessageBox.Show(" ExecuteGiftCardClicked:" + e.Message);
            }


        }

        public void ExecuteStampCardClicked(object button)
        {
            try
            {
                m_salesmodel.ProcessStampCard(m_parent, CurrentTicket);
                PostProcessPayment();
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
                m_salesmodel.ProcessCheck(m_parent, CurrentTicket);
                PostProcessPayment();
            }
            catch (Exception e)
            {
                TouchMessageBox.Show(" ExecuteCheckClicked:" + e.Message);
            }
        }



        public void ExecuteGiftCertificateClicked(object button)
        {
            try
            {

                //Ask for Gift certificate number
                m_salesmodel.ProcessGiftCertificate(m_parent, CurrentTicket);
                PostProcessPayment();

            }
            catch (Exception e)
            {
                TouchMessageBox.Show(" ExecuteGiftCertificateClicked:" + e.Message);
            }
        }
        public void ExecuteRewardClicked(object button)
        {
            try
            {
                RewardView ccv = new RewardView(CurrentTicket);
                Utility.OpenModal(m_parent, ccv);
                CurrentTicket.LoadPayment();
                CurrentTicket.CloseTicket();
                NotifyPropertyChanged("Payments");

                PostProcessPayment();
            }
            catch (Exception e)
            {
                TouchMessageBox.Show(" ExecuteRewardClicked:" + e.Message);
            }
        }


        private void PostProcessPayment()
        {
            try
            {
                //test for close - display change given and such
                if (CurrentTicket.IsClosed)
                {
                    if (CurrentTicket.CurrentCustomer != null)
                    {

                        if (AutoSendReceiptToCustomer || AutoSendMessageToCustomer) worker.RunWorkerAsync();  //send SMS

                    }

                    LoadTickets();

                }


                if (CurrentTicket.IsClosed && AutoHideClosedTicket) CurrentTicket = null;
                SetVisibility();
            }
            catch(Exception ex)
            {
                TouchMessageBox.Show("Post Process Payment:" + ex.Message);
            }
         
        }

        public void RunSMSAsync(object sender, DoWorkEventArgs e)
        {
            bool AutoSendReceiptToCustomer = GlobalSettings.Instance.AutoSendReceiptToCustomer;
            bool AutoSendMessageToCustomer = GlobalSettings.Instance.AutoSendMessageToCustomer;
            string MessageText = GlobalSettings.Instance.SMSCustomerMessage;
            string phonenumber = CurrentTicket.CurrentCustomer.Phone1;
            string receiptstr = GlobalSettings.Instance.Shop.Name + " Receipt: Total Spent= " + CurrentTicket.Total + " ,  Reward Balance=" + CurrentTicket.CurrentCustomer.RewardBalance;

            if (AutoSendReceiptToCustomer) SMSModel.SendSMS(receiptstr, phonenumber);

            if (AutoSendMessageToCustomer) SMSModel.SendSMS(MessageText, phonenumber);

        }

        public void ExecuteCheckInClicked(object button)
        {
            if (m_security.WindowNewAccess("CheckIn"))
            {
                DataTable dt = m_dbemployee.GetCheckIn(m_security.CurrentEmployee.ID, DateTime.Now);
                if (dt.Rows.Count > 0)
                {
                    TouchMessageBox.Show("Already check in!!");
                }
                else
                {
                    m_dbemployee.InsertCheckIn(m_security.CurrentEmployee.ID, DateTime.Now);
                    EmployeeList = m_dbemployee.GetCheckInList(DateTime.Now);
                }

            }
        }



        /*
                public void ExecuteHoldClicked(object button)
                {
                    // Tickets.Add(CurrentTicket);
                    CurrentTicket = null;
                    m_vfd.WriteDisplay("Ticket : ", "none", "Total", 0);
                    //need to refresh ticket list
                    LoadTickets();




                }*/





        public void ExecuteMoreClicked(object button)
        {

            More dlg = new More(this);
            Utility.OpenModal(m_parent, dlg);

        }

        public void ExecuteSettingsClicked(object button)
        {

            if (!m_security.WindowNewAccess("Settings"))
            {
              
                return;
            }
            Settings set = new Settings(m_parent);
            Utility.OpenModal(m_parent, set);
            LoadSettings();

        }


        public void ExecuteBackClicked(object button)
        {
                VisibleCategory = Visibility.Visible;
                VisibleProduct = Visibility.Hidden;
              
        }

        public void ExecuteCategoryBackClicked(object button)
        {
            VisibleEmployee = Visibility.Visible;
            VisibleCategory = Visibility.Hidden;
            VisibleProduct = Visibility.Hidden;

        }

        public void ExecuteHoldClicked(object button)
        {
            CurrentTicket = null;
            SetVisibility();
        }


        public void ExecuteMenuPrefixClicked(object objmenuprefix)
        {

            try
            {
                m_menuprefix1 = objmenuprefix.ToString();
                MenuPrefixes2 = m_salesmodel.GetMenuPrefix2(m_menuprefix1);

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Menu Prefix Clicked:" + e.Message);

            }
        }

        public void ExecuteMenuPrefix2Clicked(object objmenuprefix)
        {

            try
            {
                string menuprefix;
                int productid;

                menuprefix = m_menuprefix1 + objmenuprefix.ToString();
                productid = m_salesmodel.GetProductID(menuprefix);
                if (productid > 0)
                {
                    ExecuteItemClicked(productid);
                    // m_menuprefix1 = "";

                }

            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Menu Prefix 2 Clicked:" + e.Message);

            }
        }



        public void ExecuteTurnsCheckInClicked(object obj)
        {
            if (m_security.WindowNewAccess("EditTurns"))
            {
                Turns dlg = new Turns(m_parent);
                Utility.OpenModal(m_parent, dlg);
            
                LoadCheckInList(m_techid);
            }

           
        }

        public void ExecuteTicketHistoryClicked(object obj)
        {
            if (!m_security.WindowNewAccess("TicketHistory"))
            {
                // Message("Access Denied.");
                return;
            }


            OrderHistory dlg = new OrderHistory(m_security,this.CCP);
            Utility.OpenModal(m_parent, dlg);

            LoadTickets();

        }



        #endregion














        //-----------------------------------------------------------------------------------------------------------------------------
        //     ____ ____  _____ ____ ___ _____    ____    _    ____  ____    ____  ____   ___   ____ _____ ____ ____ ___ _   _  ____ 
        //    / ___|  _ \| ____|  _ \_ _|_   _|  / ___|  / \  |  _ \|  _ \  |  _ \|  _ \ / _ \ / ___| ____/ ___/ ___|_ _| \ | |/ ___|
        //   | |   | |_) |  _| | | | | |  | |   | |     / _ \ | |_) | | | | | |_) | |_) | | | | |   |  _| \___ \___ \| ||  \| | |  _ 
        //   | |___|  _ <| |___| |_| | |  | |   | |___ / ___ \|  _ <| |_| | |  __/|  _ <| |_| | |___| |___ ___) |__) | || |\  | |_| |
        //    \____|_| \_\_____|____/___| |_|    \____/_/   \_\_| \_\____/  |_|   |_| \_\\___/ \____|_____|____/____/___|_| \_|\____|
        //                                                                                                                           
        //-------------------------------------------------------------------------------------------------------------------------------



       


        #region CreditCard
        public void ExecuteCreditCardManagerClicked(object obj)
        {
            if (!m_security.WindowNewAccess("CreditCardManager"))
            {
                // Message("Access Denied.");
                return;
            }

            CurrentTicket = null;

            switch(GlobalSettings.Instance.CreditCardProcessor)
            {
                case "HeartSIP":
                    CreditCardManager dlg = new CreditCardManager(CCP, m_security.CurrentEmployee);
                    Utility.OpenModal(m_parent, dlg);

                    break;
                case "PAX_S300":
                    PAXManager dlg2 = new PAXManager(m_security.CurrentEmployee);
                    Utility.OpenModal(m_parent, dlg2);

                    break;
            }
        
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

                switch(GlobalSettings.Instance.CreditCardProcessor)
                {
                    case "HeartSIP":
                        HeartSIPRefund ccp = new HeartSIPRefund(m_parent, CurrentTicket, m_ccp, "REFUND", "");
                        Utility.OpenModal(m_parent, ccp);

                        break;

                    case "PAX_S300":
                    case "HSIP_ISC250":
                    case "VANTIV":
                        PAXRefund pax = new PAXRefund(CurrentTicket);
                        Utility.OpenModal(m_parent, pax);
                        break;


                    case "Clover":
                        CloverRefund clover = new CloverRefund(CurrentTicket,m_security);
                        Utility.OpenModal(m_parent, clover);
                        break;


                    case "External":

                        CreditCardView ccv = new CreditCardView(m_parent, CurrentTicket,"REFUND");
                        Utility.OpenModal(m_parent, ccv);
                        break;

                }
          
              

                CurrentTicket.LoadPayment();

              //  CurrentTicket.CloseTicket(); //need to load payment  to refresh object first before trying to close ticket

                NotifyPropertyChanged("Payments");
                //test for close - display change given and such
                if (CurrentTicket.IsClosed) LoadTickets();



            }
            catch (Exception e)
            {

                TouchMessageBox.Show(" ExecuteRefundClicked:" + e.Message);
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


        private void Response_Gift(GiftResponseArgs e)
        {
            m_parent.Dispatcher.Invoke(new Action(() =>
            {
                Process_Response_Gift(e);
            }));

        }

        private void Process_Response_Gift(GiftResponseArgs e)
        {


            //send reset command
            m_ccp.SIPSetCommand(HeartPOS.ResetCommand);




            if (e.ResultText.ToUpper() == "SUCCESS")
            {


                switch (e.TransType)
                {
                    case "BalanceInquiry":
                        TouchMessageBox.Show("Balance :" + int.Parse(e.AvailableBalance) / 100m);
                        break;
                    case "AddValue":
                        decimal authamt = int.Parse(e.AuthorizedAmount) / 100m;
                        TouchMessageBox.Show("$" + authamt + "Added to Gift Card");
                        break;
                }




                m_ccp.RequestAmount = 0;

                //Close 
                if (m_ccp.closeform != null) m_ccp.closeform();

            }
            else
            {

                if (e.GatewayRspMsg != "")
                    TouchMessageBox.Show("GIFT TRANSACTION FAILED !!!! ERROR:  " + e.GatewayRspMsg);
                else
                    TouchMessageBox.Show("GIFT TRANSACTION FAILED !!!! ERROR:  " + e.ResultText);


            }




        }







        private void Response_Sales(ResponseEventArgs e)
        {
            m_parent.Dispatcher.Invoke(new Action(() =>
            {
                Process_Response_Sales(e);
            }));

        }
        private void Process_Response_Sales(ResponseEventArgs e)
        {
            // throw new NotImplementedException();

            if (e.Response != "Sale")
            {
                TouchMessageBox.Show(e.ResponseText);
                return;
            }


            //send reset command
            m_ccp.SIPSetCommand(HeartPOS.ResetCommand);


            if (e.AuthorizedAmount == "")
            {
                TouchMessageBox.Show("AuthorizedAmount not in XML Response!!");
                logger.Error("AuthorizedAmount missing from XML Response");
                return;
            }

            if (e.ResultText.ToUpper() == "SUCCESS")
            {
                decimal authamt = int.Parse(e.AuthorizedAmount) / 100m;

                if (authamt >= m_ccp.RequestAmount)
                {
                    bool result = AddCreditPayment(m_ccp.RequestAmount, e, DateTime.Now);

                    if (result == false)
                    {
                        TouchMessageBox.Show("Payment record insert failed.");
                        return;
                    }


                }
                else
                {
                    bool result = AddCreditPayment(authamt, e, DateTime.Now);

                    if (result == false)
                    {
                        TouchMessageBox.Show("Payment record insert failed.");
                        return;
                    }
                }


                //print credit slip
                Payment payment = m_salesmodel.GetPayment(e.ResponseId);
                if (payment != null)
                {
                    ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
                }


                if (e.CardGroup.ToUpper() == "CREDIT" || e.CardGroup.ToUpper() == "GIFT")
                {
                    //this will run the auto tip logic or bring up tip screen if manual mode
                    if (CurrentTicket != null)
                        CurrentTicket.SplitTips();

                }




                /* remove becasue already has Change Back message 08/06/2018
          if (e.CardGroup.ToUpper() == "DEBIT")
          {

              //check to see if there is cashback
              decimal cashbackamount = 0;

              if (e.CashbackAmount != "") cashbackamount = int.Parse(e.CashbackAmount) / 100m;



              if (cashbackamount > 0)
              {
                  // new TouchMessageBox(String.Format("Change Due: {0:c}", cashbackamount)).ShowDialog();



          }
                TouchMessageBox.Show(String.Format("Cash Back: {0:c}", cashbackamount));
              }*/




                m_ccp.RequestAmount = 0;

                //Close 
                m_ccp.closeform();

            }
            else
            {

                if (e.GatewayRspMsg != "")
                {
                    logger.Error(e.GatewayRspMsg);
                    TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + e.GatewayRspMsg);
                }

                else
                {
                    logger.Error(e.ResultText);
                    TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + e.ResultText);
                }





            }




        }




        private void Response_Refund(ResponseEventArgs e)
        {
            m_parent.Dispatcher.Invoke(new Action(() =>
            {
                Process_Response_Refund(e);
            }));

        }
        private void Process_Response_Refund(ResponseEventArgs e)
        {
            // throw new NotImplementedException();

            if (e.Response != "Refund")
            {
                TouchMessageBox.Show(e.ResponseText);
                return;
            }


            //send reset command
            m_ccp.SIPSetCommand(HeartPOS.ResetCommand);




            if (e.ResultText.ToUpper() == "SUCCESS")
            {
                decimal authamt = int.Parse(e.AuthorizedAmount) / 100m;

                if (authamt >= m_ccp.RequestAmount)
                {
                    bool result = AddCreditPayment(m_ccp.RequestAmount, e, DateTime.Now);

                    if (result == false)
                    {
                        TouchMessageBox.Show("Refund record insert failed.");
                        return;
                    }




                    //print credit slip
                    Payment payment = m_salesmodel.GetPayment(e.ResponseId);
                    if (payment != null)
                    {
                        ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
                    }


                }
                else
                {
                    TouchMessageBox.Show("Authorized amount is less than requested.  Authorized:" + authamt);
                }


                m_ccp.RequestAmount = 0;
                //Close 
                if (m_ccp.closeform != null) m_ccp.closeform();

            }
            else
            {

                if (e.GatewayRspMsg != "")
                    TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + e.GatewayRspMsg);
                else
                    TouchMessageBox.Show("SALE TRANSACTION FAILED !!!! ERROR:  " + e.ResultText);


            }




        }

        private void Response_ALL(string response)
        {
            m_parent.Dispatcher.Invoke(new Action(() =>
            {
                Process_Response_ALL(response);
            }));
        }


        private void Process_Response_ALL(string response)
        {
            switch (response)
            {
                case "EOD":
                    ReceiptPrinterModel.PrintResponse("End of Day Report", CCP.POSMessage);
                    break;
                case "GetBatchReport":
                    // ReceiptPrinterModel.PrintEOD("Batch Report", CCP.POSMessage);
                    break;
            }



        }
        private void Response_GetCardData(string requestid, string trackdata)
        {
            m_parent.Dispatcher.Invoke(new Action(() =>
            {
                Process_Response_GetData(requestid, trackdata);
            }));
        }


        private void Process_Response_GetData(string requestid, string trackdata)
        {

        }

        private void Response_TipAdjust(int paymentid, string tipamount, string totalamount)
        {
            m_parent.Dispatcher.Invoke(new Action(() =>
            {
                Process_Response_TipAdjust(paymentid, tipamount, totalamount);
            }));
        }

        private void Process_Response_TipAdjust(int paymentid, string tipamount, string totalamount)
        {
            decimal tip = decimal.Parse(tipamount) / 100m;
            decimal total = decimal.Parse(totalamount) / 100m;

            m_salesmodel.UpdateTipAmount(paymentid, tip, total);

            //this function will also reload the payments and tips
            CurrentTicket.SplitTips();


        }


        private void Response_Void(VoidResponseArgs e)
        {
            m_parent.Dispatcher.Invoke(new Action(() =>
            {
                Process_Response_Void(e);
            }));
        }

        private void Process_Response_Void(VoidResponseArgs e)
        {

            string originalTransactionid = e.OrigTransactionId;

            //Version 3.2 HeartSIP does not return original transaction ID so we have to get it from temp storage on the CCP object
            if (originalTransactionid == "") originalTransactionid = m_ccp.OriginalTransactionId;

            m_ccp.OriginalTransactionId = ""; //reset id for next transaction


            //send reset command
            m_ccp.SIPSetCommand(HeartPOS.ResetCommand);


            if (e.ResultText.ToUpper() == "SUCCESS")
            {

                //update payment

                SalesModel.VoidCreditPayment(originalTransactionid);

                if (CurrentTicket == null)
                {
                    Payment payment = m_salesmodel.GetPayment(originalTransactionid);
                    //print debit slip
                    if (payment != null)
                    {
                        ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
                    }
                }
                else
                {
                    //force ticket to load payment into object so it will contain the tip
                    CurrentTicket.LoadGratuity();
                    CurrentTicket.LoadPayment();


                    Payment payment = CurrentTicket.Payments.Where(x => x.ResponseId == originalTransactionid).First();
                    if (payment.TipAmount > 0)
                    {
                        //if tip is already assigned to current employee, then need to remove tip first for current employee
                        //passing 0 for employee ID will delete all gratuity for that ticket
                        //this only affects nail salons
                        CurrentTicket.DeleteGratuity();
                    }






                    //print debit slip
                    if (payment != null)
                    {
                        ReceiptPrinterModel.AutoPrintCreditSlip(CurrentTicket, payment);
                    }


                    if (m_ccp.closeform != null) m_ccp.closeform();

                }





            }
            else
            {

                if (e.GatewayRspCode == "3")

                    TouchMessageBox.Show("VOID FAILED !!!! Transaction has been previously settled.  " + e.GatewayRspMsg);
                else
                    TouchMessageBox.Show(e.GatewayRspMsg);


            }




        }


        private void Response_Message(int messagetype, string message, string command)
        {
            m_parent.Dispatcher.Invoke(new Action(() =>
            {

                Process_Response_Message(messagetype, message, command);
            }));
        }



        private void Process_Response_Message(int MessageType, string DisplayMessage, string command)
        {
            int id = 0;


            if (MessageType == 2 && command.ToUpper() == "CARDVERIFY")
            {
                TouchMessageBox.Show(XMLGetTagValue(DisplayMessage, "ResponseText"));
            }


            if (MessageType < 3)  //response message
            {
                string requestid = XMLGetTagValue(DisplayMessage, "RequestId");
                int.TryParse(requestid, out id);
            }



            //Save message to database for debugging / auditing
            if (CurrentTicket == null)
                AuditModel.WriteLog(CurrentEmployee.DisplayName, "Heart SIP", DisplayMessage, command, id);
            else
                AuditModel.WriteLog(CurrentTicket.CurrentEmployee.DisplayName, "Heart SIP", DisplayMessage, command, CurrentTicket.SalesID);

            try
            {
                logger.Debug(DisplayMessage);

            }
            catch (Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
            }


        }




        public bool AddCreditPayment(decimal requested_amount, ResponseEventArgs response, DateTime paymentdate)
        {

            try
            {



                if (CurrentTicket == null)
                    return SalesModel.InsertCreditPayment(0, requested_amount, response, paymentdate);
                else
                {
                    vfd.WriteDisplay(response.CardType + ":", requested_amount, "Balance:", CurrentTicket.Balance);
                    return SalesModel.InsertCreditPayment(CurrentTicket.SalesID, requested_amount, response, paymentdate);
                }



            }
            catch (Exception e)
            {
                TouchMessageBox.Show("AddPayment:" + e.Message);
                return false;
            }
        }


        public String XMLGetTagValue(string Message, String Tag)
        {

            XDocument XMLDoc = new XDocument();
            XMLDoc = XDocument.Parse(Message);

            XElement element = XMLDoc.Root.Element(Tag);
            if (element != null)
                return element.Value;

            return "";
        }

        #endregion


        //----------------------------  END CREDIT CARD Processing -----------------------------------



















    } //---------- End of SalesViewModel Class



}
