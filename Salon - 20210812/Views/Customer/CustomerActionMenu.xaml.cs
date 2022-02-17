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
    /// Interaction logic for CustomerMenu.xaml
    /// </summary>
    public partial class CustomerActionMenu : Window
    {

        public string Action = "";
        private CustomerViewModel customerviewmodel;
        public CustomerActionMenu(SecurityModel security, int customerid)
        {
            InitializeComponent();
            customerviewmodel = new CustomerViewModel(this, security, customerid);
            this.DataContext = customerviewmodel;
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            Action = "View";
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Action = "Delete";
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
