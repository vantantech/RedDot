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
    /// Interaction logic for EmployeeReport.xaml
    /// </summary>
    public partial class EmployeeReport : Window
    {

        private CommissionReportVM _employeecommissionvm;

        public EmployeeReport(SecurityModel security)
        {
            InitializeComponent();

            _employeecommissionvm = new CommissionReportVM(security);

            this.DataContext = _employeecommissionvm;
        }



        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
