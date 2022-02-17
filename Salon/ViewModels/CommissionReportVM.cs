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
using RedDot.DataManager;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Data;

namespace RedDot
{
    public class CommissionReportVM:EmployeeListViewModel
    {

      public Employee CurrentEmployee { get; set; }

      private ObservableCollection<EmployeeCommissionVM> _currentreport;

        public ICommand EmployeeClicked { get; set; }

        public ICommand PrintCommissionDetailsClicked { get; set; }
        public ICommand PrintCommissionClicked { get; set; }
      public ICommand PrintCommissionSummaryClicked { get; set; }

      public ICommand PrintCommissionDailyClicked { get; set; }

      public ICommand ExportCommissionCSVClicked { get; set; }
      public ICommand ExportCommissionCSVDailyClicked { get; set; }
      public ICommand ExportCommissionCSVSummaryClicked { get; set; }

      public ICommand PreviousDayClicked { get; set; }
      public ICommand NextDayClicked { get; set; }

      public ICommand TodayClicked { get; set; }

      public ICommand CustomClicked { get; set; }


      private decimal _grandtotalsales = 0;
      private decimal _grandtotalcommission = 0;
        private decimal _grandtotaldailyfee = 0;
        private decimal _grandtotalgratuities = 0;
      private decimal _grandtotalnetgratuities = 0;
      private decimal _grandtotalsupplyfee = 0;
      private decimal _custom1 = 0;
      private decimal _custom2 = 0;
      private decimal _custom3 = 0;

      private Reports _reports;
      private int _currentid = 0;
      private bool CanExecute = true;
      private List<ReportDate> m_reportdates;
      private ReportDate m_currentdate;
      private int m_selecteddateid;
      private bool m_cut = false;
 


      public ReportDate CurrentDate {
          get { return m_currentdate; }
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


      public bool Cut
      {
          get { return m_cut; }
          set
          {
              m_cut= value;
              NotifyPropertyChanged("Cut");
           
          }
      }


        private bool m_isbusy = false;
        public bool IsBusy
        {
            get { return m_isbusy; }
            set
            {
                m_isbusy = value;
                NotifyPropertyChanged("IsBusy");
            }
        }


        private string m_reporttext;
        private object _sentenceLock;

        public string ReportText
        {
            get { return m_reporttext; }
            set
            {
                m_reporttext = value;
                NotifyPropertyChanged("ReportText");
            }
        }

        public CommissionReportVM(SecurityModel security):base(security)
        {
            CurrentReport = new ObservableCollection<EmployeeCommissionVM>();



            _sentenceLock = new object();

           BindingOperations.EnableCollectionSynchronization(CurrentReport, _sentenceLock);


            EmployeeClicked = new RelayCommand(ExecuteEmployeeClicked, param => this.CanExecute);

            ExportCommissionCSVClicked = new RelayCommand(ExecuteExportCommissionCSVClicked, param => this.CanClick);
            ExportCommissionCSVDailyClicked = new RelayCommand(ExecuteExportCommissionCSVDailyClicked, param => this.CanClick);
            ExportCommissionCSVSummaryClicked = new RelayCommand(ExecuteExportCommissionCSVSummaryClicked, param => this.CanClick);

            PrintCommissionDetailsClicked = new RelayCommand(ExecutePrintCommissionDetailsClicked, param => this.CanClick);
            PrintCommissionClicked = new RelayCommand(ExecutePrintCommissionClicked, param => this.CanClick);
            PrintCommissionSummaryClicked = new RelayCommand(ExecutePrintCommissionSummaryClicked, param => this.CanClick);
            PrintCommissionDailyClicked = new RelayCommand(ExecutePrintCommissionDailyClicked, param => this.CanClick);

            TodayClicked = new RelayCommand(ExecuteTodayClicked, param => this.CanClick);
            CustomClicked = new RelayCommand(ExecuteCustomClicked, param => this.CanClick);
            PreviousDayClicked = new RelayCommand(ExecutePreviousDayClicked, param => this.CanExecute);
            NextDayClicked = new RelayCommand(ExecuteNextDayClicked, param => this.CanExecute);

            CurrentEmployee = security.CurrentEmployee;
            _currentid = CurrentEmployee.ID;
            _reports = new Reports();
            ReportDates = new List<ReportDate>();
            CurrentDate = new ReportDate();
            CurrentDate.StartDate = DateTime.Today;
            CurrentDate.EndDate = DateTime.Today;

            if (_currentid == 0) CurrentEmployee.DisplayName = "All Employees";

      


            if (_currentid != 0)
            {
                // GetCommissionReport(_currentid, m_currentdate);
            }

            FillDateList();
            SelectedDateID = 1;


        }

        private void FillDateList()
        {
            DateTime startdate;
            ReportDate newdate;
            startdate = DateTime.Today;
            int j = 1;


            newdate = new ReportDate();
  

            DateTime firstday = GlobalSettings.Instance.PayPeriodStartDate;
            int diff = (int)DateTime.Now.DayOfWeek - (int)firstday.DayOfWeek;

            switch(GlobalSettings.Instance.PayPeriodType.ToUpper())
            {

                case "SEMI-MONTHLY":

                case "SEMIMONTHLY":

                    for (int i = 1; i <= 12; i++)
                    {


                        newdate = new ReportDate();
                        newdate.ReportDateID = j;
                        newdate.StartDate = new DateTime(startdate.Year, startdate.Month, 16);
                        newdate.EndDate = newdate.StartDate.AddMonths(1).AddDays(-16);

                        if(DateTime.Today > newdate.StartDate)
                        {
                            ReportDates.Add(newdate);
                            j++;
                        }
            


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
                        CurrentDate.StartDate = rptdate.StartDate;
                        CurrentDate.EndDate = rptdate.EndDate;
                        GetCommissionReport(_currentid,rptdate);

                      
                    }
                }


            NotifyPropertyChanged("SelectedDateID");
            }
        }

  

        public decimal GrandTotalSales
        {

            get { return _grandtotalsales; }

            set { _grandtotalsales = value; NotifyPropertyChanged("GrandTotalSales"); }

        }

        public decimal GrandTotalSupplyFee
        {

            get { return _grandtotalsupplyfee; }

            set { _grandtotalsupplyfee = value; NotifyPropertyChanged("GrandTotalSupplyFee"); }

        }

        public decimal GrandTotalCommission
        {

            get { return _grandtotalcommission; }

            set { _grandtotalcommission = value; NotifyPropertyChanged("GrandTotalCommission"); }

        }

        public decimal GrandTotalDailyFee
        {

            get { return _grandtotaldailyfee; }

            set { _grandtotaldailyfee = value; NotifyPropertyChanged("GrandTotalDailyFee"); }

        }

        public decimal Custom1
        {
            get { return _custom1; }
            set { _custom1 = value;
            NotifyPropertyChanged("Custom1");
            }
        }

        public decimal Custom2
        {
            get { return _custom2; }
            set
            {
                _custom2 = value;
                NotifyPropertyChanged("Custom2");
            }
        }

        public decimal Custom3
        {
            get { return _custom3; }
            set
            {
                _custom3 = value;
                NotifyPropertyChanged("Custom3");
            }
        }

        public decimal GrandTotalGratuity
        {
            get { return _grandtotalgratuities; }
            set { _grandtotalgratuities = value; NotifyPropertyChanged("GrandTotalGratuity"); }
        }

        public decimal GrandTotalNetGratuity
        {
            get { return _grandtotalnetgratuities; }
            set { _grandtotalnetgratuities = value; NotifyPropertyChanged("GrandTotalNetGratuity"); }
        }


        public ObservableCollection<EmployeeCommissionVM> CurrentReport
        {
            get { return _currentreport; }

            set { _currentreport = value; NotifyPropertyChanged("CurrentReport"); }
        }



        //------------------------------------------------------   Public Functions ------------------------------------------------------------------------




        /// <summary>
        /// Functions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentdate"></param>
        /// <returns></returns>



        public void GetCommissionReport(int id, ReportDate currentdate)
        {
            CurrentReport = new ObservableCollection<EmployeeCommissionVM>();
            DBEmployee dbemployee = new DBEmployee();
            EmployeeCommissionVM employeecommission;

            BackgroundWorker worker = new BackgroundWorker();

            if (id > 0) CurrentEmployee = new Employee(id);

            //if integer = 1000 , then its for all employees , otherwise, just one
            // employee 999 is all employee combined so still consider as one employee
           // var report = new ObservableCollection<EmployeeCommissionVM>();

            worker.DoWork += (o, ea) =>
            {

                if (id == 1000)
                {

                    // dt = dbemployee.GetEmployeeAll();

                    DataTable dt = _reports.GetWorkedEmployees(currentdate.StartDate, currentdate.EndDate);

                    if (dt.Rows.Count > 0)
                    {
                        float total = 0;
                        int count = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            count++;
                            var timer = new Stopwatch();
                            timer.Start();
                            employeecommission = new EmployeeCommissionVM((int)row["employeeid"], currentdate.StartDate, currentdate.EndDate,true);
                            employeecommission.SubTotalVisiblity = Visibility.Visible;
                            if (employeecommission.GrandTotalSales > 0)
                            {


                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    CurrentReport.Add(employeecommission);
                                    ////NotifyPropertyChanged("CurrentReport");
                                    //Thread.Sleep(100);
                                });

                            }

                            timer.Stop();
                            logger.Debug(count + " " + employeecommission.CurrentEmployee.DisplayName + " " + timer.Elapsed.ToString() + " to finish");
                            total += timer.Elapsed.Seconds;

                        }


          


                        logger.Debug("total time " + total + " seconds");

                

                    }



                }
                else //single employee
                {
                    if (id > 0)
                    {
                        employeecommission = new EmployeeCommissionVM(id, currentdate.StartDate, currentdate.EndDate,false);

                        App.Current.Dispatcher.Invoke((Action)delegate 
                        {
                            CurrentReport.Add(employeecommission);
                        });

                      


                    }
                    else
                    if (CurrentEmployee.ID > 0)
                    {

                        employeecommission = new EmployeeCommissionVM(CurrentEmployee.ID, currentdate.StartDate, currentdate.EndDate,false);

                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            CurrentReport.Add(employeecommission);
                        });



                    }

                }
            };



            worker.RunWorkerCompleted += (o, ea) =>
            {
                CalculateGrandTotals();
                IsBusy = false;
            };

            IsBusy = true;

            worker.RunWorkerAsync();


      

        }

        public bool CanClick
        {
            get
            {
                if (_currentid ==0) return false; else return true;

            }
    }
        //-------------------------------------Public Methods -----------------------------------------

        public void ExecutePreviousDayClicked(object button)
        {

            CurrentDate.StartDate = CurrentDate.StartDate.AddDays(-1);
            CurrentDate.EndDate = CurrentDate.StartDate;
            NotifyPropertyChanged("CurrentDate");

             GetCommissionReport(_currentid, m_currentdate);

            CalculateGrandTotals();

        }

        public void ExecuteNextDayClicked(object button)
        {
            CurrentDate.StartDate = CurrentDate.StartDate.AddDays(1);
            CurrentDate.EndDate = CurrentDate.StartDate;
            NotifyPropertyChanged("CurrentDate");

            GetCommissionReport(_currentid, m_currentdate);

            CalculateGrandTotals();
        }

        public void ExecutePrintCommissionDetailsClicked(object tagstr)
        {

            _reports.PrintCommission(CurrentReport, m_currentdate.ReportString, m_cut, "extradetail");

        }

        public void ExecutePrintCommissionClicked(object tagstr)
        {

                  _reports.PrintCommission(CurrentReport, m_currentdate.ReportString, m_cut,"detail");

        }

        public void ExecutePrintCommissionSummaryClicked(object tagstr)
        {

            _reports.PrintCommission(CurrentReport, m_currentdate.ReportString, m_cut,"summary");

        }


        public void ExecutePrintCommissionDailyClicked(object tagstr)
        {

            _reports.PrintCommission(CurrentReport, m_currentdate.ReportString, m_cut, "daily");

        }


        public void ExecuteExportCommissionCSVClicked(object tagstr)
        {


            _reports.ExportCommissionCSV(_currentid, m_currentdate.StartDate, m_currentdate.EndDate,"detail");

        }

        public void ExecuteExportCommissionCSVDailyClicked(object tagstr)
        {


            _reports.ExportCommissionCSV(_currentid, m_currentdate.StartDate, m_currentdate.EndDate,"daily");

        }

        public void ExecuteExportCommissionCSVSummaryClicked(object tagstr)
        {


            _reports.ExportCommissionCSV(_currentid, m_currentdate.StartDate, m_currentdate.EndDate,"summary");

        }

        public void ExecuteTodayClicked(object tagstr)
        {
            SelectedDateID = 1;
            CurrentDate.StartDate = DateTime.Today;
            CurrentDate.EndDate = DateTime.Today;
            NotifyPropertyChanged("CurrentDate");

            GetCommissionReport(_currentid,m_currentdate);

            CalculateGrandTotals();

        }



        public void ExecuteCustomClicked(object tagstr)
        {

            CustomDate cd = new CustomDate(Visibility.Visible);
            cd.Topmost = true;
            cd.ShowDialog();
          

            CurrentDate.StartDate = cd.StartDate;
            CurrentDate.EndDate = cd.EndDate;
            NotifyPropertyChanged("CurrentDate");

            
            GetCommissionReport(_currentid, m_currentdate);

            CalculateGrandTotals();
          

        }
        public void ExecuteEmployeeClicked(object employeeid)
        {
            if (employeeid != null)
            {
              
                
                _currentid = int.Parse(employeeid.ToString());
                GetCommissionReport(_currentid, m_currentdate);
        

            
            }
        }

        public void CalculateGrandTotals()
        {

            decimal totalsales = 0;
            decimal totalcommission = 0;
            decimal totalgratuities = 0;
            decimal totalnetgratuities = 0;
            decimal totalsupplyfee = 0;
            decimal totaldailyfee = 0;


            if (CurrentReport == null) return;

            foreach (EmployeeCommissionVM subreport in CurrentReport)
            {

                if ((subreport.CurrentEmployee.ID < 999 && _currentid == 1000) || _currentid < 1000)
                {
                    /*
                    foreach (SalesData salesdata in subreport.EmployeeSales)
                    {
                        totalsales = totalsales + salesdata.TotalSales;
                        totalcommission = totalcommission + salesdata.TotalCommission;
                        totalgratuities = totalgratuities + salesdata.Gratuity;
                        totalnetgratuities = totalnetgratuities + salesdata.NetGratuity;
                        totalsupplyfee = totalsupplyfee + salesdata.TotalSupplyFee;
                     
                    }
                    */

                    subreport.CalculateGrandTotals();
                    totaldailyfee += subreport.GrandTotalDailyFees;
                    totalsales += subreport.GrandTotalSales;
                    totalcommission += subreport.GrandTotalCommission;
                    totalgratuities += subreport.GrandTotalGratuity;
                    totalnetgratuities += subreport.GrandTotalNetGratuity;
                    totalsupplyfee += subreport.GrandTotalSupplyFees;

                }


            }






            GrandTotalSales = totalsales;
            GrandTotalCommission = totalcommission;
            GrandTotalGratuity = totalgratuities;
            GrandTotalNetGratuity = totalnetgratuities;
            GrandTotalSupplyFee = totalsupplyfee;
            GrandTotalDailyFee = totaldailyfee;
        }

    }





}
