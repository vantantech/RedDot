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
using NLog;
using RedDot.DataManager;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SalonMainWindow : Window
    {

        private DBConnect dbConnect;
        private MainWindowVM mainwindowViewModel;
        private SecurityModel m_security;
        private static Logger logger = LogManager.GetCurrentClassLogger();

      


        System.Windows.Threading.DispatcherTimer dispatcherTimer;


        public string BottomMessage
        {
            set
            {
                mainwindowViewModel.Message = value;
            }

        }

        public SalonMainWindow()
        {
            InitializeComponent();



            // AddInsertUSBHandler();
            // AddRemoveUSBHandler();
            dbConnect = new DBConnect();

            txtMessage.Text = "Red Dot POS version " + GlobalSettings.Instance.VersionNumber + "  " + (dbConnect.TestConnection() ? " Connected to:" + dbConnect.DataBase : "Database Connect Error") + " Server:" + GlobalSettings.Instance.DatabaseServer + " Database:" + GlobalSettings.Instance.DatabaseName + " Port:" + GlobalSettings.Instance.PortNo;


            reportsmenu.Visibility = Visibility.Hidden;
            customersmenu.Visibility = Visibility.Hidden;
            adminmenu.Visibility = Visibility.Hidden;
            messagemenu.Visibility = Visibility.Hidden;
            giftcardmenu.Visibility = Visibility.Hidden;
            employeemenu.Visibility = Visibility.Hidden;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);




            m_security = new SecurityModel();


            //var culture = CultureInfo.GetCultureInfo(GlobalSettings.Instance.LanguageCode);
            // Dispatcher.Thread.CurrentCulture = culture;
            // Dispatcher.Thread.CurrentUICulture = culture;
            // LocalizationManager.UpdateValues();



            mainwindowViewModel = new MainWindowVM(this);
            this.DataContext = mainwindowViewModel;



         
          



        }
    
    
     
        private void Message(string message)
        {
            tbMessage.Text = message;
            messagemenu.Visibility = Visibility.Visible;
            reportsmenu.Visibility = Visibility.Hidden;
            customersmenu.Visibility = Visibility.Hidden;
            adminmenu.Visibility = Visibility.Hidden;
            giftcardmenu.Visibility = Visibility.Hidden;
            employeemenu.Visibility = Visibility.Hidden;

            ClearPanel(2);

        }

        private void ClearPanel(int seconds)
        {

            dispatcherTimer.Interval = new TimeSpan(0, 0, seconds);
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            reportsmenu.Visibility = Visibility.Hidden;
            customersmenu.Visibility = Visibility.Hidden;
            adminmenu.Visibility = Visibility.Hidden;
            messagemenu.Visibility = Visibility.Hidden;
            giftcardmenu.Visibility = Visibility.Hidden;
            employeemenu.Visibility = Visibility.Hidden;
        }



        private void Customer_Click(object sender, RoutedEventArgs e)
        {


            if (!m_security.WindowNewAccess("CustomerView"))
            {
                // Message("Access Denied.");
                return;
            }


            reportsmenu.Visibility = Visibility.Hidden;
            adminmenu.Visibility = Visibility.Hidden;
            customersmenu.Visibility = Visibility.Visible;
            messagemenu.Visibility = Visibility.Hidden;
            giftcardmenu.Visibility = Visibility.Hidden;
            employeemenu.Visibility = Visibility.Hidden;
            ClearPanel(5);


        }
        private void GiftCard_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("GiftCard"))
            {
                //Message("Access Denied.");
                return;
            }

            giftcardmenu.Visibility = Visibility.Visible;
            reportsmenu.Visibility = Visibility.Hidden;
            adminmenu.Visibility = Visibility.Hidden;
            customersmenu.Visibility = Visibility.Hidden;
            messagemenu.Visibility = Visibility.Hidden;
            employeemenu.Visibility = Visibility.Hidden;

            ClearPanel(5);

        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            if (m_security.WindowNewAccess("Reports") == false)
            {
                // Message("Access Denied.");
                return;
            }
            reportsmenu.Visibility = Visibility.Visible;
            customersmenu.Visibility = Visibility.Hidden;
            adminmenu.Visibility = Visibility.Hidden;
            messagemenu.Visibility = Visibility.Hidden;
            giftcardmenu.Visibility = Visibility.Hidden;
            employeemenu.Visibility = Visibility.Hidden;

            ClearPanel(5);
        }
        private void EmployeePortal_Clicked(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("EmployeeMenu"))
            {
                //Message("Access Denied.");
                return;
            }

            employeemenu.Visibility = Visibility.Visible;
            reportsmenu.Visibility = Visibility.Hidden;
            customersmenu.Visibility = Visibility.Hidden;
            adminmenu.Visibility = Visibility.Hidden;
            messagemenu.Visibility = Visibility.Hidden;
            giftcardmenu.Visibility = Visibility.Hidden;
            ClearPanel(5);

        }


        private void Admin_Clicked(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("BackOffice"))
            {
                // Message("Access Denied.");
                return;
            }
            reportsmenu.Visibility = Visibility.Hidden;
            customersmenu.Visibility = Visibility.Hidden;
            adminmenu.Visibility = Visibility.Visible;
            messagemenu.Visibility = Visibility.Hidden;
            giftcardmenu.Visibility = Visibility.Hidden;
            employeemenu.Visibility = Visibility.Hidden;
            ClearPanel(5);
        }



        private void Appointment_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("Appointment"))
            {
                // Message("Access Denied.");
                return;
            }
            AppointmentView dlg = new AppointmentView(m_security);
            Utility.OpenModal(this, dlg);

        }



        private void Exit_Click(object sender, RoutedEventArgs e)
        {

       



            logger.Info("User exited program");
            Application.Current.Shutdown();
            Environment.Exit(0);
        }
        private void Nosale_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("NoSale"))
            {
                // Message("Access Denied.");
                return;
            }

            AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "No Sale", "Drawer Open", "none", 0);

            logger.Info("No Sale Clicked:" + m_security.CurrentEmployee.DisplayName);

            // ReceiptPrinter _printer;
            //  _printer = new ReceiptPrinter(GlobalSettings.Instance.ReceiptPrinter);
            //  _printer.CashDrawer();

            ReceiptPrinter.CashDrawer(GlobalSettings.Instance.ReceiptPrinter);



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

                BackupSelect bk = new BackupSelect(backupdirectory);

                bk.ChosenDrive = "";

                Utility.OpenModal(this, bk);

                if (bk.ChosenDrive != "")
                {
                    DBConnect db = new DBConnect();
                    if (bk.ChosenDrive != backupdirectory) backupdirectory = bk.ChosenDrive + "backup";
                    //"C:\\Program Files\\MySQL\\MySQL Server 5.6\\bin\\mysqldump"
                    db.Backup(backupdirectory, GlobalSettings.Instance.MySQLDumpPath);
                    MessageBox.Show("Backup successful to  " + backupdirectory);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }



        }





        private void TicketHistory_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("TicketHistory"))
            {
                // Message("Access Denied.");
                return;
            }


          //  OrderHistory dlg = new OrderHistory(m_security);
          //  Utility.OpenModal(this, dlg);




        }





        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("Verify"))
            {
                // Message("Access Denied.");
                return;
            }
            GCVerify gcv = new GCVerify();
            Utility.OpenModal(this, gcv);
        }





        //-----------------------------Reports Sub Menu ------------------------------------------
        private void Commission_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("OwnerCommission"))
            {
                //  Message("Access Denied.");
                return;
            }
            OwnerReport dlg = new OwnerReport(m_security);
            Utility.OpenModal(this, dlg);

            AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Open", "Owner Commission", "none", 0);

        }

        private void SalesReport_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("SalesReport"))
            {
                // Message("Access Denied.");
                return;
            }

            SalesReport dlg = new SalesReport();
            Utility.OpenModal(this, dlg);

            AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Open", "Sales Report", "none", 0);
        }



        private void RewardReport_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("RewardReport"))
            {
                //  Message("Access Denied.");
                return;
            }
            RewardReport dlg = new RewardReport();
            Utility.OpenModal(this, dlg);



        }



        //--------------------------- Customers Sub Menu -------------------------------------------------------------


        private void Validate_Click(object sender, RoutedEventArgs e)
        {

            if (!m_security.WindowNewAccess("CustomerView"))
            {
                //  Message("Access Denied.");
                return;
            }

            string action = "";
            Button chosen = sender as Button;
            action = chosen.Tag.ToString().Trim();
            Validate(action);

        }


        private void Validate(string action)
        {
            CustomerModel cust = new CustomerModel();
            int CustomerID = 0;


            if (action == "ID")
            {

                NumPad pad = new NumPad("Enter Customer ID:", true);
                pad.Amount = "";
                Utility.OpenModal(this, pad);

                //if user cancel , returns ""
                if (pad.Amount != "")
                {
                    if (CustomerModel.CheckID(int.Parse(pad.Amount)))
                    {
                        CustomerID = int.Parse(pad.Amount);
                    }
                    else CustomerID = 0;

                }
            }

            if (action == "Phone")
            {

                CustomerPhone pad = new CustomerPhone();
                pad.Amount = "";
                Utility.OpenModal(this, pad);

                //if user cancel , returns ""
                if (pad.Amount != "")
                {
                    DataTable dt = cust.LookupByPhone(pad.Amount);
                    if (dt.Rows.Count == 1)
                    {
                        CustomerID = int.Parse(dt.Rows[0]["id"].ToString());

                    }
                    else
                    {
                        //Display list of names to pick from
                        CustomerFoundList cfl = new CustomerFoundList(dt);
                        cfl.ShowDialog();
                        CustomerID = cfl.CustomerID;
                    }

                }
            }

            if (action == "Name")
            {

                SearchbyName dlg;
                bool stillsearching = true;
                do
                {
                    dlg = new SearchbyName();
                    Utility.OpenModal(this, dlg);

                    //if user cancel , returns ""
                    if (dlg.FirstName != "" || dlg.LastName != "")
                    {

                        DataTable dt;

                        if (dlg.FirstName == "") dt = cust.GetCustomerbyLastName(dlg.LastName);
                        else if (dlg.LastName == "") dt = cust.GetCustomerbyFirstName(dlg.FirstName);
                        else dt = cust.GetCustomerbyNames(dlg.FirstName, dlg.LastName);

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("None Found...");

                        }
                        else
                        {
                            if (dt.Rows.Count == 1)
                            {
                                CustomerID = int.Parse(dt.Rows[0]["id"].ToString());
                                stillsearching = false;
                            }
                            else
                            {
                                //Display list of names to pick from
                                CustomerFoundList cfl = new CustomerFoundList(dt);
                                cfl.ShowDialog();
                                CustomerID = cfl.CustomerID;
                                stillsearching = false;
                            }
                        }


                    }
                    else
                    {
                        stillsearching = false;
                    }
                } while (stillsearching);







            }

            if (action == "All")
            {

                DataTable dt = cust.GetAllCustomer();


                //Display list of names to pick from
                CustomerFoundList cfl = new CustomerFoundList(dt);
                Utility.OpenModal(this, cfl);
                CustomerID = cfl.CustomerID;


            }



            if (CustomerID > 0)
            {
                CustomerView custvw = new CustomerView(m_security, CustomerID);
                Utility.OpenModal(this, custvw);
            }
        }


        //-------------------  GiftCard Sub Menu --------------------------------------

        private void GiftCardManage_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("GiftCardManage"))
            {
                //  Message("Access Denied.");
                return;
            }
            GCManage gcm = new GCManage(m_security);
            Utility.OpenModal(this, gcm);
            AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Open", "Gift Card Manage", "none", 0);

        }

        //--------------------Employee Portal ---------------------------------

        private void btnCommission_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("EmployeeCommission"))
            {
                //  Message("Access Denied.");
                return;
            }
            EmployeeReport rpt = new EmployeeReport(m_security);
            Utility.OpenModal(this, rpt);


        }

        private void EmployeeProfile_Clicked(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("EmployeeView"))
            {
                //  Message("Access Denied.");
                return;
            }
            EmployeeView rpt = new EmployeeView(m_security.CurrentEmployee, false, false);
            Utility.OpenModal(this, rpt);
            AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Open", "Employee Profile", "none", 0);

        }


        //-------------------   Administration Sub Menu -----------------------------------------------



        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("Inventory"))
            {
                //  Message("Access Denied.");
                return;
            }
            CategoryList rpt = new CategoryList(m_security);
            Utility.OpenModal(this, rpt);

            AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Open", "Menu Setup", "none", 0);

        }

        private void Security_Clicked(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("Security"))
            {
                //  Message("Access Denied.");
                return;
            }
            SecurityView dlg = new SecurityView();
            Utility.OpenModal(this, dlg);


        }

        private void License_Clicked(object sender, RoutedEventArgs e)
        {



            LicenseView dlg = new LicenseView();
            Utility.OpenModal(this, dlg);


        }

        private void Send_Clicked(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("SMS"))
            {
                //  Message("Access Denied.");
                return;
            }
            SendSMS sms = new SendSMS();
            Utility.OpenModal(this, sms);

        }
        private void EmployeeList_Clicked(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("EmployeeList"))
            {
                //  Message("Access Denied.");
                return;
            }
            EmployeeList emp = new EmployeeList(null, m_security);
            GlobalSettings.Instance.HomeClicked = false;
            emp._employeelistviewmodel.CanAddEmployeeClicked = true;
          
            Utility.OpenModal(this, emp);

            AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Open", "Employee List", "none", 0);

        }

        private void Database_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("DatabaseView"))
            {
                //  Message("Access Denied.");
                return;
            }
            Admin_Database adm = new Admin_Database();
            Utility.OpenModal(this, adm);

        }




        private void Promotions_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("Promotions"))
            {
                //   Message("Access Denied.");
                return;
            }
            PromotionList adm = new PromotionList();

            Utility.OpenModal(this, adm);

        }

        private void Help_Clicked(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.reddotpos.com/salon/help");
        }




        private void btnCreditCardManager_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("CreditCardManager"))
            {
                // Message("Access Denied.");
                return;
            }


            CreditCardManager dlg = new CreditCardManager(null, m_security.CurrentEmployee);
            Utility.OpenModal(this, dlg);




        }


    }





}
