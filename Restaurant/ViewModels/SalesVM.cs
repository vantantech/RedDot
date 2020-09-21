using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace RedDot
{
    public class SalesVM : SalesBaseVM
    {

  

        private ObservableCollection<Product> _quickproducts;
        private ObservableCollection<Product> _simpleproducts;

        private DataTable m_products;
        private DataTable m_services;
        private DataTable m_favorites;
        private DataTable m_searchresults;
        private DataTable m_categories1;


        private Visibility m_visible_quicksale;
        private Visibility m_visible_newticketbutton;
        private Visibility m_visible_newseatbutton;
        private Visibility m_paidinfull;
        private Visibility m_salestaxvisibility;
        private Visibility m_gratuityvisibility;
        private Visibility m_creditcardsurchargevisibility;

      
        public Visibility VisibleBarTabCustomer { get; set; }
        public Visibility VisibleCustomer { get; set; }


        private bool m_isbalance;
        private bool m_ischangedue;
        private bool m_loadedtickets = false;
        private OrderType m_ordertype;
        private SubOrderType m_subordertype;
  

  
        private SalesModel m_salesmodel;
        private PromotionsModel m_promomodel;
        private PaymentModel m_paymentmodel;
        private MenuSetupModel m_inventorymodel;
     

        private TableModel m_tablemodel;
        private int m_selectedindex;
        private decimal m_quantity = 1;

        private int m_tablenumber = 0;
        private int m_customercount = 0;
        private int m_currentlineitemid = 0;

        public Visibility VisiblePrice { get; set; } 

  

        public ICommand CategoryProductClicked { get; set; }
      
        public ICommand ProductClicked { get; set; }
        //  public ICommand ServiceClicked { get; set; }

        public ICommand LineItemClicked { get; set; }
        public ICommand LineItemActionClicked { get; set; }

        public ICommand CashTenderClicked { get; set; }
        public ICommand CCPClicked { get; set; }
        public ICommand PreAuthClicked { get; set; }
        public ICommand CashClicked { get; set; }
        public ICommand GiftCardClicked { get; set; }
        public ICommand StampCardClicked { get; set; }
        public ICommand CheckClicked { get; set; }
        public ICommand HouseCreditClicked { get; set; }
        public ICommand GiftCertificateClicked { get; set; }
        public ICommand RewardClicked { get; set; }
        public ICommand ExternalClicked { get; set; }


        public ICommand PrintReceiptClicked { get; set; }
        public ICommand SendOrderClicked { get; set; }

        public ICommand ClearOrderClicked { get; set; }

        public ICommand EmployeeClicked { get; set; }
        public ICommand CustomerClicked { get; set; }
        public ICommand BarTabCustomerClicked { get; set; }

        public ICommand TableClicked { get; set; }

        //public ICommand SettingsClicked { get; set; }
        public ICommand DiscountClicked { get; set; }
 
        public ICommand EditClicked { get; set; }

     

        public ICommand SeatClicked { get; set; }
        public ICommand AddChairClicked { get; set; }

    

        public ICommand TableLayoutClicked { get; set; }

     

        public ICommand TaxExemptClicked { get; set; }

        public ICommand BackClicked { get; set; }
      
        public ICommand QuantityClicked { get; set; }
       public ICommand PaymentDeleteClicked { get; set; }
        public ICommand PreAuthDeleteClicked { get; set; }
        public ICommand IDCheckClicked { get; set; }

        public ICommand OrderTypeClicked { get; set; }

        public ICommand HoldClicked { get; set; }

        public ICommand ChangeTableClicked { get; set; }


        private string m_customername="";
 

        private int m_currentseat;


        public int ReceiptHeight { get; set; }



       

        public SalesVM(Window parent, SecurityModel security, Ticket passedinticket, int tablenumber, int customercount, OrderType ordertype, SubOrderType subordertype):base(parent,security)
        {
       
     
            CurrentEmployee = security.CurrentEmployee;
            SelectedIndex = 0;
            m_currentseat = 1;
            TableNumber = tablenumber;
            CustomerCount = customercount;
            m_ordertype = ordertype;
            m_subordertype = subordertype;
      
            LoadSettings();
            Initialize();

            //set default ticket if passed ID , these tickets must be CLOSED
            if ( passedinticket != null)
            {
                m_mode = "edit";

                LoadTicket(passedinticket);  //open ticket passed in
                VisibleNewticketbutton = Visibility.Collapsed;  // the new button since we don't want it to be visible
                VisibleNewseatbutton = Visibility.Collapsed;  // the new button since we don't want it to be visible
            }
            else
            {
                m_mode = "new";

                if (tablenumber ==0)
                {
                    VisibleNewticketbutton = Visibility.Visible;
                    VisibleNewseatbutton = Visibility.Collapsed;
                }else
                {
                    VisibleNewticketbutton = Visibility.Collapsed;
                    VisibleNewseatbutton = Visibility.Visible;
                }
                
           
            }

         

            SetOrderType(ordertype);

            if (passedinticket == null)
            {
                CreateNewTicket();
                AskCustomerPhone(ordertype);
            }

        }

        private void SetOrderType(OrderType ordertype)
        {

            VisibleChair = Visibility.Collapsed;
            VisibleCustomer = Visibility.Visible;
            VisibleBarTabCustomer = Visibility.Collapsed;
            ReceiptHeight = 665;
            m_ordertype = ordertype;
            QuickProducts = null;  //resets if items are already displayed, foreced user to click on category again



            switch (ordertype)
            {
                case OrderType.DineIn:
                    VisibleChair = Visibility.Visible;
                    ReceiptHeight = 565;
                     break;
                case OrderType.Bar:
                    VisibleCustomer = Visibility.Collapsed;
                    VisibleBarTabCustomer = Visibility.Visible;
                 
                    break;

                case OrderType.ToGo:
                
     

                    break;
                case OrderType.Delivery:
                 
                    break;


               

            }
        }

        private void AskCustomerPhone(OrderType ordertype)
        {
            switch (ordertype)
            {
                case OrderType.DineIn:
                    if (GlobalSettings.Instance.DineInAskForCustomerName) ExecuteBarTabCustomerClicked(null);
                    if ( GlobalSettings.Instance.DineInAskCustomerPhone) ExecuteCustomerClicked(null);
                    break;
                case OrderType.Bar:
         
                    if (GlobalSettings.Instance.BarAskForCustomerName) ExecuteBarTabCustomerClicked(null);
                    if (GlobalSettings.Instance.BarAskCustomerPhone) ExecuteCustomerClicked(null);
                    break;

                case OrderType.ToGo:


               
                    if (CurrentTicket.TicketSubOrderType == SubOrderType.WalkIn)
                    {
                       
                        if (GlobalSettings.Instance.WalkInAskForCustomerName) ExecuteBarTabCustomerClicked(null);
                        if (GlobalSettings.Instance.WalkInAskCustomerPhone) ExecuteCustomerClicked(null);
                    }
                    else
                    {
                       if(CurrentTicket.TicketSubOrderType  == SubOrderType.CallIn)
                        {
                            if (GlobalSettings.Instance.CallInAskForCustomerName) ExecuteBarTabCustomerClicked(null);

                            if (GlobalSettings.Instance.CallInCustomerInfoRequired)
                            {
                                //info is required .. so must enter info before orders can be taken
                                ExecuteCustomerClicked(null);
                                if (CurrentTicket.CurrentCustomer == null)
                                {
                                    TouchMessageBox.Show("Customer Info is required on ticket for Call-In Orders.");
                                    // m_parent.Close();
                                }
                            }
                            else
                                if (GlobalSettings.Instance.CallInAskCustomerPhone) ExecuteCustomerClicked(null);
                        }
                     
                    }


                    break;

                case OrderType.Delivery:
                    if (GlobalSettings.Instance.DeliveryAskForCustomerName) ExecuteBarTabCustomerClicked(null);
                    if ( GlobalSettings.Instance.DeliveryAskCustomerPhone) ExecuteCustomerClicked(null);

                    break;

            }
        }
        public void LoadSettings()
        {
            CategoryWidth = GlobalSettings.Instance.CategoryWidth ; 
            CategoryHeight = GlobalSettings.Instance.CategoryHeight;
            CategoryFontSize = GlobalSettings.Instance.CategoryFontSize;

            ProductWidth = GlobalSettings.Instance.ProductWidth; 
            ProductHeight = GlobalSettings.Instance.ProductHeight ; 
            ProductFontSize = GlobalSettings.Instance.ProductFontSize;

            if (GlobalSettings.Instance.SalesTaxPercent > 0) m_salestaxvisibility = Visibility.Visible; else m_salestaxvisibility = Visibility.Collapsed;
    
       
        }



        public void Initialize()
        {

            //Assign command relays
            CategoryProductClicked = new RelayCommand(ExecuteCategoryProductClicked, param => this.CanCategoryClicked);
         
       
            ProductClicked = new RelayCommand(ExecuteProductClicked, param => this.CanExecute);



            LineItemClicked = new RelayCommand(ExecuteLineItemClicked, param => this.CanExecute);
            LineItemActionClicked = new RelayCommand(ExecuteLineItemActionClicked, param => this.CanLineItem);

            CashTenderClicked = new RelayCommand(ExecuteCashTenderClicked, param => this.CanExecuteCashTenderClicked);
            CCPClicked = new RelayCommand(ExecuteCCPClicked, param => this.CanExecuteCreditDebitClicked);
            PreAuthClicked = new RelayCommand(ExecutePreAuthClicked, param => this.CanExecutePreAuthClicked);
            CashClicked = new RelayCommand(ExecuteCashClicked, param => this.CanExecuteCashTenderClicked);
            GiftCardClicked = new RelayCommand(ExecuteGiftCardClicked, param => this.CanExecutePaymentClicked);
            GiftCertificateClicked = new RelayCommand(ExecuteGiftCertificateClicked, param => this.CanExecutePaymentClicked);
            CheckClicked = new RelayCommand(ExecuteCheckClicked, param => this.CanExecutePaymentClicked);
            RewardClicked = new RelayCommand(ExecuteRewardClicked, param => this.CanExecuteReward);
            ExternalClicked = new RelayCommand(ExecuteCreditCardClicked, param => this.CanExecuteCreditDebitClicked);



            DiscountClicked = new RelayCommand(ExecuteAdjustTicketClicked, param => this.CanExecuteDiscountClicked);

   
            PrintReceiptClicked = new RelayCommand(ExecutePrintReceiptClicked, param => CanPrintReceiptClickedExecute);
            SendOrderClicked = new RelayCommand(ExecuteSendOrderClicked, param => this.CanSendOrder);


            ClearOrderClicked = new RelayCommand(ExecuteClearOrderClicked, param => this.CanClearTicket);

            CustomerClicked = new RelayCommand(ExecuteCustomerClicked, param => this.CanExecute);
            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => this.CanExecuteOpenTicket);
            ChangeTableClicked = new RelayCommand(ExecuteChangeTableClicked, param => this.CanExecuteOpenTicket);

            BarTabCustomerClicked = new RelayCommand(ExecuteBarTabCustomerClicked, param => true);


            //SettingsClicked = new RelayCommand(ExecuteSettingsClicked, param => this.CanExecute);
            EditClicked = new RelayCommand(ExecuteEditClicked, param => this.CanExecuteOpenTicket);
      

            TaxExemptClicked = new RelayCommand(ExecuteTaxExemptClicked, param => this.CanExecuteOpenTicket);
            BackClicked = new RelayCommand(ExecuteBackClicked, param => this.CanCancel);

            QuantityClicked = new RelayCommand(ExecuteQuantityClicked, param => this.CanExecute);
        

            SeatClicked = new RelayCommand(ExecuteSeatClicked, param => this.CanExecute);
            AddChairClicked = new RelayCommand(ExecuteAddChairClicked, param => this.CanExecute);

            TableLayoutClicked = new RelayCommand(ExecuteTableLayoutClicked, param => this.CanExecute);


            PaymentDeleteClicked = new RelayCommand(ExecutePaymentDeleteClicked, param => this.CanExecutePaymentDeleteClicked);
            PreAuthDeleteClicked = new RelayCommand(ExecutePreAuthDeleteClicked, param => this.CanExecutePaymentDeleteClicked);

            IDCheckClicked = new RelayCommand(ExecuteIDCheckClicked, param => this.CanExecute);

            OrderTypeClicked = new RelayCommand(ExecuteOrderTypeClicked, param => this.CanExecuteOpenTicket);

            HoldClicked = new RelayCommand(ExecuteHoldClicked, param => this.CanExecuteHoldTicket);


            m_salesmodel = new SalesModel(m_security);
            m_promomodel = new PromotionsModel();
            m_inventorymodel = new MenuSetupModel();
            m_paymentmodel = new PaymentModel(m_security);



            m_tablemodel = new TableModel();

            //set startup defaults
            VisibleQuickSale = Visibility.Hidden;
 
         
            PaidInFull = Visibility.Hidden;

            QTYStr = "QTY";
            IsChangeDue = false;
            IsBalance = true;
            //CatLoaded = false;

            SelectedColor = "transparent";

            //fill the categories but only once
            // if (CatLoaded == false)
            LoadSeats(false);


            LoadCategories();

            VisiblePrice =  GlobalSettings.Instance.DisplayPriceOnButton ? Visibility.Visible : Visibility.Collapsed;

        }



        //------------------------------------------------------------------------------------------
        //  ____        _     _ _        __  __      _   _               _     
        // |  _ \ _   _| |__ | (_) ___  |  \/  | ___| |_| |__   ___   __| |___ 
        // | |_) | | | | '_ \| | |/ __| | |\/| |/ _ \ __| '_ \ / _ \ / _` / __|
        // |  __/| |_| | |_) | | | (__  | |  | |  __/ |_| | | | (_) | (_| \__ \
        // |_|    \__,_|_.__/|_|_|\___| |_|  |_|\___|\__|_| |_|\___/ \__,_|___/
        //    
        //------------------------------------------------------------------------------------------  

        public void LoadSeats(bool refresh)
        {
            if (TableNumber > 0)
            {
                List<Seat> seats = new List<Seat>();

                Table tab = m_tablemodel.GetTableByNumber(TableNumber);
                if (tab.Seats == 0) return;


                if (CustomerCount > 0)
                {
                    for (int i = 1; i <= CustomerCount; i++)
                    {
                        seats.Add(new Seat() { SeatNumber = i });
                    }
                }
                else
                {
                    //if customer count is not used , then default to table count
                    for (int i = 1; i <= tab.Seats; i++)
                    {
                        seats.Add(new Seat() { SeatNumber = i });
                    }

                }

                if (refresh)
                {
                    seats[seats.Count - 1].Selected = true;
                    m_currentseat = seats.Count;
                }
                else seats[0].Selected = true;

                Seats = seats;
            }
        }


        public void LoadTicket(int salesid)
        {
            CurrentTicket = m_salesmodel.LoadTicket(salesid);
            GlobalSettings.Instance.CurrentTicket = CurrentTicket;
            GlobalSettings.Instance.RemoteScreen.remotescreenvm.RefreshTicket();

            CustomerName = CurrentTicket.CustomerName;
            if (CurrentTicket != null)
            {
                GlobalSettings.CustomerDisplay.WriteDisplay("Ticket : ", CurrentTicket.SalesID.ToString(), "Total", CurrentTicket.Total);

            }

            if (Categories1 == null) LoadCategories();


            CalculateCashButtons();
            SetVisibility();

        }

        public void LoadTicket(Ticket ticket)
        {
            CurrentTicket = ticket;
            GlobalSettings.Instance.CurrentTicket = CurrentTicket;
            GlobalSettings.Instance.RemoteScreen.remotescreenvm.RefreshTicket();

            CustomerName = CurrentTicket.CustomerName;
            if (CurrentTicket != null)
            {
                GlobalSettings.CustomerDisplay.WriteDisplay("Ticket : ", CurrentTicket.SalesID.ToString(), "Total", CurrentTicket.Total);

            }

            if (Categories1 == null) LoadCategories();


            CalculateCashButtons();

            SetVisibility();

        }

        public void LoadCategories()
        {
            Categories1 = m_inventorymodel.GetCategoryList(GlobalSettings.Instance.ActiveMenu, true);
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


                }
                else //open ticket
                {
                    PaidInFull = Visibility.Hidden;
                   
                }

                if (CurrentTicket.CreditCardSurcharge > 0) CreditCardSurchargeVisibility = Visibility.Visible;
                else CreditCardSurchargeVisibility = Visibility.Collapsed;

            }
            else
            {
             
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

        private Visibility m_visiblechair;
        public Visibility VisibleChair
        {
            get { return m_visiblechair; }
            set
            {
                m_visiblechair = value;
                NotifyPropertyChanged("VisibleChair");
            }
        }



        private string m_selectedcolor;
        public string SelectedColor
        {
            get { return m_selectedcolor; }
            set
            {
                m_selectedcolor = value;
                NotifyPropertyChanged("SelectedColor");
            }
        }

        private string m_qtystr;
        public string QTYStr
        {
            get { return m_qtystr; }
            set
            {
                m_qtystr = value;
                NotifyPropertyChanged("QTYStr");
            }
        }

        public string CustomerName
        {
            get { return m_customername; }
            set
            {
                m_customername = value;
                NotifyPropertyChanged("CustomerName");
            }
        }

        public int CategoryWidth { get; set; }


        public int CategoryHeight { get; set; }


        public int ProductWidth { get; set; }
 

        public int ProductHeight { get; set; }


        public int ProductFontSize { get; set; }

        public int CategoryFontSize { get; set; }














        private bool m_showpriceonbutton;
        public bool ShowPriceOnButton
        {
            get { return m_showpriceonbutton; }
            set
            {
                m_showpriceonbutton = value;
                NotifyPropertyChanged("ShowPriceOnButton");
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
                  
                }
                NotifyPropertyChanged("SelectedIndex");
            }
        }

        List<Seat> _seats;
        public List<Seat> Seats
        {
            get { return _seats; }
            set
            {
                _seats = value;
                NotifyPropertyChanged("Seats");
            }
        }



        public int TableNumber
        {
            get { return m_tablenumber; }
            set { m_tablenumber = value;
            NotifyPropertyChanged("TableNumber");
            }
        }


        public int CustomerCount
        {
            get { return m_customercount; }
            set
            {
                m_customercount = value;
                NotifyPropertyChanged("CustomerCount");
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



 

        public ObservableCollection<Product> QuickProducts
        {

            get
            {

                return this._quickproducts;
            }

            set
            {
                if (value != this._quickproducts)
                {
                    this._quickproducts = value;
                    NotifyPropertyChanged("QuickProducts");
                }
            }
        }

        public ObservableCollection<Product> SimpleProducts
        {

            get
            {

                return this._simpleproducts;
            }

            set
            {
                if (value != this._simpleproducts)
                {
                    this._simpleproducts = value;
                    NotifyPropertyChanged("SimpleProducts");
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

        public Visibility VisibleNewseatbutton
        {
            get
            {
                return m_visible_newseatbutton;
            }
            set
            {
                m_visible_newseatbutton = value;
                NotifyPropertyChanged("VisibleNewseatbutton");
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

        public decimal Quantity
        {
            get { return m_quantity; }
            set
            {
                m_quantity = value;
                NotifyPropertyChanged("Quantity");
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

        public bool CanCategoryClicked
        {
            get
            {
                if (CurrentTicket == null) return false;

                if (CurrentTicket.TicketOrderType == OrderType.ToGo)
                {
                    if (CurrentTicket.TicketSubOrderType == SubOrderType.CallIn && GlobalSettings.Instance.CallInCustomerInfoRequired)
                    {
                        if (CurrentTicket.CurrentCustomer == null) return false; else return true;
                    }
                    else return true;
                }
                else
                    return true;
            }
        }

        public bool CanExecuteNoPayment
        {
            get
            {
                if (CurrentTicket == null) return false;
                return !CurrentTicket.HasPayment;
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


        public bool CanExecuteSplitTicket
        {
            get
            {
                if (m_currentticket == null) return false;
                if (m_currentticket.TableNumber > 0)
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

                if (m_currentticket.Status != "Open" || m_currentticket.Status != "OpenTemp")
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

        public bool CanExecuteDiscountClicked
        {
            get
            {

                if (m_currentticket == null) return false;
                if (CurrentTicket.Status != "Voided" && CurrentTicket.Status != "Closed" && m_currentticket.ActiveItemCount >= 1 )
                {
                    return true;
                }
                else return false;
            }

        }

        public bool CanExecutePaymentClicked
        {
            get
            {

                if (m_currentticket == null) return false;
                if (CurrentTicket.Status != "Voided" && CurrentTicket.Status != "Closed" && m_currentticket.ActiveItemCount >= 1 && m_currentticket.Balance >= 0)
                {
                    return true;
                }
                else return false;
            }

        }


        public bool CanExecutePaymentDeleteClicked
        {
            get
            {

                if (m_currentticket == null) return false;
                if (m_currentticket.Status != "Closed" )
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
              //  if (m_currentticket.HasBeenPaid("Cash")) return false;
                return CanExecutePaymentClicked;
            }

        }


        public bool CanExecuteCreditDebitClicked
        {
            get
            {


                if (m_currentticket == null) return false;

                //balance can be positive or negative but not 0
                if (CurrentTicket.Status != "Voided" && CurrentTicket.Status != "Closed" && m_currentticket.ActiveItemCount >= 1 && m_currentticket.Balance != 0)
                {
                    return true;
                }
                else return false;
            }

        }

        public bool CanExecutePreAuthClicked
        {
            get
            {
                if (GlobalSettings.Instance.OpenTabPreAuthAmount <= 0) return false;

                if (CurrentTicket != null)
                {
                    if (CurrentTicket.TotalPayment == 0 && (CurrentTicket.PreAuth == null || CurrentTicket.PreAuth != null && CanExecutePaymentClicked )) return true;
                    else return false;
                }
                else return true;
            }
        }

        public bool CanExecuteGratuityClicked
        {
            get
            {
                if (CanExecuteNotNullTicket && m_currentticket.Status == "Closed" && m_currentticket.HasCreditPayment) return true;
                else return false;
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


        public bool CanLineItem
        {
            get
            {
                if (m_currentticket == null) return false;

                if (m_currentticket.CurrentLine != null) return true; else return false;
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
                    if (m_currentticket.Seats == null) return false;


                    if (m_currentticket.Seats.Count > 0) return true;
                    else return false;

                }

            }
        }

        public bool CanSendOrder
        {
            get
            {
                if (m_currentticket == null)
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




        //------------------------------------------------------------------------------------------
        //  ____        _   _                 ____                                          _     
        // | __ ) _   _| |_| |_ ___  _ __    / ___|___  _ __ ___  _ __ ___   __ _ _ __   __| |___ 
        // |  _ \| | | | __| __/ _ \| '_ \  | |   / _ \| '_ ` _ \| '_ ` _ \ / _` | '_ \ / _` / __|
        // | |_) | |_| | |_| || (_) | | | | | |__| (_) | | | | | | | | | | | (_| | | | | (_| \__ \
        // |____/ \__,_|\__|\__\___/|_| |_|  \____\___/|_| |_| |_|_| |_| |_|\__,_|_| |_|\__,_|___/
        //
        //------------------------------------------------------------------------------------------ 

    


        public void ExecuteIDCheckClicked(object obj)
        {
            if (CurrentTicket == null) CreateNewTicket();

            GetDriverLicense dl = new GetDriverLicense(CurrentTicket.Licenses);
            Utility.OpenModal(m_parent, dl);

            if (dl.CustomerLicense.LicenseNo != "")
            {
                CurrentTicket.InsertIDCheck(dl.CustomerLicense);
            }

            /*
            if(CurrentTicket.CurrentCustomer == null)
            {
                CustomerModelCore cust = new CustomerModelCore();
               int  customerid = cust.LookupCustomerbyDL(m_parent, m_security, dl.CustomerLicense);
                CurrentTicket.UpdateCustomerID(customerid);
            }

          */


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

                    bool paid = m_salesmodel.ProcessQuickCash(m_parent, m_currentticket,amt);

                    CleanUPOrRefresh(paid);

                }
                else MessageBox.Show("Error getting Quick Cash amount");

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
               bool paid =  m_salesmodel.ProcessCashTender(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {
                MessageBox.Show(" Execute CashTenderClicked:" + e.Message);
            }
        }

        public void ExecuteCCPClicked(object button)
        {
            try
            {

                if(CurrentTicket.Balance < 0)
                {
                   bool approverequired = m_security.ManagerOverrideAccess("Refund", "Negative Balance!!" + (char)13 + (char)10 + "Manager Override Required.");
                    if (!approverequired) return;
                }


              bool paid =   m_salesmodel.ProcessCCP(m_parent, CurrentTicket);



                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {
                TouchMessageBox.Show(" ExecuteCCPClicked:" + e.Message);
            }
        }

        public void ExecutePreAuthClicked(object button)
        {
            try
            {
                //create new ticket
                if (CurrentTicket == null)
                {
                    CreateNewTicket();
                }

                bool paid = m_salesmodel.ProcessPreAuth(m_parent, CurrentTicket);
                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {
                TouchMessageBox.Show(" ExecuteCCPClicked:" + e.Message);
            }
        }

        public void ExecuteGiftCardClicked(object button)
        {
            try
            {
                bool paid = m_salesmodel.ProcessGiftCard(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {
                MessageBox.Show("Gift Card Clicked:" + e.Message);
            }

        }


        public void ExecuteGiftCertificateClicked(object button)
        {
            try
            {
                bool paid = m_salesmodel.ProcessGiftCertificate(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {
                MessageBox.Show(" ExecuteGiftCertificateClicked:" + e.Message);
            }
        }

        public void ExecuteCheckClicked(object button)
        {
            try
            {
                bool paid = m_salesmodel.ProcessCheck(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {
                MessageBox.Show(" ExecuteCheckClicked:" + e.Message);
            }
        }

        public void ExecuteRewardClicked(object button)
        {
            try
            {
                bool paid = m_salesmodel.ProcessRewardCredit(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {
                MessageBox.Show(" Execute Reward Clicked:" + e.Message);
            }
        }

        public void ExecuteCreditCardClicked(object button)
        {
            try
            {
                bool paid = m_salesmodel.ProcessExternalPay(m_parent, m_currentticket);
                CleanUPOrRefresh(paid);
            }
            catch (Exception e)
            {
                MessageBox.Show("3rd Party Clicked:" + e.Message);
            }
        }

        private void CleanUPOrRefresh(bool paid)
        {
            try
            {
                if (paid)
                {   
                    if(GlobalSettings.Instance.RemoteScreen != null)
                        if(GlobalSettings.Instance.RemoteScreen.remotescreenvm != null)
                            GlobalSettings.Instance.RemoteScreen.remotescreenvm.CurrentTicket = null;


                    if(m_parent != null) m_parent.Close();
                }

                CalculateCashButtons();

            }catch(Exception ex)
            {
                TouchMessageBox.Show("Clean Up/ Refresh:" + ex.Message);
                logger.Error("Clean Up/Refresh Error:" + ex.Message);
            }
          
        }





















        public void ExecuteCategoryProductClicked(object objid)
        {

            int id = 0;


            try
            {
                if (objid != null) id = (int)objid;

                if (id != 0)
                {
                    
                        QuickProducts = m_inventorymodel.FillProductbyCatID(id,m_ordertype);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteCategoryProductClicked:" + e.Message);

            }
        }



 

  

        private void CreateNewTicket()
        {
            //create new ticket
        
                if (CurrentEmployee == null) return;

                //create object
                CurrentTicket = new Ticket(CurrentEmployee);



                //creates a sales record
                CurrentTicket.CreateTicket(m_ordertype,m_subordertype, m_tablenumber);
                CurrentTicket.UpdateBarTabCustomer(CustomerName);
                CurrentTicket.UpdateCustomerCount(CustomerCount);

            GlobalSettings.Instance.CurrentTicket = CurrentTicket;
            GlobalSettings.Instance.RemoteScreen.remotescreenvm.RefreshTicket();

        }



        public void ExecuteProductClicked(object obj_id)
        {

            int productid = 0;
       
            try
            {

                if(CurrentTicket != null)
                    if (CurrentTicket.Status == "Closed")
                    {
                        TouchMessageBox.Show("Cannot add item to Closed ticket");
                        return;
                    }

                if (obj_id != null)
                {
                    if (obj_id.ToString() != "") productid = int.Parse(obj_id.ToString());

                }
          

                //create new ticket
                if (CurrentTicket == null)
                {
                    CreateNewTicket();
                }

                if (m_currentticket.Status == "Closed") return;


                //test to see if it's age restricted item
                Product product = new Product(productid,m_ordertype);
                if (product.AgeRestricted)
                {
                    GetDriverLicense dl = new GetDriverLicense(CurrentTicket.Licenses);
                    Utility.OpenModal(m_parent, dl);

                    if (dl.CustomerLicense.LicenseNo != "")
                    {
                        CurrentTicket.InsertIDCheck(dl.CustomerLicense); //inserts if it's not already in license array
                    }

                    if (dl.CustomerLicense.Age < GlobalSettings.Instance.MinimumAgeRestriction)
                    {
                        TouchMessageBox.Show("Customer is less than " + GlobalSettings.Instance.MinimumAgeRestriction + " years old!!");
                        return;
                    }

             

                }

                m_salesmodel.AddItemToTicket(CurrentTicket,m_parent,m_currentseat,m_quantity, productid,m_ordertype);

             

                Quantity = 1;
                QTYStr = "QTY";

                //auto discount 
                Promotion promo = m_promomodel.GetProductAutoDiscount(product.ID);
                if(promo != null)
                {
                    //if require full pricing ,then skip
                    if(promo.FullPriceOnly && CurrentTicket.CurrentLine.SpecialPricing)
                    {
                        CalculateCashButtons();
                        return;
                    }

                    if (promo.LimitedUseOnly && promo.UsageNumber == 0)
                    {
                        CalculateCashButtons();
                        return;
                    }

                    if (CurrentTicket.CurrentLine.AdjustedPrice < promo.MinimumAmount)
                    {
                        CalculateCashButtons();
                        return;
                    }

                    if (CurrentTicket.CurrentLine.Weight < promo.MinimumWeight)
                    {
                        CalculateCashButtons();
                        return;
                    }

                    if (CurrentTicket.CurrentLine.Quantity < promo.MinimumQuantity)
                    {
                        CalculateCashButtons();
                        return;
                    }


                    //apply discount 
                    decimal discount = 0;
                    switch (promo.DiscountMethod)
                    {
                        case "PERCENT":
                            discount = CurrentTicket.CurrentLine.Price *  CurrentTicket.CurrentLine.Weight * promo.DiscountAmount / 100;
                            break;

                        case "AMOUNT":
                            discount = promo.DiscountAmount;
                            break;

                    }


                    CurrentTicket.DiscountLineItem(CurrentTicket.CurrentLine.ID, discount, promo.DiscountType, promo.Description);
                    if (promo.LimitedUseOnly)
                    {
                        //reduce usage by one
                        promo.DeductUsage();
                    }



                }

       

                CurrentTicket.Reload();
                CalculateCashButtons();
            }
            catch (Exception e)
            {

                TouchMessageBox.Show("Product Clicked: " + e.Message);
            }
        }



        public void ExecuteLineItemActionClicked(object action)
        {
            string temp = action as string;
            m_salesmodel.ProcessLineItemAction(m_parent,m_security,  temp, CurrentTicket);
            CalculateCashButtons();
            if (CurrentTicket.CurrentLine == null)
            {
                SelectedColor = "transparent";
                m_currentlineitemid = 0;
            }
                 

        }

        public void ExecuteLineItemClicked(object iditemtype)
        {


            string temp = iditemtype as string;
            string[] portion = temp.Split(',');

            int id = 0;
            string itemtype = portion[1]; // type = service, product , giftcard .. etc..

            id = int.Parse(portion[0]);

            if (id == 0) return;

            if(m_currentlineitemid == id)
            {
                ExecuteLineItemActionClicked("Modifiers");
            }else
            {
                CurrentTicket.GetLineItemLine(id);
                SelectedColor = "Green";
                m_currentlineitemid = id;
            }


   

        }

        public void ExecuteOrderTypeClicked(object obj)
        {
            List<CustomList> list = new List<CustomList>();
            list.Add(new CustomList { id = 1, description = "To Go", returnstring = "ToGo" });
            list.Add(new CustomList { id = 2, description = "Dine In", returnstring = "DineIn" });
            list.Add(new CustomList { id = 2, description = "Delivery", returnstring = "Delivery" });
            list.Add(new CustomList { id = 2, description = "Bar", returnstring = "Bar" });

            ListPad lp = new ListPad("Select Order Type", list);
            Utility.OpenModal(m_parent, lp);

            switch (lp.ReturnString)
            {
                case "ToGo":
                    CurrentTicket.UpdateOrderType(OrderType.ToGo);
                    break;
                case "Bar":
                    CurrentTicket.UpdateOrderType(OrderType.Bar);
                    break;
                case "DineIn":
                    CurrentTicket.UpdateOrderType(OrderType.DineIn);
                    break;
                case "Delivery":
                    CurrentTicket.UpdateOrderType( OrderType.Delivery);
                    break;
            }

          
            CurrentTicket.Reload();
            SetOrderType(CurrentTicket.TicketOrderType);


        }

        public void ExecuteClearOrderClicked(object obj)
        {
            if (CurrentTicket.SentToKitchen)
            {
                if (m_security.WindowAccess("VoidSentTicket"))
                {
                    if (Confirm.Ask("Clear entire ticket??!!"))
                    {
                        CurrentTicket.DeleteAllLineItem();

                        AuditModel.WriteLog(CurrentEmployee.DisplayName, "Clear Sent Ticket", "Ticket has sent items", "sales", CurrentTicket.SalesID);
                        logger.Info("Clear Ticket:reason=>Sent Ticket Items Delete , ticket=>" + CurrentTicket.SalesID + ", employee: " + CurrentEmployee.DisplayName);

                    }

                }
            }else
            {
                CurrentTicket.DeleteAllLineItem();
            }
      
         
        }







     

        public void ExecutePaymentDeleteClicked(object paymentid)
        {
            if (paymentid != null)
                m_paymentmodel.PaymentDelete(CurrentTicket, (int)paymentid, m_parent);
            CalculateCashButtons();
        }


        public void ExecutePreAuthDeleteClicked(object paymentid)
        {
            if(paymentid != null)
                m_paymentmodel.PreAuthDelete(CurrentTicket, (int)paymentid, m_parent);
            CalculateCashButtons();
        }

        public void ExecuteTableLayoutClicked(object button)
        {
            if (m_security.WindowAccess("TableLayout"))
            {
                TableLayout vw = new TableLayout(m_security);
                Utility.OpenModal(m_parent, vw);
            }
        }


        public void ExecuteBackClicked(object button)
        {
            //need to void current ticket if it's not sent
            if (CurrentTicket != null)
            {
               
                    //if nothing on ticket has been sent and HOLD is off then remove entire ticket
                    if (CurrentTicket.SentToKitchen == false)
                    {
                        CurrentTicket.VoidTicket("Ticket Cancelled");
                    } else
                    {
                        // some item has been sent .. so just need to loop thru and void unsent items only
                        foreach(Seat seat in CurrentTicket.Seats)
                        {
                            foreach(LineItem line in seat.LineItems)
                            {
                                if(!line.Sent)
                                {
                                    //CurrentTicket.VoidLineItem(line.ID, "Item Cancelled", false);
                                    CurrentTicket.DeleteLineItem(line.ID);
                                }
                            }
                        }
                    }
                      

            
               
            }

            GlobalSettings.Instance.CurrentTicket = null;
            GlobalSettings.Instance.RemoteScreen.remotescreenvm.RefreshTicket();
            m_parent.Close();

        }


        public void ExecuteHoldClicked(object obj)
        {
            TicketHold th = new TicketHold(CurrentTicket);
            Utility.OpenModal(m_parent, th);

        }

        public void ExecuteSendOrderClicked(object button)
        {
            switch (CurrentTicket.TicketOrderType)
            {
                case OrderType.DineIn:
                    if(CurrentTicket.TicketSubOrderType == SubOrderType.CounterService)
                    {
                        if (GlobalSettings.Instance.CounterServiceMustPayFirst)
                        {
                            TouchMessageBox.Show("This order type require payment or manager override.");
                            if (!m_security.ManagerOverrideAccess("OrderOverride"))
                                return;
                        }
                    }
                

                    break;
                case OrderType.Bar:
                    if (GlobalSettings.Instance.BarMustPayFirst)
                    {
                        TouchMessageBox.Show("This order type require payment or manager override.");
                        if (!m_security.ManagerOverrideAccess("OrderOverride"))
                            return;
                    }

                    break;

                case OrderType.ToGo:
                    if ((GlobalSettings.Instance.CallInMustPayFirst && CurrentTicket.TicketSubOrderType == SubOrderType.CallIn) || (GlobalSettings.Instance.WalkInMustPayFirst && CurrentTicket.TicketSubOrderType == SubOrderType.WalkIn))
                    {
                        TouchMessageBox.Show("This order type require payment or manager override.");
                        if (!m_security.ManagerOverrideAccess("OrderOverride"))
                            return;
                    }

                    break;

                case OrderType.Delivery:
                    if (GlobalSettings.Instance.DeliveryMustPayFirst)
                    {
                        TouchMessageBox.Show("This order type require payment or manager override.");
                        if (!m_security.ManagerOverrideAccess("OrderOverride"))
                            return;
                    }

                    break;

            }

            if (CurrentTicket.Status == "OpenTemp") CurrentTicket.UpdateStatus(TicketStatus.Open);  //Open, OpenTemp, Voided, Closed

            //if does not have a hold , then send to kitchen
            if (!CurrentTicket.HasHoldDate)
            {
                //reload ticket before sending just incase it was modified by tablets
                CurrentTicket.Reload();
                ReceiptPrinterModel.SendKitchen(CurrentTicket);
            }

            GlobalSettings.Instance.CurrentTicket = null;
            GlobalSettings.Instance.RemoteScreen.remotescreenvm.RefreshTicket();
            m_parent.Close();
        }



        public void ExecutePrintReceiptClicked(object button)
        {
            ReceiptPrinterModel.PrintReceipt(CurrentTicket, GlobalSettings.Instance.ReceiptPrinter);
        }



        public void ExecuteCustomerClicked(object button)
        {
            if (CurrentTicket == null) CreateNewTicket();

            CustomerModel.AddEditCustomer(CurrentTicket, m_security, m_parent);

        }
        public void ExecuteBarTabCustomerClicked(object button)
        {

            if(CurrentTicket == null)
            {
                CustomerName = GetCustomerName(CustomerName);
            }
            else
            {
                CustomerName  = GetCustomerName(CurrentTicket.CustomerName);
               CurrentTicket.UpdateBarTabCustomer(CustomerName);
            }
   
        }

        private string GetCustomerName(string name)
        {
            TicketName text = new TicketName(name);
            text.Topmost = true;
            text.ShowDialog();

            if (text.Action == TicketName.OK)
            {
                return text.ReturnText;
            }
            else
                return name;
           
        }



        public void ExecuteEmployeeClicked(object button)
        {


            if (m_security.WindowAccess("EmployeeList") == false) return;
    
            PickEmployee empl = new PickEmployee(m_parent,m_security);
            Utility.OpenModal(m_parent, empl);
            if (empl.EmployeeID > 0)
            {
                m_currentticket.ChangeServer(empl.EmployeeID);

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
                    
                }
            }catch(Exception ex)
            {
                TouchMessageBox.Show("Table Change Error:" + ex.Message);
            }
        
        }


        public void ExecuteAdjustTicketClicked(object button)
        {
           // if (m_security.WindowAccess("Discount") == false) return;
           //security not required since security has been added to the discount item itself

            m_salesmodel.AdjustTicket(CurrentTicket);
  
            CalculateCashButtons();

        }


      



    

        public void ExecuteEditClicked(object salesid)
        {

           
            QuickSales sales = new QuickSales(m_security, CurrentTicket, CurrentTicket.TableNumber,CurrentTicket.CustomerCount,CurrentTicket.TicketOrderType, CurrentTicket.TicketSubOrderType);
            Utility.OpenModal(m_parent, sales);

            CurrentTicket.Reload();


        }








        public void ExecuteTaxExemptClicked(object button)
        {

            if (CurrentTicket.Status == "Closed") MessageBox.Show("Ticket is Closed!");
            else
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

    


        public void ExecuteQuantityClicked(object objquantity)
        {

            try
            {
               Quantity = decimal.Parse(objquantity.ToString());
                QTYStr = "QTY";


                if (Quantity == -99)
                {
                    NumPad num = new NumPad("Enter Quantity", true,true, "1");  //don't allow partials .. so all integers .. then they  can edit for   1/2s and such
                    Utility.OpenModal(m_parent, num);
                    if (num.Amount != "") Quantity = decimal.Parse(num.Amount);
                    else Quantity = 1;

                    if(Quantity != (int) Quantity || Quantity > 9)
                        QTYStr = Math.Round(Quantity, 2).ToString();

                }

              
            }
            catch (Exception e)
            {
                MessageBox.Show("Quantity:" + e.Message);

            }
        }


  

 




        public void ExecuteSeatClicked(object obj)
        {

            try
            {
                
                int seatnumber;


                seatnumber = int.Parse(obj.ToString());
                if (seatnumber > 0)
                {
                    m_currentseat = seatnumber;

                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Menu Prefix 2 Clicked:" + e.Message);

            }
        }


 public void ExecuteAddChairClicked(object obj)
        {

            CustomerCount++;
            CurrentTicket.UpdateCustomerCount(CustomerCount);


            LoadSeats(true);

         
        }













    } //---------- End of SalesViewModel Class



}
