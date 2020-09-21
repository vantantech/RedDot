using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Configuration;
using System.Data;
using System.ComponentModel;
using RedDot;
using RedDot.ViewModels;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Admin_Database : Window
    {
     
        private ObservableCollection<TableEdit> _tablelist;
        private DBConnect dbConnect;
        private AdminDatabaseVM adminvm;

        public Admin_Database()
        {
            InitializeComponent();
            dbConnect = new DBConnect();
            adminvm = new AdminDatabaseVM();
            this.DataContext = adminvm;
        }

        public DataTable FillData(string query)
        {
            if (dbConnect == null) dbConnect = new DBConnect();
            DataTable table = dbConnect.GetData(query, "Table");
            return table;

        }

     
        public ObservableCollection<TableEdit> TableList
        {
            get
            {
                if (_tablelist == null)
                {

                    //DataView field;
                    _tablelist = new ObservableCollection<TableEdit>();



                    _tablelist.Add(new TableEdit("Category", FillData("Select description from category")));
                    _tablelist.Add(new TableEdit("Product", FillData("Select description, price, type from product")));
                    _tablelist.Add(new TableEdit("Sales", FillData("Select * from sales")));
                      _tablelist.Add(new TableEdit("Sales Item", FillData("Select * from salesitem")));
                     _tablelist.Add( new TableEdit("Employee",FillData("Select * from employee")));
                     _tablelist.Add(new TableEdit("Customer", FillData("Select * from customer")));
                     _tablelist.Add(new TableEdit("Payment", FillData("Select * from payment")));
                     _tablelist.Add(new TableEdit("Security", FillData("Select * from security")));
                     _tablelist.Add(new TableEdit("Promotion", FillData("Select * from promotion")));
                     _tablelist.Add(new TableEdit("Settings", FillData("Select * from settings")));
                }

                return _tablelist;
            }
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            

            this.Close();
        }

        private void Button_WipeSalesData(object sender, RoutedEventArgs e)
        {
            SecurityModel sec = new SecurityModel();

            if(sec.WindowNewAccess("DatabaseView"))
            {
                if(Confirm.Ask("Wipe All Sales Data?????") == true)
                {
                    DBSettings dbsettings = new DBSettings();
                    if (dbsettings.WipeSalesData()) TouchMessageBox.Show("Sales Data Wiped Successfully!!");
                }
              
                
            }
          
           
        }



        private void Button_WipeCustomerData(object sender, RoutedEventArgs e)
        {
            SecurityModel sec = new SecurityModel();

            if (sec.WindowNewAccess("DatabaseView"))
            {
                if (Confirm.Ask("Wipe All Sales Data?????") == true)
                {
                    DBSettings dbsettings = new DBSettings();
                    if (dbsettings.WipeCustomerData()) TouchMessageBox.Show("Customer Data Wiped Successfully!!");
                }
            }
        }

        private void Button_WipeGiftCardData(object sender, RoutedEventArgs e)
        {
            SecurityModel sec = new SecurityModel();

            if (sec.WindowNewAccess("DatabaseView"))
            {
                if (Confirm.Ask("Wipe All Sales Data?????") == true)
                {
                    DBSettings dbsettings = new DBSettings();
                    if (dbsettings.WipeGiftCardData()) TouchMessageBox.Show("Gift Card Data Wiped Successfully!!");
                }
            }
        }
    }

    public class TableEdit : INPCBase
    {
        private string _header;


        private DataTable _tablename;


        public TableEdit( string header, DataTable tablename)
        {
            _header = header;

            _tablename = tablename;

        }

        /// <summary>
        ///     Header Property 
        /// </summary>
        public string Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    NotifyPropertyChanged("Header");
                }
            }
        }




        public DataTable EditableTable
        {

            get {

                return _tablename;
           
            }
            set
            {
                if (_tablename != EditableTable)
                {
                    _tablename = EditableTable;
                    NotifyPropertyChanged("EditableTable");
                }
            }
        }


    }









}
