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
    /// Interaction logic for Refund.xaml
    /// </summary>
    /// 
  
    public partial class HeartSIPRefund : Window
    {
        private PaymentViewModel paymentviewmodel;
        public HeartSIPRefund(Window parent, Ticket m_ticket, HeartPOS ccp, string transtype, string authcode)
        {
            InitializeComponent();

            paymentviewmodel = new PaymentViewModel(this, m_ticket, ccp, transtype, authcode);
            this.DataContext = paymentviewmodel;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            // tbAmount.Text = tbAmount.Text + button.Content.ToString();
            paymentviewmodel.RefundAmount = paymentviewmodel.RefundAmount + button.Content.ToString();
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            //Clear 
            // tbAmount.Text = "";
            paymentviewmodel.RefundAmount = "";
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
