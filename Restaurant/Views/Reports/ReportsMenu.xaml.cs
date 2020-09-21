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
    /// Interaction logic for ReportsMenu.xaml
    /// </summary>
    public partial class ReportsMenu : Window
    {

        private SecurityModel _security;
        public ReportsMenu(SecurityModel security)
        {
            InitializeComponent();

            _security = security;
        }


 
        private void SalesReport_Click(object sender, RoutedEventArgs e)
        {

            if (_security.WindowAccess("SalesReport"))
            {

                SalesReportView rpt = new SalesReportView() { Topmost = true };
                rpt.ShowDialog();
                this.Close();
            }
        }

        private void EmployeesReport_Click(object sender, RoutedEventArgs e)
        {

            if (_security.WindowAccess("EmployeesReport"))
            {

                EmployeeReports rpt = new EmployeeReports(_security) { Topmost = true };
                rpt.ShowDialog();
                this.Close();
            }
        }


        private void CustomerReport_Click(object sender, RoutedEventArgs e)
        {

            if (_security.WindowAccess("CustomerReport"))
            {

                CustomerReport rpt = new CustomerReport(_security) { Topmost = true };
                rpt.ShowDialog();
                this.Close();
            }
        }


        private void CallerIDReport_Click(object sender, RoutedEventArgs e)
        {

            if (_security.WindowAccess("CallerIDReport"))
            {

                CallerID rpt = new CallerID() { Topmost = true };
                rpt.ShowDialog();
                this.Close();
            }
        }



        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
