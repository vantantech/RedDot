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
    /// Interaction logic for AvailableInventory.xaml
    /// </summary>
    public partial class AvailableInventory : Window
    {
        public AvailableInventory(Window parent, InventoryVM inventoryvm)
        {
            InitializeComponent();
            this.DataContext = inventoryvm;
            this.Left = parent.Left;
            this.Top = parent.Top;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
