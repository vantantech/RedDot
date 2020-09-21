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
      
       // private SalonMainWindow mainwindow;
        private static Logger logger = LogManager.GetCurrentClassLogger();
      
 

 
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


   

        private void dispatcherTimer_Tick2(object sender, EventArgs e)
        {
            counter++;
            GlobalSettings.Instance.DemoLeft--;
       
          if(demo && counter > 30)
          {
              TouchMessageBox.Show("Demo period has ended.  Exiting.");
              Application.Current.Shutdown();
              Environment.Exit(0);
          }

            if (counter > 30) counter = 0;
        }

        private bool CheckLicense()
        {
            try
            {
                string license = LicenseModel.GetLicense();
                string fingerprint = FingerPrint.Value();

                if(license == fingerprint)
                {
                    return true;
                }else
                {
                   // TouchMessageBox.Show("Error:License is not for this machine!!");
                    LicenseModel.CreateMachineFile();
                  //  TouchMessageBox.Show("License Request File has been created in your Red Dot folder");
                    return false;
                }

            }catch
            {
           
                return false;
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
                GlobalSettings.Instance.MachineID = FingerPrint.Value();
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
                            string file = "C:\\reddot\\salon_init.sql";
                           db.Restore(file);
                                MessageBox.Show("Database initialized .. please restart Red Dot");

                        
                  

                    Application.Current.Shutdown();
                    Environment.Exit(0);

                }

                DateTime lastlogon = GlobalSettings.Instance.LastLogon;


                if (CheckLicense())
                {
                    if (!LicenseModel.CheckPermission("salon"))
                    {
                     
                            logger.Warn("This license is invalid for RedDot Salon POS...  Exiting.");
                            TouchMessageBox.Show("Not licensed for Salon App.  Exiting.");
                            Application.Current.Shutdown();
                            Environment.Exit(0);
                    
                    }

                    //need to compare today's date to last login to see if user changed system time to cheat
                    //subtract one day just incase it's first time and field just got init
                    if (DateTime.Now < lastlogon.AddDays(-1))
                    {
                       
                        TouchMessageBox.Show("Invalid License Date.  Program will now Exit!!!");
                        Application.Current.Shutdown();
                        Environment.Exit(0);
                    }

                    GlobalSettings.Instance.LastLogon = DateTime.Now;

                    //check for valid expiration date , today's date is before license start so not valid yet 
                    if(DateTime.Now < LicenseModel.GetStartDate())
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
                    }else
                    {
                        TimeSpan diff =  LicenseModel.GetEndDate() - DateTime.Now;
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
                        else
                        {
                            GlobalSettings.Instance.ProVersion = Visibility.Collapsed;
                            GlobalSettings.Instance.BaseVersion = Visibility.Collapsed;
                        }
                    }


                }
                else
                {
                    logger.Warn("License invalid .. demo mode only.");
                    LicenseModel.CreateMachineFile();

                    GlobalSettings.Instance.Demo = true;
                    demo = true;
                }


                //LIcense check is done so start application window 

                //--------------------------   Open Salon Window ------------------------------------------------------------------------------


                GlobalSettings.Instance.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                GlobalSettings.Instance.DotNetVersion = Utility.Get45or451FromRegistry();


                Splash spl = new Splash();
                spl.Topmost = true;
                spl.ShowDialog();



                System.Windows.Threading.DispatcherTimer dispatcherTimer2;



                        SalonMainWindow saleswindow3 = new SalonMainWindow();
                        saleswindow3.Show();





            }
            catch (Exception ex)
            {
                TouchMessageBox.Show("App.xaml.cs :Program Error:" + ex.Message);
             
                Application.Current.Shutdown();
                Environment.Exit(0);
            }
                
        

     
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
