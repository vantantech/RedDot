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
    /// Interaction logic for PAXManager.xaml
    /// </summary>
    public partial class PAXManager: Window
    {

        PAXManagerVM paxmanagervm;
        public PAXManager(Employee currentemployee)
        {
            InitializeComponent();
            paxmanagervm = new PAXManagerVM(this, currentemployee);
            this.DataContext = paxmanagervm;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            tbAmount.Text = tbAmount.Text + button.Content.ToString();
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            //Clear 
            tbAmount.Text = "";
        }

    }
}
