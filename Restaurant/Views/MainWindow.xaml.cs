using System;

using System.Windows;

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
        private SecurityModel _security;

        private bool _loaded = false;
   

        static ManagementEventWatcher w = null;


        public MainWindow()
        {
            InitializeComponent();



            dbConnect = new DBConnect();

            txtMessage.Text = "Red Dot POS version " + GlobalSettings.Instance.VersionNumber + "    " + (dbConnect.TestConnection() ? "Connected to:" + dbConnect.DataBase : "Database Connect Error");
            txtMessage2.Text = "Server:" + GlobalSettings.Instance.DatabaseServer + " Database:" + GlobalSettings.Instance.DatabaseName + " Port:" + GlobalSettings.Instance.PortNo + "  Station No: " + GlobalSettings.Instance.StationNo;

            _security = new SecurityModel();


     


            mainwindowViewModel = new MainWindowVM(this);
            this.DataContext = mainwindowViewModel;




          //  this.Top = GlobalSettings.Instance.r0.Top;
           // this.Left = GlobalSettings.Instance.r0.Left;
            _loaded = true;

        }


 


        private void GiftCard_Click(object sender, RoutedEventArgs e)
        {
           
                GiftCardMenu dlg = new GiftCardMenu(_security);
                Utility.OpenModal(this,dlg);
            _security.LogOff();
           
        }

 
  





    
        private void Nosale_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowNewAccess("NoSale"))
            {
                string m_mode = GlobalSettings.Instance.ReceiptPrinterMode;
                ReceiptPrinter _printer;
                _printer = new ReceiptPrinter(GlobalSettings.Instance.ReceiptPrinter,m_mode);
                _printer.CashDrawer(GlobalSettings.Instance.StationNo);
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

            try
            {
                string backupdirectory = GlobalSettings.Instance.BackupDirectory;


                    DBConnect db = new DBConnect();
                    db.Backup(backupdirectory);
                this.togobutton.Focus();
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Error:" + ex.Message);
            }


        }

        static void AddInsertUSBHandler()
        {

            WqlEventQuery q;
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;

            try
            {

                q = new WqlEventQuery();
                q.EventClassName = "__InstanceCreationEvent";
                q.WithinInterval = new TimeSpan(0, 0, 3);
                q.Condition = "TargetInstance ISA 'Win32_USBControllerdevice'";
                w = new ManagementEventWatcher(scope, q);
                w.EventArrived += USBInserted;

                w.Start();
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                if (w != null)
                {
                    w.Stop();

                }
            }

        }
        static void AddRemoveUSBHandler()
        {

            WqlEventQuery q;
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            scope.Options.EnablePrivileges = true;

            try
            {

                q = new WqlEventQuery();
                q.EventClassName = "__InstanceDeletionEvent";
                q.WithinInterval = new TimeSpan(0, 0, 3);
                q.Condition = "TargetInstance ISA 'Win32_USBControllerdevice'";
                w = new ManagementEventWatcher(scope, q);
                w.EventArrived += USBRemoved;

                w.Start();
            }
            catch (Exception e)
            {


                Console.WriteLine(e.Message);
                if (w != null)
                {
                    w.Stop();

                }
            }

        }

        static void USBInserted(object sender, EventArgs e)
        {

            TouchMessageBox.Show("A USB device inserted");

        }

        static void USBRemoved(object sender, EventArgs e)
        {

            TouchMessageBox.Show("A USB device removed");

        }






        private void SettleTicket_Click(object sender, RoutedEventArgs e)
        {


            //if security level of "orderhistory" is set to 0, then a guest employee will be returned
            if (_security.WindowNewAccess("SettleTicket"))
            {
                SettleTickets odh = new SettleTickets(_security);
                Utility.OpenModal(this, odh);

                _security.LogOff();
            }


        }

        private void CashierInOut_Click(object sender, RoutedEventArgs e)
        {


            if (_security.WindowNewAccess("CashierInOut"))
            {
                CashierInOut rpt = new CashierInOut(_security,false);
                Utility.OpenModal(this, rpt);
                _security.LogOff();
            }


        }


        private void VoidTicket_Click(object sender, RoutedEventArgs e)
        {
           //if security level of "orderhistory" is set to 0, then a guest employee will be returned
            if (_security.WindowNewAccess("VoidClosedTicket"))
            {
                OrderHistory odh = new OrderHistory(_security);
                Utility.OpenModal(this, odh);
                _security.LogOff();
            }
        }


        private void RefundTicket_Click(object sender, RoutedEventArgs e)
        {
            //if security level of "orderhistory" is set to 0, then a guest employee will be returned
            if (_security.WindowNewAccess("Refund"))
            {
                OrderHistory odh = new OrderHistory(_security);
                Utility.OpenModal(this, odh);
                _security.LogOff();
            }
        }




        private void Customer_Click(object sender, RoutedEventArgs e)
        {

            if (_security.WindowNewAccess("CustomerView") == false) return;
 


            CustomerSearch cs = new CustomerSearch(_security);
            Utility.OpenModal(this, cs);

            //need to check existence of customerid
            if (cs.CustomerID > 0)
            {
                CustomerView custvw = new CustomerView(_security,cs.CustomerID);
                Utility.OpenModal(this, custvw);
            }

            _security.LogOff();


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




        private void Employees_Clicked(object sender, RoutedEventArgs e)
        {
            if (_security.WindowNewAccess("EmployeeMenu","",GlobalSettings.Instance.FingerPrintRequiredForTimeCard))
            {
                EmployeeMenu rpt = new EmployeeMenu(_security);
                Utility.OpenModal(this, rpt);
                _security.LogOff();

            }
        }


        private void btnOperation_Click(object sender, RoutedEventArgs e)
        {



            _security.LogOff();
        }



        private void OPenOrders_Click(object sender, RoutedEventArgs e)
        {
            //if security level of "orderhistory" is set to 0, then a guest employee will be returned
            if (_security.WindowNewAccess("OpenOrders"))
            {
                OpenTickets ord = new OpenTickets(_security, 0, -1);
                Utility.OpenModal(this,ord);
                _security.LogOff();

            }
        }
    }
}
