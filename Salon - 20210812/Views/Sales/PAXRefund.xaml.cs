using RedDot.ViewModels;
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
    public partial class PAXRefund : Window
    {
        private GlobalPaymentVM gpvm;
        private TriPOSVM triposvm;
        private CardConnectVM cardconnectvm;
        public PAXRefund(Ticket ticket)
        {
            InitializeComponent();

            switch (GlobalSettings.Instance.CreditCardProcessor.ToUpper())
            {
                case "PAX_S300":
                    gpvm = new GlobalPaymentVM(this, ticket, "REFUND", null);
                    this.DataContext = gpvm;
                    break;



                case "WORLDPAY":
                case "VANTIV":
                    triposvm = new TriPOSVM(this,  ticket, "REFUND", null);
                    this.DataContext = triposvm;
                    break;

                case "CARDCONNECT":
                    cardconnectvm = new CardConnectVM(this, ticket, "REFUND", null);
                    this.DataContext = cardconnectvm;
                    break;

            }
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
