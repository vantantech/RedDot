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
    /// Interaction logic for SelectedInventory.xaml
    /// </summary>
    public partial class SelectedInventory : Window
    {
        public SelectedInventory(int id, Security security)
        {
            
            InitializeComponent();

            InventoryVM inventoryvm = new InventoryVM(this,id, security);



            this.DataContext = inventoryvm;
         
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
