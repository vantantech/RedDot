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
    public partial class SalesReportLarge : Window
    {
        private SalesReportVM m_salesreportvm;
        public SalesReportLarge()
        {
            InitializeComponent();
            m_salesreportvm = new SalesReportVM(this, -1); //owner sales report so pass 0 in as employeeid to mean ALL employee
            this.DataContext = m_salesreportvm;
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
