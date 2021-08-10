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
using RedDot.Models;

namespace RedDot
{
    public class SalesVM : INPCBase
    {


        private ObservableCollection<Product>      m_products;
        private ObservableCollection<Product> m_services;  //these are products
        private DataTable      m_favorites;    //these are labors
        private DataTable m_shippings;


        private ObservableCollection<Product> m_searchresults;
        private DataTable                   m_searchresults2;    //these are labors
        private DataTable      m_categories1;
        private DataTable      m_categories2;
        private DataTable      m_categories3;
        private DataTable       m_categories4;
        private DataTable m_subcategories1;
        private DataTable m_subcategories2;
        private DataTable m_subcategories3;
        private DataTable m_subcategories4;
        private DataTable      m_tickets;
        private Ticket         m_currentticket;
        private Visibility     m_visible_category;
        private Visibility     m_visible_product;
        private Visibility     m_visible_quicksale;
        private Visibility     m_visible_newticketbutton;
        private Visibility     m_paidinfull;
        private Visibility     m_shopfeevisibility;
        private Visibility      m_shippingvisibility;
        private Visibility     m_salestaxvisibility;
        private Visibility     m_creditcardsurchargevisibility;
        public bool            m_isbalance;
        public bool            m_ischangedue;
        public bool            m_editmode = false;
        public bool            m_loadedtickets = false;


        private SalesModel          m_salesModel;
        private InventoryModel m_inventorymodel;
        private Security       m_security;
     
        private CustomerModel  m_customermodel;
        private int            m_selectedindex;
        private bool CanExecute = true;
        //private bool CatLoaded = false;

        private string m_searchmessage;
        private string m_searchmessage2;

        private string m_shoptype;

        private Window m_parent;
        private ReceiptPrinter m_printer;


        public ICommand WorkOrderClicked { get; set; }
        public ICommand ShipOrderClicked { get; set; }

        public ICommand CategoryProductClicked { get; set; }
        public ICommand CategoryServiceClicked { get; set; }
        public ICommand CategoryFavoriteClicked { get; set; }
        public ICommand CategoryShippingClicked { get; set; }


        public ICommand ProductClicked { get; set; }
        //  public ICommand ServiceClicked { get; set; }
        public ICommand SearchProductClicked { get; set; }
        public ICommand SearchServiceClicked { get; set; }
        public ICommand LineItemClicked { get; set; }
        public ICommand PaymentDeleteClicked { get; set; }
        public ICommand CashTenderClicked { get; set; }
        public ICommand CreditCardClicked { get; set; }
        public ICommand GiftCardClicked { get; set; }

        public ICommand CheckClicked { get; set; }
        public ICommand StoreCreditClicked { get; set; }
        public ICommand GiftCertificateClicked { get; set; }

        public ICommand NewTicketClicked { get; set; }
        public ICommand PrintReceiptClicked { get; set; }
        public ICommand PrintLargeReceiptClicked { get; set; }
        public ICommand PrintPDFClicked { get; set; }

        public ICommand EmailPDFClicked { get; set; }
        public ICommand NoSaleClicked { get; set; }
        public ICommand VoidClicked { get; set; }

        public ICommand CustomerClicked { get; set; }
        public ICommand EmployeeClicked { get; set; }
        //public ICommand SettingsClicked { get; set; }
        public ICommand DiscountClicked { get; set; }
        public ICommand RewardClicked { get; set; }
  
        public ICommand HoldClicked { get; set; }
        public ICommand ExitClicked { get; set; }
        public ICommand TicketClicked { get; set; }
        public ICommand MoreClicked { get; set; }

        public ICommand VerifyClicked { get; set; }

        public ICommand ReverseOrderClicked { get; set; }
        public ICommand SettleClicked { get; set; }
        public ICommand PendingClicked { get; set; }

        public ICommand ReOpenClicked { get; set; }

        public ICommand TaxExemptClicked { get; set; }


        //Search Ticket Buttons
        public ICommand SearchByCustomerClicked { get; set; }
        public ICommand SearchByTicketClicked { get; set; }
        public ICommand SearchByDateClicked { get; set; }
        public ICommand SearchByEmployeeClicked { get; set; }
        public ICommand BrowseAllClicked { get; set; }

        public ICommand CreateRefundClicked { get; set; }

        public ICommand InternalNoteClicked { get; set; }

        public ICommand NoteClicked { get; set; }


        // DispatcherTimer dispatchTimer = new DispatcherTimer();


        public SalesVM(Window parent, Security security, int id)
        {

            m_parent = parent;
            m_security = security;
            SelectedIndex = 0;

            LoadSettings();
            Initialize();

            //set default ticket if passed ID , these tickets must be CLOSED
            if (id > 0)
            {
                LoadTicket(id); 
                VisibleNewticketbutton = Visibility.Hidden;  // the new button since we don't want it to be visible
                m_editmode = true;
            }
            else
            {

                //Need to Load Existing OPEN tickets from Database but only if not ticket (reversed) was passed in
                m_editmode = false;
                LoadTickets();
            }
        }



        public void LoadSettings()
        {


            //get settings from Database
            m_printer = new ReceiptPrinter(GlobalSettings.Instance.ReceiptPrinter);

            if (GlobalSettings.Instance.ShopFeePercent > 0) m_shopfeevisibility = Visibility.Visible; else m_shopfeevisibility = Visibility.Collapsed;
            if (GlobalSettings.Instance.SalesTaxPercent > 0) m_salestaxvisibility = Visibility.Visible; else m_salestaxvisibility = Visibility.Collapsed;
 
            m_shoptype = GlobalSettings.Instance.Shop.Type;
        }



        public void Initialize()
        {

            //Assign command relays
            CategoryProductClicked = new RelayCommand(ExecuteCategoryProductClicked, param => this.CanExecute);
            CategoryServiceClicked = new RelayCommand(ExecuteCategoryServiceClicked, param => this.CanExecute);
            CategoryFavoriteClicked = new RelayCommand(ExecuteCategoryFavoriteClicked, param => this.CanExecute);
            CategoryShippingClicked = new RelayCommand(ExecuteCategoryShippingClicked, param => this.CanExecute);

            ProductClicked = new RelayCommand(ExecuteProductClicked, param => this.CanExecute);
            // ServiceClicked = new RelayCommand(ExecuteServiceClicked, param => this.CanExecute);
            SearchProductClicked = new RelayCommand(ExecuteSearchProductClicked, param => this.CanExecute);
            SearchServiceClicked = new RelayCommand(ExecuteSearchServiceClicked, param => this.CanExecute);


            LineItemClicked = new RelayCommand(ExecuteLineItemClicked, param => this.CanExecute);
            PaymentDeleteClicked = new RelayCommand(ExecutePaymentDeleteClicked, param => this.CanExecute);
            CashTenderClicked = new RelayCommand(ExecuteCashTenderClicked, param => this.CanExecuteCashTenderClicked);
            CreditCardClicked = new RelayCommand(ExecuteCreditCardClicked, param => this.CanExecuteCreditDebitClicked);
            GiftCardClicked = new RelayCommand(ExecuteGiftCardClicked, param => this.CanExecutePaymentClicked);
      
            CheckClicked = new RelayCommand(ExecuteCheckClicked, param => this.CanExecutePaymentClicked);
            StoreCreditClicked = new RelayCommand(ExecuteStoreCreditClicked, param => false);
            GiftCertificateClicked = new RelayCommand(ExecuteGiftCertificateClicked, param => this.CanExecutePaymentClicked);


            DiscountClicked = new RelayCommand(ExecuteDiscountClicked, param => this.CanExecutePaymentClicked);
            RewardClicked = new RelayCommand(ExecuteRewardClicked, param => this.CanExecuteReward);
   
            NewTicketClicked = new RelayCommand(ExecuteNewTicketClicked, param => this.CanExecuteNullCloseTicket);
            CreateRefundClicked = new RelayCommand(ExecuteCreateRefundClicked, param => this.CanExecuteRefundTicket);

            PrintReceiptClicked = new RelayCommand(ExecutePrintReceiptClicked, param => CanPrintReceiptClickedExecute);
            PrintLargeReceiptClicked = new RelayCommand(ExecutePrintLargeReceiptClicked, param => CanPrintReceiptClickedExecute);
            PrintPDFClicked = new RelayCommand(ExecutePrintPDFClicked, param => CanPrintReceiptClickedExecute);
            EmailPDFClicked = new RelayCommand(ExecuteEmailPDFClicked, param => CanPrintReceiptClickedExecute);

            NoSaleClicked = new RelayCommand(ExecuteNoSaleClicked, param => this.CanExecute);
            VoidClicked = new RelayCommand(ExecuteVoidClicked, param => this.CanExecuteNotClosed);
            CustomerClicked = new RelayCommand(ExecuteCustomerClicked, param => this.CanExecuteNotNullTicket);

            WorkOrderClicked = new RelayCommand(ExecuteWorkOrderClicked, param => this.CanExecuteNotNullTicket);
            ShipOrderClicked = new RelayCommand(ExecuteShipOrderClicked, param => this.CanExecuteNotNullTicket);

            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => true);
            //SettingsClicked = new RelayCommand(ExecuteSettingsClicked, param => this.CanExecute);
            HoldClicked = new RelayCommand(ExecuteHoldClicked, param => this.CanExecuteOpenTicket);
            ExitClicked = new RelayCommand(ExecuteExitClicked, param => this.CanExecute);
            TicketClicked = new RelayCommand(ExecuteTicketClicked, param => this.CanExecuteNullCloseTicket);
            MoreClicked = new RelayCommand(ExecuteMoreClicked, param => this.CanExecute);
            VerifyClicked = new RelayCommand(ExecuteVerifyClicked, param => this.CanExecute);
            ReverseOrderClicked = new RelayCommand(ExecuteReverseOrderClicked, param => this.CanExecuteClosedTicket);
            SettleClicked = new RelayCommand(ExecuteSettleClicked, param => this.CanExecuteReversedTicket);
            PendingClicked = new RelayCommand(ExecutePendingClicked, param => this.CanExecuteOpenTicket);
            ReOpenClicked = new RelayCommand(ExecuteReOpenClicked, param => this.CanExecutePendingTicket);
            // OpenOrderClicked = new RelayCommand(ExecuteOpenOrderClicked, param => this.CanExecute);
            TaxExemptClicked = new RelayCommand(ExecuteTaxExemptClicked, param => this.CanExecuteNotClosed);

            //Ticket Search Buttons
            SearchByCustomerClicked = new RelayCommand(ExecuteSearchByCustomerClicked, param => this.CanExecute);
            SearchByTicketClicked = new RelayCommand(ExecuteSearchByTicketClicked, param => this.CanExecute);
            SearchByDateClicked = new RelayCommand(ExecuteSearchByDateClicked, param => this.CanExecute);
            SearchByEmployeeClicked = new RelayCommand(ExecuteSearchByEmployeeClicked, param => this.CanExecute);
            BrowseAllClicked = new RelayCommand(ExecuteBrowseAllClicked, param => this.CanExecute);


            InternalNoteClicked = new RelayCommand(ExecuteInternalNoteClicked, param => this.CanExecuteNotNullTicket);
            NoteClicked = new RelayCommand(ExecuteNoteClicked, param => this.CanExecuteNotNullTicket);


            m_salesModel = new SalesModel();
            m_inventorymodel = new InventoryModel();




            m_customermodel = new CustomerModel(m_parent);



            //set startup defaults
            VisibleQuickSale = Visibility.Hidden;
            VisibleCategory = Visibility.Hidden;  //Collapsed, Hidden , Visible
            VisibleProduct = Visibility.Hidden;
            VisibleNewticketbutton = Visibility.Visible;
            PaidInFull = Visibility.Hidden;


            IsChangeDue = false;
            IsBalance = true;
            HasPayment = false;


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
            CurrentTicket = new Ticket(salesid);


            if (CurrentTicket != null)
            {
                VFD.WriteDisplay("Ticket : ", CurrentTicket.SalesID.ToString(), "Total", CurrentTicket.Total);

            }

            if (Categories1 == null )
            {
                Categories1 = m_inventorymodel.GetCategoryList("Cat1",0);
                Categories2 = m_inventorymodel.GetCategoryList("Cat2",0);
                Categories3 = m_inventorymodel.GetCategoryList("Cat3",0);
                Categories4 = m_inventorymodel.GetCategoryList("Cat4",0);
                // CatLoaded = true;
            }


            SetVisibility();

        }


        public void LoadTickets()
        {
            if(!m_loadedtickets)  Tickets = m_salesModel.LoadOpenTicketsByEmployee(m_security.CurrentEmployee.ID, HasPayment);
            SelectedIndex = 5;
            SetVisibility();

        }

  

        public void SetVisibility()
        {
            if (m_currentticket != null)
            {

                if (m_currentticket.Balance < 0)
                {
                    IsChangeDue = true;
                    IsBalance = false;
                }
                else
                {
                    IsChangeDue = false;
                    IsBalance = true;
                }



                if (m_currentticket.Status == "Closed") //closed ticket - paid in full and allow create new
                {
                    PaidInFull = Visibility.Visible;
                    VisibleCategory = Visibility.Hidden;
                    if (m_editmode) VisibleNewticketbutton = Visibility.Hidden;
                    else VisibleNewticketbutton = Visibility.Visible;
                    Products = null;

                }
                else //open ticket
                {
                    PaidInFull = Visibility.Hidden;

                    VisibleCategory = Visibility.Visible;
                    VisibleNewticketbutton = Visibility.Hidden;
                }

                if (CurrentTicket.CreditCardSurcharge > 0) CreditCardSurchargeVisibility = Visibility.Visible;
                else CreditCardSurchargeVisibility = Visibility.Collapsed;

            }
            else
            {
                //null ticket so allow to create new
                VisibleCategory = Visibility.Hidden;
                VisibleProduct = Visibility.Hidden;
                VisibleNewticketbutton = Visibility.Visible;
                CreditCardSurchargeVisibility = Visibility.Collapsed;

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

        public Security CurrentSecurity
        {
            get { return m_security; }
            set { m_security = value;
            NotifyPropertyChanged("CurrentSecurity");
            }
        }
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
        public bool IsBalance
        {
            get { return m_isbalance; }
            set { m_isbalance = value; NotifyPropertyChanged("IsBalance"); }
        }

        public bool IsChangeDue
        {
            get { return m_ischangedue; }
            set { m_ischangedue = value; NotifyPropertyChanged("IsChangeDue"); }
        }

        private bool m_haspayment;
        public bool HasPayment
        {
            get { return m_haspayment; }
            set
            {
                m_haspayment = value;
                NotifyPropertyChanged("HasPayment");
            }
        }

        public Ticket CurrentTicket
        {
            get { return m_currentticket; }

            set
            {
                m_currentticket = value;
                NotifyPropertyChanged("CurrentTicket");
            }
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
        public DataTable Categories1
        {
            get
            {
                return m_categories1;
            }

            set
            {
                if (value != m_categories1)
                {
                    m_categories1 = value;
                    NotifyPropertyChanged("Categories1");
                }
            }
        }

        public DataTable SubCategories1
        {
            get
            {
                return m_subcategories1;
            }

            set
            {
                if (value != m_subcategories1)
                {
                    m_subcategories1 = value;
                    NotifyPropertyChanged("SubCategories1");
                }
            }
        }
        public DataTable Categories2
        {
            get
            {
                return m_categories2;
            }

            set
            {
                if (value != m_categories2)
                {
                    m_categories2 = value;
                    NotifyPropertyChanged("Categories2");
                }
            }
        }

        public DataTable SubCategories2
        {
            get
            {
                return m_subcategories2;
            }

            set
            {
                if (value != m_subcategories2)
                {
                    m_subcategories2 = value;
                    NotifyPropertyChanged("SubCategories2");
                }
            }
        }


        public DataTable Categories3
        {
            get
            {
                return m_categories3;
            }

            set
            {
                if (value != m_categories3)
                {
                    m_categories3 = value;
                    NotifyPropertyChanged("Categories3");
                }
            }
        }

        public DataTable SubCategories3
        {
            get
            {
                return m_subcategories3;
            }

            set
            {
                if (value != m_subcategories3)
                {
                    m_subcategories3 = value;
                    NotifyPropertyChanged("SubCategories3");
                }
            }
        }
        public DataTable Categories4
        {
            get
            {
                return m_categories4;
            }

            set
            {
                if (value != m_categories4)
                {
                    m_categories4 = value;
                    NotifyPropertyChanged("Categories4");
                }
            }
        }
        public DataTable SubCategories4
        {
            get
            {
                return m_subcategories4;
            }

            set
            {
                if (value != m_subcategories4)
                {
                    m_subcategories4 = value;
                    NotifyPropertyChanged("SubCategories4");
                }
            }
        }


        public ObservableCollection<Product> Products
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


        public ObservableCollection<Product> Services //products
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


        public DataTable Shippings
        {

            get
            {
                return this.m_shippings;
            }

            set
            {
                if (value != this.m_shippings)
                {
                    this.m_shippings = value;
                    NotifyPropertyChanged("Shippings");
                }
            }
        }


        public ObservableCollection<Product> SearchResults
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

        public DataTable SearchResults2
        {

            get
            {

                return this.m_searchresults2;
            }

            set
            {
                if (value != this.m_searchresults2)
                {
                    this.m_searchresults2 = value;
                    NotifyPropertyChanged("SearchResults2");
                }
            }
        }

        public string SalesCustomName1
        {
            get { return GlobalSettings.Instance.SalesCustomName1; }
        }

        public string SalesCustomName2
        {
            get { return GlobalSettings.Instance.SalesCustomName2; }
        }
        public string SalesCustomName3
        {
            get { return GlobalSettings.Instance.SalesCustomName3; }
        }
        public string SalesCustomName4
        {
            get { return GlobalSettings.Instance.SalesCustomName4; }
        }




        public string SearchMessage
        {
            get { return m_searchmessage; }

            set
            {
                if (value != null) m_searchmessage = value;
                NotifyPropertyChanged("SearchMessage");
            }
        }

        public string SearchMessage2
        {
            get { return m_searchmessage2; }

            set
            {
                if (value != null) m_searchmessage2 = value;
                NotifyPropertyChanged("SearchMessage2");
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


        public Visibility SalesTaxVisibility
        {
            get
            {
                return m_salestaxvisibility;
            }
            set
            {
                m_salestaxvisibility = value;
                NotifyPropertyChanged("SalesTaxVisibility");
            }
        }
        public Visibility ShopFeeVisibility
        {
            get
            {
                return m_shopfeevisibility;
            }
            set
            {
                m_shopfeevisibility = value;
                NotifyPropertyChanged("ShopFeeVisibility");
            }
        }


        public Visibility ShippingVisibility
        {
            get
            {
                return m_shippingvisibility;
            }
            set
            {
                m_shippingvisibility = value;
                NotifyPropertyChanged("ShippingVisibility");
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
        //------------------------------------------------------------------------------------------
        //  ____        _   _                ____  _           _     _      
        // | __ ) _   _| |_| |_ ___  _ __   |  _ \(_)___  __ _| |__ | | ___ 
        // |  _ \| | | | __| __/ _ \| '_ \  | | | | / __|/ _` | '_ \| |/ _ \
        // | |_) | |_| | |_| || (_) | | | | | |_| | \__ \ (_| | |_) | |  __/
        // |____/ \__,_|\__|\__\___/|_| |_| |____/|_|___/\__,_|_.__/|_|\___|                                                                 
        //
        //------------------------------------------------------------------------------------------ 
        public bool CanExecuteNoPayment
        {
            get
            {
                if (CurrentTicket == null) return false;
                return !CurrentTicket.HasPayment;
            }
        }

        public bool CanExecuteReversedTicket
        {
            get
            {
                if (m_currentticket == null) return false;
                if (m_currentticket.Status == "Reversed")
                {
                    return true;
                }
                else return false;
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

        public bool CanExecutePendingTicket
        {
            get
            {
                if (m_currentticket == null) return false;
                if (m_currentticket.Status == "Pending")
                {
                    return true;
                }
                else return false;
            }
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

        public bool CanExecuteRefundTicket
        {
            get
            {
                if (m_currentticket == null) return false;
                if (CanExecuteClosedTicket && !CurrentTicket.IsRefund)
                {
                    return true;
                }
                else return false;
            }
        }

        public bool CanExecuteNotOpen
        {
            get
            {

                if (m_currentticket == null) return true;

                if (m_currentticket.Status != "Open")
                {
                    return true;
                }
                else return false;
            }

        }
        public bool CanExecuteNotClosed
        {
            get
            {

                if (m_currentticket == null) return false;

                if (m_currentticket.Status != "Closed")
                {
                    return true;
                }
                else return false;
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

                if (m_currentticket == null) return false;
                if (m_currentticket.Status != "Closed" && m_currentticket.Status != "Pending" && m_currentticket.ItemCount >= 1)
                {
                    return true;
                }
                else return false;
            }

        }
        public bool CanExecuteCashTenderClicked
        {
            get
            {

                if (m_currentticket == null) return false;
              //  if (_currentticket.HasBeenPaid("Cash")) return false;
                return CanExecutePaymentClicked;
            }

        }


        public bool CanExecuteCreditDebitClicked
        {
            get
            {

                if (m_currentticket == null) return false;
               // if (_currentticket.CreditPaymentID > 0) return false;
                return CanExecutePaymentClicked;
            }

        }



  


        public bool CanExecuteNullCloseTicket
        {
            get
            {
                if (m_currentticket == null) return true;
                else
                {
                    if (m_currentticket.Status == "Closed") return true;
                    else return false;
                }
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
                    if (m_currentticket.LineItems.Count > 0) return true;
                    else return false;

                }

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
        public void ExecuteSettleClicked(object salesid)
        {
            if (m_security.WindowNewAccess("SettleTicket") == false)
            {
                MessageBox.Show("Access Denied.");
                return;
            }


            Confirm conf = new Confirm("Are you sure??");
            Utility.OpenModal(m_parent, conf);
            if (conf.Action == "Yes")
            {
                CurrentTicket.CloseTicket(true);
                //test for close
                if (m_currentticket.Status == "Closed")
                {

                    Decimal dec = 0m;

                    SetVisibility();


                    if (m_currentticket.Balance > 0)  //still has a balance
                    {
                        MessageBox.Show(String.Format("Ticket has a Balance: {0:c}", m_currentticket.Balance));

                    }
                    else
                    {
                        dec = m_currentticket.Balance * (-1m);
                        if (dec > 0) //change is due
                            MessageBox.Show(String.Format("Change Due: {0:c}", dec));
                    }

                }
            }


        }

        public void ExecutePendingClicked(object salesid)
        {
            if (m_security.WindowNewAccess("PendingTicket") == false)
            {
                MessageBox.Show("Access Denied.");
                return;
            }


            Confirm conf = new Confirm("Are you sure??");
            Utility.OpenModal(m_parent, conf);
            if (conf.Action == "Yes")
            {
                CurrentTicket.PendingTicket();
         
            }


        }

        public void ExecuteReOpenClicked(object salesid)
        {
            if (m_security.WindowNewAccess("ReOpenTicket") == false)
            {
                MessageBox.Show("Access Denied.");
                return;
            }


            Confirm conf = new Confirm("Are you sure??");
            Utility.OpenModal(m_parent, conf);
            if (conf.Action == "Yes")
            {
                CurrentTicket.ReOpenTicket();

            }


        }


        public void ExecuteReverseOrderClicked(object salesid)
        {
            if (m_security.WindowAccess("ReverseTicket") == false)
            {
                MessageBox.Show("Access Denied.");
                return;
            }


            Confirm conf = new Confirm("Are you sure??");
            Utility.OpenModal(m_parent, conf);
            if (conf.Action == "Yes")
            {
                CurrentTicket.ReverseTicket();
                m_parent.Close();
            }


        }

        public void ExecuteCategoryProductClicked(object objid)
        {

            int id = 0;
            ObservableCollection<Product> selprod = new ObservableCollection<Product>();
            DataTable sel;

            try
            {
                if (objid != null) id = (int)objid;

                if (id != 0)
                {
                   
                        sel = m_salesModel.GetProductsByCat(id);

                        foreach (DataRow row in sel.Rows)
                        {
                            Product prod = new Product(row);
                            selprod.Add(prod);
                        }
                        Products = selprod;

                    SubCategories1 = m_inventorymodel.GetCategoryList("SubCat", id);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteCategoryProductClicked:" + e.Message);

            }
        }

        public void ExecuteCategoryServiceClicked(object objid)
        {
            int id = 0;
            ObservableCollection<Product> selprod = new ObservableCollection<Product>();
            DataTable sel;

            try
            {
                if (objid != null) id = (int)objid;


                if (id != 0)
                {

                    sel = m_salesModel.GetProductsByCat(id);
                    foreach (DataRow row in sel.Rows)
                    {
                        Product prod = new Product(row);
                        selprod.Add(prod);
                    }
                    Services = selprod;


                    SubCategories2 = m_inventorymodel.GetCategoryList("SubCat", id);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteCategoryServiceClicked:" + e.Message);

            }
        }

        public void ExecuteCategoryFavoriteClicked(object objid)
        {
            int id = 0;

            try
            {
                if (objid != null) id = (int)objid;


                if (id != 0)
                {

                    Favorites = m_salesModel.GetProductsByCat(id);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteCategoryFavoriteClicked:" + e.Message);

            }
        }


        public void ExecuteCategoryShippingClicked(object objid)
        {
            int id = 0;

            try
            {
                if (objid != null) id = (int)objid;


                if (id != 0)
                {

                    Shippings = m_salesModel.GetProductsByCat(id);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteCategoryShippingClicked:" + e.Message);

            }
        }



        public void ExecuteSearchProductClicked(object searchtag)
        {

            string searchvalue;
            searchvalue = "";
            ObservableCollection<Product> selprod = new ObservableCollection<Product>();
            DataTable sel;

            try
            {
                if (searchtag != null)
                {
                    searchvalue = searchtag.ToString().Trim();
                }


                if (searchvalue != "")
                {
                    var watch = Stopwatch.StartNew();
                    sel= m_salesModel.FindProducts("product", searchvalue);
                    foreach (DataRow row in sel.Rows)
                    {
                        Product prod = new Product(row);
                        selprod.Add(prod);
                    }
                    SearchResults = selprod;
                    SearchResults2 = null;
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    SearchMessage = sel.Rows.Count.ToString() + " Items found in " + elapsedMs + " millisec";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteSearchProductClicked:" + e.Message);

            }
        }

        public void ExecuteSearchServiceClicked(object searchtag)
        {

            string searchvalue;
            searchvalue = "";
     

            try
            {
                if (searchtag != null)
                {
                    searchvalue = searchtag.ToString().Trim();
                }


                if (searchvalue != "")
                {
                    var watch = Stopwatch.StartNew();
                    SearchResults2 = m_salesModel.FindProducts("service", searchvalue);
                    SearchResults = null;

                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    SearchMessage = SearchResults2.Rows.Count.ToString() + " Items found in " + elapsedMs + " milliseconds";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteSearchServiceClicked:" + e.Message);

            }
        }




        public void ExecuteProductClicked(object obj_id)
        {


            int productid = 0;
            decimal price;


            Product product;



            try
            {
                if (CurrentTicket.Status == "Closed")
                {
                    MessageBox.Show("Cannot add item to Closed ticket");
                    return;
                }

                if (obj_id != null)
                {
                    if (obj_id.ToString() != "") productid = int.Parse(obj_id.ToString());

                }

                product = new Product(productid);
                price = product.Price;

                if (price < 0)
                {
                    NumPad np = new NumPad("Please Enter Custom Amount:",false);
                    Utility.OpenModal(m_parent, np);
                    if (np.Amount != "") price = decimal.Parse(np.Amount);
                    else return;
                }


                switch (product.Type)
                {
                    case "giftcard":

                        //gift card requires special handling
                        CardScanner gcard = new CardScanner();
                        Utility.OpenModal(m_parent, gcard);


                        if (gcard.CardNumber != "")
                        {
                            if (m_currentticket.GiftCardOnLineItem(gcard.CardNumber))
                            {
                                MessageBox.Show("Gift Card already on ticket...");

                            }
                            else
                            {
                                decimal balance = GiftCardModel.GetGiftCardBalance(gcard.CardNumber);
                                if (balance != -99)
                                {
                                    MessageBox.Show("Balance: " + balance + " \n Amount will be added to Card Balance...");
                                }
                                AddItemToTicket(false, product, price, "", gcard.CardNumber, "", "", "");
                            }

                        }
                        break;



                    case "product":

                        //other items
                        AddItemToTicket(false, product, price, "", "", "", "", "");
                        break;

                    case "shipping":

                        //other items
                        AddItemToTicket(false, product, price, "", "", "", "", "");
                        break;


                    case "service":

                        if (GlobalSettings.Instance.AskSurcharge && product.Surcharge > 0)
                        {

                            //need to ask if customer parts or store
                            Confirm conf = new Confirm(GlobalSettings.Instance.SurchargeQuestion);
                            Utility.OpenModal(m_parent, conf);

                            if (conf.Action == "Yes")
                            {
                                AddItemToTicket(true, product, price, "", "", "", "", "");
                            }
                            else
                            {
                                AddItemToTicket(false, product, price, "", "", "", "", "");
                            }

                        }
                        else
                        {
                            AddItemToTicket(false, product, price, "", "", "", "", "");
                        }
                        break;

                }




            }
            catch (Exception e)
            {

                MessageBox.Show("Product Clicked: " + e.Message);
            }
        }



        public void AddItemToTicket(bool addSurcharge, Product prod, decimal price, string note, string custom1, string custom2, string custom3, string custom4)
        {
         
            int quantity = 1;
            LineItem newsalesitem;

            try
            {
                if (m_currentticket == null) return;
                if (m_currentticket.Status == "Closed") return;

                if (CurrentTicket.IsRefund) quantity = -1;

                newsalesitem = m_currentticket.AddProductLineItem(addSurcharge, prod, price, quantity,  note, custom1, custom2, custom3, custom4);

                if (newsalesitem != null)
                {
                    VFD.WriteDisplay(prod.Description, prod.AdjustedPrice, "Total:", m_currentticket.Total);

                }

            }
            catch (Exception e)
            {
                MessageBox.Show("AddItemToTicket:" + e.Message);
            }
        }


        /// ---------------------------------------------------------------------------------------------------------------------Execute Line Item Clicked -----------------------------
        public void ExecuteLineItemClicked(object iditemtype)
        {

            string temp = iditemtype as string;
            string[] portion = temp.Split(',');

            int id = 0;
            string itemtype = portion[1]; // type = service, product , giftcard .. etc..
            LineItemActionView linevw;

            try
            {

                id = int.Parse(portion[0]);

                if (id == 0) return;

                LineItem line = CurrentTicket.GetLineItemLine(id);

                if (CurrentTicket.Status == "Closed") linevw = new LineItemActionView(m_parent, "Closed", line);
                else linevw = new LineItemActionView(m_parent, itemtype, line);
                Utility.OpenModal(m_parent, linevw);


                switch (linevw.Action)
                {

                    case "Quantity":
                        NumPad np1 = new NumPad("Enter Quantity",true);
                        Utility.OpenModal(m_parent, np1);

                        if(np1.Amount != "")
                        {
                            CurrentTicket.UpdateSalesItemQuantity(line.ID, int.Parse(np1.Amount));

                        }
                        break;

                    case "Delete":


                        if (CurrentTicket.Status == "Closed") MessageBox.Show("Ticket is Closed!");
                        else
                        {
                            CurrentTicket.DeleteLineItem(id);
                       

                        }


                        break;

                   

                    case "Cost":

                       
                            if (m_security.WindowAccess("Cost") == false)
                            {
                                MessageBox.Show("Access Denied");
                                return; //jump out of routine
                            }
                            NumPad cost = new NumPad("Old Cost:" + line.Cost + " , Enter New Cost:",false);
                            Utility.OpenModal(m_parent, cost);
                            if (cost.Amount != "")
                            {
                                if (decimal.Parse(cost.Amount) >= 0)
                                {
                                    CurrentTicket.UpdateCost(id, decimal.Parse(cost.Amount));
                                  
                                }
                            }

                     
                        break;
                    case "PriceOverride":

                        if (CurrentTicket.Status == "Closed") MessageBox.Show("Ticket is Closed!");
                        else
                        {
                            if (m_security.WindowAccess("PriceOverride") == false)
                            {
                                MessageBox.Show("Access Denied");
                                return; //jump out of routine
                            }
                            NumPad np = new NumPad("Enter NEW Price:",false);
                            Utility.OpenModal(m_parent, np);
                            if (np.Amount != "")
                            {
                                if (decimal.Parse(np.Amount) >= 0)
                                {
                                    CurrentTicket.OverrideLineItemPrice(id, decimal.Parse(np.Amount));
                                    //if (portion[1] == "Service") CurrentTicket.OverrideServicePrice(id, decimal.Parse(np.Amount));
                                }
                            }

                        }
                        break;


                    case "Surcharge":

                        if (CurrentTicket.Status == "Closed") MessageBox.Show("Ticket is Closed!");
                        else
                        {
                            if (m_security.WindowAccess("Surcharge") == false)
                            {
                                MessageBox.Show("Access Denied");
                                return; //jump out of routine
                            }
                            NumPad np = new NumPad("Enter Surcharge Amount:",false);
                            Utility.OpenModal(m_parent, np);
                            if (np.Amount != "")
                            {
                                if (decimal.Parse(np.Amount) >= 0)
                                {
                                    CurrentTicket.LineItemSurcharge(id, decimal.Parse(np.Amount));
                                    //if (portion[1] == "Service") CurrentTicket.OverrideServicePrice(id, decimal.Parse(np.Amount));
                                }
                            }

                        }
                        break;



                    case "Discount":

                        if (CurrentTicket.Status == "Closed") MessageBox.Show("Ticket is Closed!");
                        else
                        {
                            if (m_security.WindowAccess("Discount") == false)
                            {
                                MessageBox.Show("Access Denied");
                                return; //jump out of routine
                            }
                            NumPad np = new NumPad("Enter Discount Amount:",false);
                            Utility.OpenModal(m_parent, np);
                            if (np.Amount != "")
                            {
                                if (decimal.Parse(np.Amount) >= 0)
                                {
                                    CurrentTicket.DiscountLineItem(id, decimal.Parse(np.Amount));
                                    // if (portion[1] == "Service") CurrentTicket.DiscountService(id, decimal.Parse(np.Amount));
                                }
                            }
                            np = null;
                        }
                        break;


                    case "Notes":


                        TextPad tp = new TextPad("Item Notes:", line.Note);
                        Utility.OpenModal(m_parent, tp);
                        CurrentTicket.UpdateLineItemNote(id, tp.ReturnText);
                        tp = null;



                        break;

                    case "InternalNotes":


                        TextPad tp2 = new TextPad("Iternal Notes:", line.InternalNote);
                        Utility.OpenModal(m_parent, tp2);
                        CurrentTicket.UpdateLineItemInternalNote(id, tp2.ReturnText);
                        tp = null;



                        break;

                }


            }
            catch (Exception e)
            {

                MessageBox.Show("Execute Line Item Clicked: " + e.Message);
            }
        }


        public void ExecutePaymentDeleteClicked(object paymentid)
        {
            Payment pay = CurrentTicket.GetPaymentLine((int)paymentid);
            if (pay.Voided) return;




            if (!m_security.ManagerOverrideAccess("VoidPayment", "Manager Override Needed")) return;

            Selection sel = new Selection("Edit", "Void");
            sel.ShowDialog();
            if(sel.Action == "Edit")
            {
                CustomDate cd = new CustomDate(Visibility.Hidden,pay.PaymentDate);
              
                cd.ShowDialog();

                CurrentTicket.UpdatePaymentDate(pay.ID, cd.StartDate);
                CurrentTicket.Reload();

            }else
            {
                Confirm dlg;

                try
                {
                    if (CurrentTicket.Status == "Closed")
                    {
                        MessageBox.Show("Ticket is closed.  Can not modify");
                        return;
                    }



                    if (pay.Description == "Gift Card")
                    {
                        dlg = new Confirm("Deleting Gift Card from payment will refund charged amount back to gift card. Proceed?");
                    }
                    else
                    {
                        dlg = new Confirm("Delete " + pay.Description + " Payment?");
                    }

                    Utility.OpenModal(m_parent, dlg);
                    if (dlg.Action == "Yes")
                    {
                        AuditModel.WriteLog(m_security.CurrentEmployee.FullName, "Delete Payment", "amount:" + pay.Amount.ToString() + " netamount: " + pay.NetAmount.ToString(), "payment", CurrentTicket.SalesID);
                        CurrentTicket.VoidPayment((int)paymentid);


                        SetVisibility();

                    }



                }
                catch (Exception e)
                {

                    MessageBox.Show("Error deleting line item: " + e.Message);
                }
            }


        }

        public void ExecuteVoidClicked(object salesid)
        {
            if (CurrentTicket == null) return;
            if (CurrentTicket.HasPayment)
            {
                MessageBox.Show("Ticket has payment!! Must void payment first.");
                return;
            }





            if (CurrentTicket.Status == "Open")
            {
                if (!m_security.ManagerOverrideAccess("VoidOpen","Manager Override Needed"))
                {
                    return;
                }
            }else
            {
                if (!m_security.ManagerOverrideAccess("VoidClosed","Manager Override Needed"))
                {
                    return;
                }
            }
  


        

       

            ConfirmAudit win;


            try
            {
                win = new ConfirmAudit();
                Utility.OpenModal(m_parent, win);
                if (win.Reason != "")
                {



                    //inside quicksales screen
                    AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Void Ticket", win.Reason, "sales", CurrentTicket.SalesID);

                        CurrentTicket.VoidTicket(win.Reason);
                        CurrentTicket = null;


                        SetVisibility();
                        LoadTickets();

                  

                }


            }
            catch (Exception e)
            {
                MessageBox.Show("Error deleting line item: " + e.Message);
            }
        }
        public void ExecuteCashTenderClicked(object button)
        {
            try
            {
                m_salesModel.ProcessCashTender(m_parent, m_currentticket);
                if (m_currentticket.CheckforBalance()) LoadTickets();  //test for close - display change given and such
                if (m_currentticket.Status == "Closed" && m_currentticket.HasShipping) ExecuteShipOrderClicked(null);


            }
            catch (Exception e)
            {
                MessageBox.Show(" ExecuteCashTenderClicked:" + e.Message);
            }
        }


        public void ExecuteCreditCardClicked(object button)
        {
            try
            {
                CreditCardView ccv = new CreditCardView(m_parent, m_currentticket,574);
                Utility.OpenModal(m_parent, ccv);
                m_currentticket.LoadPayment();
                m_currentticket.CloseTicket(); //need to load payment  to refresh object first before trying to close ticket
                NotifyPropertyChanged("Payments");

                //test for close - display change given and such
                if (m_currentticket.CheckforBalance()) LoadTickets();

                if (m_currentticket.Status == "Closed" && m_currentticket.HasShipping) ExecuteShipOrderClicked(null);
            }
            catch (Exception e)
            {

                MessageBox.Show(" ExecuteCashTenderClicked:" + e.Message);
            }
        }

        public void ExecuteVerifyClicked(object button)
        {

            GCVerify gcv = new GCVerify();
            Utility.OpenModal(m_parent, gcv);
        }


        public void ExecuteNewTicketClicked(object button)
        {

            if (m_security.CurrentEmployee == null) return;


            CurrentTicket = new Ticket(m_security.CurrentEmployee);
         
            //creates a sales record
            CurrentTicket.CreateTicket(0);
            AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "new ticket", "", "sales", CurrentTicket.SalesID);
            LoadTicket(m_currentticket.SalesID); //load ticket will call SetVisibility

            //Ask cashier to enter customer phone number
            if (GlobalSettings.Instance.AskCustomerPhone)
            {

                ExecuteCustomerClicked(null);
            }

            if (GlobalSettings.Instance.AskTaxExempt)
            {
                if (m_security.WindowAccess("Taxexempt") == false)
                {
                    return; //jump out of routine
                }

                Confirm np = new Confirm("Tax Exempt?");
                Utility.OpenModal(m_parent, np);
                if (np.Action == "Yes") CurrentTicket.SetTaxExempt(true);
                else CurrentTicket.SetTaxExempt(false);

            }

        }

        public void ExecuteCreateRefundClicked(object obj)
        {

            if (m_security.WindowNewAccess("Refund"))
            {
                if (CurrentTicket == null) return;

                int parentid = CurrentTicket.SalesID; //save id of current ticket;

                CurrentTicket = new Ticket(m_security.CurrentEmployee);

                //creates a sales record
                CurrentTicket.CreateTicket(parentid);

                AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "new refund", "", "sales", CurrentTicket.SalesID);

                RetailSales qs = new RetailSales(m_security, m_currentticket.SalesID);
                Utility.OpenModal(m_parent, qs);
            }

           
        }

        public void ExecutePrintReceiptClicked(object button)
        {
            CurrentTicket.PrintReceipt();
        }

        public void ExecutePrintLargeReceiptClicked(object button)
        {
            CurrentTicket.PrintLargeReceipt();
        }


        public void ExecutePrintPDFClicked(object button)
        {
           // CurrentTicket.PrintPDFReceipt();

            PrintModel.PrintReceiptPDF(CurrentTicket,true);


            
        }


        public void ExecuteEmailPDFClicked(object button)
        {
            PrintModel.PrintReceiptPDF(CurrentTicket,false);

            if(CurrentTicket.WorkOrder != null)
            CurrentTicket.WorkOrder.PrintPDF(false);

            CurrentTicket.EmailPDF();
        }

        public void ExecuteWorkOrderClicked(object obj)
        {
          



            //check security for editing also
            Security sec = new Security();
            if (sec.WindowNewAccess("WorkOrder"))
            {

                //if work order is blank , then create
                if (CurrentTicket.WorkOrder == null)
                {
                    Confirm conf = new Confirm("Create New Work Order??");
                    Utility.OpenModal(m_parent, conf);
                    if (conf.Action == "Yes")
                    {
                        CurrentTicket.CreateWorkOrder(sec);
                        AuditModel.WriteLog(sec.CurrentEmployee.FullName, "Work Order", "Create", "Work Order", CurrentTicket.SalesID);
                    }
                    else return;
                }



                //edit work order
                bool CanEdit = false;


                // original code say only owner of ticket can edit
                //  if (CurrentTicket.WorkOrder.custom17 == "" || sec.CurrentEmployee.FullName == CurrentTicket.WorkOrder.custom17 || sec.CurrentEmployee.SecurityLevel >= 100) CanEdit = true;


                //new code as of 2/9/2020 , requested by Lou and Adam , anyone can edit but log their name
                CanEdit = true;
                AuditModel.WriteLog(sec.CurrentEmployee.FullName, "Work Order", "Open/Edit", "Work Order", CurrentTicket.SalesID);

                    WorkOrderEdit wk = new WorkOrderEdit(CurrentTicket,CanEdit);
                    Utility.OpenModal(m_parent, wk);
              
              
            }

              
  
            


        }


        public void ExecuteShipOrderClicked(object obj)
        {




            //check security for editing also
           
            if (m_security.WindowAccess("ShippingOrder"))
            {

     
                AuditModel.WriteLog(m_security.CurrentEmployee.FullName, "Ship Order", "Open/Edit", "Ship Order", CurrentTicket.SalesID);

                ShipOrderEdit wk = new ShipOrderEdit(CurrentTicket);
                Utility.OpenModal(m_parent, wk);


            }






        }


        public void ExecuteCustomerClicked(object button)
        {

            if (m_security.WindowAccess("CustomerView") == false)
            {
                MessageBox.Show("Access Denied");
                return; //jump out of routine
            }



            //if ticket already has customer number, then bring up edit screen
            if (CurrentTicket.CurrentCustomer != null)
            {
                CurrentTicket.CurrentCustomer=new Customer(m_customermodel.EditViewCustomer(CurrentTicket.CurrentCustomer.ID, m_security),true);

            }else
            {


                //if no customers then code continues below
                int custid = m_customermodel.GetCreateCustomer();
                if(custid > 0) CurrentTicket.CurrentCustomer = new Customer(custid,true);

                if (GlobalSettings.Instance.DisplayRewardAlert)
                {
                    if (CurrentTicket.CurrentCustomer == null) return;
                    //check to see if customer has usable rewards
                    if (CurrentTicket.CurrentCustomer.UsableBalance > 0)
                    {
                        string message;
                        message = "Customer has Reward: " + m_currentticket.CurrentCustomer.UsableBalance.ToString("c");
                        Utility.Alert(message);
                    }

                }

            }






        }



        /*
        public void ExecuteCustomerClicked(object button)
        {

            //if ticket already has customer number, then bring up edit screen
            if (CurrentTicket.CurrentCustomer != null)
            {
                if (_security.WindowAccess("CustomerView") == false)
                {
                    MessageBox.Show("Access Denied");
                    return; //jump out of routine
                }

                if (CurrentTicket.CurrentCustomer.ID > 0)
                {
                    //Ask if view or delete
                    CustomerMenu cm = new CustomerMenu();
                    Utility.OpenModal(_parent, cm);

                    if (cm.Action == "View")
                    {
                        CustomerView custvw = new CustomerView(_security, CurrentTicket.CurrentCustomer.ID);
                        Utility.OpenModal(_parent, custvw);
                        CurrentTicket.CurrentCustomer = new Customer(CurrentTicket.CurrentCustomer.ID);  //loades new
                    }

                    if (cm.Action == "Delete")
                    {

                        CurrentTicket.UpdateCustomerID(0); //VFD display is shown here
                        CurrentTicket.CurrentCustomer = null;

                    }

                    return; //exits code here

                }

            }


            //if no customers then code continues below

            CustomerModel cust = new CustomerModel();
            int customerid = 0;
            string customerphonenumber = "";



            //ask for customer phone number
            CustomerPhone pad = new CustomerPhone();
            pad.Amount = "";
            Utility.OpenModal(_parent, pad);
            //if user cancel , returns ""
            if (pad.Amount != "")
            {
                customerphonenumber = pad.Amount;
                DataTable dt = cust.LookupByPhone(customerphonenumber);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count == 1)
                    {
                        customerid = int.Parse(dt.Rows[0]["id"].ToString());

                    }
                    else
                    {
                        //Display list of names to pick from
                        CustomerFoundList cfl = new CustomerFoundList(dt);
                        Utility.OpenModal(_parent, cfl);
                        customerid = cfl.CustomerID;
                    }
                }


            }
            else return;




            if (customerid > 0)
            {
                CurrentTicket.UpdateCustomerID(customerid);
                if (GlobalSettings.Instance.DisplayRewardAlert)
                {
                    //check to see if customer has usable rewards
                    if (_currentticket.CurrentCustomer.UsableBalance > 0)
                    {
                        string message;
                        message = "Customer has Reward: " + _currentticket.CurrentCustomer.UsableBalance.ToString("c");
                        Utility.Alert(message);
                    }

                }
            }
            else
            {
                MessageBox.Show("Customer Not Found");

                if (customerphonenumber.Length == 10)
                {
                    DataTable dt = cust.LookupByPhone(customerphonenumber);
                    if (dt.Rows.Count == 0)
                    {
                        //Create new customer

                        customerid = cust.CreateNew(pad.Amount);
                        MessageBox.Show("New Customer Created");
                        //add customer id to ticket .. it will update database
                        CurrentTicket.UpdateCustomerID(customerid);

                    }
                    else
                    {
                        MessageBox.Show("Can not create customer, phone number already exist.");
                    }


                }
                else
                {

                    MessageBox.Show("Please Enter 10 digit number to create customer account");

                    pad = new CustomerPhone();
                    pad.Amount = "";
                    pad.FullNumberRequired = true;
                    VFD.WriteRaw("Please Enter Phone", "");
                    Utility.OpenModal(_parent, pad);
                    if (pad.Amount != "")
                    {

                        DataTable dt = cust.LookupByPhone(pad.Amount);
                        if (dt.Rows.Count == 0)
                        {
                            //Create new customer

                            customerid = cust.CreateNew(pad.Amount);
                            MessageBox.Show("New Customer Created");
                            //add customer id to ticket .. it will update database
                            CurrentTicket.UpdateCustomerID(customerid);

                        }
                        else
                        {
                            MessageBox.Show("Can not create customer, phone number already exist.");
                        }
                    }


                }


            }







        }
        */

        public void ExecuteEmployeeClicked(object button)
        {


            if (m_security.WindowAccess("EmployeeList") == false)
            {
                MessageBox.Show("Access Denied");
                return; //jump out of routine
            }



            EmployeeList empl = new EmployeeList(m_parent,m_security);
            Utility.OpenModal(m_parent, empl);
            if (empl.EmployeeID > 0)
            {
                m_currentticket.ChangeServer(empl.EmployeeID);

            }
        }

        public void ExecuteNoSaleClicked(object button)
        {
            //Open cash drawer
            if (m_security.WindowAccess("NoSale") == true)
            {
                m_printer.CashDrawer();
            }
           

        }


        public void ExecuteDiscountClicked(object button)
        {

            NumPad pad = new NumPad("Enter Discount Amount:",false);
            pad.Amount = "";
            Utility.OpenModal(m_parent, pad);
            if (pad.Amount != "")
            {
                m_currentticket.AddDiscount(decimal.Parse(pad.Amount));
                SetVisibility();
            }


        }


        public void ExecuteGiftCardClicked(object button)
        {
            try
            {
                GiftCardView ccv = new GiftCardView(m_currentticket);
                Utility.OpenModal(m_parent, ccv);
                m_currentticket.LoadPayment();
                m_currentticket.CloseTicket(); //need to load payment  to refresh object first before trying to close ticket
                NotifyPropertyChanged("Payments");

                //test for close - display change given and such
                if (m_currentticket.CheckforBalance()) LoadTickets();
                if (m_currentticket.Status == "Closed" && m_currentticket.HasShipping) ExecuteShipOrderClicked(null);
            }
            catch (Exception e)
            {

                MessageBox.Show(" ExecuteGiftCardClicked:" + e.Message);
            }


        }

  


        public void ExecuteCheckClicked(object button)
        {
            try
            {
                m_salesModel.ProcessCheck(m_parent, m_currentticket);
                //test for close - display change given and such
                if (m_currentticket.CheckforBalance()) LoadTickets();
                if (m_currentticket.Status == "Closed" && m_currentticket.HasShipping) ExecuteShipOrderClicked(null);
            }
            catch (Exception e)
            {
                MessageBox.Show(" ExecuteCheckClicked:" + e.Message);
            }
        }

        public void ExecuteStoreCreditClicked(object button)
        {
            try
            {
                m_salesModel.ProcessStoreCredit(m_parent, m_currentticket);
                //test for close - display change given and such
                if (m_currentticket.CheckforBalance()) LoadTickets();
                if (m_currentticket.Status == "Closed" && m_currentticket.HasShipping) ExecuteShipOrderClicked(null);
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
                m_salesModel.ProcessGiftCertificate(m_parent, m_currentticket);
                //test for close - display change given and such
                if (m_currentticket.CheckforBalance()) LoadTickets();
                if (m_currentticket.Status == "Closed" && m_currentticket.HasShipping) ExecuteShipOrderClicked(null);
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
                RewardView ccv = new RewardView(m_currentticket);
                Utility.OpenModal(m_parent, ccv);
                //test for close - display change given and such
                if (m_currentticket.CheckforBalance()) LoadTickets();

                if (m_currentticket.Status == "Closed" && m_currentticket.HasShipping) ExecuteShipOrderClicked(null);
            }
            catch (Exception e)
            {
                MessageBox.Show(" ExecuteRewardClicked:" + e.Message);
            }
        }

   

        public void ExecuteTicketClicked(object salesid)
        {


            //these ticket status must be OPEN , closed ticket can not be operated on
            LoadTicket((int)salesid);


           // LoadTickets();

            SelectedIndex = 0;

        }




        public void ExecuteHoldClicked(object button)
        {
            // Tickets.Add(CurrentTicket);
            CurrentTicket = null;
            VFD.WriteDisplay("Ticket : ", "none", "Total", 0);
            //need to refresh ticket list
            LoadTickets();




        }

        public void ExecuteExitClicked(object button)
        {


        }



        public void ExecuteMoreClicked(object button)
        {

            More dlg = new More(this);
            Utility.OpenModal(m_parent, dlg);


        }


        public void ExecuteTaxExemptClicked(object button)
        {

            if (CurrentTicket.Status == "Closed") MessageBox.Show("Ticket is Closed!");
            else
            {
                if (m_security.WindowAccess("Taxexempt") == false)
                {
                    MessageBox.Show("Access Denied");
                    return; //jump out of routine
                }

                Confirm np = new Confirm("Tax Exempt?");
                Utility.OpenModal(m_parent, np);
                if (np.Action == "Yes") CurrentTicket.SetTaxExempt(true);
                else CurrentTicket.SetTaxExempt(false);
            }

        }



        //----------------------  Ticket Search Functions
        public void ExecuteSearchByCustomerClicked(object searchtag)
        {

            try
            {
                int customerid = CustomerModel.LookupCustomer();
                var watch = Stopwatch.StartNew();
                Tickets = m_salesModel.LoadOpenTicketsByCustomer(customerid, HasPayment);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                if (elapsedMs > 1000) SearchMessage2 = Tickets.Rows.Count.ToString() + " Items found in " + elapsedMs / 1000.0 + " seconds";
                else SearchMessage2 = Tickets.Rows.Count.ToString() + " Items found in " + elapsedMs + " milli-seconds";
            }
            catch (Exception e)
            {
                MessageBox.Show("Search by Customer:" + e.Message);

            }
        }

        public void ExecuteSearchByTicketClicked(object searchtag)
        {

            try
            {
                NumPad np = new NumPad("Enter Ticket #:",true);
                Utility.OpenModal(m_parent, np);
                if(np.Amount != "")
                {
                    Tickets = m_salesModel.LoadOpenTicketsByTicket(int.Parse(np.Amount));

                    if (Tickets.Rows.Count > 0) ExecuteTicketClicked(int.Parse(np.Amount));
                    else SearchMessage2 = "Ticket " + np.Amount + "Not Found";

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Search by ticket:" + e.Message);

            }
        }
        public void ExecuteSearchByDateClicked(object searchtag)
        {

            try
            {
                CustomDate cd = new CustomDate(Visibility.Visible, DateTime.Now);
               

                Utility.OpenModal(m_parent, cd);

                var watch = Stopwatch.StartNew();
                Tickets = m_salesModel.LoadOpenTicketsByDate(cd.StartDate, cd.EndDate, HasPayment);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                if (elapsedMs > 1000) SearchMessage2 = Tickets.Rows.Count.ToString() + " Items found in " + elapsedMs / 1000.0 + " seconds";
                else SearchMessage2 = Tickets.Rows.Count.ToString() + " Items found in " + elapsedMs + " milli-seconds";
            }
            catch (Exception e)
            {
                MessageBox.Show("Search by Date:" + e.Message);

            }
        }
        public void ExecuteSearchByEmployeeClicked(object searchtag)
        {

            try
            {
                var watch = Stopwatch.StartNew();
                Tickets = m_salesModel.LoadOpenTicketsByEmployee(m_security.CurrentEmployee.ID, HasPayment);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                if(elapsedMs > 1000) SearchMessage2 = Tickets.Rows.Count.ToString() + " Items found in " + elapsedMs/1000.0 + " seconds";
                else SearchMessage2 = Tickets.Rows.Count.ToString() + " Items found in " + elapsedMs  + " milli-seconds";
            }
            catch (Exception e)
            {
                MessageBox.Show("Search by Employee:" + e.Message);

            }
        }
        public void ExecuteBrowseAllClicked(object searchtag)
        {

            try
            {
                var watch = Stopwatch.StartNew();
                Tickets = m_salesModel.LoadAllOpenTickets(HasPayment);
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                if (elapsedMs > 1000) SearchMessage2 = Tickets.Rows.Count.ToString() + " Items found in " + elapsedMs / 1000.0 + " seconds";
                else SearchMessage2 = Tickets.Rows.Count.ToString() + " Items found in " + elapsedMs + " milli-seconds";

            }
            catch (Exception e)
            {
                MessageBox.Show("Browse ALL:" + e.Message);

            }
        }




        public void ExecuteInternalNoteClicked(object obj)
        {
            TextPad tp = new TextPad("Internal Notes:", CurrentTicket.InternalNote);
            Utility.OpenModal(m_parent, tp);
            CurrentTicket.UpdateInternalNote( tp.ReturnText);
         

        }

        public void ExecuteNoteClicked(object obj)
        {
            TextPad tp = new TextPad("Notes:", CurrentTicket.Note);
            Utility.OpenModal(m_parent, tp);
            CurrentTicket.UpdateNote(tp.ReturnText);


        }























    } //---------- End of SalesViewModel Class



}
