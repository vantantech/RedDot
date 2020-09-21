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
    public class WeeklySalesReportVM:INPCBase
    {
        Reports m_report;
        private ObservableCollection<ReportCat> m_revenuelist;
        private ObservableCollection<ReportCat> m_settlementlist;

        private ObservableCollection<DailyRevenue> m_weeklyrevenuesalesreport;
        private ObservableCollection<DailySettlement> m_weeklysettlementsalesreport;
        private ObservableCollection<DailySettlement> m_weeklypaymentsalesreport;



        private ObservableCollection<Difference> m_weeklydifferencesalesreport;

        
        private DateTime m_weeklystartdate;
        private DateTime m_weeklyenddate;
  


        private bool      m_CanExecute = true;
        private bool      m_includeopen = true;
        private int       m_selectedindex;
 


        public ICommand PreviousWeekClicked { get; set; }
        public ICommand NextWeekClicked { get; set; }
        public ICommand CustomWeekClicked { get; set; }
        public ICommand PrintWeekClicked { get; set; }

        

   


        public ICommand ExportCSVDayClicked { get; set; }
        public ICommand ExportCSVMonthlyClicked { get; set; }

        public WeeklySalesReportVM()
        {
   

            PreviousWeekClicked = new RelayCommand(ExecutePreviousWeekClicked, param => this.m_CanExecute);
            NextWeekClicked = new RelayCommand(ExecuteNextWeekClicked, param => this.m_CanExecute);
            CustomWeekClicked = new RelayCommand(ExecuteCustomWeekClicked, param => this.m_CanExecute);
            PrintWeekClicked = new RelayCommand(ExecutePrintWeekClicked, param => this.m_CanExecute);




            m_report = new Reports();
            m_revenuelist = new ObservableCollection<ReportCat>();
            



            RevenueList = m_report.GetRevenueList();
            SettlementList = m_report.GetSettlementList();

          

            int daynum = (int)DateTime.Now.DayOfWeek;
            WeeklyStartDate = DateTime.Now.AddDays(-daynum);
            WeeklyEndDate = WeeklyStartDate.AddDays(6);

            RunWeeklyReport();

     

        }

      





        private void RunWeeklyReport()
        {
            WeeklyRevenueSalesReport = m_report.GetWeeklyRevenue(WeeklyStartDate);
            WeeklySettlementSalesReport = m_report.GetWeeklySettlement(WeeklyStartDate);
            WeeklyPaymentSalesReport = m_report.GetWeeklyPayment(WeeklyStartDate);

       



            CalculateWeeklyDifference();
        }



        private void CalculateWeeklyDifference()
        {
            WeeklyDifferenceSalesReport = new ObservableCollection<Difference>();
            Difference dff;
            foreach (DailyRevenue daily in WeeklyRevenueSalesReport)
            {
                dff = new Difference();
                dff.NetRevenue = daily.TotalRevenue;
                WeeklyDifferenceSalesReport.Add(dff);
            }
            int i = 0;
            foreach (DailySettlement daily in WeeklySettlementSalesReport)
            {
                dff = WeeklyDifferenceSalesReport[i];
                i++;
                dff.NetPayment = daily.TotalPayment;

            }
            
        }



      

    


      


       


        public bool IncludeOpen
        {
            get { return m_includeopen; }
            set { m_includeopen = value;
            NotifyPropertyChanged("IncludeOpen");
            RunWeeklyReport();
            }
        }


       










        /// <summary>
        /// Weekly Report Arrays
        /// </summary>
        /// 

    


        public ObservableCollection<DailyRevenue> WeeklyRevenueSalesReport
        {
            get { return m_weeklyrevenuesalesreport; }
            set { m_weeklyrevenuesalesreport = value;
            NotifyPropertyChanged("WeeklyRevenueSalesReport");
            }
        }
        public ObservableCollection<DailySettlement> WeeklySettlementSalesReport
        {
            get { return m_weeklysettlementsalesreport; }
            set
            {
                m_weeklysettlementsalesreport = value;
                NotifyPropertyChanged("WeeklySettlementSalesReport");
            }
        }

        public ObservableCollection<DailySettlement> WeeklyPaymentSalesReport
        {
            get { return m_weeklypaymentsalesreport; }
            set
            {
                m_weeklypaymentsalesreport = value;
                NotifyPropertyChanged("WeeklyPaymentSalesReport");
            }
        }
        public ObservableCollection<Difference> WeeklyDifferenceSalesReport
        {
            get { return m_weeklydifferencesalesreport; }
            set
            {
                m_weeklydifferencesalesreport = value;
                NotifyPropertyChanged("WeeklyDifferenceSalesReport");
            }
        }

   


   



        public ObservableCollection<ReportCat> RevenueList
        {
            get { return m_revenuelist; }
            set { m_revenuelist = value;
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

        public int SelectedIndex
        {
            get { return m_selectedindex; }
            set { m_selectedindex = value;
                switch(m_selectedindex)
                {
                    case 0: //daily
                        break;
                    case 1: //weekly
                        RunWeeklyReport();
                        break;
                   
                }
                NotifyPropertyChanged("SelectedIndex"); }
        }


    
        /// <summary>
        /// Weekly Dates
        /// </summary>
        public DateTime WeeklyStartDate
        {
            get { return m_weeklystartdate; }
            set { m_weeklystartdate = value; NotifyPropertyChanged("WeeklyStartDate"); }
        }

        public DateTime WeeklyEndDate
        {
            get { return m_weeklyenddate; }
            set { m_weeklyenddate = value; NotifyPropertyChanged("WeeklyEndDate"); }
        }

    
   


        public void ExecutePreviousWeekClicked(object tagstr)
        {

           
            WeeklyStartDate = WeeklyStartDate.AddDays(-7);
            WeeklyEndDate = WeeklyStartDate.AddDays(6);
            RunWeeklyReport();

        }
        public void ExecuteNextWeekClicked(object tagstr)
        {
            WeeklyStartDate = WeeklyStartDate.AddDays(7);
            WeeklyEndDate = WeeklyStartDate.AddDays(6);
            RunWeeklyReport();
        }

        public void ExecuteCustomWeekClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Hidden);
     
            cd.ShowDialog();
            //Utility.OpenModal(this, cd);

            
            int daynum = (int)cd.StartDate.DayOfWeek;
            WeeklyStartDate = cd.StartDate.AddDays(-daynum);
            WeeklyEndDate = WeeklyStartDate.AddDays(6);
            RunWeeklyReport();
        }

        public void ExecutePrintWeekClicked(object tagstr)
        {
            m_report.PrintSalesSummary(WeeklyStartDate, WeeklyEndDate,  WeeklyRevenueSalesReport, WeeklySettlementSalesReport, RevenueList, SettlementList,true);
        }




    }
}
