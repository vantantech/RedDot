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
    /// Interaction logic for WorkOrder.xaml
    /// </summary>
    public partial class ShipOrderEdit : Window
    {
        ShipOrderVM workordervm;
        public ShipOrderEdit(Ticket ticket)
        {
            workordervm = new ShipOrderVM(this, ticket);
            this.DataContext = workordervm;

            InitializeComponent();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
