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
    /// Interaction logic for SalesDetailsReport.xaml
    /// </summary>
    public partial class EmployeeSalesReport : Window
    {
      
       private CommissionReportVM _commissionreportvm;

        public EmployeeSalesReport(Security security)
        {
            InitializeComponent();

            _commissionreportvm = new CommissionReportVM(this, security);


            this.DataContext = _commissionreportvm;

        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
