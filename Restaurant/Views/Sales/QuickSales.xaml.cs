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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Printing;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Effects;
using RedDot;
using System.Globalization;
using WpfLocalization;
using CustomScreens;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class QuickSales : Window
    {

        private SalesVM salesViewModel;
      

        public QuickSales(SecurityModel security, Ticket ticket, int tablenumber,int customercount, OrderType ordertype, SubOrderType subordertype)
        {
            InitializeComponent();
            salesViewModel = new SalesVM(this, security,ticket,tablenumber,customercount,ordertype,subordertype);

            this.mainArea.Content = new Classic();
         
            this.DataContext = salesViewModel; //link to Sales View Model

        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




    }
}
