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
    public partial class WorkOrderEdit : Window
    {
        WorkOrderVM workordervm; 
        public WorkOrderEdit(Ticket ticket,bool canedit)
        {
           workordervm = new WorkOrderVM(this,ticket,canedit);
            this.DataContext = workordervm;

            InitializeComponent();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
