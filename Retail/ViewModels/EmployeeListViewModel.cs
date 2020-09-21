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

     
    public class EmployeeListViewModel:INPCBase
    {

        private ObservableCollection<Employee> _employees;
        private DBEmployee _dbemployees;
        public ICommand AddEmployeeClicked { get; set; }
        private bool _canaddemployee = false;
        private bool _showall = false;

        public Visibility AddEmployeeVisibility { get; set; }
        public EmployeeListViewModel()
        {
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
            _dbemployees.AddEmployee(Utility.RandomPin(GlobalSettings.Instance.PinLength));

            LoadEmployees();
        }

    }
}
