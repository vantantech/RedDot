using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Shell;
using System.Globalization;
using System.Threading;
using RedDot;
using System.Diagnostics;
using NLog;
using NLog.Targets;
using System.Drawing;
using RedDot.DataManager;
using System.Reflection;
using System.Runtime.Remoting;
using com.clover.remotepay.sdk;
using com.clover.sdk.v3.payments;
using com.clover.remotepay.transport;
using Clover;


namespace RedDot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "RedDotApp"; //use to create single instance rule
        private int counter = 0;
        private bool demo = false;

        bool AppointmentVersion = false;

        // private SalonMainWindow mainwindow;
        private static Logger logger = LogManager.GetCurrentClassLogger();
      
        RemoteScreen m_remotescreen;

 
        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {

         


                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }else
            {

                bool confirmed = Confirm.Ask("Red Dot is already running.  Kill current app and run new instance?");

                if (confirmed)
                {
                    foreach (var process in Process.GetProcessesByName("RedDot"))
                    {
                        var current = Process.GetCurrentProcess();
                        if (process.Id != current.Id) process.Kill();
                    }
                    var application = new App();

                    application.InitializeComponent();
                    application.Run();

                    // Allow single instance code to perform cleanup operations
                    SingleInstance<App>.Cleanup();
                }

            }
        }


   


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {

             


                logger.Info("Killing WebSync");
                //kill websync and restart it
                foreach (var process in Process.GetProcessesByName("WebSync"))
                {
                     process.Kill();
                }


                try
                {
                    logger.Info("Starting WebSync Process from Salon..");
                   Process.Start("WebSync.exe");
                    logger.Info("WebSync Started from Salon Successfully.");
                }catch(Exception ex)
                {
                    logger.Info("WebSync Restart Process:" + ex.Message);
                }






                logger.Info("Loading Salon Application...");


                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                GlobalSettings.Instance.MachineID = LicenseModel.GetMachineID();
                GlobalSettings.Instance.Demo = false;
                GlobalSettings.Instance.DemoLeft = 30;
                logger.Info("Application Started...");


                logger.Info("Testing database connecton");
                GlobalSettings.Instance.Init();//default mode - select correct database driver

                //Check database
                var conn = new DBConnect();
                var res = conn.TestConnection();
                if (res != 1)
                {

                 
                   
                            MessageBox.Show("Click OK to INIT data ");

                            DBConnect db = new DBConnect();


                            //"C:\\Program Files\\MySQL\\MySQL Server 5.6\\bin\\mysqldump"
                            string file = "salon_init.sql";
                           db.Restore(file);
                                MessageBox.Show("Database initialized .. please restart Red Dot");

                        
                  

                    Application.Current.Shutdown();
                    Environment.Exit(0);

                }

                string query1 = "set global sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';";
                conn.Command(query1);

                string query2 = "set session sql_mode='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION';";
                conn.Command(query2);


                if (LicenseModel.CheckLicense())
                {
                    //load program into full mode once license is downloaded
                    CheckLicensePermissions();
                }
                else
                {
                    if(LicenseModel.RequestLicense("Salon"))
                    {
                        //load program into full mode once license is downloaded
                        CheckLicensePermissions();
                    }
                    else
                    {
                        GlobalSettings.Instance.Demo = true;
                        demo = true;
                    }
                }



                if (GlobalSettings.Instance.EnableFingerPrint)
                {
                    //failed to detect finger printing libraries
                    if (!FingerPrint.Testforfingerprint())
                    {
                        TouchMessageBox.Show("Finger Print reader or libraries are missing.");
                        GlobalSettings.Instance.EnableFingerPrint = false;
                    }
                }


                //LIcense check is done so start application window 

                //--------------------------   Open Salon Window ------------------------------------------------------------------------------


                GlobalSettings.Instance.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                GlobalSettings.Instance.DotNetVersion = Utility.Get45or451FromRegistry();

                if (demo) GlobalSettings.Instance.VersionNumber = "DEMO " + GlobalSettings.Instance.VersionNumber;


                m_remotescreen = new RemoteScreen();
                GlobalSettings.Instance.RemoteScreen = m_remotescreen;

                System.Windows.Forms.Screen s1;


                System.Windows.Forms.Screen s0 = System.Windows.Forms.Screen.AllScreens[0]; //main screen
                GlobalSettings.Instance.r0 = s0.WorkingArea;
                Rectangle resolution = s0.Bounds;

                int screencount = System.Windows.Forms.Screen.AllScreens.Count();
                if (screencount > 1 && GlobalSettings.Instance.ShowCustomerScreen)
                {

                    s1 = System.Windows.Forms.Screen.AllScreens[1]; //last screen .. so most likely the customer screen

                    GlobalSettings.Instance.r1 = s1.WorkingArea;

                    m_remotescreen.WindowState = System.Windows.WindowState.Normal;
                    m_remotescreen.WindowStyle = WindowStyle.None;
                    m_remotescreen.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                    m_remotescreen.Top = GlobalSettings.Instance.r1.Top;
                    m_remotescreen.Left = GlobalSettings.Instance.r1.Left;
                    m_remotescreen.Show();
                }


                //System.Windows.Forms.Screen s2 = System.Windows.Forms.Screen.AllScreens[2]; //right screen



                //  System.Drawing.Rectangle r2 = s2.WorkingArea;



                //m_remotescreen.WindowState = System.Windows.WindowState.Maximized;



                GlobalSettings.Instance.RemoteScreen = m_remotescreen;

                Splash spl = new Splash();
                spl.Topmost = true;
                spl.ShowDialog();


                VFD stat = new VFD(GlobalSettings.Instance.DisplayComPort);
                stat.WriteRaw("Welcome", "RedDot POS");


                System.Windows.Threading.DispatcherTimer dispatcherTimer2;

                if (demo) //start demo timer
                {
                    dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
                    dispatcherTimer2.Tick += new EventHandler(dispatcherTimer_Tick2);
                    dispatcherTimer2.Interval = new TimeSpan(0, 1, 0);
                    dispatcherTimer2.Start();
                }


                //find creditcard ip

                //  var res = IpFinder.FindIpAddressByMacAddress("B4-00-16-23-85-1D",IpFinder.GetLocalIPAddress());
                // var res = finder.FindIpAddressByMacAddress("B4-00-16-23-85-1D");

                // TouchMessageBox.Show(res);


              //  AppointmentVersion = true;

                //Loads finger print database into memory


               // GlobalSettings.Instance.LoadAllFmdsUserIDs();


                if(AppointmentVersion)
                {
                      SecurityModel m_security = new SecurityModel();

                    if (!m_security.WindowNewAccess("Appointment"))
                    {
                        // Message("Access Denied.");
                        return;
                    }
                    AppointmentViewLarge dlg = new AppointmentViewLarge(m_security);
                    dlg.ShowDialog();
                    Application.Current.Shutdown();
                    Environment.Exit(0);
                }
                else
                {
                    switch (GlobalSettings.Instance.SalesViewMode.ToUpper())
                    {

                        case "LARGE":
                        case "WIDE":
                            SalonSalesLarge saleswindow = new SalonSalesLarge();

                            saleswindow.Show();
                            break;


                        case "MEDIUM":
                            SalonSalesMedium saleswindowm = new SalonSalesMedium();

                            saleswindowm.Show();
                            break;


                        case "SMALL":
                        case "COMPACT":

                            SalonSalesCompact saleswindow2 = new SalonSalesCompact();
                            saleswindow2.Show();

                            break;

                        case "NORMAL":
                        case "REGULAR":
                        default:
                            SalonMainWindow saleswindow3 = new SalonMainWindow();
                            saleswindow3.Show();
                            break;


                    }
                }

               


            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("App.xaml.cs :Program Error:" + ex.Message);
             
                Application.Current.Shutdown();
                Environment.Exit(0);
            }
                
        

     
        }


        private void CheckLicensePermissions()
        {
            if (LicenseModel.GetSoftware() != "Salon")
            {


                logger.Warn("This license is invalid for RedDot Salon POS...  Exiting.");
                TouchMessageBox.Show("Not licensed for Salon App.  Exiting.");
                Application.Current.Shutdown();
                Environment.Exit(0);



            }

            if (!LicenseModel.CheckPermission("sales"))
                if (LicenseModel.CheckPermission("appointment"))
                {
                    AppointmentVersion = true;
                }


            //need to compare today's date to last login to see if user changed system time to cheat
            //subtract one day just incase it's first time and field just got init

            DateTime lastlogon = GlobalSettings.Instance.LastLogon;

            if (DateTime.Now < lastlogon.AddDays(-1))
            {

                TouchMessageBox.Show("Invalid License Date.  Program will now Exit!!!");
                Application.Current.Shutdown();
                Environment.Exit(0);
            }

            GlobalSettings.Instance.LastLogon = DateTime.Now;

            //check for valid expiration date , today's date is before license start so not valid yet 
            if (DateTime.Now < LicenseModel.GetStartDate())
            {
                logger.Warn("License date starts on :" + LicenseModel.GetStartDate().ToShortDateString());
                TouchMessageBox.Show("License date starts on :" + LicenseModel.GetStartDate().ToShortDateString());
                TouchMessageBox.Show("Date is invalid.  Program will now Exit!!!");
                Application.Current.Shutdown();
                Environment.Exit(0);
            }

            //check for valid expiration date.  Today's date has passed , so license is expired.  Need to warn them 7 days prior.
            if (DateTime.Now > LicenseModel.GetEndDate())
            {
                logger.Warn("License date expired:" + LicenseModel.GetEndDate().ToShortDateString());
                TouchMessageBox.Show("License date expired:" + LicenseModel.GetEndDate().ToShortDateString());
                TouchMessageBox.Show("Date is invalid.  Program will now Exit!!!");
                Application.Current.Shutdown();
                Environment.Exit(0);
            }
            else
            {
                TimeSpan diff = LicenseModel.GetEndDate() - DateTime.Now;
                if (diff.Days < 7) TouchMessageBox.Show("You have less than 1 week on your license.  You must renew ASAP.");
            }

            if (LicenseModel.CheckPermission("pro"))
            {
                GlobalSettings.Instance.ProVersion = Visibility.Visible;
                GlobalSettings.Instance.BaseVersion = Visibility.Visible;
            }
            else
            {
                if (LicenseModel.CheckPermission("base"))
                {
                    GlobalSettings.Instance.ProVersion = Visibility.Collapsed;
                    GlobalSettings.Instance.BaseVersion = Visibility.Visible;
                }

            }
        }

        private void dispatcherTimer_Tick2(object sender, EventArgs e)
        {
            counter++;
            GlobalSettings.Instance.DemoLeft--;

            if (demo && counter > 30)
            {
                TouchMessageBox.Show("Demo period has ended.  Exiting.");
                Application.Current.Shutdown();
                Environment.Exit(0);
            }

            if (counter > 30) counter = 0;
        }








        //-------------------------- Single Instance Code ------------------------------------------------------------
        private void Application_DispatcherUnhandledException(object sender , System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            TouchMessageBox.Show("An unhandled exception: " + e.Exception.Message);
        }


        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        #endregion

    }
}
