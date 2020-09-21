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
    /// Interaction logic for LineItemActionView.xaml
    /// </summary>
    public partial class LineItemActionView : Window
    {

        public string Action;
  
        public LineItemActionView(Window parent,string itemtype, LineItem line)
        {

            InitializeComponent();
            tbitem.Text = line.ReceiptStr;
            


            if (parent != null)
            {
                this.Left = parent.Left;
                this.Top = parent.Top;
            }
            else
            {
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }
            Action = "";
            if (itemtype == "giftcard") btnPickEmployee.Visibility = Visibility.Collapsed;
            if (GlobalSettings.Instance.Shop.Type != "Salon") btnPickEmployee.Visibility = Visibility.Collapsed;
            if (itemtype.ToLower() == "closed")
            {
                btnDelete.IsEnabled = false;
                btnDiscount.IsEnabled = false;
             
                btnPriceOverride.IsEnabled = false;
                btnSurcharge.IsEnabled = false;
                btnQuantity.IsEnabled = false;

            }
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Button chosen = sender as Button;
            Action = chosen.Tag.ToString().Trim();
            this.Close();
        }


    }
}
