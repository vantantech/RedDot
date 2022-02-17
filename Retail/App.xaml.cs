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
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.IO;


namespace RedDot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

     

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            GlobalSettings.Instance.Demo = !LicenseModel.CheckLicense("Retail");
 




            //Check database
        
            if (!DBConnect.TestConnection())
            {

                MessageBox.Show("Database Connection Fail.  Please Check Database Server.   Program Exiting!!!");
                Application.Current.Shutdown();
                Environment.Exit(0);
            

            }

            //foreach (var process in Process.GetProcessesByName("RedDot"))
            //  {
            // process.Kill();
            //  }


            /*
            if (e.Args.Length > 0)
            {
                if (e.Args[0].ToUpper() == "TRAINING")
                {
                    // GlobalSettings.Instance.ApplicationMode = "training";
                }

            }
            */

            GlobalSettings.Instance.Init();//default mode


            GlobalSettings.Instance.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();



            System.Windows.Forms.Screen s0 = System.Windows.Forms.Screen.AllScreens[0]; //main screen

            Rectangle resolution = s0.Bounds;


            Splash spl = new Splash();
            spl.ShowDialog();

            //Loads finger print database into memory
            GlobalSettings.Instance.LoadAllFmdsUserIDs();


            // GlobalSettings.Instance.ScreenHeight = resolution.Height;
            //  GlobalSettings.Instance.ScreenWidth = resolution.Width;


            if (GlobalSettings.Instance.ScreenHeight < 500 || GlobalSettings.Instance.ScreenHeight > resolution.Height) GlobalSettings.Instance.ScreenHeight = 768;


            if (GlobalSettings.Instance.ScreenWidth < 500 || GlobalSettings.Instance.ScreenWidth > resolution.Height) GlobalSettings.Instance.ScreenWidth = 1024;


          
    
            MainWindow wnd;

            wnd = new MainWindow();
            wnd.ShowDialog();



        }



   






























        //-------------------------- Single Instance Code ------------------------------------------------------------
        private void Application_DispatcherUnhandledException(object sender , System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception: " + e.Exception.Message);
        }





    }
}
