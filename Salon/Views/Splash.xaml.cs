using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Clover;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using RedDot;
using RedDot.DataManager;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        DispatcherTimer dispatchTimer = new DispatcherTimer();

        public string VersionNumber { get; set; }
        public string DataBase { get; set; }

        public string Message { get; set; }

        public bool CheckDone { get; set; }
        public Splash()
        {
            InitializeComponent();
            this.DataContext = this;
            CheckDone = false;
            VersionNumber = GlobalSettings.Instance.VersionNumber;

            DBConnect dbconnect = new DBConnect();
     
            DataBase = dbconnect.DataBase;


            tbMessage.Text = "Looking for Processor..." + (char)13 + (char)10;

            dispatchTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatchTimer.Interval = new TimeSpan(0, 0, 2);
            dispatchTimer.Start();

        }






        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatchTimer.Stop();


            switch (GlobalSettings.Instance.CreditCardProcessor)
            {
                case "PAX_S300":
                    tbMessage.Text = "Looking for PAX S300 ..." + GlobalSettings.Instance.SIPDefaultIPAddress + (char)13 + (char)10;
                    break;

                case "Clover":

                    tbMessage.Text = "Looking for Clover ..." + (char)13 + (char)10;
                   

                    const String APPLICATION_ID = "RedDot Salon:1.1.2";
                    ICloverConnector cloverConnector;
                    CloverDeviceConfiguration USBConfig = new USBCloverDeviceConfiguration("__deviceID__", APPLICATION_ID, false, 1);


                    CloverListener ccl;

                    //initialize device
                    cloverConnector = CloverConnectorFactory.createICloverConnector(USBConfig);
                    ccl = new CloverListener(cloverConnector);
                    cloverConnector.AddCloverConnectorListener(ccl);
                    cloverConnector.InitializeConnection();

                    int retries = 0;

                    while (!ccl.deviceReady && retries < 10)
                    {
                        Thread.Sleep(1000);
                        retries++;
                        if (retries == 10)
                        {
                            TouchMessageBox.ShowSmall("Connection to Clover via USB Timed Out!!!");

                        }
                    }

                    GlobalSettings.Instance.cloverConnector = cloverConnector;
                    GlobalSettings.Instance.ccl = ccl;

                    if (ccl.deviceReady) TouchMessageBox.ShowSmall("Clover Device Ready.");

                    break;
            }


            // Message = "Checking for updates...";

            //since it's a licensed copy , it will now update
            // var updater = new Updater();

            // if(updater.Check())
            // {
            //  updater.Update();
            //  }

            CheckDone = true;

            this.Close();

        }


        private void move(object sender, MouseButtonEventArgs e)
        {

            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if(CheckDone) this.Close();
        }



 

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckDone) this.Close();
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            if (CheckDone) this.Close();
        }
    }
}
