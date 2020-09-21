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
    /// Interaction logic for EmployeeList.xaml
    /// </summary>
    public partial class EmployeeList : Window
    {
        public EmployeeListVM _employeelistviewmodel;
        public bool AutoClose = true;
        public int EmployeeID { get; set; }
        private SecurityModel _security;

        public EmployeeList(Window parent, SecurityModel security)
        {
            InitializeComponent();


            _employeelistviewmodel = new EmployeeListVM(security);
            this.DataContext = _employeelistviewmodel;
            _security = security;
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
          
            this.Close();
        }

        private void EmployeeClicked(object sender, RoutedEventArgs e)
        {
            Button picked = sender as Button;
            EmployeeID = int.Parse(picked.Tag.ToString());

            if (EmployeeID > 0)
            {
                bool candelete = true;

                if (_security.CurrentEmployee.ID == EmployeeID) candelete = false;

                EmployeeView empview = new EmployeeView(_security,new Employee(EmployeeID), candelete, true) { Topmost = true };
                empview.ShowDialog();
                AuditModel.WriteLog(_security.CurrentEmployee.DisplayName, "Open", "Employee View by Admin", "none", 0);


                if (GlobalSettings.Instance.HomeClicked) this.Close();
                _employeelistviewmodel.LoadEmployees();
            }

        }

        public string ExitCaption
        {
             set{ tbExit.Text = value.ToString();}

        }
        public bool HideExit
        {

            set { if ((bool) value == true) btnExit.Visibility = Visibility.Hidden; else  btnExit.Visibility = Visibility.Visible; }

        }

    }
}
