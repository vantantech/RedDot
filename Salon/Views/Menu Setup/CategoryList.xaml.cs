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
    /// Interaction logic for Inventory.xaml
    /// </summary>
    public partial class CategoryList : Window
    {
        MenuSetupVM m_inventoryvm;
        public CategoryList(SecurityModel security)
        {
            InitializeComponent();
            m_inventoryvm = new MenuSetupVM(this,security);
            this.DataContext = m_inventoryvm;
        }



        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
