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
    /// Interaction logic for PickEmployee.xaml
    /// </summary>
    public partial class PickEmployee : Window
    {
        public EmployeeListViewModel _employeelistviewmodel;
        public bool AutoClose = true;
        
        public int EmployeeID { get; set; }

        public PickEmployee(Window parent)
        {
            InitializeComponent();
            if (parent != null)
            {
                this.Left = parent.Left ;
                this.Top = parent.Top + 80;
            }else
            {
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }

            if (GlobalSettings.Instance.ForceEmployeeAssign) btnExit.Visibility = Visibility.Hidden;
            _employeelistviewmodel = new EmployeeListViewModel();
            this.DataContext = _employeelistviewmodel;
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
          
            this.Close();
        }

        private void EmployeeClicked(object sender, RoutedEventArgs e)
        {
            Button picked = sender as Button;
            EmployeeID = int.Parse(picked.Tag.ToString());
            this.Close();
        }



    }
}