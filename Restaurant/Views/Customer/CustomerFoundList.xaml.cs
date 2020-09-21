using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for CustomerFoundList.xaml
    /// </summary>
    public partial class CustomerFoundList : Window
    {
        public int CustomerID = 0;
        public DataTable CustomerList { get; set; }
       // private DataTable m_customerlist;
        public CustomerFoundList(DataTable customerlist)
        {

            CustomerList = customerlist;
            this.DataContext = this;

            InitializeComponent();
        }


        private void CustomerClicked(object sender, RoutedEventArgs e)
        {
            Button picked = sender as Button;
            CustomerID = int.Parse(picked.Tag.ToString());
            this.Close();
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //CustomerList = m_customerlist;
           
        }



    }
}
