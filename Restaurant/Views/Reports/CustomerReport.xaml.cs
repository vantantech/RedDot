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
    /// Interaction logic for CustomerReport.xaml
    /// </summary>
    public partial class CustomerReport : Window
    {

        private CustomerReportVM m_customervm;
        public CustomerReport(SecurityModel security)
        {
            InitializeComponent();

            m_customervm = new CustomerReportVM(this, security);
            this.DataContext = m_customervm;
        }


        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
