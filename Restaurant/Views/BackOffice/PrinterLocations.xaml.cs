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
    /// Interaction logic for Printers.xaml
    /// </summary>
    public partial class PrinterLocations : Window
    {
        private PrinterLocationVM locationvm; 
        public PrinterLocations()
        {
            InitializeComponent();
            locationvm = new PrinterLocationVM();
            this.DataContext = locationvm;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TapTipKeyboard.ShowKeyboard();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    
    }
}
