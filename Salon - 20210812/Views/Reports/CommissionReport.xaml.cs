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
    /// Interaction logic for OwnerReport.xaml
    /// </summary>
    public partial class CommissionReport : Window
    {
        private CommissionReportVM _commissionreportvm;
     
        public CommissionReport(SecurityModel security)
        {
            InitializeComponent();

            _commissionreportvm = new CommissionReportVM(security);
     
 
            this.DataContext = _commissionreportvm;
         
        }

        private void ScrollDown_Click(object sender, RoutedEventArgs e)
        {
            // ScrollViewer1.PageDown();
            ScrollViewer1.ScrollToVerticalOffset(ScrollViewer1.VerticalOffset + 50);
        }

        private void ScrollUp_Click(object sender, RoutedEventArgs e)
        {
            // ScrollViewer1.PageUp();
            ScrollViewer1.ScrollToVerticalOffset(ScrollViewer1.VerticalOffset - 50);
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
