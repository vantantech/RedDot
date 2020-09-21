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
    /// Interaction logic for PickProduct.xaml
    /// </summary>
    public partial class PickProduct : Window
    {
        public int ProductID = 0;
        PickProductVM pickproductvm;
        public PickProduct()
        {
            InitializeComponent();
            pickproductvm = new PickProductVM();

            this.DataContext = pickproductvm;
        }

        private void ProductClicked(object sender, RoutedEventArgs e)
        {
            Button picked = sender as Button;
            ProductID = int.Parse(picked.Tag.ToString());
            this.Close();
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
