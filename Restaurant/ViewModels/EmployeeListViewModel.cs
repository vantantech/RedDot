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

namespace RedDot
{

     
    public class EmployeeListVM:INPCBase
    {

        private ObservableCollection<Employee> _employees;
        private DBEmployee _dbemployees;
        public ICommand AddEmployeeClicked { get; set; }
        private bool _canaddemployee = false;
        private bool _showall = false;

        public Visibility AddEmployeeVisibility { get; set; }
        protected SecurityModel m_security;
        public EmployeeListVM(SecurityModel sec)
        {
            m_security = sec;


            AddEmployeeVisibility = Visibility.Hidden;

            AddEmployeeClicked = new RelayCommand(ExecuteAddEmployeeClicked, param => this.CanAddEmployeeClicked);

            _dbemployees = new DBEmployee();
            _employees = new ObservableCollection<Employee>();
            LoadEmployees();
        }

        public void LoadEmployees()
        {
            DataTable tbl;
            Employee current;
            ObservableCollection<Employee> employees;
            employees = new ObservableCollection<Employee>();

            if (_showall) tbl = _dbemployees.GetEmployeeAll();
            else tbl = _dbemployees.GetEmployeeActive();

            foreach(DataRow row in tbl.Rows)
            {
               // current = new Employee(int.Parse(row["id"].ToString()));
                current = new Employee(row);
                employees.Add(current);
            }

            Employees = employees;
            GlobalSettings.Instance.LoadAllFmdsUserIDs();

        }


        public ObservableCollection<Employee> Employees
        {
            get{ return _employees;}
            set { _employees = value; NotifyPropertyChanged("Employees"); }
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

            TextPad step1 = new TextPad("Enter First Name", "")
            {
                Topmost = true,
                ShowInTaskbar = false
            };
            step1.ShowDialog();

            if (step1.Action == TextPad.Cancel) return;


            TextPad step2 = new TextPad("Enter Last Name", "")
            {
                Topmost = true,
                ShowInTaskbar = false
            };
            step2.ShowDialog();

            if (step2.Action == TextPad.Cancel) return;


            SecurityLevel dlg = new SecurityLevel
            {
                Topmost = true,
                ShowInTaskbar = false
            };
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
                    EmployeeView empview = new EmployeeView(m_security, new Employee(empid), true, true)
                    {
                        Topmost = true,
                        ShowInTaskbar = false
                    };
                    empview.ShowDialog();
                }



                LoadEmployees();

            }





        }

    }
}
