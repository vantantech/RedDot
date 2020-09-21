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
    /// Interaction logic for CloverPayment.xaml
    /// </summary>
    public partial class CloverPayment : Window
    {
        private CloverVM clovervm; 
        public CloverPayment(Ticket ticket,SecurityModel security, string transtype, Payment payment,string reason)
        {
            InitializeComponent();

            clovervm = new CloverVM(this,security,ticket,transtype,payment,reason);
            this.DataContext = clovervm;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            tbAmount.Text = tbAmount.Text + button.Content.ToString();
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            //Clear 
            tbAmount.Text = "";
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
