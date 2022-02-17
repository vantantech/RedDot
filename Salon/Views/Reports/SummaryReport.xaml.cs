using RedDot.ViewModels;
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

namespace RedDot.Views.Reports
{
    /// <summary>
    /// Interaction logic for SummaryReport.xaml
    /// </summary>
    public partial class SummaryReport : Window
    {

        private SummaryReportVM m_summaryreportvm;
        public SummaryReport()
        {
            InitializeComponent();
            m_summaryreportvm = new SummaryReportVM(this); //owner sales report so pass 0 in as employeeid to mean ALL employee
            this.DataContext = m_summaryreportvm;
        }
    }
}
