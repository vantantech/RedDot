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
  
        public LineItemActionView(Window parent,string itemtype, LineItem line, OrderType m_ordertype)
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

            if (itemtype.ToLower() == "closed")
            {
                btnDelete.IsEnabled = false;
                btnDiscount.IsEnabled = false;
                btnModifiers.IsEnabled = false;
                btnPriceOverride.IsEnabled = false;
            
                btnQuantity.IsEnabled = false;

            }

           
           // if (line.Modifiers.Count == 0) btnModifiers.IsEnabled = false;

            Product prod = new Product(line.ProductID,m_ordertype);
            if (prod.ModProfileID > 0) btnModifiers.IsEnabled = true; else btnModifiers.IsEnabled = false;
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Button chosen = sender as Button;
            Action = chosen.Tag.ToString().Trim();
            this.Close();
        }


    }
}
