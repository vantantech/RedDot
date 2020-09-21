using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using System.Windows;
using RedDot;
//using System.Windows.Forms;
using RedDot.DataManager;
using Microsoft.Win32;

namespace RedDot
{

     
    public class EmployeeListViewModel:INPCBase
    {

        private DataTable _employees;
        private DBEmployee _dbemployees;
        public ICommand AddEmployeeClicked { get; set; }
        public ICommand ExportCSVClicked { get; set; }
        private bool _canaddemployee = false;
        private bool _showall = false;

        public Visibility AddEmployeeVisibility { get; set; }
        private SecurityModel m_security;
     

        public EmployeeListViewModel(SecurityModel sec)
        {
            AddEmployeeVisibility = Visibility.Hidden;

          

            m_security = sec;

            AddEmployeeClicked = new RelayCommand(ExecuteAddEmployeeClicked, param => this.CanAddEmployeeClicked);
            ExportCSVClicked = new RelayCommand(ExecuteExportCSVClicked, param => true);

            _dbemployees = new DBEmployee();
         
            LoadEmployees();
        }

        public void LoadEmployees()
        {

            if (_showall)
            {
                Employees = _dbemployees.GetEmployeeList(false); //all employee
                SalesTechs = _dbemployees.GetSalesTechList(false,GlobalSettings.Instance.Demo);//all employee
            }
            else
            {
                Employees = _dbemployees.GetEmployeeList(true); //active employee only
                SalesTechs = _dbemployees.GetSalesTechList(true, GlobalSettings.Instance.Demo);
            }

      
        }


        public DataTable Employees
        {
            get{ return _employees;}
            set { _employees = value; NotifyPropertyChanged("Employees"); }
        }

        private DataTable _salestechs;
        public DataTable SalesTechs
        {
            get { return _salestechs; }
            set { _salestechs = value; NotifyPropertyChanged("SalesTechs"); }
        }

        public bool CanAddEmployeeClicked
        {
            get { return _canaddemployee; }

            set
            {
                _canaddemployee = value;
                if (_canaddemployee) AddEmployeeVisibility = Visibility.Visible;
                else AddEmployeeVisibility = Visibility.Hidden;
                NotifyPropertyChanged("CanAddEmployeeClicked");
            }
          
        }


        public bool ShowAll
        {
            get { return _showall; }
            set
            {
                _showall = value;
                NotifyPropertyChanged("ShowAll");
                LoadEmployees();
            }
        }


        public void ExecuteAddEmployeeClicked(object tag)
        {
            //start wizard

            TextPad step1 = new TextPad("Enter First Name", "");
            step1.Topmost = true;
            step1.ShowDialog();

            if (step1.Action == TextPad.Cancel) return;


            TextPad step2 = new TextPad("Enter Last Name", "");
            step2.Topmost = true;
            step2.ShowDialog();

            if (step2.Action == TextPad.Cancel) return;


            SecurityLevel dlg = new SecurityLevel();
            dlg.Topmost = true;
            dlg.ShowDialog();

            if (dlg.Level > -1)
            {

                int empid = _dbemployees.AddEmployee(Utility.RandomPin(GlobalSettings.Instance.PinLength));

                AuditModel.WriteLog(m_security.CurrentEmployee.DisplayName, "Add Employee", step1.ReturnText + " " + step2.ReturnText, "none", 0);

                if (empid > 0)
                {
                    Employee newemployee = new Employee(empid);

                    newemployee.FirstName = step1.ReturnText;
                    newemployee.LastName = step2.ReturnText;
                    newemployee.DisplayName = step1.ReturnText;
                    newemployee.SecurityLevel = dlg.Level;


                    //send into screen with new employee instance so it will be refreshed.
                    EmployeeView empview = new EmployeeView(m_security,new Employee(empid), true, true);
                    empview.Topmost = true;
                    empview.ShowDialog();
                }



                LoadEmployees();

            }




           
        }


        public void ExecuteExportCSVClicked(object tag)
        {
            try
            {
                DataTable tbl;
                if (_showall) tbl = _dbemployees.GetEmployeeList(false,true);
                else tbl = _dbemployees.GetEmployeeList(true,true);

          
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                Nullable<bool> result = ofd.ShowDialog();

                if (result == true)
                {
                    CSVWriter.WriteDataTable(tbl, ofd.FileName, true);

                }

            }catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

              
        }
    }
}
