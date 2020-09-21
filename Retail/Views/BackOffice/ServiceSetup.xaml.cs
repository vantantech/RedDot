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
    /// Interaction logic for ServiceSetup.xaml
    /// </summary>
    public partial class ServiceSetup : Window
    {
        ServiceVM m_servicevm;
        public ServiceSetup(Security security)
        {
            InitializeComponent();
            m_servicevm = new ServiceVM(this, security);
            this.DataContext = m_servicevm;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
