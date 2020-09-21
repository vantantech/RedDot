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
    /// Interaction logic for CashierInOut.xaml
    /// </summary>
    public partial class CashierInOut : Window
    {
        public CashierInOut(SecurityModel security, bool editmode)
        {
            InitializeComponent();
            CashierInOutVM cashiervm = new CashierInOutVM(this, security, security.CurrentEmployee,editmode);
            this.DataContext = cashiervm;
        }


        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

  
}


