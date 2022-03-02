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
        private VirtualPaymentVM virtualvm;
        private CardConnectVM cardconnectvm;

      

        public CCPPayment(Ticket ticket,SecurityModel security, string transtype, Payment payment, string reason)
        {
        
                InitializeComponent();
     

            switch(GlobalSettings.Instance.CreditCardProcessor.ToUpper() )
            {
                case "PAX_S300":
                gpvm = new GlobalPaymentVM(this,security, ticket, transtype, payment,reason);
                this.DataContext = gpvm;
                    break;

                case "VIRTUAL":
                    virtualvm = new VirtualPaymentVM(this, security, ticket, transtype, payment, reason);
                    this.DataContext = virtualvm;
                    break;

                case "WORLDPAY":
                case "VANTIV":
                triposvm = new TriPOSVM(this,security, ticket, transtype, payment,reason);
                this.DataContext = triposvm;
                    break;
                case "CARDCONNECT":
                    cardconnectvm = new CardConnectVM(this, security, ticket, transtype, payment, reason);
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
