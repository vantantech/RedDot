using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;
using RedDot;
using System.Globalization;
using WpfLocalization;
using System.Security.Cryptography;
using System.IO;
using System.Management;
using System.Threading;
using NLog;
using System.Data.SQLite;

namespace RedDot
{




    public class MainWindowVM : INPCBase
    {
      
        // private bool canExecute = true;
        private Window m_parent;
        private SecurityModel m_security;
        private DBEmployee m_dbemployee;
        private int stationno = GlobalSettings.Instance.StationNo;
   
        private string m_message = "";
        public string Message
        {
            get { return m_message; }
            set
            {
                m_message = value;
                NotifyPropertyChanged("Message");
            }
        }


       

        // Public variables
        public static string StoredName;
        public static Boolean UseStoredName;


        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        System.Windows.Threading.DispatcherTimer dispatcherTimer2;







        public List<LanguageType> LanguageList { get; set; }
        public string SelectedLanguage
        {
            get { return GlobalSettings.Instance.LanguageCode; }
            set
            {
                GlobalSettings.Instance.LanguageCode = value;
                NotifyPropertyChanged("SelectedLanguage");


           

                var culture = CultureInfo.GetCultureInfo(value);
                // Dispatcher.Thread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                LocalizationManager.UpdateValues();
            }
        }


        public ICommand BarClicked { get; set; }
        public ICommand ECRClicked { get; set; }
        public ICommand ToGoClicked { get; set; }
        public ICommand CounterServiceClicked { get; set; }
        public ICommand BackofficeClicked { get; set; }
        public ICommand DeliveryClicked { get; set; }
        


        public ICommand TableServiceClicked { get; set; }
        public ICommand CreditCardManagerClicked { get; set; }
        public ICommand ExitClicked { get; set; }


        public Visibility VisibleHide { get; set; }
         public Visible ButtonVisibility { get; set; }
        // private string _appmode;



        private bool ProcessCreditCard = false;


        public Visibility VisibleDemo { get; set; }

        public int ButtonBorderThickness { get; set; }
        public string ButtonBorderColor { get; set; }

        private bool busy = false;

        public MainWindowVM(Window parent)
        {

            ToGoClicked = new RelayCommand(ExecuteToGoClicked, param => true);
            BarClicked = new RelayCommand(ExecuteBarClicked, param => true);
            CounterServiceClicked = new RelayCommand(ExecuteCounterServiceClicked, param => true);
            ECRClicked = new RelayCommand(ExecuteECRClicked, param => true);
            DeliveryClicked = new RelayCommand(ExecuteDeliveryClicked, param => true);

            BackofficeClicked = new RelayCommand(ExecuteBackofficeClicked, param => true);
        

            TableServiceClicked = new RelayCommand(ExecuteTableServiceClicked, param => true);

            CreditCardManagerClicked = new RelayCommand(ExecuteCreditCardManagerClicked, param => this.CanExecuteHeartSIP);
            ExitClicked = new RelayCommand(ExecuteExitClicked, param => true);

            m_parent = parent;
           // _appmode = GlobalSettings.Instance.ApplicationMode;
            m_dbemployee = new DBEmployee();
        

            ButtonVisibility = new Visible();
            m_security = new SecurityModel();
         

            GlobalSettings.CustomerDisplay.WriteRaw("VTT Red Dot POS", "Ready...");

            SelectedLanguage = GlobalSettings.Instance.LanguageCode;

            var culture = CultureInfo.GetCultureInfo(SelectedLanguage);
            // Dispatcher.Thread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            LocalizationManager.UpdateValues();


            LanguageList = new List<LanguageType>();
            LanguageList.Add(new LanguageType() { Language = "English (US)", LanguageCode = "en-US", Flag = "/media/USA Flag.png" });
            LanguageList.Add(new LanguageType() { Language = "Tiếng Việt", LanguageCode = "vi-VN", Flag = "/media/vietnam.png" });
            LanguageList.Add(new LanguageType() { Language = "Française", LanguageCode = "fr-FR", Flag = "/media/french.png" });

            InitSettings();


            if (GlobalSettings.Instance.Demo) VisibleDemo = Visibility.Visible; else VisibleDemo = Visibility.Hidden;


            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);  //test every minute
            dispatcherTimer.Start();


            dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer2.Tick += new EventHandler(dispatcherTimer_Tick2);
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 2);  //test every minute
            dispatcherTimer2.Start();



        }

  
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
          

            if (busy) return;



            busy = true;
           DataTable dt =  SalesModel.GetHoldTickets(GlobalSettings.Instance.StationNo);  //gets all ticket that meets the firing date ... basically hold date is expired
            if(dt != null)
            {
                foreach(DataRow row in dt.Rows)
                {
                    Ticket ticket = new Ticket((int)row["id"]);
                    ReceiptPrinterModel.SendKitchen(ticket);
                }
              
            }

            busy = false;
        }


        private void dispatcherTimer_Tick2(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
            Message = "Station No: " + stationno + "   " +  CurrentTime.ToString();

       
        }

        public void InitSettings()
        {
            ProcessCreditCard = (GlobalSettings.Instance.CreditCardProcessor == "PAX_S300");

            VisibleToGo = GlobalSettings.Instance.EnableToGoOrders ? Visibility.Visible : Visibility.Collapsed;
            VisibleBar = GlobalSettings.Instance.EnableBar ? Visibility.Visible : Visibility.Collapsed;
            VisibleDineIn = GlobalSettings.Instance.EnableDineIn ? Visibility.Visible : Visibility.Collapsed;
            VisibleDelivery = GlobalSettings.Instance.EnableDelivery ? Visibility.Visible : Visibility.Collapsed;
            VisibleECR = GlobalSettings.Instance.EnableECR ? Visibility.Visible : Visibility.Collapsed;
            VisibleCounterService = GlobalSettings.Instance.EnableCounterService ? Visibility.Visible : Visibility.Collapsed;
            VisibleCallIn = GlobalSettings.Instance.EnableCallInOrders ? Visibility.Visible : Visibility.Collapsed;
            VisibleWalkIn = GlobalSettings.Instance.EnableWalkInOrders ? Visibility.Visible : Visibility.Collapsed;
            VisibleDriveThru = GlobalSettings.Instance.EnableDriveThruOrders? Visibility.Visible : Visibility.Collapsed;

            ButtonBorderThickness = GlobalSettings.Instance.ButtonBorderThickness;
            ButtonBorderColor = GlobalSettings.Instance.ButtonBorderColor;
            StoreLogo = "pack://siteoforigin:,,,/" + GlobalSettings.Instance.StoreLogo; 
            MainBackgroundImage = "pack://siteoforigin:,,,/" + GlobalSettings.Instance.MainBackgroundImage;
            MainBackgroundColor = GlobalSettings.Instance.WindowBackgroundColor;
            
            NotifyPropertyChanged("RedDotLogo");
            

            NotifyPropertyChanged("VisibleToGoOrders");
            NotifyPropertyChanged("VisibleBar");
            NotifyPropertyChanged("VisibleDineIn");
            NotifyPropertyChanged("VisibleECR");
            NotifyPropertyChanged("VisibleCounterService");
            NotifyPropertyChanged("ButtonBorderThickness");
            NotifyPropertyChanged("StoreLogo");
            NotifyPropertyChanged("MainBackgroundImage");
            NotifyPropertyChanged("MainBackgroundColor");
        }

     






        /*


        ██████╗ ██╗   ██╗██████╗ ██╗     ██╗ ██████╗    ██████╗ ██████╗  ██████╗ ██████╗ ███████╗██████╗ ████████╗██╗   ██╗
        ██╔══██╗██║   ██║██╔══██╗██║     ██║██╔════╝    ██╔══██╗██╔══██╗██╔═══██╗██╔══██╗██╔════╝██╔══██╗╚══██╔══╝╚██╗ ██╔╝
        ██████╔╝██║   ██║██████╔╝██║     ██║██║         ██████╔╝██████╔╝██║   ██║██████╔╝█████╗  ██████╔╝   ██║    ╚████╔╝ 
        ██╔═══╝ ██║   ██║██╔══██╗██║     ██║██║         ██╔═══╝ ██╔══██╗██║   ██║██╔═══╝ ██╔══╝  ██╔══██╗   ██║     ╚██╔╝  
        ██║     ╚██████╔╝██████╔╝███████╗██║╚██████╗    ██║     ██║  ██║╚██████╔╝██║     ███████╗██║  ██║   ██║      ██║   
        ╚═╝      ╚═════╝ ╚═════╝ ╚══════╝╚═╝ ╚═════╝    ╚═╝     ╚═╝  ╚═╝ ╚═════╝ ╚═╝     ╚══════╝╚═╝  ╚═╝   ╚═╝      ╚═╝   


        */

        public Visibility VisibleToGo { get; set; }
        public Visibility VisibleBar { get; set; }
        public Visibility VisibleDineIn { get; set; }
        public Visibility VisibleECR { get; set; }
        public Visibility VisibleCounterService { get; set; }
        public Visibility VisibleDelivery { get; set; }
        public Visibility VisibleCallIn { get; set; }
        public Visibility VisibleWalkIn { get; set; }
        public Visibility VisibleDriveThru { get; set; }

        public string StoreLogo { get; set; }
        private DateTime _currenttime;
        public DateTime CurrentTime
        {
            get { return _currenttime; }
            set
            {
                _currenttime = value;
                NotifyPropertyChanged("CurrentTime");
            }
        }

        public string MainBackgroundImage { get; set; }
        public string MainBackgroundColor { get; set; }


        public string RedDotLogo
        {
            get
            {
                if (MainBackgroundImage == "pack://siteoforigin:,,,/") return "/RedDot;component/media/sphere.png";
                else return "";
            }
        }


        //--------------------------------------------------------------  Button Enable ----------------------------------------------

        public bool CanExecuteHeartSIP
        {
            get
            {
                return ProcessCreditCard;
            }
        }




        //---------------------------------------------------------------------- Button Commands ------------------------------------------------

        public void ExecuteECRClicked(object button)
        {
            if (m_security.WindowNewAccess("Sales"))
            {
                if (GlobalSettings.Instance.ClockInRequired)
                {
                    if(!m_security.HasAccess("OrderClockInBypass"))
                        if (!m_security.CurrentEmployee.ClockedIn)
                        {
                            TouchMessageBox.Show("Please Clock In First");
                            return;
                        }
                }

                QuickSales salonsales = new QuickSales(m_security, null,0, -1,OrderType.DineIn, SubOrderType.None);
                Utility.OpenModal(m_parent, salonsales);

            }

            m_security.CurrentEmployee = null;
        }


        public void ExecuteToGoClicked(object selection)
        {
            if (m_security.WindowNewAccess("Sales"))
            {
                if (GlobalSettings.Instance.ClockInRequired)
                {
                    if (!m_security.HasAccess("OrderClockInBypass"))
                        if (!m_security.CurrentEmployee.ClockedIn)
                        {
                            TouchMessageBox.Show("Please Clock In First");
                            return;
                        }
                }

                switch(selection.ToString())
                {
                    case "CallIn":
                        QuickSales salonsales = new QuickSales(m_security, null, 0, 0, OrderType.ToGo, SubOrderType.CallIn);
                        Utility.OpenModal(m_parent, salonsales);
                        break;
                    case "WalkIn":
                        QuickSales salonsales2 = new QuickSales(m_security, null, 0, 0, OrderType.ToGo, SubOrderType.WalkIn);
                        Utility.OpenModal(m_parent, salonsales2);
                        break;
                    case "DriveThru":
                        QuickSales salonsales3 = new QuickSales(m_security, null, 0, 0, OrderType.ToGo, SubOrderType.DriveThru);
                        Utility.OpenModal(m_parent, salonsales3);
                        break;
                    default:
                        ExecuteToGo();
                        break;

                }
                
            }

            m_security.CurrentEmployee = null;
        }

        private void ExecuteToGo()
        {
            List<CustomList> list = new List<CustomList>();
            list.Add(new CustomList { description = "Walk-In", returnstring = "WalkIn" });
            list.Add(new CustomList { description = "Call-In", returnstring = "CallIn" });

            ListPad lp = new ListPad("Please select To Go Type:", list);
            lp.Topmost = true;
            lp.ShowDialog();

            if (lp.ReturnString == "") return;



            if (lp.ReturnString == "WalkIn")
            {

                QuickSales salonsales = new QuickSales(m_security, null, 0, 0, OrderType.ToGo, SubOrderType.WalkIn);
                Utility.OpenModal(m_parent, salonsales);

            }
            else
            {
                QuickSales salonsales = new QuickSales(m_security, null, 0, 0, OrderType.ToGo, SubOrderType.CallIn);
                Utility.OpenModal(m_parent, salonsales);
            }

            m_security.CurrentEmployee = null;
        }


        public void ExecuteDeliveryClicked(object button)
        {
            if (m_security.WindowNewAccess("Sales"))
            {
                if (GlobalSettings.Instance.ClockInRequired)
                {
                    if (!m_security.HasAccess("OrderClockInBypass"))
                        if (!m_security.CurrentEmployee.ClockedIn)
                        {
                            TouchMessageBox.Show("Please Clock In First");
                            return;
                        }
                }
                QuickSales salonsales = new QuickSales(m_security, null, 0, 0, OrderType.Delivery,SubOrderType.None);
                Utility.OpenModal(m_parent, salonsales);

            }

            m_security.CurrentEmployee = null;
        }




        public void ExecuteCounterServiceClicked(object button)
        {
            if (m_security.WindowNewAccess("Sales"))
            {
                if (GlobalSettings.Instance.ClockInRequired)
                {
                    if (!m_security.HasAccess("OrderClockInBypass"))
                        if (!m_security.CurrentEmployee.ClockedIn)
                        {
                            TouchMessageBox.Show("Please Clock In First");
                            return;
                        }
                }
                QuickSales salonsales = new QuickSales(m_security, null,0, 0,OrderType.DineIn, SubOrderType.CounterService);
                Utility.OpenModal(m_parent, salonsales);

            }

            m_security.CurrentEmployee = null;
        }


        public void ExecuteBarClicked(object button)
        {
            if (m_security.WindowNewAccess("Sales"))
            {
                if (GlobalSettings.Instance.ClockInRequired)
                {
                    if (!m_security.HasAccess("OrderClockInBypass"))
                        if (!m_security.CurrentEmployee.ClockedIn)
                        {
                            TouchMessageBox.Show("Please Clock In First");
                            return;
                        }
                }

     

                QuickSales salonsales = new QuickSales(m_security, null, 0,0, OrderType.Bar,SubOrderType.None);
                Utility.OpenModal(m_parent, salonsales);

            }

            m_security.CurrentEmployee = null;
        }







        public void ExecuteTableServiceClicked(object button)
        {
            if (m_security.WindowNewAccess("Sales"))
            {
                if (GlobalSettings.Instance.ClockInRequired)
                {
                    if (!m_security.HasAccess("OrderClockInBypass"))
                        if (!m_security.CurrentEmployee.ClockedIn)
                        {
                            TouchMessageBox.Show("Please Clock In First");
                            return;
                        }
                }
                TableService tb = new TableService(m_security);
                Utility.OpenModal(m_parent, tb);

            }

            m_security.CurrentEmployee = null;
        }


        public void ExecuteBackofficeClicked(object button)
        {
            if (m_security.WindowNewAccess("BackOffice"))
            {
                BackOfficeMenu backoffice = new BackOfficeMenu(m_parent, m_security);
                Utility.OpenModal(m_parent, backoffice);
                InitSettings();
            }

            m_security.CurrentEmployee = null;
        }


        public void ExecuteCreditCardManagerClicked(object obj)
        {
         


            try
            {
                if (m_security.WindowNewAccess("CreditCardManager"))
                {
                    string ipaddress = GlobalSettings.Instance.SIPDefaultIPAddress;
                    string displaycomport = GlobalSettings.Instance.DisplayComPort;

                    PAXManager dlg2 = new PAXManager(ipaddress, displaycomport, ReceiptPrinterModel.PrintResponse)
                    {
                        Topmost = true,
                        ShowInTaskbar = false
                    };
                    dlg2.ShowDialog();

                }

                m_security.CurrentEmployee = null;

            }
            catch(Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
            }
           
                   

              
            

        }



        public void ExecuteExitClicked(object obj)
        {
            if (m_security.WindowNewAccess("ExitProgram"))
            {
                Application.Current.Shutdown();
                Environment.Exit(0);
            }


            
        }


        public void PrintResponse()
        {

        }


    }
    public class LanguageType
    {
        public string Language { get; set; }
        public string LanguageCode { get; set; }
        public string Flag { get; set; }
    }

}
