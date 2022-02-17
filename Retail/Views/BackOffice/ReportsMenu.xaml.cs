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
    /// Interaction logic for ReportsMenu.xaml
    /// </summary>
    public partial class ReportsMenu : Window
    {

        private Security _security;
        public ReportsMenu(Security security)
        {
            InitializeComponent();

            _security = security;
        }


        private void Commission_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("OwnerCommission"))
            {
                CommisionReport rpt = new CommisionReport(_security);
                rpt.ShowDialog();

            }
        }



        private void SalesRep_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("OwnerCommission"))
            {
                SalesRepReport rpt = new SalesRepReport(_security);
                rpt.ShowDialog();

            }
        }


        private void EmployeeSalesReport_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("EmployeeSales"))
            {
                EmployeeSalesReport rpt = new EmployeeSalesReport(_security);
                rpt.ShowDialog();

            }
        }
        private void DailySalesReport_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("SalesReport"))
            {

                        RetailDailySalesReport rpt = new RetailDailySalesReport(_security);
                        rpt.ShowDialog();

            }
        }

        private void WeeklySalesReport_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("SalesReport"))
            {

                RetailWeeklySalesReport rpt = new RetailWeeklySalesReport();
                rpt.ShowDialog();

            }
        }

        private void MonthlySalesReport_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("SalesReport"))
            {

                RetailMonthlySalesReport rpt = new RetailMonthlySalesReport();
                rpt.ShowDialog();

            }
        }

        private void InventoryReport_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("InventoryReport"))
            {

                InventoryReport rpt = new InventoryReport();
                rpt.ShowDialog();

            }
        }

        private void ProductReport_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("SalesReport"))
            {

                ProductReport rpt = new ProductReport();
                rpt.ShowDialog();

            }
        }

        private void ShippingReport_Click(object sender, RoutedEventArgs e)
        {
            if (_security.WindowAccess("SalesReport"))
            {

                ShippingReport rpt = new ShippingReport(_security);
                rpt.ShowDialog();

            }
        }
        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
