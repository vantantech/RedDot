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
    /// Interaction logic for EmployeeMenu.xaml
    /// </summary>
    public partial class EmployeeMenu : Window
    {
        private Security _security;
        public EmployeeMenu(Security security)
        {
            InitializeComponent();
            _security = security;
        }


        private void btnCommission_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("EmployeeCommission"))
            {
                RetailEmployeeCommissionReport rpt = new RetailEmployeeCommissionReport(_security,_security.CurrentEmployee.ID);
                Utility.OpenModal(this, rpt);

            }
        }

        private void Employees_Clicked(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("EmployeeView"))
            {
                EmployeeView rpt = new EmployeeView(_security.CurrentEmployee,_security,false,false);
                Utility.OpenModal(this, rpt);

            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Hours_Clicked(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("EmployeeView"))
            {
                EmployeeHours rpt = new EmployeeHours(_security.CurrentEmployee,_security, false, false);
                Utility.OpenModal(this, rpt);

            }
        }
    }
}
