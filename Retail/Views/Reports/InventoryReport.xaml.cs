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
    /// Interaction logic for InventoryReport.xaml
    /// </summary>
    public partial class InventoryReport : Window
    {
        private InventoryReportVM inventoryreportvm;
        public InventoryReport()
        {
            InitializeComponent();
            inventoryreportvm = new InventoryReportVM();
            this.DataContext = inventoryreportvm;
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
