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
    /// Interaction logic for CreditCardManager.xaml
    /// </summary>
    public partial class CreditCardManager : Window
    {
        CreditCardManagerVM creditcardmanagervm; 
        public CreditCardManager(HeartPOS ccp, Employee currentemployee)
        {
            InitializeComponent();
            creditcardmanagervm = new CreditCardManagerVM(this,ccp,currentemployee);
            this.DataContext = creditcardmanagervm;

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









    }
}
