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
    /// Interaction logic for SalesModifierView.xaml
    /// </summary>
    public partial class SalesModifierView : Window
    {
        private AddModifierVM addmodifiervm;
        public SalesModifierView(Window parent, Ticket ticket, int lineid, Product prod, bool newticket)
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
            addmodifiervm = new AddModifierVM(this, ticket, lineid, prod,newticket);
            this.DataContext = addmodifiervm;
        }
    }
}
