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
    /// Interaction logic for ModGroupList.xaml
    /// </summary>
    public partial class ComboSetList : Window
    {

        public ComboSetList(Window parent, MenuSetupVM inventoryvm)
        {
            InitializeComponent();
            this.DataContext = inventoryvm;
            this.Left = parent.Left;
            this.Top = parent.Top;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
