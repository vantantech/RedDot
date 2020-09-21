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
  
    public partial class ProductReport : Window
    {
        private ProductReportVM inventoryreportvm;
        public ProductReport()
        {
            InitializeComponent();
            inventoryreportvm = new ProductReportVM();
            this.DataContext = inventoryreportvm;
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
