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
using System.Diagnostics;
using NLog;



namespace WebSync
{

    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "WebSync"; //use to create single instance rule
        private System.Windows.Forms.NotifyIcon _notifyIcon;
       
        DateTime lastchecked;
        DateTime lastsynced;
        DateTime lastdailysynced;

        private string websynctime;
        private bool busy = false;
        SalonWebClient m_webclient;
        MainWindow mainwindow;
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
            }
            else
            {

                Confirm conf = new Confirm("Web Sync is already running.  Kill current app and run new instance?");
                conf.ShowDialog();
                if (conf.Action.ToUpper() == "YES")
                {
                    foreach (var process in Process.GetProcessesByName("WebSync"))
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
           
            mainwindow= new MainWindow();
            logger.Debug("app started");
           // MainWindow.Closing += MainWindow_Closing;

            _notifyIcon = new System.Windows.Forms.NotifyIcon();

            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();

            _notifyIcon.MouseClick += (s,args) =>NotifyIcon1_Click(s,args);


           // _notifyIcon.Icon = new System.Drawing.Icon("/media/websync.ico");
            _notifyIcon.Icon = WebSync.Properties.Resources.websync;
            _notifyIcon.Visible = true;

            CreateContextMenu();

            websynctime = GlobalSettings.Instance.WebSyncUpdateTime;
            int timer_interval = GlobalSettings.Instance.WebSyncCheckInterval;
            mainwindow.BottomMessage = "Check Interval =" + timer_interval.ToString();
         

            m_webclient = new SalonWebClient();

         
                System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0,timer_interval , 0);
                dispatcherTimer.Start();
         
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            try
            {
               
                    if (busy) return;

                    lastchecked = DateTime.Now;
                    int WebUserId = GlobalSettings.Instance.WebUserID;

                //do daily sync here
                    if((DateTime.Now - lastdailysynced).TotalHours > 24 )
                    {

                       //employee sync
                       try
                        {
                            m_webclient.SyncEmployees(WebUserId);
                            lastdailysynced = DateTime.Now;
                            logger.Info("Employee Sync Successfully.");
                        }
                        catch(Exception ex)
                        {
                            logger.Error(ex.Message);
                        }



                    //backup
                    DBBackup();


                    }




                   var conn = new DBConnect();
                   if (conn.TestConnection() == false)
                   {
                       mainwindow.BottomMessage = "Last Checked:" + lastchecked.ToString() +  " = > Data Connection Failed";
                       logger.Info("Fail to connect to database");
                       return;
                   }


                   mainwindow.BottomMessage = "Last Checked:" + lastchecked.ToString() + "   Last Synced:" + lastsynced.ToString();

                    busy = true;
                  

                    if (websynctime != "") //if not blank , then test for time match
                    {
                        DateTime syncstarttime = Convert.ToDateTime(websynctime);
                        DateTime syncendtime = syncstarttime.AddMinutes(10);
                        if (DateTime.Now < syncstarttime || DateTime.Now > syncendtime) return;  //does not match .. exit
                    }



                    if (WebUserId < 1) return;  // if invalid webuserid then exit


                    //start web sync

                    History m_history = new History();

                   // DateTime StartDate = DateTime.Today;
                   // DateTime EndDate = DateTime.Today;
                    DateTime starttime = DateTime.Now;

                    DataTable SyncList = m_history.GetAutoSyncList();
                    MainWindowVM mw = (MainWindowVM)mainwindow.DataContext;
                    mw.LoadHistory();


                    if (SyncList.Rows.Count > 0)
                    {
                        lastsynced = DateTime.Now;
                    logger.Info("Sync tickets:" + SyncList.Rows.Count + " tickets found.");

                        foreach (DataRow item in SyncList.Rows)
                        {
                                try
                                {
                                    int salesid = int.Parse(item["id"].ToString());
                                    m_webclient.SyncTicket(WebUserId, salesid);

                                    Thread.Sleep(500);
                                }catch(Exception ex)
                                {
                                    logger.Error("Error syncing tickets:" + item["id"].ToString() + " => "  + ex.Message);
                                }
                        }
                            

                    }

                    TimeSpan delta = DateTime.Now - starttime;

                    mainwindow.BottomMessage = "Last Checked:" + lastchecked.ToString() + "   Last Synced:" + lastsynced.ToString() + " ProcessTime:" + Math.Round(delta.TotalMinutes,2) + " Minutes";
                    logger.Info("Last Checked:" + lastchecked.ToString() + "   Last Synced:" + lastsynced.ToString() + " ProcessTime:" + Math.Round(delta.TotalMinutes, 2) + " Minutes");

                    busy = false;
               

            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                busy = false;
                return;
            }



        }


        private void DBBackup()
        {
            try
            {
                string backupdirectory = GlobalSettings.Instance.BackupDirectory;

              

             
                    DBConnect db = new DBConnect();
                 
                    //"C:\\Program Files\\MySQL\\MySQL Server 5.6\\bin\\mysqldump"
                    db.Backup(backupdirectory);
                    logger.Info("Backup successful to  " + backupdirectory);

            }
            catch (Exception ex)
            {
                logger.Error("Error:" + ex.Message);
            }



        }


        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
              new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Open").Click += (s, e) => ShowMainWindow();
           // _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            logger.Info("exit application");
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        private void NotifyIcon1_Click(object sender, System.Windows.Forms.MouseEventArgs e)
        {
           if(e.Button == System.Windows.Forms.MouseButtons.Left)
           {
               ShowMainWindow();
           }

        }


        private void ShowMainWindow()
        {
            if (mainwindow.IsVisible)
            {
                if (mainwindow.WindowState == WindowState.Minimized)
                {
                    mainwindow.WindowState = WindowState.Normal;
                }
                mainwindow.Activate();
            }
            else
            {
                mainwindow.Show();
            }
        }



        //-------------------------- Single Instance Code ------------------------------------------------------------
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
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

