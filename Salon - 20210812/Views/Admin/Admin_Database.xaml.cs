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
using RedDot.DataManager;

namespace RedDot
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Admin_Database : Window
    {

        private ObservableCollection<TableEdit> _tablelist;
        private DBConnect dbConnect;
        private IDataInterface _dbsettings;

        public Admin_Database()
        {
            InitializeComponent();
            dbConnect = new DBConnect();
            _dbsettings = GlobalSettings.Instance.RedDotData;
        }

        public DataTable FillData(string query)
        {
            if (dbConnect == null) dbConnect = new DBConnect();
            DataTable table = dbConnect.GetData(query);
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



                    _tablelist.Add(new TableEdit("Category", FillData("Select * from category")));
                    _tablelist.Add(new TableEdit("Product", FillData("Select * from product")));
                    _tablelist.Add(new TableEdit("Cat2Prod", FillData("Select * from cat2prod")));
                    _tablelist.Add(new TableEdit("Sales", FillData("Select * from sales")));
                    _tablelist.Add(new TableEdit("Sales Item", FillData("Select * from salesitem")));
                    _tablelist.Add(new TableEdit("Employee", FillData("Select * from employee")));
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

            if (sec.WindowNewAccess("DatabaseView"))
            {
                if (Confirm.Ask("Wipe All Sales Data?????") == true)
                {

                    if (_dbsettings.WipeSalesData()) TouchMessageBox.Show("Sales Data Wiped Successfully!!");
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

                    if (_dbsettings.WipeCustomerData()) TouchMessageBox.Show("Customer Data Wiped Successfully!!");
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

                    if (_dbsettings.WipeGiftCardData()) TouchMessageBox.Show("Gift Card Data Wiped Successfully!!");
                }
            }
        }


        private void Button_ExportDatabase(object sender, RoutedEventArgs e)
        {
            try
            {
                string backupdirectory = GlobalSettings.Instance.BackupDirectory;

                BackupSelect bk = new BackupSelect(backupdirectory);

                bk.ChosenDrive = "";

                Utility.OpenModal(this, bk);

                if (bk.ChosenDrive != "")
                {
                    DBConnect db = new DBConnect();
                    if (bk.ChosenDrive != backupdirectory) backupdirectory = bk.ChosenDrive + "backup";
                    //"C:\\Program Files\\MySQL\\MySQL Server 5.6\\bin\\mysqldump"
                    db.Backup(backupdirectory);
                    MessageBox.Show("Backup successful to  " + backupdirectory);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void Button_ImportDatabase(object sender, RoutedEventArgs e)
        {
            try
            {
                string backupdirectory = GlobalSettings.Instance.BackupDirectory;

                // Create OpenFileDialog
                Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

                // Launch OpenFileDialog by calling ShowDialog method
                Nullable<bool> result = openFileDlg.ShowDialog();
                // Get the selected file name and display in a TextBox.
                // Load content of file in a TextBlock
                if (result == true)
                {
                 
                    DBConnect db = new DBConnect();

                    db.Restore(openFileDlg.FileName);
                    MessageBox.Show("Restore from " + openFileDlg.FileName + " successful");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
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
