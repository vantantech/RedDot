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

namespace RedDot
{
    /// <summary>
    /// Interaction logic for TouchMessageBox.xaml
    /// </summary>
    public partial class TouchMessageBox : Window
    {
        DispatcherTimer dispatchTimer = new DispatcherTimer();
        int counter = 0;

        private string _message;
        public string Message {

            get { return _message; }
            set { _message = value; tbMessage.Text = value; }
        }
        public TouchMessageBox(string message, int timeout = 0)
        {
            InitializeComponent();
            _message = message;
            counter = timeout;
            tbMessage.Text = message;

            dispatchTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatchTimer.Interval = new TimeSpan(0, 0, 1);
            this.tbCounter.Text = "";

            if (timeout > 0)
                dispatchTimer.Start();

        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            counter--;
            this.tbCounter.Text = counter.ToString();
            if (counter <= 0)
            {
                dispatchTimer.Stop();
      
                this.Close();
            }


        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void Show(string message,int timeout=10)
        {
            TouchMessageBox msg = new TouchMessageBox(message,timeout)
            {
                Width = 800,
                Height = 500,
                Topmost = true,
                ShowInTaskbar = false
            };
            msg.ShowDialog();
        }

        public static void ShowSmall(string message, int timeout=0)
        {
            TouchMessageBox msg = new TouchMessageBox(message,timeout)
            {
                Width = 400,
                Height = 300,
                Topmost = true,
                ShowInTaskbar = false
            };
            msg.ShowDialog();
        }
    }
}
