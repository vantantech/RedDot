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
    /// Interaction logic for CreditSelection.xaml
    /// </summary>
    public partial class CreditSelection : Window
    {
        public string Action = "";
        public CreditSelection()
        {
            InitializeComponent();
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Button chosen = sender as Button;
            Action = chosen.Tag.ToString().Trim();
            this.Close();
        }
    }
}
