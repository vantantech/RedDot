using System;
using System.Collections.Generic;
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
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using RedDot;
using System.Globalization;
using WpfLocalization;
using System.Text.RegularExpressions;
using NLog;
using System.Data;
using RedDot.DataManager;
using System.Net;

namespace RedDot
{
    public partial class SalonSalesLarge : Window
    {


        private SalonSalesVM salesViewModel;

  
        private Point scrollStartPoint;
        private Point scrollStartOffset;
        private double limit;

        private SecurityModel m_security;
        private DBConnect dbConnect;

        private string generic_string = "";
        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        private static Logger logger = LogManager.GetCurrentClassLogger();
     
        public SalonSalesLarge()
        {
            InitializeComponent();
            salesViewModel = new SalonSalesVM(this,0);
            this.DataContext = salesViewModel; //link to Sales View Model

           

            reportsmenu.Visibility = Visibility.Hidden;
            customersmenu.Visibility = Visibility.Hidden;
            adminmenu.Visibility = Visibility.Hidden;
            messagemenu.Visibility = Visibility.Hidden;
            giftcardmenu.Visibility = Visibility.Hidden;
            employeemenu.Visibility = Visibility.Hidden;



            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            m_security = new SecurityModel();
            dbConnect = new DBConnect();
            txtMessage.Text = "Red Dot POS version " + GlobalSettings.Instance.VersionNumber + "  " + (dbConnect.TestConnection() ==1 ? " Connected to:" + dbConnect.DataBase : "Database Connect Error") + " Server:" + GlobalSettings.Instance.DatabaseServer + " Database:" + GlobalSettings.Instance.DatabaseName + " Port:" + GlobalSettings.Instance.PortNo;
          

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


   

        private void ClearPanel(int seconds)
        {

            dispatcherTimer.Interval = new TimeSpan(0, 0, seconds);
            dispatcherTimer.Start();
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

            if(GlobalSettings.Instance.SalesViewMode.ToUpper() == "LARGE")
            {
                AppointmentViewLarge dlg = new AppointmentViewLarge(m_security);
                Utility.OpenModal(this, dlg);
            }
            else
            {
                AppointmentView dlg = new AppointmentView(m_security);
                Utility.OpenModal(this, dlg);
            }

            //need to reload

            salesViewModel.LoadTickets();
        }



        private void Exit_Click(object sender, RoutedEventArgs e)
        {

            if (GlobalSettings.Instance.cloverConnector != null)
            {

                try
                {
                    GlobalSettings.Instance.cloverConnector.Dispose();
                }
                catch (Exception)
                {
                    GlobalSettings.Instance.cloverConnector = null;
                }

            }

            this.Close();



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
                    db.Backup(backupdirectory);
                    TouchMessageBox.Show("Backup successful to  " + backupdirectory);

                }
            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("Error:" + ex.Message);
            }



        }





  

  


        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //Test tst = new Test();
            // tst.ShowDialog();

            // Utility.CreateMachineFile();
            // TouchMessageBox.Show("Machine File Created:");


            // string appname = System.AppDomain.CurrentDomain.FriendlyName;
            // TouchMessageBox.Show(appname);


            // Ticket ticket = new Ticket(1);
            // ticket.PrintReceipt();

        }

   





        //-----------------------------Reports Sub Menu ------------------------------------------
        private void Commission_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("OwnerCommission"))
            {
                //  Message("Access Denied.");
                return;
            }
            CommissionReport dlg = new CommissionReport(m_security);
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

            SalesReportLarge dlg = new SalesReportLarge();
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
                        CustomerFoundList cfl = new CustomerFoundList(dt) { Topmost = true };
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
                            TouchMessageBox.Show("None Found...");

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
                                CustomerFoundList cfl = new CustomerFoundList(dt) { Topmost = true };
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


        private void btnSalesReport_Click(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("EmployeeSales"))
            {
                //  Message("Access Denied.");
                return;
            }
            EmployeeSales rpt = new EmployeeSales(m_security.CurrentEmployee.ID);
            Utility.OpenModal(this, rpt);


        }

        private void EmployeeProfile_Clicked(object sender, RoutedEventArgs e)
        {
            if (!m_security.WindowNewAccess("EmployeeView"))
            {
                //  Message("Access Denied.");
                return;
            }
            EmployeeView rpt = new EmployeeView(m_security, m_security.CurrentEmployee, false, false);
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


































        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (ScrollViewer0.IsMouseOver)
            {
                // Save starting point, used later when determining 
                //how much to scroll.
                scrollStartPoint = e.GetPosition(this);
                scrollStartOffset.X = ScrollViewer0.HorizontalOffset;
                scrollStartOffset.Y = ScrollViewer0.VerticalOffset;
                limit = ScrollViewer0.ViewportWidth;


                // Update the cursor if can scroll or not.
                this.Cursor = (ScrollViewer0.ExtentWidth >
                   ScrollViewer0.ViewportWidth) ||
                    (ScrollViewer0.ExtentHeight >
                    ScrollViewer0.ViewportHeight) ?
                   Cursors.ScrollAll : Cursors.Arrow;

                //this.CaptureMouse();
            }

            if (ScrollViewer1.IsMouseOver)
            {
                // Save starting point, used later when determining 
                //how much to scroll.
                scrollStartPoint = e.GetPosition(this);
                scrollStartOffset.X = ScrollViewer1.HorizontalOffset;
                scrollStartOffset.Y = ScrollViewer1.VerticalOffset;
                limit = ScrollViewer1.ViewportWidth;


                // Update the cursor if can scroll or not.
                this.Cursor = (ScrollViewer1.ExtentWidth >
                   ScrollViewer1.ViewportWidth) ||
                    (ScrollViewer1.ExtentHeight >
                    ScrollViewer1.ViewportHeight) ?
                   Cursors.ScrollAll : Cursors.Arrow;

                //this.CaptureMouse();
            }





            base.OnPreviewMouseDown(e);
        }


        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {




            //start of generic product processing
            if (e.Text == "\r")
            {
                if (generic_string.Length == 12)
                {
                    salesViewModel.AddItemByBarCode(generic_string);
                }
                generic_string = "";
               
            }
            else
                generic_string += e.Text;




        }


        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.Cursor == System.Windows.Input.Cursors.ScrollAll)
            {
                if (ScrollViewer0.IsMouseOver)
                {
                    // Get the new scroll position.
                    Point point = e.GetPosition(this);

                    // Determine the new amount to scroll.
                    Point delta = new Point(
                        (point.X > this.scrollStartPoint.X) ?
                            -(point.X - this.scrollStartPoint.X) :
                            (this.scrollStartPoint.X - point.X),

                        (point.Y > this.scrollStartPoint.Y) ?
                            -(point.Y - this.scrollStartPoint.Y) :
                            (this.scrollStartPoint.Y - point.Y));

                    // Scroll to the new position

                    // ScrollViewer1.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
                    if (point.X < limit)
                        ScrollViewer0.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
                }



                if (ScrollViewer1.IsMouseOver)
                {
                    // Get the new scroll position.
                    Point point = e.GetPosition(this);

                    // Determine the new amount to scroll.
                    Point delta = new Point(
                        (point.X > this.scrollStartPoint.X) ?
                            -(point.X - this.scrollStartPoint.X) :
                            (this.scrollStartPoint.X - point.X),

                        (point.Y > this.scrollStartPoint.Y) ?
                            -(point.Y - this.scrollStartPoint.Y) :
                            (this.scrollStartPoint.Y - point.Y));

                    // Scroll to the new position

                    // ScrollViewer1.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
                    if (point.X < limit)
                        ScrollViewer1.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
                }
            }




            base.OnPreviewMouseMove(e);
        }


        private void GetNewPosition(ScrollViewer scrollviewer, MouseEventArgs e)
        {
            // Get the new scroll position.
            Point point = e.GetPosition(this);

            // Determine the new amount to scroll.
            Point delta = new Point(
                (point.X > this.scrollStartPoint.X) ?
                    -(point.X - this.scrollStartPoint.X) :
                    (this.scrollStartPoint.X - point.X),

                (point.Y > this.scrollStartPoint.Y) ?
                    -(point.Y - this.scrollStartPoint.Y) :
                    (this.scrollStartPoint.Y - point.Y));

            // Scroll to the new position.
            // ScrollViewer2.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
            if (point.X < limit) scrollviewer.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);

        }
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Arrow;
            this.ReleaseMouseCapture();

            base.OnPreviewMouseUp(e);
        }

        private void ScrollDown_Click(object sender, RoutedEventArgs e)
        {
            // ScrollViewer1.PageDown();
            ScrollViewer1.ScrollToVerticalOffset(ScrollViewer1.VerticalOffset + 20);
        }

        private void ScrollUp_Click(object sender, RoutedEventArgs e)
        {
            // ScrollViewer1.PageUp();
            ScrollViewer1.ScrollToVerticalOffset(ScrollViewer1.VerticalOffset - 20);
        }


        private void btnDownloadFile_Click(object sender, RoutedEventArgs eventArgs)
        {


            WebClient Client = new WebClient();
            Client.DownloadFile("http://salon.reddotpos.com/update/reddot.exe", @"C:\reddot\update\reddot.exe");
            Client.DownloadFile("http://salon.reddotpos.com/update/datamanager.dll", @"C:\reddot\update\datamanager.dll");
        }

    }
}
