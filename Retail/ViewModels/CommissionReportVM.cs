using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace RedDot
{
    public class CommissionReportVM:EmployeeListViewModel
    {

        public Employee CurrentEmployee { get; set; }

        private ObservableCollection<EmployeeSalesData> _currentreport;
      public ICommand EmployeeClicked { get; set; }
      public ICommand ViewTicketClicked { get; set; }

      public ICommand PrintCommissionClicked { get; set; }
      public ICommand PrintCommissionLargeClicked { get; set; }
      public ICommand ExportCommissionCSVClicked { get; set; }

 

      public ICommand TodayClicked { get; set; }

      public ICommand CustomClicked { get; set; }

      private decimal _grandtotalsales = 0;
      private decimal _grandtotalcost = 0;
      private decimal _grandtotalcommission = 0;
        private decimal _grandtotaladjustment = 0;
        private decimal _grandtotalcommissionadjustment = 0;
        private decimal _grandnetcommission = 0;
      private decimal _grandtotalmargin = 0;
      private Reports _reports;
      private int _currentemployeeid = 0;
      private bool CanExecute = true;
      private List<ReportDate> m_reportdates;
      private ReportDate m_currentdate;
      private int m_selecteddateid;
      private bool m_enablereportdates;
      public ReportDate CurrentDate { get { return m_currentdate; }
          set {
              m_currentdate = value;
              NotifyPropertyChanged("CurrentDate");
          }
      }
      public List<ReportDate> ReportDates {
          get { return m_reportdates; }

          set { m_reportdates = value;
          NotifyPropertyChanged("ReportDates");
          }
      }


        public bool EnableReportDates
      {
          get { return m_enablereportdates; }
          set
          {
              m_enablereportdates = value;
              NotifyPropertyChanged("EnableReportDates");
          }
      }

      private Window m_parent;
      private Security m_security;
        public CommissionReportVM(Window parent, Security security, int employeeid =0)
        {
            EmployeeClicked                      = new RelayCommand(ExecuteEmployeeClicked, param => this.CanExecute);
            ExportCommissionCSVClicked           = new RelayCommand(ExecuteExportCommissionCSVClicked, param => this.CanClick);
            PrintCommissionClicked               = new RelayCommand(ExecutePrintCommissionClicked, param => this.CanClick);
            PrintCommissionLargeClicked          = new RelayCommand(ExecutePrintCommissionLargeClicked, param => this.CanClick);
            TodayClicked                         = new RelayCommand(ExecuteTodayClicked, param => this.CanClick);
            CustomClicked                        = new RelayCommand(ExecuteCustomClicked, param => this.CanClick);
            ViewTicketClicked                       = new RelayCommand(ExecuteViewTicketClicked, param => this.CanExecute);

            CurrentEmployee                      = new Employee(employeeid);
            _currentemployeeid                   = employeeid;
            m_parent = parent;
            m_security = security;

            _reports                             = new Reports();
            ReportDates                          = new List<ReportDate>();
            m_currentdate                        = new ReportDate();
            m_currentdate.StartDate              = DateTime.Today;
            m_currentdate.EndDate                = DateTime.Today;

            if (employeeid == 0) CurrentEmployee.DisplayName = "All Employees";


    
            if (employeeid != 0)
            {
                CurrentReport                    = GetCommission((int)employeeid,m_currentdate);
            }

            FillDateList();
            SelectedDateID                       = 1;
        }

        private void FillDateList()
        {
            DateTime startdate;
            ReportDate newdate;
            startdate = DateTime.Today;
            int j = 1;


            newdate = new ReportDate();
            newdate.ReportDateID = j;
            newdate.StartDate = startdate;
            newdate.EndDate = startdate;
            ReportDates.Add(newdate);
            j++;


            int diff = (int)DateTime.Now.DayOfWeek - GlobalSettings.Instance.PayPeriodStartDay;

            switch(GlobalSettings.Instance.PayPeriodType.ToUpper())
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
                        newdate.EndDate = startdate.AddDays(13);
                        ReportDates.Add(newdate);
                        j++;

                        startdate = startdate.AddDays(-14);
                    }
                    break;


            }


        }

        public int SelectedDateID
        {
            get { return m_selecteddateid; }
            set
            {
                m_selecteddateid = value;
                foreach(ReportDate rptdate in ReportDates)
                {
                    if (rptdate.ReportDateID == m_selecteddateid)
                    {
                        CurrentDate = rptdate;
                        if(_currentemployeeid > 0)
                        {
                            CurrentReport = GetCommission(_currentemployeeid, rptdate);
                            CalculateTotals();
                        }

                    }
                }


            NotifyPropertyChanged("SelectedDateID");
            }
        }

        public decimal GrandTotalCommission { get { return _grandtotalcommission; } set { _grandtotalcommission = value; NotifyPropertyChanged("GrandTotalCommission"); } }
        public decimal GrandNetCommission { get { return _grandnetcommission; } set { _grandnetcommission = value; NotifyPropertyChanged("GrandNetCommission"); } }

        public decimal GrandTotalAdjustment { get { return _grandtotaladjustment; } set { _grandtotaladjustment = value; NotifyPropertyChanged("GrandTotalAdjustment"); } }
        public decimal GrandTotalCommissionAdjustment { get { return _grandtotalcommissionadjustment; } set { _grandtotalcommissionadjustment = value; NotifyPropertyChanged("GrandTotalCommissionAdjustment"); } }
        public decimal GrandTotalMargin { get { return _grandtotalmargin; } set { _grandtotalmargin = value; NotifyPropertyChanged("GrandTotalMargin"); } }




        public decimal GrandTotalSales
        {

            get { return _grandtotalsales; }

            set { _grandtotalsales = value; NotifyPropertyChanged("GrandTotalSales");   }

        }


        public decimal GrandTotalCost
        {
            get { return _grandtotalcost; }
            set { _grandtotalcost = value; NotifyPropertyChanged("GrandTotalCost"); }
        }

        public ObservableCollection<EmployeeSalesData> CurrentReport
        {
            get { return _currentreport; }

            set { _currentreport = value; NotifyPropertyChanged("CurrentReport"); }
        }
        public ObservableCollection<EmployeeSalesData> GetCommission(int id, ReportDate currentdate)
        {


            ObservableCollection<EmployeeSalesData> allemployeesales;
            DBEmployee dbemployee = new DBEmployee();
            EmployeeSalesData employeecommission;

            if(id > 0)  CurrentEmployee = new Employee(id);

            //if integer = 1000 , then its for all employees , otherwise, just one
            // employee 999 is all employee combined so still consider as one employee
            allemployeesales = new ObservableCollection<EmployeeSalesData>();

            

            if (id == 1000)
            {

              // dt = dbemployee.GetEmployeeAll();

               DataTable dt = _reports.GetWorkedEmployees(currentdate.StartDate, currentdate.EndDate);

                //collection of all employees , each one with a SalesDataList

                if(dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        employeecommission = new EmployeeSalesData((int)row["employeeid"], currentdate.StartDate, currentdate.EndDate);
                        employeecommission.SubTotalVisiblity = Visibility.Visible;
                        if(employeecommission.GrandTotalSales > 0)  allemployeesales.Add(employeecommission);

                    }

                    return allemployeesales;
                }
                else return null;


            }else //single employee
            {
                if (id > 0)
                {
                    employeecommission = new EmployeeSalesData(id,currentdate.StartDate,currentdate.EndDate);
                    allemployeesales.Add(employeecommission);

                    return allemployeesales;

                }
                else
                    if (CurrentEmployee.ID > 0)
                    {

                        employeecommission = new EmployeeSalesData(CurrentEmployee.ID, currentdate.StartDate, currentdate.EndDate);
                        allemployeesales.Add(employeecommission);

                        return allemployeesales;
                    }
                    else return null;


            }

          

        }

        public bool CanClick
        {
            get
            {
                if (_currentemployeeid ==0) return false; else return true;

            }
    }
        //-------------------------------------Public Methods -----------------------------------------



        public void ExecutePrintCommissionClicked(object tagstr)
        {

                  _reports.PrintCommission(CurrentReport, m_currentdate.ReportString);

        }


        public void ExecutePrintCommissionLargeClicked(object tagstr)
        {

            _reports.PrintCommissionLarge(CurrentReport, m_currentdate.ReportString);

        }


        public void ExecuteExportCommissionCSVClicked(object tagstr)
        {


            _reports.ExportCommissionCSV(CurrentReport[0].CurrentEmployee.ID, m_currentdate.StartDate, m_currentdate.EndDate);

        }



        public void ExecuteTodayClicked(object tagstr)
        {
            SelectedDateID = 1;
         //  m_currentdate.StartDate = DateTime.Today;
         //  m_currentdate.EndDate = DateTime.Today;
      

            CurrentReport = GetCommission(_currentemployeeid,m_currentdate);

            CalculateTotals();

        }

        public void ExecuteViewTicketClicked(object salesid)
        {

            try
            {
                int id;

                if (salesid == null) return;

                if (salesid.ToString() != "") id = int.Parse(salesid.ToString());
                else id = 0;

                RetailSalesView ord = new RetailSalesView(m_security, id);
                Utility.OpenModal(m_parent, ord);

                CurrentReport = GetCommission(_currentemployeeid, m_currentdate);
                CalculateTotals();
            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteEditClicked: " + e.Message);
            }
        }

        public void ExecuteCustomClicked(object tagstr)
        {

            CustomDate cd = new CustomDate(Visibility.Visible, DateTime.Now);
            cd.ShowDialog();
            //Utility.OpenModal(this, cd);

            m_currentdate.StartDate = cd.StartDate;
            m_currentdate.EndDate = cd.EndDate;

            CurrentReport = GetCommission(_currentemployeeid, m_currentdate);

            CalculateTotals();

        }
        public void ExecuteEmployeeClicked(object employeeid)
        {
            if (employeeid != null)
            {

                
                _currentemployeeid = int.Parse(employeeid.ToString());
                CurrentReport = GetCommission(_currentemployeeid, m_currentdate);
                CalculateTotals();
                EnableReportDates = true;
            }
        }

        public void CalculateTotals()
        {

            decimal totalsales = 0;
            decimal totalcost = 0;
            decimal totalmargin = 0;
            decimal totalcommission = 0;
            decimal totaladjustment = 0;
            decimal totalcommissionadjustment = 0;

            if (CurrentReport == null) return;

                foreach(EmployeeSalesData subreport in CurrentReport)
                {

                    if((subreport.CurrentEmployee.ID < 999 && _currentemployeeid == 1000) || _currentemployeeid < 1000 )
                    {
                        foreach (SalesData salesdata in subreport.EmployeeSales)
                        {
                            totalsales = totalsales + salesdata.TotalSales;
                            totalcost = totalcost + salesdata.TotalCost;
                            totalcommission = totalcommission + salesdata.TotalCommission;
                            totalmargin = totalmargin + salesdata.TotalMargin;
                            totalcommissionadjustment += salesdata.TotalCommissionAdjustment;
                            totaladjustment += salesdata.TotalAdjustments;

                        }
                        subreport.CalculateTotals();
                    }


                }






            GrandTotalSales = totalsales;
            GrandTotalCost = totalcost;
            GrandTotalCommission = totalcommission;
            GrandTotalMargin = totalmargin;
            GrandTotalAdjustment = totaladjustment;
            GrandTotalCommissionAdjustment = totalcommissionadjustment;
            GrandNetCommission = GrandTotalCommission - GrandTotalCommissionAdjustment;



        }

    }





}
