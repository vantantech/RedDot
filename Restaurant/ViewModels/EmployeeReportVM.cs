using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace RedDot
{
    public class EmployeeReportVM:INPCBase
    {

        private Window _parent;
        private DBEmployee _dbemployees;
        private int _currentemployeeid;
        
        private DateTime _startdate;
        private DateTime _enddate;
        private int m_selectedindex;
        private bool CanExecute = true;
        private List<ReportDate> m_reportdates;
        private ReportDate m_currentdate;
        private int m_selecteddateid;

        public ReportDate CurrentDate
        {
            get { return m_currentdate; }
            set
            {
                m_currentdate = value;
                NotifyPropertyChanged("CurrentDate");
            }
        }
        public List<ReportDate> ReportDates
        {
            get { return m_reportdates; }

            set
            {
                m_reportdates = value;
                NotifyPropertyChanged("ReportDates");
            }
        }

        private ObservableCollection<Employee> m_employeereports;
        public ObservableCollection<Employee> EmployeeList
        {
            get { return m_employeereports; }
            set
            {
                m_employeereports = value;
                NotifyPropertyChanged("EmployeeList");
            }
        }

        public ICommand EmployeeClicked { get; set; }

        public ICommand PrintClicked { get; set; }
        public ICommand ExportClicked { get; set; }




        public EmployeeReportVM(Window parent, SecurityModel security)
        {
            _parent = parent;

            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => this.CanExecute);
            PrintClicked = new RelayCommand(ExecutePrintClicked, param => this.CanPrintSales);
            ExportClicked = new RelayCommand(ExecuteExportClicked, param => this.CanPrintSales);

            DateTime startdate = DateTime.Today;


            ReportDates = new List<ReportDate>();
            m_currentdate = new ReportDate();
            _dbemployees = new DBEmployee();
            _employees = new ObservableCollection<Employee>();
            _serversonly = true;
            _showall = false;

            LoadServers();

            FillDateList();
            SelectedDateID = 0;  //this will set selected date to today



        }


        public bool CanPrintSales
        {
            get
            {
                if (SelectedIndex == 3) return false; else return true;
            }
        }

        public void LoadServers()
        {
            DataTable tbl;
            Employee current;
            ObservableCollection<Employee> employees;
            employees = new ObservableCollection<Employee>();

            if (_showall) tbl = _dbemployees.GetEmployeeAll();
            else tbl = _dbemployees.GetEmployeeActive(ServersOnly?"Server":"");

            foreach (DataRow row in tbl.Rows)
            {
                // current = new Employee(int.Parse(row["id"].ToString()));
                current = new Employee(row);
                employees.Add(current);
            }

            Employees = employees;

        }
        private void FillDateList()
        {
            DateTime startdate;
            ReportDate newdate;
            startdate = DateTime.Today;
            int j = 0;


            DateTime firstday = GlobalSettings.Instance.PayPeriodStartDate;
            int diff = (int)DateTime.Now.DayOfWeek - (int)firstday.DayOfWeek;


            //default is always today's date
            newdate = new ReportDate();
            newdate.ReportDateID = j;
            newdate.StartDate = DateTime.Today;
            newdate.EndDate = DateTime.Today;
            ReportDates.Add(newdate);
            j++;



            switch (GlobalSettings.Instance.PayPeriodType.ToUpper())
            {

                case "SEMIMONTHLY":

                    for (int i = 1; i <= 12; i++)
                    {


                        newdate = new ReportDate();
                        newdate.ReportDateID = j;
                        newdate.StartDate = new DateTime(startdate.Year, startdate.Month, 16);
                        newdate.EndDate = newdate.StartDate.AddMonths(1).AddDays(-16);
                        ReportDates.Add(newdate);
                        j++;


                        newdate = new ReportDate();
                        newdate.ReportDateID = j;
                        newdate.StartDate = new DateTime(startdate.Year, startdate.Month, 1);
                        newdate.EndDate = new DateTime(startdate.Year, startdate.Month, 15);
                        ReportDates.Add(newdate);
                        j++;


                        startdate = startdate.AddMonths(-1);
                    }

                    break;

                case "WEEKLY":

                    //difference between today and the first day of pay week
                    if (diff >= 0)
                    {
                        //positive or equal ..that means the start
                        startdate = DateTime.Now.AddDays(-diff);

                    }
                    else
                    {
                        startdate = DateTime.Now.AddDays((diff * -1) - 7);
                    }

                    for (int i = 1; i <= 12; i++)
                    {


                        newdate = new ReportDate();
                        newdate.ReportDateID = j;
                        newdate.StartDate = startdate;
                        newdate.EndDate = startdate.AddDays(6);
                        ReportDates.Add(newdate);
                        j++;

                        startdate = startdate.AddDays(-7);
                    }


                    break;

                case "BIWEEKLY":


                    startdate = GlobalSettings.Instance.PayPeriodStartDate;
                    TimeSpan datediff = DateTime.Now.Subtract(startdate);
                    int totalcount = (int)datediff.TotalDays / 14;
                    startdate = startdate.AddDays(totalcount * 14);

                    for (int i = 1; i <= 12; i++)
                    {


                        newdate = new ReportDate();
                        newdate.ReportDateID = j;
                        newdate.StartDate = startdate;
                        newdate.EndDate = startdate.AddDays(14);
                        ReportDates.Add(newdate);
                        j++;

                        startdate = startdate.AddDays(-7);
                    }
                    break;


            }


        }

        public int SelectedIndex
        {
            get { return m_selectedindex; }
            set
            {
                m_selectedindex = value;
                NotifyPropertyChanged("SelectedIndex");
            }
        }


        public int SelectedDateID
        {
            get { return m_selecteddateid; }
            set
            {
                m_selecteddateid = value;
                
                foreach (ReportDate rptdate in ReportDates)
                {
                    if (rptdate.ReportDateID == m_selecteddateid)
                    {
                        CurrentDate = rptdate;
                        RunReports();
                   
                    }
                }
                

                NotifyPropertyChanged("SelectedDateID");
            }
        }


        private ObservableCollection<Employee> _employees;
        public ObservableCollection<Employee> Employees
        {
            get { return _employees; }
            set { _employees = value; NotifyPropertyChanged("Employees"); }
        }

        private bool _showall;
        public bool ShowAll
        {
            get { return _showall; }
            set
            {
                _showall = value;
                NotifyPropertyChanged("ShowAll");
                LoadServers();
            }
        }

        private bool _serversonly;
        public bool ServersOnly
        {
            get { return _serversonly; }
            set
            {
                _serversonly = value;
                NotifyPropertyChanged("ServersOnly");
                LoadServers();
            }
        }




        private Employee _employee;
        public Employee CurrentEmployee
        {
            get { return _employee; }
            set
            {
                _employee = value;
                NotifyPropertyChanged("CurrentEmployee");
            }
        }

        public DateTime StartDate
        {
            get { return _startdate; }
            set
            {
                _startdate = value;
                NotifyPropertyChanged("StartDate");
            }
        }

        public DateTime EndDate
        {
            get { return _enddate; }
            set
            {
                _enddate = value;
                NotifyPropertyChanged("EndDate");
            }
        }


        private decimal _totaltips;
        public decimal TotalTips
        {
            get { return _totaltips; }
            set
            {
                _totaltips = value;
                NotifyPropertyChanged("TotalTips");
            }
        }


        private decimal _totalsales;
        public decimal TotalSales
        {
            get { return _totalsales; }
            set
            {
                _totalsales = value;
                NotifyPropertyChanged("TotalSales");
            }
        }

        private decimal _totaltickets;
        public decimal TotalTickets
        {
            get { return _totaltickets; }
            set
            {
                _totaltickets = value;
                NotifyPropertyChanged("TotalTickets");
            }
        }

        private decimal _totalcash;
        public decimal TotalCash
        {
            get { return _totalcash; }
            set
            {
                _totalcash = value;
                NotifyPropertyChanged("TotalCash");
            }
        }

        private decimal _totalcredit;
        public decimal TotalCredit
        {
            get { return _totalcredit; }
            set
            {
                _totalcredit = value;
                NotifyPropertyChanged("TotalCredit");
            }
        }


        private decimal _totaldebit;
        public decimal TotalDebit
        {
            get { return _totaldebit; }
            set
            {
                _totaldebit = value;
                NotifyPropertyChanged("TotalDebit");
            }
        }



        private decimal _totalother;
        public decimal TotalOther
        {
            get { return _totalother; }
            set
            {
                _totalother = value;
                NotifyPropertyChanged("TotalOther");
            }
        }
  


        private void RunReports()
        {
            EmployeeList = new ObservableCollection<Employee>();


            if (_currentemployeeid == 999)
            {
                foreach (Employee emp in Employees)
                {
                    CurrentEmployee = emp;
                    if (CurrentEmployee != null) CurrentEmployee.RunReports(CurrentDate.StartDate, CurrentDate.EndDate);
                    EmployeeList.Add(CurrentEmployee);
                }


            }
            else
            {
                CurrentEmployee = new Employee(_currentemployeeid);
                if (CurrentEmployee != null) CurrentEmployee.RunReports(CurrentDate.StartDate, CurrentDate.EndDate);
                EmployeeList.Add(CurrentEmployee);
            }

            decimal totaltickets = 0;
            decimal totalsales = 0;
            decimal totaltips = 0;
            decimal totalcash = 0;
            decimal totalcredit = 0;
            decimal totaldebit = 0;
            decimal totalother = 0;


            foreach(Employee emp in EmployeeList)
            {
                totaltickets += emp.EmployeeSales.TotalTickets;
                totalsales += emp.EmployeeSales.TotalSales;
                totaltips += emp.EmployeeSales.TotalTips;
                totalcash += emp.EmployeeSales.TotalCash;
                totalcredit += emp.EmployeeSales.TotalCredit;
                totaldebit += emp.EmployeeSales.TotalDebit;
                totalother += emp.EmployeeSales.TotalOther;
               
            }


            TotalTickets = totaltickets;
            TotalSales = totalsales;
            TotalTips = totaltips;
            TotalCash = totalcash;
            TotalCredit = totalcredit;
            TotalDebit = totaldebit;
            TotalOther = totalother;
          
        }



        public void ExecuteEmployeeClicked(object employeeid)
        {
            if (employeeid != null)
            {


                _currentemployeeid = int.Parse(employeeid.ToString());
                RunReports();

            }
        }

        public void ExecutePrintClicked(object obj)
        {
            switch (SelectedIndex)
            {
                case 0:
                    ReceiptPrinterModel.PrintEmployeeSales(CurrentEmployee, CurrentDate.StartDate, CurrentDate.EndDate, true); //summary
                    break;
                case 1:
                    ReceiptPrinterModel.PrintEmployeeSales(CurrentEmployee, CurrentDate.StartDate, CurrentDate.EndDate);
                    break;
                case 2:
                    
                    break;

            }
               
         
                
        }

        public void ExecuteExportClicked(object obj)
        {
            try
            {

                switch (SelectedIndex)
                {
                    case 0:

                        SaveFileDialog ofd = new SaveFileDialog();
                        ofd.DefaultExt = "csv";
                        ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                        ofd.ShowDialog();
                        if (ofd.FileName != "")
                            CSVWriter.WriteDataTable(CurrentEmployee.EmployeeSalesSummary.Sales, ofd.FileName, true); //summary
                        break;

                    case 1:

                        SaveFileDialog ofd2 = new SaveFileDialog();
                        ofd2.DefaultExt = "csv";
                        ofd2.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                        ofd2.ShowDialog();
                        if (ofd2.FileName != "")
                            CSVWriter.WriteDataTable(CurrentEmployee.EmployeeSales.Sales, ofd2.FileName, true);
                        break;


                    case 2:
                        // CSVWriter.WriteDataTable(EmployeeList, ofd.FileName, true);
                        break;

                }


            }
            catch (Exception e)
            {
                MessageBox.Show("Sales Export:" + e.Message);
            }

        }
    }
}
