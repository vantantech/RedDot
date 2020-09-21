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
using System.Diagnostics;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for GratuityView.xaml
    /// </summary>
    public partial class GratuityView : Window
    {

        private GratuityViewModel _gratuityviewmodel;


        public GratuityView( Ticket ticket, HeartPOS ccp)
        {
            InitializeComponent();
           // if (ticket == null) ticket = new Ticket(salesid);

            _gratuityviewmodel = new GratuityViewModel(this, ticket,ccp);
            this.DataContext = _gratuityviewmodel;

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
