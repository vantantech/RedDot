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
    /// Interaction logic for CreditCardView.xaml
    /// </summary>
    public partial class CreditCardView : Window
    {

        private PaymentViewModel paymentviewmodel;
        public CreditCardView(Window parent, Ticket m_ticket, SecurityModel security, int width)
        {
            InitializeComponent();
            if (parent != null)
            {
                this.Left = parent.Left;
                this.Top = parent.Top;
                this.Width = width;
            }
            else
            {
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }
            paymentviewmodel = new PaymentViewModel(this, security, m_ticket, "", "");
            this.DataContext = paymentviewmodel;
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

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
