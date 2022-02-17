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
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        public string ReturnText { get; set; }
        public string Action { get; set; }

        public static string OK = "Ok";
        public static string Cancel = "Cancel";
        public ColorPicker(string currentcolor)
        {
            CurrentColor = currentcolor;

            InitializeComponent();
            this.DataContext = this;
        }

        public string CurrentColor { get; set; }

        public string NewColor { get; set; }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            Action = Cancel;
            ReturnText = CurrentColor;
            this.Close();
        }

        private void Button_OK(object sender, RoutedEventArgs e)
        {
            Action = OK;
            ReturnText = NewColor;
            this.Close();
        }
    }
}
