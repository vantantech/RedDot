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
    /// Interaction logic for SalesView.xaml
    /// </summary>
    public partial class SalesView : Window
    {
        private SalesViewVM salesViewModel;
        public SalesView(int salesid)
        {
            InitializeComponent();
            salesViewModel = new SalesViewVM(salesid);
            this.DataContext = salesViewModel; //link to Sales View Model
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
