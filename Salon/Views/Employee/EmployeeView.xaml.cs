using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
    public partial class EmployeeView : Window
    {
      


        public EmployeeView(SecurityModel security, Employee employee, bool candelete, bool admin)
        {
            InitializeComponent();

            EmployeeViewModel employeeViewModel = new EmployeeViewModel(this,security,employee,candelete, admin);
            this.DataContext = employeeViewModel;
             chkActive.IsEnabled = candelete;  //users can't delete themselves or deactivate themselves
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            TapTipKeyboard.HideKeyboard();


            this.Close();
        }

   


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {

            TapTipKeyboard.ShowKeyboard();

            // Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\osk.exe");
        }

  


      
    }
}
