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
    /// Interaction logic for Employee.xaml
    /// </summary>
    public partial class EmployeeHours : Window
    {
        public EmployeeHours(SecurityModel security, bool candelete, bool admin)
        {
            InitializeComponent();

            EmployeeVM employeeViewModel = new EmployeeVM(this,security,security.CurrentEmployee,candelete, admin);
            this.DataContext = employeeViewModel;

        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
