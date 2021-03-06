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
    /// Interaction logic for SearchbyName.xaml
    /// </summary>
    public partial class SearchbyName : Window
    {
        public string FirstName;
        public string LastName;
        public SearchbyName()
        {
            InitializeComponent();
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            FirstName = tbFirstName.Text;
            LastName = tbLastName.Text;

            this.Close();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            FirstName = "";
            LastName = "";
            this.Close();
        }
    }
}
