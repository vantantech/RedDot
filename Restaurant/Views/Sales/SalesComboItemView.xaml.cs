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
    /// Interaction logic for AddModifierView.xaml
    /// </summary>
    public partial class SalesComboItemView : Window
    {
        private AddComboItemVM addcomboitemvm;
        public SalesComboItemView(Window parent, Ticket ticket, int lineid, Product prod, bool newticket)
        {
            InitializeComponent();
            if (parent != null)
            {
                this.Left = parent.Left;
                this.Top = parent.Top;
            }
            else
            {
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            }
            addcomboitemvm = new AddComboItemVM(this, ticket, lineid, prod,newticket);
            this.DataContext = addcomboitemvm;
        }

     
    }
}
