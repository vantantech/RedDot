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
    public partial class CustomerMenu : Window
    {
        private Security _security;
        public CustomerMenu(Security security)
        {
            InitializeComponent();
            _security = security;
        }


 



        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //customer search
            CustomerSearch cs = new CustomerSearch();
            Utility.OpenModal(this, cs);

            //need to check existence of customerid
            if (cs.CustomerID > 0)
            {
                CustomerView custvw = new CustomerView(_security, cs.CustomerID);
                Utility.OpenModal(this, custvw);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //customer merge

        }

   
    }
}
