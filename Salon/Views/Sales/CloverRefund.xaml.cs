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
    /// Interaction logic for PAXRefund.xaml
    /// </summary>
    public partial class CloverRefund : Window
    {
        private CloverVM gpvm;
        public CloverRefund(Ticket ticket, SecurityModel security)
        {
            InitializeComponent();

            gpvm = new CloverVM(this,security, ticket, "REFUND", null,"");
            this.DataContext = gpvm;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            // tbAmount.Text = tbAmount.Text + button.Content.ToString();
            gpvm.RefundAmount = gpvm.RefundAmount + button.Content.ToString();
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            //Clear 
            // tbAmount.Text = "";
            gpvm.RefundAmount = "";
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
