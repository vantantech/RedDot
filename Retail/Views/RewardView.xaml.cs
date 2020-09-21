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
    /// Interaction logic for RewardView.xaml
    /// </summary>
    public partial class RewardView : Window
    {
        private PaymentViewModel paymentviewmodel;
        public RewardView(Ticket currentticket)
        {
            InitializeComponent();

            paymentviewmodel = new PaymentViewModel(this,currentticket);
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
