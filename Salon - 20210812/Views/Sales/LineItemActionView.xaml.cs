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

namespace RedDot
{
    /// <summary>
    /// Interaction logic for LineItemActionView.xaml
    /// </summary>
    public partial class LineItemActionView : Window
    {

        public string Action;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        public LineItemActionView(Window parent, string itemtype, LineItem line)
        {

            InitializeComponent();
            tbitem.Text = line.ReceiptStr;

       
          
      
            Action = "";


            if (itemtype == "REVERSEDGIFTCARD" )
            {
                btnDelete.IsEnabled = false;
                btnDiscount.IsEnabled = false;
                btnUpgrade.IsEnabled = false;
                btnDiscount2.IsEnabled = false;
                btnUpgrade2.IsEnabled = false;
                btnPriceOverride.IsEnabled = true;
            }


       
            if (itemtype.ToLower() == "closed")
            {
                btnDelete.IsEnabled = false;
                btnDiscount.IsEnabled = false;
                btnUpgrade.IsEnabled = false;
                btnDiscount2.IsEnabled = false;
                btnUpgrade2.IsEnabled = false;
                btnPriceOverride.IsEnabled = false;
           
             

            }


            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            dispatcherTimer.Interval = new TimeSpan(0, 5, 3);
            dispatcherTimer.Start();

        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            Action = "";
            this.Close();
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Button chosen = sender as Button;
            Action = chosen.Tag.ToString().Trim();
            this.Close();
        }


    }
}
