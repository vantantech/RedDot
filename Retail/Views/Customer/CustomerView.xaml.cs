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


using System.Data;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class CustomerView : Window
    {
        private CustomerVM customerviewmodel;


        public CustomerView(Security security, int customerid)
        {
            InitializeComponent();
            customerviewmodel = new CustomerVM(this, security, customerid);
            this.DataContext = customerviewmodel;

        }


        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
