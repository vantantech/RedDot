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
    /// Interaction logic for SalesReport.xaml
    /// </summary>
    public partial class RetailMonthlySalesReport : Window
    {
        private MonthlySalesReportVM m_salesreportvm;
        public RetailMonthlySalesReport()
        {
            InitializeComponent();
            m_salesreportvm = new MonthlySalesReportVM();
            this.DataContext = m_salesreportvm;
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
