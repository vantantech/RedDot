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
    /// Interaction logic for ModifierDetail.xaml
    /// </summary>
    public partial class ModifierDetail : Window
    {
       
        public ModifierDetail(Window parent, MenuSetupVM mod)
        {
            InitializeComponent();

            this.DataContext = mod;
            this.Left = parent.Left;
            this.Top = parent.Top;
           
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
