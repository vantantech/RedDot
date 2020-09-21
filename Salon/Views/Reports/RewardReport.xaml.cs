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
    /// Interaction logic for RewardReport.xaml
    /// </summary>
    public partial class RewardReport : Window
    {

        private RewardReportVM m_rewardreportvm;
        public RewardReport()
        {
            InitializeComponent();
            m_rewardreportvm = new RewardReportVM();
            this.DataContext = m_rewardreportvm;
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
