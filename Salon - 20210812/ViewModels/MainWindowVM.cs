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
using RedDot.DataManager;


namespace RedDot
{




    public class MainWindowVM : INPCBase
    {

        // private bool canExecute = true;
        private Window m_parent;
        private SecurityModel m_security;
        private DBEmployee m_dbemployee;

        private DataTable m_employeelist;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        string m_salesviewmode = GlobalSettings.Instance.SalesViewMode;

        public List<LanguageType> LanguageList { get; set; }



        public string SelectedLanguage {
            get { return GlobalSettings.Instance.LanguageCode; }
            set {
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


        private VFD vfd;
        public Visibility VisibleEditTurns { get; set; }

        public ICommand SalonSalesClicked { get; set; }

       
        public ICommand SettingsClicked { get; set; }

           public ICommand CheckInClicked { get; set; }
        public ICommand EditTurnsClicked { get; set; }

        public ICommand CreditCardManagerClicked { get; set; }
        public ICommand TicketHistoryClicked { get; set; }




        public Visibility ProVersion { get; set; }
        public Visibility BaseVersion { get; set; }

        public Visibility Demo { get; set; }


        public Visibility VisibleHide { get; set; }
        public Visible ButtonVisibility { get; set; }
        public bool CanExecuteHeartSIP { get; set; }




        private bool ProcessCreditCard = false;



        // private string _appmode;
        //private int m_employeeid;
        public MainWindowVM(Window parent)
        {
            ProVersion = GlobalSettings.Instance.ProVersion;
            BaseVersion = GlobalSettings.Instance.BaseVersion;
            if (GlobalSettings.Instance.Demo) Demo = Visibility.Visible;
            else Demo = Visibility.Hidden;

            vfd = new VFD(GlobalSettings.Instance.DisplayComPort);
            CanExecuteHeartSIP = (GlobalSettings.Instance.CreditCardProcessor == "HeartSIP" || GlobalSettings.Instance.CreditCardProcessor == "PAX_S300" || GlobalSettings.Instance.CreditCardProcessor == "HSIP_ISC250");


            SalonSalesClicked = new RelayCommand(ExecuteSalonSalesClicked, param => true);
           

            SettingsClicked = new RelayCommand(ExecuteSettingsClicked, param => true);
        
            CheckInClicked = new RelayCommand(ExecuteCheckInClicked, param => true);
            EditTurnsClicked = new RelayCommand(ExecuteEditTurnsClicked, param => true);

            CreditCardManagerClicked = new RelayCommand(ExecuteCreditCardManagerClicked, param => this.CanExecuteHeartSIP);
            TicketHistoryClicked = new RelayCommand(ExecuteTicketHistoryClicked, param => true);





            m_parent = parent;
          //  _appmode = GlobalSettings.Instance.ApplicationMode;
            m_dbemployee = new DBEmployee();

            ButtonVisibility = new Visible();
            m_security = new SecurityModel();
  

            vfd.WriteRaw("VTT Red Dot POS", "Ready...");
            EmployeeList = m_dbemployee.GetCheckInList(DateTime.Now);

            SelectedLanguage = GlobalSettings.Instance.LanguageCode;


            var culture = CultureInfo.GetCultureInfo(SelectedLanguage);
            // Dispatcher.Thread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            LocalizationManager.UpdateValues();
        

            LanguageList = new List<LanguageType>();
            LanguageList.Add(new LanguageType() { Language = "English (US)", LanguageCode = "en-US", Flag = "/media/USA Flag.png" });
            LanguageList.Add(new LanguageType() { Language = "Tiếng Việt", LanguageCode = "vi-VN", Flag = "/media/vietnam.png" });
            LanguageList.Add(new LanguageType() { Language = "Française", LanguageCode = "fr-FR", Flag = "/media/french.png" });

            ProcessCreditCard = (GlobalSettings.Instance.CreditCardProcessor != "External");
            Message = "Dot Net Version:" + GlobalSettings.Instance.DotNetVersion;
        }

        public DataTable EmployeeList
        {
            get { return m_employeelist; }
            set
            {
                m_employeelist = value;
                NotifyPropertyChanged("EmployeeList");
            }
        }
 

        public string StoreLogo
        {
            get { return "pack://siteoforigin:,,,/" + GlobalSettings.Instance.StoreLogo; }
        }

        public string MainBackgroundImage
        {
            get { return "pack://siteoforigin:,,,/" + GlobalSettings.Instance.MainBackgroundImage; }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            { 
                message = value;
                NotifyPropertyChanged("Message");
            }
        }

   

        //---------------------------------------------------------------------- Button Commands ------------------------------------------------

        public void ExecuteTicketHistoryClicked(object obj)
        {
            if (!m_security.WindowNewAccess("TicketHistory"))
            {
                // Message("Access Denied.");
                return;
            }


            OrderHistory dlg = new OrderHistory(m_security,null);
            Utility.OpenModal(m_parent, dlg);

            m_security.CurrentEmployee = null;
        }



        public void ExecuteSalonSalesClicked(object button)
        {
             
    
           //string windowClass = "RedDot.SalonSalesCustom1";
            //System.Type type = Assembly.GetExecutingAssembly().GetType(windowClass);
           // ObjectHandle handle = Activator.CreateInstance(null, windowClass);
           // MethodInfo method = type.GetMethod("Show");
          //  method.Invoke(handle.Unwrap(), null);


            if(GlobalSettings.Instance.SalesViewScreen == "SalonSales")
            {
                SalonSales salonsales = new SalonSales(0);
                Utility.OpenModal(m_parent, salonsales);
            }
            else
            {
                SalonSalesCustom1 salonsales = new SalonSalesCustom1(0);
                Utility.OpenModal(m_parent, salonsales);
            }

           

            
             
                EmployeeList = m_dbemployee.GetCheckInList(DateTime.Now);

            m_security.CurrentEmployee = null;

        }


        public void ExecuteCreditCardManagerClicked(object obj)
        {
            if (!m_security.WindowNewAccess("CreditCardManager"))
            {
                // Message("Access Denied.");
                return;
            }

         

            switch (GlobalSettings.Instance.CreditCardProcessor)
            {
                case "HeartSIP":
                    CreditCardManager dlg = new CreditCardManager(null, m_security.CurrentEmployee);
                    Utility.OpenModal(m_parent, dlg);

                    break;
                case "PAX_S300":
                    PAXManager dlg2 = new PAXManager(m_security.CurrentEmployee);
                    Utility.OpenModal(m_parent, dlg2);

                    break;
            }

            m_security.CurrentEmployee = null;

        }






        public void ExecuteCheckInClicked(object button)
        {
            if (m_security.WindowNewAccess("CheckIn"))
            {
                DataTable dt = m_dbemployee.GetCheckIn(m_security.CurrentEmployee.ID,DateTime.Now);
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("Already check in!!");
                }
                else
                {
                    m_dbemployee.InsertCheckIn(m_security.CurrentEmployee.ID, DateTime.Now);
                    EmployeeList = m_dbemployee.GetCheckInList(DateTime.Now);
                }

            }

            m_security.CurrentEmployee = null;
        }


        public void ExecuteEditTurnsClicked(object button)
        {
            if (m_security.WindowNewAccess("EditTurns"))
            {
                Turns dlg = new Turns(m_parent);
                Utility.OpenModal(m_parent, dlg);
                EmployeeList = m_dbemployee.GetCheckInList(DateTime.Now);
            }

            m_security.CurrentEmployee = null;
        }

        public void ExecuteNoSaleClicked(object button)
        {
            //Open cash drawer
            if (m_security.WindowNewAccess("NoSale") == true)
            {
                ReceiptPrinter.CashDrawer(GlobalSettings.Instance.ReceiptPrinter);
                logger.Info("No Sale Clicked:" + m_security.CurrentEmployee.DisplayName);

            }

            m_security.CurrentEmployee = null;


        }


        public void ExecuteSettingsClicked(object button)
        {
            if (!m_security.WindowNewAccess("Settings"))
            {
                // Message("Access Denied.");
                return;
            }
            Settings set = new Settings(m_parent);
            Utility.OpenModal(m_parent, set);


            NotifyPropertyChanged("MainBackgroundImage");
            NotifyPropertyChanged("StoreLogo");
            m_salesviewmode = GlobalSettings.Instance.SalesViewMode;


            m_security.CurrentEmployee = null;

        }


    }

 
}
