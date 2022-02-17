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
    /// Interaction logic for Turns.xaml
    /// </summary>
    public partial class Turns : Window
    {
        TurnsVM turnsvm;
        public Turns(Window parent)
        {
            InitializeComponent();
            turnsvm = new TurnsVM(parent);
            this.DataContext = turnsvm;

        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
