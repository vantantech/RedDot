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
    /// Interaction logic for TicketList.xaml
    /// </summary>
    public partial class OpenTickets : Window
    {
        private OpenTicketsVM settlevm;

        public OpenTickets(SecurityModel   security, int salesid, int tablenumber)
        {
            InitializeComponent();

            settlevm = new OpenTicketsVM(this, security, salesid, tablenumber);
            this.DataContext = settlevm; 
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
