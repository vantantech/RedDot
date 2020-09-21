using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for CallerID.xaml
    /// </summary>
    public partial class CallerID : Window
    {

        private DBConnect dbConnect;

        private DataTable m_reporttable;
        public DataTable ReportTable {
            get
            {
                if (m_reporttable != null) return m_reporttable;
                else
                {
                    Fill();
                    return m_reporttable;
                }
            }
        }
      
        public CallerID()
        {

            InitializeComponent();

            this.DataContext = this;
        }

        private void Fill()
        {
            dbConnect = new DBConnect();
            if (dbConnect == null) dbConnect = new DBConnect();
            string query = "Select * from callerid";
             m_reporttable = dbConnect.GetData(query, "Table");
        }
        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
