using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class SalesReportVM:INPCBase
    {
        Reports m_report;
        private ObservableCollection<ReportCat> m_revenuelist;
        private ObservableCollection<ReportCat> m_settlementlist;

        private DailyRevenue m_dailyrevenuesalesreport;
        private DailySettlement m_dailysettlementsalesreport;
        private Difference m_dailydifferencesalesreport;


        private ObservableCollection<DailyRevenue> m_weeklyrevenuesalesreport;
        private ObservableCollection<DailySettlement> m_weeklysettlementsalesreport;
        private ObservableCollection<Difference> m_weeklydifferencesalesreport;



        private ObservableCollection<DailyRevenue> m_monthlyrevenuesalesreport;
        private ObservableCollection<DailySettlement> m_monthlysettlementsalesreport;
        private ObservableCollection<Difference> m_monthlydifferencesalesreport;

        private ObservableCollection<DailyRevenue> m_customrevenuesalesreport;
        private ObservableCollection<DailySettlement> m_customsettlementsalesreport;
        private ObservableCollection<Difference> m_customdifferencesalesreport;



        private DateTime m_dailystartdate;
        
        private DateTime m_weeklystartdate;
        private DateTime m_weeklyenddate;

        private DateTime m_monthlystartdate;
        private DateTime m_monthlyenddate;


        private DateTime m_customstartdate;
        private DateTime m_customenddate;

        private bool CanExecute = true;
        private int m_selectedindex;
        private DataTable m_dailysales;
        private DataTable m_dailypayments;
        private ObservableCollection<DailyPayment> m_dailypaymentsummary;
        private ObservableCollection<DailySales> m_dailysalessummary;

        private DataTable m_monthlysummary;

        public ICommand BackClicked { get; set; }

        public ICommand PreviousDayClicked { get; set; }
        public ICommand NextDayClicked { get; set; }
        public ICommand CustomDayClicked { get; set; }


        public ICommand PreviousWeekClicked { get; set; }
        public ICommand NextWeekClicked { get; set; }
        public ICommand CustomWeekClicked { get; set; }



        public ICommand PreviousMonthlyClicked { get; set; }
        public ICommand NextMonthlyClicked { get; set; }
        public ICommand CustomMonthlyClicked { get; set; }


        public ICommand CustomDateClicked { get; set; }

        public ICommand ExportCSVDayClicked { get; set; }
        public ICommand ExportCSVWeeklyClicked { get; set; }
        public ICommand ExportCSVMonthlyClicked { get; set; }
        public ICommand ExportCSVCustomClicked { get; set; }
        public ICommand ExportDetailCSVCustomClicked { get; set; }

        public ICommand PrintWeekClicked { get; set; }
        public ICommand PrintDayClicked { get; set; }
        public ICommand PrintMonthClicked { get; set; }
        public ICommand PrintCustomClicked { get; set; }

        public ICommand PrintDailyPaymentClicked { get; set; }


  






        private Window m_parent;

        private int m_employeeid;

        public SalesReportVM(Window parent, int employeeid)
        {
            BackClicked = new RelayCommand(ExecuteBackClicked, param => this.CanExecute);

            PreviousDayClicked = new RelayCommand(ExecutePreviousDayClicked, param => this.CanExecute);
            NextDayClicked = new RelayCommand(ExecuteNextDayClicked, param => this.CanExecute);
            CustomDayClicked = new RelayCommand(ExecuteCustomDayClicked, param => this.CanExecute);

            NextWeekClicked = new RelayCommand(ExecuteNextWeekClicked, param => this.CanExecute);
            PreviousWeekClicked = new RelayCommand(ExecutePreviousWeekClicked, param => this.CanExecute);
            CustomWeekClicked = new RelayCommand(ExecuteCustomWeekClicked, param => this.CanExecute);

        
            PreviousMonthlyClicked = new RelayCommand(ExecutePreviousMonthlyClicked, param => this.CanExecute);
            NextMonthlyClicked = new RelayCommand(ExecuteNextMonthlyClicked, param => this.CanExecute);
            CustomMonthlyClicked = new RelayCommand(ExecuteCustomMonthlyClicked, param => this.CanExecute);

            CustomDateClicked = new RelayCommand(ExecuteCustomDateClicked, param => this.CanExecute);

            ExportCSVDayClicked = new RelayCommand(ExecuteExportCSVDayClicked, param => this.CanExecute);
            ExportCSVWeeklyClicked = new RelayCommand(ExecuteExportCSVWeeklyClicked, param => this.CanExecute);
            ExportCSVMonthlyClicked = new RelayCommand(ExecuteExportCSVMonthlyClicked, param => this.CanExecute);
            ExportCSVCustomClicked = new RelayCommand(ExecuteExportCSVCustomClicked, param => this.CanExecute);
            ExportDetailCSVCustomClicked = new RelayCommand(ExecuteExportDetailCSVCustomClicked, param => this.CanExecute);


            PrintWeekClicked = new RelayCommand(ExecutePrintWeekClicked, param => this.CanExecute);
            PrintDayClicked = new RelayCommand(ExecutePrintDayClicked, param => this.CanExecute);
            PrintMonthClicked = new RelayCommand(ExecutePrintMonthClicked, param => this.CanExecute);
            PrintCustomClicked = new RelayCommand(ExecutePrintCustomClicked, param => this.CanExecute);

            PrintDailyPaymentClicked = new RelayCommand(ExecutePrintDailyPaymentClicked, param => this.CanExecute);

            m_report = new Reports();
            m_revenuelist = new ObservableCollection<ReportCat>();


            m_parent = parent;
            m_employeeid = employeeid;

            RevenueList = m_report.GetRevenueList();
            SettlementList = m_report.GetSettlementList();

            DailyStartDate = DateTime.Now;
            RunDailyReport();
            DateTime firstday = GlobalSettings.Instance.PayPeriodStartDate;
            int diff = (int)DateTime.Now.DayOfWeek - (int)firstday.DayOfWeek;

           
            if (diff >= 0)
            {
                //positive or equal ..that means the start
                WeeklyStartDate = DateTime.Now.AddDays(-diff);

            }
            else
            {
                WeeklyStartDate = DateTime.Now.AddDays((diff * -1) - 7);
            }
          
            WeeklyEndDate = WeeklyStartDate.AddDays(6);

            MonthlyStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);

  

        }

        private void RunDailyReport()
        {
            DailyRevenueSalesReport = m_report.GetDailyRevenue(DailyStartDate, DailyStartDate);
            DailySettlementSalesReport = m_report.GetDailySettlement(DailyStartDate, DailyStartDate);
            m_dailydifferencesalesreport = new Difference();
            m_dailydifferencesalesreport.NetRevenue = DailyRevenueSalesReport.NetRevenue;
            m_dailydifferencesalesreport.NetPayment = DailySettlementSalesReport.TotalPayment;

            DailyDifferenceSalesReport = m_dailydifferencesalesreport;









            //Sales Report

           // DailySales = m_report.GetDailySales(DailyStartDate,0);
            DataTable salesdat= m_report.GetDailySalesSummary(DailyStartDate);

            DailySalesSummary = new ObservableCollection<DailySales>();

            foreach(DataRow row in salesdat.Rows)
            {
                DailySales newrec = new DailySales(row);
                newrec.SalesDetails = m_report.GetDailySales(DailyStartDate, newrec.EmployeeId);
                DailySalesSummary.Add(newrec);
            }








            //Payment Report
      
            DataTable dat = m_report.GetDailyPaymentSummary(DailyStartDate,m_employeeid);

            DailyPaymentSummary = new ObservableCollection<DailyPayment>();

            int count = 0;
            foreach (DataRow row in dat.Rows)
            {
                DailyPayment newrec = new DailyPayment(row);

                newrec.PaymentDetails = m_report.GetDailyPayments(DailyStartDate,newrec.EmployeeId );

                DailyPaymentSummary.Add(newrec);
                count += newrec.TotalTicket;

            }

            TotalTickets = count;
            TotalAllPayments = dat.AsEnumerable().Sum(x => x.Field<Decimal>("allpayments"));
            TotalCash = dat.AsEnumerable().Sum(x => x.Field<Decimal>("cash"));
            TotalCredit = dat.AsEnumerable().Sum(x => x.Field<Decimal>("credit"));
            TotalDebit = dat.AsEnumerable().Sum(x => x.Field<Decimal>("debit"));
            TotalGiftCard = dat.AsEnumerable().Sum(x => x.Field<Decimal>("giftcard"));
            TotalGiftCert = dat.AsEnumerable().Sum(x => x.Field<Decimal>("giftcertificate"));
            TotalReward = dat.AsEnumerable().Sum(x => x.Field<Decimal>("reward"));
            TotalStampCard = dat.AsEnumerable().Sum(x => x.Field<Decimal>("stampcard"));
            TotalTips = dat.AsEnumerable().Sum(x => x.Field<Decimal>("tips"));

        }

        private void RunWeeklyReport()
        {
            WeeklyRevenueSalesReport = m_report.GetWeeklyRevenue(WeeklyStartDate);
            WeeklySettlementSalesReport = m_report.GetWeeklySettlement(WeeklyStartDate);
            CalculateWeeklyDifference();
        }



        private void CalculateWeeklyDifference()
        {
            WeeklyDifferenceSalesReport = new ObservableCollection<Difference>();
            Difference dff;
            foreach (DailyRevenue daily in WeeklyRevenueSalesReport)
            {
                dff = new Difference();
                dff.NetRevenue = daily.NetRevenue;
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



  


        private void RunMonthlyReport()
        {
           
            MonthlyRevenueSalesReport = m_report.GetMonthlyRevenue(MonthlyStartDate);
            MonthlySettlementSalesReport = m_report.GetMonthlySettlement(MonthlyStartDate);



            CalculateMonthlyDifference();
        }


        private void CalculateMonthlyDifference()
        {
            MonthlyDifferenceSalesReport = new ObservableCollection<Difference>();
            Difference dff;
            foreach (DailyRevenue daily in MonthlyRevenueSalesReport)
            {
                dff = new Difference();
                dff.NetRevenue = daily.NetRevenue;
                MonthlyDifferenceSalesReport.Add(dff);
            }
            int i = 0;
            foreach (DailySettlement daily in MonthlySettlementSalesReport)
            {
                dff = MonthlyDifferenceSalesReport[i];
                i++;
                dff.NetPayment = daily.TotalPayment;

            }

        }





        private void RunCustomReport()
        {

            CustomRevenueSalesReport = m_report.GetCustomRevenue(CustomStartDate,CustomEndDate);
            CustomSettlementSalesReport = m_report.GetCustomSettlement(CustomStartDate, CustomEndDate);



            CalculateCustomDifference();
        }


        private void CalculateCustomDifference()
        {
            CustomDifferenceSalesReport = new ObservableCollection<Difference>();
            Difference dff;
            foreach (DailyRevenue daily in CustomRevenueSalesReport)
            {
                dff = new Difference();
                dff.NetRevenue = daily.NetRevenue;
                CustomDifferenceSalesReport.Add(dff);
            }
            int i = 0;
            foreach (DailySettlement daily in CustomSettlementSalesReport)
            {
                dff = CustomDifferenceSalesReport[i];
                i++;
                dff.NetPayment = daily.TotalPayment;

            }

        }




        /// <summary>
        /// Daily Report Arrays
        /// </summary>
        public DataTable DailySales
        {
            get { return m_dailysales; }
            set
            {
                m_dailysales = value;
                NotifyPropertyChanged("DailySales");
            }

        }

        public DataTable DailyPayments
        {
            get { return m_dailypayments; }
            set
            {
                m_dailypayments = value;
                NotifyPropertyChanged("DailyPayments");
            }

        }

        public ObservableCollection<DailyPayment> DailyPaymentSummary
        {
            get { return m_dailypaymentsummary; }
            set
            {
                m_dailypaymentsummary = value;
                NotifyPropertyChanged("DailyPaymentSummary");
            }

        }

        public ObservableCollection<DailySales> DailySalesSummary
        {
            get { return m_dailysalessummary; }
            set
            {
                m_dailysalessummary = value;
                NotifyPropertyChanged("DailySalesSummary");
            }
        }

        public DataTable MonthlySummary
        {
            get { return m_monthlysummary; }
            set
            {
                m_monthlysummary = value;
                NotifyPropertyChanged("MonthlySummary");
            }

        }

        public DailyRevenue DailyRevenueSalesReport
        {
            get { return m_dailyrevenuesalesreport; }
            set
            {
                m_dailyrevenuesalesreport = value;
                NotifyPropertyChanged("DailyRevenueSalesReport");
            }
        }

        public DailySettlement DailySettlementSalesReport
        {
            get { return m_dailysettlementsalesreport; }
            set
            {
                m_dailysettlementsalesreport = value;
                NotifyPropertyChanged("DailySettlementSalesReport");
            }
        }

        public Difference DailyDifferenceSalesReport
        {
            get { return m_dailydifferencesalesreport; }
            set
            {
                m_dailydifferencesalesreport = value;
                NotifyPropertyChanged("DailyDifferenceSalesReport");
            }
        }


        private decimal m_totalallpayments;
        public decimal TotalAllPayments
        {
            get { return m_totalallpayments; }
            set
            {
                m_totalallpayments = value;
                NotifyPropertyChanged("TotalAllPayments");
            }
        }

        private decimal m_totalcash;
        public decimal TotalCash
        {
            get { return m_totalcash; }
            set
            {
                m_totalcash = value;
                NotifyPropertyChanged("TotalCash");
            }
        }

        private decimal m_totalcredit;
        public decimal TotalCredit
        {
            get { return m_totalcredit; }
            set
            {
                m_totalcredit= value;
                NotifyPropertyChanged("TotalCredit");
            }
        }

        private decimal m_totaldebit;
        public decimal TotalDebit
        {
            get { return m_totaldebit; }
            set
            {
                m_totaldebit = value;
                NotifyPropertyChanged("TotalDebit");
            }
        }

        private decimal m_totalgiftcard;
        public decimal TotalGiftCard
        {
            get { return m_totalgiftcard; }
            set
            {
                m_totalgiftcard = value;
                NotifyPropertyChanged("TotalGiftCard");
            }
        }

        private decimal m_totalgiftcert;
        public decimal TotalGiftCert
        {
            get { return m_totalgiftcert; }
            set
            {
                m_totalgiftcert = value;
                NotifyPropertyChanged("TotalGiftCert");
            }
        }

        private decimal m_totalreward;
        public decimal TotalReward
        {
            get { return m_totalreward; }
            set
            {
                m_totalreward = value;
                NotifyPropertyChanged("TotalReward");
            }
        }

        private decimal m_totalstampcard;
        public decimal TotalStampCard
        {
            get { return m_totalstampcard; }
            set
            {
                m_totalstampcard = value;
                NotifyPropertyChanged("TotalStampCard");
            }
        }

        private decimal m_totaltips;
        public decimal TotalTips
        {
            get { return m_totaltips; }
            set
            {
                m_totaltips = value;
                NotifyPropertyChanged("TotalTips");
            }
        }


        private decimal m_totaltickets;
        public decimal TotalTickets
        {
            get { return m_totaltickets; }
            set
            {
                m_totaltickets = value;
                NotifyPropertyChanged("TotalTickets");
            }
        }

        /// <summary>
        /// Weekly Report Arrays
        /// </summary>
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
        public ObservableCollection<Difference> WeeklyDifferenceSalesReport
        {
            get { return m_weeklydifferencesalesreport; }
            set
            {
                m_weeklydifferencesalesreport = value;
                NotifyPropertyChanged("WeeklyDifferenceSalesReport");
            }
        }



        /// <summary>
        /// Monthly Sales Report
        /// </summary>
        /// 

        public ObservableCollection<DailyRevenue> MonthlyRevenueSalesReport
        {
            get { return m_monthlyrevenuesalesreport; }
            set
            {
                m_monthlyrevenuesalesreport = value;
                NotifyPropertyChanged("MonthlyRevenueSalesReport");
            }
        }
        public ObservableCollection<DailySettlement> MonthlySettlementSalesReport
        {
            get { return m_monthlysettlementsalesreport; }
            set
            {
                m_monthlysettlementsalesreport = value;
                NotifyPropertyChanged("MonthlySettlementSalesReport");
            }
        }
        public ObservableCollection<Difference> MonthlyDifferenceSalesReport
        {
            get { return m_monthlydifferencesalesreport; }
            set
            {
                m_monthlydifferencesalesreport = value;
                NotifyPropertyChanged("MonthlyDifferenceSalesReport");
            }
        }


        /// <summary>
        /// Custom Sales Report
        /// </summary>
        /// 

        public ObservableCollection<DailyRevenue> CustomRevenueSalesReport
        {
            get { return m_customrevenuesalesreport; }
            set
            {
                m_customrevenuesalesreport = value;
                NotifyPropertyChanged("CustomRevenueSalesReport");
            }
        }
        public ObservableCollection<DailySettlement> CustomSettlementSalesReport
        {
            get { return m_customsettlementsalesreport; }
            set
            {
                m_customsettlementsalesreport = value;
                NotifyPropertyChanged("CustomSettlementSalesReport");
            }
        }


        public ObservableCollection<Difference> CustomDifferenceSalesReport
        {
            get { return m_customdifferencesalesreport; }
            set
            {
                m_customdifferencesalesreport = value;
                NotifyPropertyChanged("CustomDifferenceSalesReport");
            }
        }
       


        /// <summary>
        /// 
        /// </summary>

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

        /*
                switch(m_selectedindex)
                {
                    case 0: //daily
                        break;

                    case 1: //daily
                        break;

                    case 2: //weekly


                        RunWeeklyReport();
                        break;
                 
                     
                    case 3: //monthly
                        RunMonthlyReport();
                     
                        break;
                } */
                NotifyPropertyChanged("SelectedIndex"); }
        }

        

        public DateTime DailyStartDate
        {
            get { return m_dailystartdate; }
            set { m_dailystartdate = value; NotifyPropertyChanged("DailyStartDate"); }
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



        public DateTime MonthlyStartDate
        {
            get { return m_monthlystartdate; }
            set { m_monthlystartdate = value; NotifyPropertyChanged("MonthlyStartDate"); }
        }

        public DateTime MonthlyEndDate
        {
            get { return m_monthlyenddate; }
            set { m_monthlyenddate = value; NotifyPropertyChanged("MonthlyEndDate"); }
        }



        public DateTime CustomStartDate
        {
            get { return m_customstartdate; }
            set { m_customstartdate = value; NotifyPropertyChanged("CustomStartDate"); }
        }

        public DateTime CustomEndDate
        {
            get { return m_customenddate; }
            set { m_customenddate = value; NotifyPropertyChanged("CustomEndDate"); }
        }







        public void ExecuteBackClicked(object tagstr)
        {

            m_parent.Close();

        }




        /// <summary>
        /// Daily Report Command Functions
        /// </summary>
        /// <param name="tagstr"></param>

        public void ExecutePreviousDayClicked(object tagstr)
        {

            DailyStartDate = DailyStartDate.AddDays(-1);
            RunDailyReport();

        }
        public void ExecuteNextDayClicked(object tagstr)
        {
            DailyStartDate = DailyStartDate.AddDays(1);
            RunDailyReport();
        }

        public void ExecuteCustomDayClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Hidden) { Topmost = true };
            cd.ShowDialog();


            DailyStartDate = cd.StartDate;
            RunDailyReport();
        }

        /// <summary>
        /// Weekly Report Command Functions
        /// </summary>
        /// <param name="tagstr"></param>
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
            CustomDate cd = new CustomDate(Visibility.Hidden) { Topmost = true };
            cd.ShowDialog();
            //Utility.OpenModal(this, cd);

            
            int daynum = (int)cd.StartDate.DayOfWeek;
            WeeklyStartDate = cd.StartDate.AddDays(-daynum);
            WeeklyEndDate = WeeklyStartDate.AddDays(6);
            RunWeeklyReport();
        }


        



        public void ExecutePreviousMonthlyClicked(object tagstr)
        {


            MonthlyStartDate = MonthlyStartDate.AddMonths(-1);
            MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);

            RunMonthlyReport();
         

        }
        public void ExecuteNextMonthlyClicked(object tagstr)
        {
            MonthlyStartDate = MonthlyStartDate.AddMonths(1);
            MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);

            RunMonthlyReport();
            
        }

        public void ExecuteCustomMonthlyClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Hidden) { Topmost = true };

            cd.ShowDialog();
            //Utility.OpenModal(this, cd);


            MonthlyStartDate = new DateTime(cd.StartDate.Year, cd.StartDate.Month, 1);
            MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);

            RunMonthlyReport();
           
        }


        public void ExecuteCustomDateClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Visible) { Topmost = true };

            cd.ShowDialog();
            //Utility.OpenModal(this, cd);


            CustomStartDate = cd.StartDate;
            CustomEndDate = cd.EndDate;

            RunCustomReport();

        }



        public void ExecuteExportCSVDayClicked(object tagstr)
        {
            m_report.ExportSalesCSV(DailyStartDate,DailyStartDate);
        }


        public void ExecuteExportCSVWeeklyClicked(object tagstr)
        {
            m_report.ExportSalesCSV(WeeklyStartDate, WeeklyStartDate.AddDays(6));
        }


        public void ExecuteExportCSVMonthlyClicked(object tagstr)
        {
            int days = DateTime.DaysInMonth(MonthlyStartDate.Year, MonthlyStartDate.Month);
           m_report.ExportSalesCSV(MonthlyStartDate, MonthlyStartDate.AddDays(days));
        }


        public void ExecuteExportCSVCustomClicked(object tagstr)
        {
            
            m_report.ExportSalesCSV(CustomStartDate, CustomEndDate);

        }

        public void ExecuteExportDetailCSVCustomClicked(object tagstr)
        {

            m_report.ExportSalesDetailCSV(CustomStartDate, CustomEndDate);

        }

        public void ExecutePrintDayClicked(object tagstr)
        {


            m_report.PrintDailySales(DailyStartDate);


        }
        public void ExecutePrintWeekClicked(object tagstr)
        {


            m_report.PrintWeeklySales(WeeklyStartDate);


        }

        public void ExecutePrintMonthClicked(object tagstr)
        {
          

            m_report.PrintMonthlySales(MonthlyStartDate, MonthlyEndDate);


        }

        public void ExecutePrintCustomClicked(object tagstr)
        {


            m_report.PrintCustomSales(CustomStartDate, CustomEndDate);


        }



        public void ExecutePrintDailyPaymentClicked(object tagstr)
        {


            m_report.PrintDailyPayments(DailyStartDate,m_employeeid);


        }
    }
}
