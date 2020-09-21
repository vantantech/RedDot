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
    /// Interaction logic for vwCashTendered.xaml
    /// </summary>
    public partial class TicketHold : Window
    {


        private TicketHoldVM ticketholdvm;

     

        public TicketHold(Ticket m_ticket)
        {
            ticketholdvm = new TicketHoldVM(this, m_ticket);

            InitializeComponent();
            this.DataContext = ticketholdvm;


        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            tbAmount.Text = tbAmount.Text + button.Content.ToString();
        }

  


        private void OKClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        

        }


        private void ClearClick(object sender, RoutedEventArgs e)
        {
            //Clear 
            tbAmount.Text = "";
        }

  


    }
}
