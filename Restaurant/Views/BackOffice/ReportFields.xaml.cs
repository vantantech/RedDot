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
    /// Interaction logic for ReportFields.xaml
    /// </summary>
    public partial class ReportFields : Window
    {
        ReportSetupVM reportvm;
        public ReportFields(Window parent)
        {
            reportvm = new ReportSetupVM(parent);
            InitializeComponent();
            this.DataContext = reportvm;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TapTipKeyboard.ShowKeyboard();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

      
    }
}
