using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Printing;
using RedDot;
using System.Globalization;
using WpfLocalization;
using System.Data;
using System.Security.Cryptography;
using System.IO;
using System.Management;





namespace RedDot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :Window
    {

        private DBConnect dbConnect;
        private MainWindowVM mainwindowViewModel;
        private Security _security;

        private bool _loaded = false;

        System.Windows.Threading.DispatcherTimer dispatcherTimer2;



        public MainWindow()
        {
            InitializeComponent();

            dbConnect = new DBConnect();

            txtMessage.Text = "Red Dot POS version " + GlobalSettings.Instance.VersionNumber ;
            dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer2.Tick += new EventHandler(dispatcherTimer_Tick2);
            dispatcherTimer2.Interval = new TimeSpan(0, 1, 0);//once per minute
            dispatcherTimer2.Start();
            dispatcherTimer_Tick2(null, null);

             _security = new Security();


            if (GlobalSettings.Instance.RunPrinterTest)
            {
                new ReceiptPrinter(GlobalSettings.Instance.ReceiptPrinter).PrinterTest();

            }


            mainwindowViewModel = new MainWindowVM(this);
            this.DataContext = mainwindowViewModel;



            comboBoxLanguage.SelectedValue = GlobalSettings.Instance.Language;

            _loaded = true;

            this.Width = GlobalSettings.Instance.ScreenWidth;
             this.Height = GlobalSettings.Instance.ScreenHeight;

           // this.Width = 1600;
          //  this.Height = 900;


            this.Background = Brushes.Black;
        }

        private void dispatcherTimer_Tick2(object sender, EventArgs e)
        {
            txtMessage2.Text = (DBConnect.TestConnection() ? "Database Active =>" + dbConnect.DataBaseServer + " : " + dbConnect.DataBase : "Database Error");


        }
        private void Appointment_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowNewAccess("Appointment"))
            {
                AppointmentView dlg = new AppointmentView(_security);
                Utility.OpenModal(this, dlg);
                _security.LogOff();
            }

          
        }


        private void GiftCard_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowNewAccess("GiftCard"))
            {
                GiftCardMenu dlg = new GiftCardMenu(_security);
                Utility.OpenModal(this,dlg);
                _security.LogOff();
            }

           
        }

 
        private void RetailSales_Click(object sender, RoutedEventArgs e)
        {


            if (_security.WindowNewAccess("RetailSales"))
            {


            
                        RetailSales rtl2 = new RetailSales(_security);
                        Utility.OpenModal(this, rtl2);
              //  _security.LogOff();

            }

           
        }





        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }
        private void Nosale_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowNewAccess("NoSale"))
            {

                ReceiptPrinter _printer;
                _printer = new ReceiptPrinter(GlobalSettings.Instance.ReceiptPrinter);
                _printer.CashDrawer();
                _security.LogOff();
            }
 
        }



        /// <summary>
        /// DataBase Backup     -------------------------------------------------- BACK UP -------------------------------------------------------
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DBBackup(object sender, RoutedEventArgs e)
        {


            string backupdirectory = GlobalSettings.Instance.BackupDirectory;
      
            BackupSelect bk = new BackupSelect(backupdirectory);
   
            bk.ChosenDrive = "";

            Utility.OpenModal(this, bk);

            if(bk.ChosenDrive != "")
            {
                DBConnect db = new DBConnect();
                if(bk.ChosenDrive != backupdirectory) backupdirectory = bk.ChosenDrive + "backup";
                //"C:\\Program Files\\MySQL\\MySQL Server 5.6\\bin\\mysqldump"
                db.Backup(backupdirectory, GlobalSettings.Instance.MySQLDumpPath);
                MessageBox.Show("Backup successful to  " + backupdirectory);

            }

        }


 

        static void USBInserted(object sender, EventArgs e)
        {

            MessageBox.Show("A USB device inserted");

        }

        static void USBRemoved(object sender, EventArgs e)
        {

            MessageBox.Show("A USB device removed");

        }
        private void TicketHistory_Click(object sender, RoutedEventArgs e)
        {


            //if security level of "orderhistory" is set to 0, then a guest employee will be returned
            if (_security.WindowNewAccess("TicketHistory"))
            {
                OrderHistory odh = new OrderHistory(_security);
                Utility.OpenModal(this, odh);
                _security.LogOff();
            }


        }


        private void Customer_Click(object sender, RoutedEventArgs e)
        {

            if (_security.WindowNewAccess("CustomerView") == false)
            {
                MessageBox.Show("Access Denied.");
                return;
            }


            CustomerSearch cs = new CustomerSearch();
            Utility.OpenModal(this, cs);

            //need to check existence of customerid
            if (cs.CustomerID > 0)
            {
                CustomerView custvw = new CustomerView(_security,cs.CustomerID);
                Utility.OpenModal(this, custvw);
            }


            _security.LogOff();

        }
 

        private void comboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = comboBoxLanguage.SelectedItem as ComboBoxItem;

            if (selectedItem == null) return;

            var culture = CultureInfo.GetCultureInfo(selectedItem.Tag as string);

            // Dispatcher.Thread.CurrentCulture = culture;
            Dispatcher.Thread.CurrentUICulture = culture;

            if (_loaded)
            {
                GlobalSettings.Instance.Language = selectedItem.Content.ToString(); //save language to settings
                GlobalSettings.Instance.LanguageCode = selectedItem.Tag as string; //save language code to settings

            }



            LocalizationManager.UpdateValues();
        }




        private void Employees_Clicked(object sender, RoutedEventArgs e)
        {
            if (_security.WindowNewAccess("EmployeeMenu"))
            {
                EmployeeMenu rpt = new EmployeeMenu(_security);
                Utility.OpenModal(this, rpt);
                _security.LogOff();
            }
        }


        private void btnOperation_Click(object sender, RoutedEventArgs e)
        {

            OperationMenu rpt = new OperationMenu();
            Utility.OpenModal(this, rpt);

            _security.LogOff();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            Environment.Exit(0);
        }



        private void SizeChanged1(object sender, SizeChangedEventArgs e)
        {
            
            GlobalSettings.Instance.ScreenHeight = (int)this.Height;
             GlobalSettings.Instance.ScreenWidth = (int)this.Width;
        }



    }
}
