using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Shell;
//using Itenso.Configuration;
using System.Globalization;
using System.Threading;
using RedDot;
using NLog;
using System.Drawing;
using DPUruNet;
using System.Diagnostics;
using Microsoft.Owin.Hosting;
using RedDot.Class;
using GlobalPayments.Api.Terminals.Abstractions;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Terminals;
using TriPOS.ResponseModels;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "RedDot-Restaurant"; //use to create single instance rule
        private bool demo = false;
        private string batchtime = "";
        private DateTime lastbatchdate = DateTime.MinValue;
        private int counter = 0;
        private bool busy = false;
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

                if(confirmed)
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



        private bool CheckLicense()
        {
            try
            {
                string license = LicenseModel.GetLicense();
                string fingerprint = FingerPrint.Value();

                if (license == fingerprint)
                {
                    return true;
                }
                else
                {
                   // MessageBox.Show("Error:License is not for this machine!!");
                    LicenseModel.CreateMachineFile();
                   // MessageBox.Show("License Request File has been created in your Red Dot folder");
                    return false;
                }

            }
            catch (Exception ex)
            {
               // MessageBox.Show("Error:License File Not Found..." + ex.Message);
                LicenseModel.CreateMachineFile();
               // MessageBox.Show("License Request File has been created in your Red Dot folder");
                return false;
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (batchtime != "") //if not blank , then test for time match
            {
                if (busy) return;
                DateTime batchstarttime = Convert.ToDateTime(batchtime);
                DateTime batchendtime = batchstarttime.AddMinutes(5);
                if (DateTime.Now < batchstarttime || DateTime.Now > batchendtime) return;  //does not match .. exit

                busy = true;

                if (lastbatchdate.ToShortDateString() == DateTime.Now.ToShortDateString()) return;

                // run batch
                lastbatchdate = DateTime.Now;

                switch (GlobalSettings.Instance.CreditCardProcessor.ToUpper())
                {
                    case "PAX_S300":
                        try
                        {

                            string POSMessage = "";


                            IDeviceInterface _device;

                            _device = DeviceService.Create(new ConnectionConfig
                            {
                                DeviceType = DeviceType.PAX_S300,
                                ConnectionMode = ConnectionModes.TCP_IP,
                                IpAddress = GlobalSettings.Instance.SIPDefaultIPAddress,
                                Port = GlobalSettings.Instance.SIPPort,
                                Timeout = 30000
                            });

                            //initialize logging event
                            _device.OnMessageSent += (message) =>
                            {
                                logger.Info("SENT:" + message);
                            };


                            IBatchCloseResponse resp = _device.BatchClose();


                            POSMessage = "Response:" + resp.DeviceResponseText + (char)13 + (char)10;
                            if (resp.DeviceResponseText == "OK")
                            {
                                var count = resp.TotalCount.Split('=');
                                var amount = resp.TotalAmount.Split('=');

                                POSMessage += "Credit Count:" + count[0] + (char)13 + (char)10;
                                POSMessage += "Credit Amount:" + Math.Round(int.Parse(amount[0]) / 100m, 2) + (char)13 + (char)10;
                                POSMessage += "Debit Count:" + count[1] + (char)13 + (char)10;
                                POSMessage += "Debit Amount:" + Math.Round(int.Parse(amount[1]) / 100m, 2) + (char)13 + (char)10;


                            }


                            logger.Info("BatchClose:RECEIVED:" + resp.ToString());
                            logger.Info("Status:" + resp.DeviceResponseText + "  BatchNumber:" + resp.SequenceNumber);
                            logger.Info("Total Count:" + resp.TotalCount + "  Total Amount:" + resp.TotalAmount);

                            logger.Info(POSMessage);

                            ReceiptPrinterModel.PrintResponse("Batch Close", POSMessage);

                         


                        }
                        catch (Exception ex)
                        {
         
                            logger.Error("Error Closing Batch:" + ex.Message);

                        }
                        break;


                    case "CLOVER":



                        break;

                    case "VANTIV":
                        TriPOSModel tripos = new TriPOSModel(GlobalSettings.Instance.LaneId);

                        BatchCloseResponse result = tripos.ExecuteBatch();
                        if (result == null)
                        {
                            logger.Error("Error Sending Batch Command.");
                            return;
                        }
                        if (result.ExpResponse.ExpressResponseCode == 0)
                        {
                            string message = "Credit Count:" + result.ExpResponse.ExpBatch.HostBatchCount + " Credit Amount:" + result.ExpResponse.ExpBatch.HostBatchAmount + "\r\n" +
                                "Refund Count:" + result.ExpResponse.ExpBatch.HostCreditReturnCount + " Refund Amount:" + result.ExpResponse.ExpBatch.HostCreditReturnAmount + "\r\n" +
                                "Total Count:" + result.ExpResponse.ExpBatch.HostBatchCount + " Total Amount:" + result.ExpResponse.ExpBatch.HostBatchAmount;
                            ReceiptPrinterModel.PrintResponse("Batch Close", message);
                            logger.Info("Batch Close:\r\n" + message);
                        }
                        else
                        {
                            logger.Error("Batch Close:\r\n" + result.ExpResponse.ExpressResponseMessage);
                        }

                        break;
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


        private void Application_Startup(object sender, StartupEventArgs e)
        {

            logger.Info("Loading Application...");

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            GlobalSettings.Instance.Demo = false;
            GlobalSettings.Instance.DemoLeft = 30;
            logger.Info("Application Started...");
         


            //Check database
            var conn = new DBConnect();
            if (conn.TestConnection())
            {
                GlobalSettings.Instance.Init();//default mode
                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                if (e.Args.Length > 0)
                {
                    if (e.Args[0].ToUpper() == "TRAINING")
                    {
                        // GlobalSettings.Instance.ApplicationMode = "training";
                    }

                }

            }
            else
            {

                MessageBox.Show("Database Connection Fail.  Please Check Database Server.   Program Exiting!!!");
                Application.Current.Shutdown();
                Environment.Exit(0);
            }


            if (CheckLicense())
            {

                if (!LicenseModel.CheckPermission("restaurant"))
                {
                    MessageBox.Show("Not licensed for Restaurant.  Exiting.");
                    Application.Current.Shutdown();
                    Environment.Exit(0);

                }

              
            }
            else
            {
              //  logger.Warn("License invalid .. demo mode only.");

                GlobalSettings.Instance.Demo = true;
                demo = true;
            }

            GlobalSettings.Instance.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (demo) GlobalSettings.Instance.VersionNumber = "DEMO " + GlobalSettings.Instance.VersionNumber;

            System.Windows.Threading.DispatcherTimer dispatcherTimer;
            System.Windows.Threading.DispatcherTimer dispatcherTimer2;

            if (demo) //start demo timer
            {
                dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer2.Tick += new EventHandler(dispatcherTimer_Tick2);
                dispatcherTimer2.Interval = new TimeSpan(0, 1, 0);
                dispatcherTimer2.Start();
            }


            batchtime = GlobalSettings.Instance.BatchTime;

            if (batchtime != "") //start demo timer
            {
                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
                dispatcherTimer.Start();
            }

            Splash spl = new Splash();
            spl.ShowDialog();



            //query items so it will create
            var init = GlobalSettings.Instance.WalkInMustPayFirst;
            init = GlobalSettings.Instance.WalkInAskCustomerPhone;
            init = GlobalSettings.Instance.WalkInAskForCustomerName;

            init = GlobalSettings.Instance.CallInMustPayFirst;
            init = GlobalSettings.Instance.CallInAskCustomerPhone;
            init = GlobalSettings.Instance.CallInAskForCustomerName;
            init = GlobalSettings.Instance.CallInCustomerInfoRequired;




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
                m_remotescreen.Top = GlobalSettings.Instance.r1.Top ;
                m_remotescreen.Left = GlobalSettings.Instance.r1.Left ;
                m_remotescreen.Show();
            }


            //System.Windows.Forms.Screen s2 = System.Windows.Forms.Screen.AllScreens[2]; //right screen



            //  System.Drawing.Rectangle r2 = s2.WorkingArea;



            //m_remotescreen.WindowState = System.Windows.WindowState.Maximized;

    
            //Loads finger print database into memory
            GlobalSettings.Instance.LoadAllFmdsUserIDs();

// for tablet ordering
            RunWebService();
   

            // TriPOSModel triposmodel = new TriPOSModel(GlobalSettings.Instance.LaneId);
            //  triposmodel.Reboot();

            MainWindow wnd;

            wnd = new MainWindow();
            if(GlobalSettings.Instance.DebugMode == false)
            {
                wnd.WindowStartupLocation = WindowStartupLocation.Manual;
                wnd.Top = s0.WorkingArea.Top;
                wnd.Left = s0.WorkingArea.Left;
            }else
            {
                wnd.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            wnd.Show();


        }

       



        private void RunWebService()
        {
            try
            {
                if (GlobalSettings.Instance.EnableWebService)
                {
                    string baseUri = GlobalSettings.Instance.WebApiLocalAddress + ":" + GlobalSettings.Instance.WebApiLocalPort;
                    WebApp.Start<Startup>(baseUri);
                }
            }catch(Exception ex)
            {
                string baseUri = GlobalSettings.Instance.WebApiLocalAddress + ":" + GlobalSettings.Instance.WebApiLocalPort;
                TouchMessageBox.Show("Web service Error:" + baseUri + ":" +  ex.Message,30);
            }
        }
























        //-------------------------- Single Instance Code ------------------------------------------------------------
        private void Application_DispatcherUnhandledException(object sender , System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception: " + e.Exception.Message);
        }


        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        #endregion

    }
}
