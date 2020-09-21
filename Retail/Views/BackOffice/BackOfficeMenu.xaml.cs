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
    /// Interaction logic for backofficemenu.xaml
    /// </summary>
    public partial class BackOfficeMenu : Window
    {
        private Security _security;
        private Window _parent;
        public BackOfficeMenu(Window parent,Security security)
        {
            InitializeComponent();
            this.Left = parent.Left;
            this.Top = parent.Top;
            _security = security;
            _parent = parent;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings set = new Settings(_parent);
            Utility.OpenModal(this,set);

        }
        private void btnReports_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("Reports"))
            {
                ReportsMenu rpt = new ReportsMenu(_security);
                Utility.OpenModal(this, rpt);

            }
        }
        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
          
                if (_security.WindowAccess("Inventory"))
                {

                    QuickInventory rpt = new QuickInventory(_security);
                    Utility.OpenModal(this, rpt);

                }
           
 
        }

        private void Security_Clicked(object sender, RoutedEventArgs e)
        {
            if (_security.WindowNewAccess("Security"))
            {
                SecurityView dlg = new SecurityView(_security);
                Utility.OpenModal(this, dlg);

            }
        }
        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Database_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowNewAccess("Security"))
            {
                Admin_Database adm = new Admin_Database();
                Utility.OpenModal(this, adm);

            }
        
        }


        private void Send_Clicked(object sender, RoutedEventArgs e)
        {
            SendSMS sms = new SendSMS();
            Utility.OpenModal(this, sms);
        }
        private void Employees_Clicked(object sender, RoutedEventArgs e)
        {
            if (_security.WindowNewAccess("EmployeeList"))
            {
                EmployeeList emp = new EmployeeList(null,_security);
                emp._employeelistviewmodel.CanAddEmployeeClicked = true;
                emp.AutoClose = false;
                Utility.OpenModal(this, emp);
            }

        }


        private void TimeCard_Clicked(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("TimeEdit"))
            {

                EmployeeTimeSheets rpt = new EmployeeTimeSheets(_security);
                Utility.OpenModal(this, rpt);
            }
        }

        private void Update_Clicked(object sender, RoutedEventArgs e)
        {
            Update upt = new Update();
            Utility.OpenModal(this, upt);
        }

        private void Labor_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("Inventory"))
            {

                ServiceSetup rpt = new ServiceSetup(_security);
                Utility.OpenModal(this, rpt);
    
            }
        }

        private void Shipping_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("Inventory"))
            {

                ShippingSetup rpt = new ShippingSetup(_security);
                Utility.OpenModal(this, rpt);

            }
        }
    }
}
