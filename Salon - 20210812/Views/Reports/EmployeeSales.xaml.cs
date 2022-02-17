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
    /// Interaction logic for EmployeeSales.xaml
    /// </summary>
    public partial class EmployeeSales : Window
    {
        private SalesReportVM m_employeesales;
        public EmployeeSales(int employeeid)
        {
            InitializeComponent();

            m_employeesales = new SalesReportVM(this, employeeid);
            this.DataContext = m_employeesales;
        }
    }
}
