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
    /// Interaction logic for PAXPayment.xaml
    /// </summary>
    public partial class CCPPayment : Window
    {

        private GlobalPaymentVM gpvm;
        private TriPOSVM triposvm;
        private PAXVM paxvm;
        private CardConnectVM cardconnectvm;
        public CCPPayment(Ticket ticket, string transtype, Payment payment)
        {
            InitializeComponent();


            switch(GlobalSettings.Instance.CreditCardProcessor.ToUpper() )
            {
                case "PAX_S300":
                case "GLOBALPAYMENT":
                    gpvm = new GlobalPaymentVM(this, ticket, transtype, payment);
                this.DataContext = gpvm;
                    break;

                case "PAX":
                    paxvm = new PAXVM(this, ticket, transtype, payment);
                    this.DataContext = paxvm;
                    break;

                case "WORLDPAY":
                case "VANTIV":
                    triposvm = new TriPOSVM(this, ticket, transtype, payment);
                    this.DataContext = triposvm;
                    break;

                case "CARDCONNECT":
                    cardconnectvm = new CardConnectVM(this, ticket, transtype, payment);
                    this.DataContext = cardconnectvm;
                    break;
            }


           
   
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
