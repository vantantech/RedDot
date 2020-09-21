using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace RedDot
{
    public class EmployeeVM:INPCBase
    {

        private Employee m_employee;
        private DBEmployee _dbemployee=new DBEmployee();
        private SecurityModel m_security;

        private int _currentemployeeid;
        public ICommand DeleteEmployeeClicked { get; set; }
        public ICommand PictureClicked { get; set; }
        public ICommand MSRSetClicked { get; set; }
        public ICommand PreviousPeriodClicked { get; set; }
        public ICommand NextPeriodClicked { get; set; }
        public ICommand PrintHoursClicked { get; set; }
        public ICommand ExportHoursClicked { get; set; }
        public ICommand EditHoursClicked { get; set; }
        public ICommand DeleteHoursClicked { get; set; }
        public ICommand AddHoursClicked { get; set; }
        public ICommand ClockinClicked { get; set; }
        public ICommand ClockoutClicked { get; set; }
        public ICommand EmployeeClicked { get; set; }
        public ICommand BackClicked { get; set; }
        public ICommand EnrollFingerPrintClicked { get; set; }
        public ICommand PrintSalesClicked { get; set; }
        public ICommand ExportSalesClicked { get; set; }


        private bool CanExecute = true;
        private bool CanDelete = false;
        private Window m_parent;

        private DateTime m_startdate;
        private DateTime m_enddate;
        private int m_selectedindex;

        private List<ReportDate> m_reportdates;
        private ReportDate m_currentdate;
        private int m_selecteddateid;

        public List<ListPair> SecurityLevels { get; set; }
        public List<ListPair> Roles { get; set; }

        public Visibility VisibleAdmin { get; set; }
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

        public bool IsAdmin {get; set;}

        private int _currrentemployeeid;


        public EmployeeVM(Window parent, SecurityModel security, Employee employee, bool candelete, bool admin)
        {
            m_employee = employee;
            m_parent = parent;
            m_security = security;
            CanDelete = candelete;


            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => this.CanExecute);
            DeleteEmployeeClicked = new RelayCommand(ExecuteDeleteEmployeeClicked, param => this.CanDeleteEmployee);
            PictureClicked = new RelayCommand(ExecutePictureClicked, param => this.CanExecute);

            MSRSetClicked = new RelayCommand(ExecuteMSRSetClicked, param => this.CanExecute);
            PreviousPeriodClicked = new RelayCommand(ExecutePreviousPeriodClicked, param => this.CanExecute);
            NextPeriodClicked = new RelayCommand(ExecuteNextPeriodClicked, param => this.CanExecute);
            PrintHoursClicked = new RelayCommand(ExecutePrintHoursClicked, param => this.CanExecute);
            ExportHoursClicked = new RelayCommand(ExecuteExportHoursClicked, param => this.CanExecute);
            EditHoursClicked = new RelayCommand(ExecuteEditHoursClicked, param => IsAdmin);
            DeleteHoursClicked = new RelayCommand(ExecuteDeleteHoursClicked, param => IsAdmin);
            AddHoursClicked = new RelayCommand(ExecuteAddHoursClicked, param => IsAdmin);
            BackClicked = new RelayCommand(ExecuteBackClicked, param => true);
            ClockinClicked = new RelayCommand(ExecuteClockinClicked, param => this.CanClockIn);
            ClockoutClicked = new RelayCommand(ExecuteClockoutClicked, param => this.CanClockOut);
            EnrollFingerPrintClicked = new RelayCommand(ExecuteEnrollFingerPrintClicked, param => this.CanExecute);
            PrintSalesClicked = new RelayCommand(ExecutePrintSalesClicked, param => this.CanPrintSales);
            ExportSalesClicked = new RelayCommand(ExecuteExportSalesClicked, param => this.CanPrintSales);

            DateTime startdate = DateTime.Today;


            ReportDates = new List<ReportDate>();
            m_currentdate = new ReportDate();


            FillDateList();
            SelectedDateID = 0;  //this will set selected date to today

            if (admin) VisibleAdmin = Visibility.Visible;
            else VisibleAdmin = Visibility.Collapsed;

            IsAdmin = admin;


            //security selection list
            SecurityLevels = new List<ListPair>();
            SecurityLevels.Add(new ListPair() { Description = "Employee", Value = 10 });
            SecurityLevels.Add(new ListPair() { Description = "Manager", Value = 50 });
            SecurityLevels.Add(new ListPair() { Description = "Admin/Owner", Value = 100 });

            Roles = new List<ListPair>();
            Roles.Add(new ListPair() { Description = "General Manager", StrValue= "General Manager" });
            Roles.Add(new ListPair() { Description = "Assistant Manager", StrValue = "Assistant Manager" });
            Roles.Add(new ListPair() { Description = "Line Cook", StrValue = "Line Cook" });
            Roles.Add(new ListPair() { Description = "Dishwasher", StrValue = "Dishwasher" });
            Roles.Add(new ListPair() { Description = "Drive-Thru Operator", StrValue = "Drive-Thru Operator" });
            Roles.Add(new ListPair() { Description = "Fast Food Cook", StrValue = "Fast Food Cook" });
            Roles.Add(new ListPair() { Description = "Cashier", StrValue = "Cashier" });
            Roles.Add(new ListPair() { Description = "Short Order Cook", StrValue = "Short Order Cook" });
            Roles.Add(new ListPair() { Description = "Barista", StrValue = "Barista" });
            Roles.Add(new ListPair() { Description = "Kitchen Manager", StrValue = "Kitchen Manager" });
            Roles.Add(new ListPair() { Description = "Server", StrValue = "Server" });
            Roles.Add(new ListPair() { Description = "Prep Cook", StrValue = "Prep Cook" });
            Roles.Add(new ListPair() { Description = "Runner", StrValue = "Runner" });
            Roles.Add(new ListPair() { Description = "Busser", StrValue = "Busser" });
            Roles.Add(new ListPair() { Description = "Host", StrValue = "Host" });
            Roles.Add(new ListPair() { Description = "Bartender", StrValue = "Bartender" });

            Roles = Roles.OrderBy(x => x.Description).ToList();
            SelectedIndex = 0;
            _currentemployeeid = 0;

            LoadEmployees();

        }

        private ObservableCollection<Employee> _employees;
        public ObservableCollection<Employee> Employees
        {
            get { return _employees; }
            set { _employees = value; NotifyPropertyChanged("Employees"); }
        }

        private bool _showall = false;
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


        public void LoadEmployees()
        {
            DataTable tbl;
            Employee current;
            ObservableCollection<Employee> employees;
            employees = new ObservableCollection<Employee>();

            if (_showall) tbl = _dbemployee.GetEmployeeAll();
            else tbl = _dbemployee.GetEmployeeActive();

            foreach (DataRow row in tbl.Rows)
            {
                // current = new Employee(int.Parse(row["id"].ToString()));
                current = new Employee(row);
                employees.Add(current);
            }

            Employees = employees;
            GlobalSettings.Instance.LoadAllFmdsUserIDs();

        }


        public bool CanPrintSales
        {
            get
            {
                if (SelectedIndex == 3) return false; else return true;
            }
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


        public int SelectedDateID
        {
            get { return m_selecteddateid; }
            set
            {
                EmployeeList = new ObservableCollection<Employee>();
                m_selecteddateid = value;

                CurrentDate = ReportDates.Where(x => x.ReportDateID == m_selecteddateid).FirstOrDefault();

                if (CurrentEmployee != null) CurrentEmployee.LoadHours(CurrentDate.StartDate, CurrentDate.EndDate);
                if (CurrentEmployee != null) CurrentEmployee.GetSales(CurrentDate.StartDate, CurrentDate.EndDate);
                if (CurrentEmployee != null) CurrentEmployee.GetSalesSummary(CurrentDate.StartDate, CurrentDate.EndDate);

                EmployeeList.Add(CurrentEmployee);

                NotifyPropertyChanged("SelectedDateID");
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

        public bool CanClockIn
        {
            get
            {
                if (CurrentEmployee == null) return false;
                if ((CurrentEmployee.ClockinTime == "" && CurrentEmployee.ClockoutTime == "") || (CurrentEmployee.ClockinTime != "" && CurrentEmployee.ClockoutTime != "")) return true;
                else return false;
            }
        }

        public bool CanClockOut
        {
            get
            {
                if (CurrentEmployee == null) return false;
                return CurrentEmployee.ClockedIn;

            }
        }

        public bool CanDeleteEmployee
        {
            get
            {
                if (m_employee == null) return false;

                if (this.CanDelete)
                {

                    if (SalesModel.GetSalesCount(m_employee.ID) > 0)
                        return false;
                    else
                        return true;

                }
                else return false;




            }

        }


        public Employee CurrentEmployee
        {
            get { return m_employee; }
            set
            {
                m_employee = value;
                NotifyPropertyChanged("CurrentEmployee");
            }
        }
        public DateTime StartDate
        {
            get { return m_startdate; }
            set
            {
                m_startdate = value;
                NotifyPropertyChanged("StartDate");
            }
        }

        public DateTime EndDate
        {
            get { return m_enddate; }
            set
            {
                m_enddate = value;
                NotifyPropertyChanged("EndDate");
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


        public void ExecutePreviousPeriodClicked(object button)
        {
            StartDate = StartDate.AddDays(-7);
            EndDate = StartDate.AddDays(6);
            if (CurrentEmployee != null) CurrentEmployee.RunReports(CurrentDate.StartDate, CurrentDate.EndDate);

        }

        public void ExecuteNextPeriodClicked(object button)
        {
            StartDate = StartDate.AddDays(7);
            EndDate = StartDate.AddDays(6);
            if (CurrentEmployee != null) CurrentEmployee.RunReports(CurrentDate.StartDate, CurrentDate.EndDate);

        }


        public void ExecuteEmployeeClicked(object employeeid)
        {
            EmployeeList = new ObservableCollection<Employee>();

            if (employeeid != null)
            {


                _currentemployeeid = int.Parse(employeeid.ToString());
                CurrentEmployee = new Employee(_currentemployeeid);
                if (CurrentEmployee != null) CurrentEmployee.RunReports(CurrentDate.StartDate, CurrentDate.EndDate);
        
                EmployeeList.Add(CurrentEmployee);
            }
        }


        public void ExecutePrintHoursClicked(object button)
        {
            ReceiptPrinterModel.PrintEmployeeHours(CurrentEmployee);

        }

        public void ExecuteExportHoursClicked(object param)
        {
            try
            {
                if (CurrentEmployee == null && param.ToString() == "1") return;


                SaveFileDialog ofd = new SaveFileDialog();
                ofd.DefaultExt = "csv";
                ofd.Filter = "CSV Files (*.csv)|*.csv|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                var result = ofd.ShowDialog();

                if(param.ToString() == "0")
                {
                    DataTable dt = _dbemployee.GetEmployeeHours(0, CurrentDate.StartDate, CurrentDate.EndDate);
                    if (ofd.FileName != "") CSVWriter.WriteDataTable(dt, ofd.FileName, true);
                } else
                {
                    if (ofd.FileName != "") CSVWriter.WriteDataTable(CurrentEmployee.Hours, ofd.FileName, true);
                }
          
            }catch(Exception e)
            {
                MessageBox.Show("Hours Export:" + e.Message);
            }
   
        }

        public void ExecuteClockinClicked(object salesid)
        {

            if (CurrentEmployee.ClockIn(DateTime.Now, false))
                TouchMessageBox.Show(CurrentEmployee.FullName + "\n\rClock In Time:" + CurrentEmployee.ClockinTime);

            if (CurrentEmployee != null) CurrentEmployee.LoadHours(CurrentDate.StartDate, CurrentDate.EndDate);

        }


        public void ExecuteClockoutClicked(object salesid)
        {

            CurrentEmployee.ClockOut(DateTime.Now);

            if (CurrentEmployee != null) CurrentEmployee.LoadHours(CurrentDate.StartDate, CurrentDate.EndDate);

        }

        public void ExecuteAddHoursClicked(object employeeid)
        {
            try
            {
                int id;
                if (employeeid == null) return;

                if (employeeid.ToString() != "") id = int.Parse(employeeid.ToString());
                else id = 0;

                var dbemployee = new DBEmployee();
                dbemployee.InsertTimeRecord(id, CurrentDate.StartDate.ToString());


                if (CurrentEmployee != null) CurrentEmployee.RunReports(CurrentDate.StartDate, CurrentDate.EndDate);



            }
            catch (Exception e)
            {
                MessageBox.Show("Delete Time Record:" + e.Message);
            }

        }


        public void ExecuteDeleteHoursClicked(object timeid)
        {
            try
            {
                int id;
                if (timeid == null) return;

                if (timeid.ToString() != "") id = int.Parse(timeid.ToString());
                else id = 0;

                var dbemployee = new DBEmployee();
                Confirm popup = new Confirm("Delete Time Record?");
                Utility.OpenModal(m_parent, popup);


                if (popup.Action == "Yes")
                {
                    dbemployee.DeleteTimeRecord(id);
                    if (CurrentEmployee != null) CurrentEmployee.RunReports(CurrentDate.StartDate, CurrentDate.EndDate);


                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Delete Time Record:" + e.Message);
            }

        }
        public void ExecuteEditHoursClicked(object timeid)
        {
            try
            {
                int id;
                if (timeid == null) return;

                if (timeid.ToString() != "") id = int.Parse(timeid.ToString());
                else id = 0;

                var dbemployee = new DBEmployee();
                var timedata = dbemployee.GetTimeRecord(id);
                if (timedata != null)
                {
                    DateTime TimeIn = DateTime.MinValue;
                    DateTime TimeOut = DateTime.MinValue;
                    if (timedata["Timein"].ToString() != "") TimeIn = (DateTime)timedata["Timein"];
                    if (timedata["timeout"].ToString() != "") TimeOut = (DateTime)timedata["timeout"];

                    //open edit pop up
                    EditHours popup = new EditHours(TimeIn,TimeOut , timedata["note"].ToString());
                    Utility.OpenModal(m_parent, popup);

                    // save hours
                    if (popup.Action == "Save")
                    {
                        dbemployee.SaveTimeRecord(id,popup.InTime ==null?"": popup.InTime.ToString(),popup.OutTime ==null?"": popup.OutTime.ToString(), popup.Note);
                    }

                }


                if (CurrentEmployee != null) CurrentEmployee.RunReports(CurrentDate.StartDate, CurrentDate.EndDate);

            }
            catch (Exception e)
            {
                MessageBox.Show("Edit Time Record:" + e.Message);
            }




        }



        public void ExecuteMSRSetClicked(object button)
        {
            CardScanner crd = new CardScanner();
            Utility.OpenModal(m_parent, crd);
            if (crd.CardNumber != "")
            {
                CurrentEmployee.MSRCard = crd.CardNumber;

            }

        }
        public void ExecutePictureClicked(object button)
        {
            var picfile = Utility.GetPictureFile();

            //will return null if user cancels
            if (picfile != null) CurrentEmployee.ImageSrc = picfile;
            

        }
        public void ExecuteDeleteEmployeeClicked(object button)
        {

            if (m_employee.DeleteEmployee())
            {
                MessageBox.Show("Deleted Successfully");

                CurrentEmployee = null;
                m_parent.Close();
            }
            else MessageBox.Show("Delete Fail");


        }

        public void ExecuteBackClicked(object butt)
        {
            m_employee.SaveSchedule();
            m_parent.Close();
        }
  

        public void ExecuteEnrollFingerPrintClicked(object obj)
        {

            if (m_security.WindowAccess("BackOffice"))
            {
                EnrollFingerPrint ep = new EnrollFingerPrint(CurrentEmployee);
                Utility.OpenModal(m_parent, ep);

                //Loads finger print database into memory
                GlobalSettings.Instance.LoadAllFmdsUserIDs();
            }
               

        }

        public void ExecutePrintSalesClicked(object obj)
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


        public void ExecuteExportSalesClicked(object obj)
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
