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
    /// Interaction logic for OperationMenu.xaml
    /// </summary>
    public partial class OperationMenu : Window
    {
    
        public int ButtonSize { get; set; }

        public OperationMenu()
        {
            InitializeComponent();
            ButtonSize = 120;

            this.DataContext = this;
        }




        private void btnOperation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
