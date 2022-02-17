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
    /// Interaction logic for Selection.xaml
    /// </summary>
    public partial class Selection : Window
    {

        public string Option1 { get; private set; }
        public string Option2 { get; private set; }

        public string Option3 { get; private set; }
        public string Action { get; set; }
        public Selection(string option1, string option2, string option3="")
        {
            Option1 = option1;
            Option2 = option2;

            Option3 = option3;

            InitializeComponent();
            this.DataContext = this;
        }


        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            Button chosen = sender as Button;
            Action = chosen.Tag.ToString().Trim();
            this.Close();
        }
    }
}
