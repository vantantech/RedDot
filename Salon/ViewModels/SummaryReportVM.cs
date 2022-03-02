using RedDot.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RedDotBase;

namespace RedDot.ViewModels
{
    public class SummaryReportVM:INPCBase
    {
        private DateTime m_summarystartdate;
        private DateTime m_summaryenddate;

        public ICommand BackClicked { get; set; }
        public ICommand RunReportClicked { get; set; }

        public ICommand SummaryDateClicked { get; set; }
        public ICommand ExportCSVSummaryClicked { get; set; }

        public ICommand ExportDetailCSVSummaryClicked { get; set; }
        public ICommand PrintSummaryClicked { get; set; }


        private bool CanExecute = true;
        private Window m_parent;
        private DailyRevenue m_summaryrevenuesalesreport;
        private DailySettlement m_summarysettlementsalesreport;
        private Difference m_summarydifferencesalesreport;

        Reports m_report;
        private ObservableCollection<ReportCat> m_revenuelist;
        private ObservableCollection<ReportCat> m_settlementlist;

        public SummaryReportVM(Window parent)
        {
            m_parent = parent;

            SummaryDateClicked = new RelayCommand(ExecuteSummaryDateClicked, param => this.CanExecute);
            BackClicked = new RelayCommand(ExecuteBackClicked, param => this.CanExecute);
            RunReportClicked = new RelayCommand(ExecuteRunReportClicked, param => this.CanExecute);
            ExportCSVSummaryClicked = new RelayCommand(ExecuteExportCSVSummaryClicked, param => this.CanExecute);
            ExportDetailCSVSummaryClicked = new RelayCommand(ExecuteExportDetailCSVSummaryClicked, param => this.CanExecute);
            PrintSummaryClicked = new RelayCommand(ExecutePrintSummaryClicked, param => this.CanExecute);

            m_report = new Reports();
            ReportDates = new List<ReportDate>();
            CurrentDate = new ReportDate();


            RevenueList = m_report.GetRevenueList();
            SettlementList = m_report.GetSettlementList();

            CurrentDate.StartDate = DateTime.Today;
            CurrentDate.EndDate = DateTime.Today.AddDays(1);


            FillDateList();
            SelectedDateID = 1;
        }


   

        private List<ReportDate> m_reportdates;
        public List<ReportDate> ReportDates
        {
            get { return m_reportdates; }

            set
            {
                m_reportdates = value;
                NotifyPropertyChanged("ReportDates");
            }
        }

        public DailyRevenue SummaryRevenueSalesReport
        {
            get { return m_summaryrevenuesalesreport; }
            set
            {
                m_summaryrevenuesalesreport = value;
                NotifyPropertyChanged("SummaryRevenueSalesReport");
            }
        }
        public DailySettlement SummarySettlementSalesReport
        {
            get { return m_summarysettlementsalesreport; }
            set
            {
                m_summarysettlementsalesreport = value;
                NotifyPropertyChanged("SummarySettlementSalesReport");
            }
        }

        private TipSummary m_tips;
        public TipSummary Tips
        {
            get { return m_tips; }
            set
            {
                m_tips = value;
                NotifyPropertyChanged("Tips");
            }
        }
        public Difference SummaryDifferenceSalesReport
        {
            get { return m_summarydifferencesalesreport; }
            set
            {
                m_summarydifferencesalesreport = value;
                NotifyPropertyChanged("SummaryDifferenceSalesReport");
            }
        }



        public ObservableCollection<ReportCat> RevenueList
        {
            get { return m_revenuelist; }
            set
            {
                m_revenuelist = value;
                NotifyPropertyChanged("RevenueList");
            }
        }


        public ObservableCollection<ReportCat> SettlementList
        {
            get { return m_settlementlist; }
            set
            {
                m_settlementlist = value;
                NotifyPropertyChanged("SettlementList");
            }
        }

        private int m_selecteddateid;
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
                        CurrentDate.StartDate = rptdate.StartDate;
                        CurrentDate.EndDate = rptdate.EndDate;
                        RunSummaryReport();


                    }
                }


                NotifyPropertyChanged("SelectedDateID");
            }
        }

        private ReportDate m_currentdate;
        public ReportDate CurrentDate
        {
            get { return m_currentdate; }
            set
            {
                m_currentdate = value;
                NotifyPropertyChanged("CurrentDate");
            }
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

            switch (GlobalSettings.Instance.PayPeriodType.ToUpper())
            {

                case "SEMI-MONTHLY":

                case "SEMIMONTHLY":

                    for (int i = 1; i <= 12; i++)
                    {


                        newdate = new ReportDate();
                        newdate.ReportDateID = j;
                        newdate.StartDate = new DateTime(startdate.Year, startdate.Month, 16);
                        newdate.EndDate = newdate.StartDate.AddMonths(1).AddDays(-16);

                        if (DateTime.Today > newdate.StartDate)
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

        public void ExecutePrintSummaryClicked(object tagstr)
        {
            m_report.PrintSummaryReport(CurrentDate.StartDate, CurrentDate.EndDate);

        }



        public void ExecuteSummaryDateClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Visible) { Topmost = true };

            cd.ShowDialog();
            //Utility.OpenModal(this, cd);


            CurrentDate.StartDate = cd.StartDate;
            CurrentDate.EndDate = cd.EndDate;

            RunSummaryReport();

        }


        public void ExecuteBackClicked(object tagstr)
        {

            m_parent.Close();

        }

        public void ExecuteRunReportClicked(object obj)
        {
            RunSummaryReport();

        }

        public void ExecuteExportCSVSummaryClicked(object tagstr)
        {

            m_report.ExportSalesCSV(CurrentDate.StartDate, CurrentDate.EndDate);

        }

        public void ExecuteExportDetailCSVSummaryClicked(object tagstr)
        {

            m_report.ExportSalesDetailCSV(CurrentDate.StartDate, CurrentDate.EndDate);

        }

        private void RunSummaryReport()
        {

            SummaryRevenueSalesReport = m_report.GetSummaryRevenue(CurrentDate.StartDate, CurrentDate.EndDate);
            SummarySettlementSalesReport = m_report.GetSummarySettlement(CurrentDate.StartDate, CurrentDate.EndDate);

            Tips = m_report.GetTipSummary(CurrentDate.StartDate, CurrentDate.EndDate);


        }


    }
}
