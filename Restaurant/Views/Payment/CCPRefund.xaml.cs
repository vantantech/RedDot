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
    public partial class CCPRefund : Window
    {
        private GlobalPaymentVM gpvm;
        private TriPOSVM triposvm;
        private CardConnectVM cardconnectvm;

        public CCPRefund(Ticket ticket, SecurityModel security)
        {
            InitializeComponent();

           
            switch (GlobalSettings.Instance.CreditCardProcessor.ToUpper())
            {
                case "PAX_S300":
                    gpvm = new GlobalPaymentVM(this,security, ticket, "REFUND", null,"");
                    this.DataContext = gpvm;
                    break;



                case "WORLDPAY":
                case "VANTIV":
                    triposvm = new TriPOSVM(this,security, ticket, "REFUND", null,"");
                    this.DataContext = triposvm;
                    break;
                case "CARDCONNECT":
                    cardconnectvm = new CardConnectVM(this,security, ticket, "REFUND", null,"");
                    this.DataContext = cardconnectvm;
                    break;

            }
        }

  

  

        private void BackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
