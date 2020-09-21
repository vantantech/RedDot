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
    /// Interaction logic for SecurityLevel.xaml
    /// </summary>
    public partial class SecurityLevel : Window
    {

        public int Level { get; set; }
        public SecurityLevel()
        {
            InitializeComponent();
            Level = -1;
        }


        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Level = int.Parse( button.Tag.ToString());
            this.Close();
        }


        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
    }
}
