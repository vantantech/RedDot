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
    /// Interaction logic for OrderHistoryMenu.xaml
    /// </summary>
    public partial class OrderHistoryMenu : Window
    {
        private SalesViewModel m_salesvm;
        public OrderHistoryMenu(Security security, int salesid)
        {
            InitializeComponent();
            m_salesvm = new SalesViewModel(this, security.CurrentEmployee, salesid);
            this.DataContext = m_salesvm;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
