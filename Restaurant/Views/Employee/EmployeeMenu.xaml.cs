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
        private SecurityModel _security;

        private EmployeeMenuVM timecardvm;


        public EmployeeMenu(SecurityModel security)
        {
            InitializeComponent();
            _security = security;

            timecardvm = new EmployeeMenuVM(this,security);
            this.DataContext = timecardvm;



        }



        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

   

     

  

    

 
    }
}
