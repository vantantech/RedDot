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
using System.Windows.Shapes;
using System.Windows.Threading;
using DataManager;

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
            dispatchTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatchTimer.Interval = new TimeSpan(0, 0, 5);
            dispatchTimer.Start();
            DataBase = dbconnect.DataBase;

            

        }

  
           



        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatchTimer.Stop();

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
