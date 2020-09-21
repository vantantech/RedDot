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
    /// Interaction logic for DeleteConfirm.xaml
    /// </summary>
    public partial class Override : Window
    {
        public string Action = "";
        public string Response = "";
        public static string OK = "Ok";
        public static string OVERRIDE = "Override";
        private int m_timeout;
        DispatcherTimer dispatchTimer = new DispatcherTimer();
        int counter = 0;
        public Override(string message = "", int timeout = 0)
        {
            m_timeout = timeout;
            InitializeComponent();
            if (message != "") tbMessage.Text = message;

            dispatchTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatchTimer.Interval = new TimeSpan(0, 0, 1);

            if (timeout > 0)
                dispatchTimer.Start();

        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            counter++;
            this.tbCounter.Text = counter.ToString();
            if (counter == m_timeout)
            {
                dispatchTimer.Stop();
                Action = "Ok";
                Response = Action;
                this.Close();
            }


        }
        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Button chosen = sender as Button;
            Action = chosen.Tag.ToString().Trim();
            Response = chosen.Tag.ToString().Trim();
            this.Close();
        }

        public static bool Ask(string message,int timeout=0)
        {
            Override conf = new Override(message,timeout)
            {
                Topmost = true,
                ShowInTaskbar = false
            };
            conf.ShowDialog();
            if (conf.Response == Override.OVERRIDE) return true; else return false;

        }
    }
}
