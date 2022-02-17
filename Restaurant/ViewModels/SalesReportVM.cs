using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class SalesReportVM:INPCBase
    {
        QSReports m_report;
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

        private decimal m_dailyproductsubtotal=0;
        private decimal m_dailylaborsubtotal=0;
        private decimal m_dailytotal=0;
        private decimal m_dailydiscount=0;
        private decimal m_dailyshopfee=0;
        private decimal m_dailysalestax=0;
        private decimal m_dailynetpayment=0;

        public ICommand BackClicked { get; set; }
        public ICommand RunReportClicked { get; set; }

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



        //By Order Type
        public ICommand PrintByOrderTypeClicked { get; set; }
        public ICommand ExportByOrderTypeClicked { get; set; }

        //by Item
        public ICommand PrintByItemClicked { get; set; }    
        public ICommand ExportByItemClicked { get; set; }


        //Refund Void
        public ICommand PrintVoidRefundClicked { get; set; }
        public ICommand ExportVoidRefundClicked { get; set; }   

        //Discounts
        public ICommand PrintDiscountClicked { get; set; }
        public ICommand ExportDiscountClicked { get; set; }

        private Window m_parent;

        private int m_employeeid;

        private DataTable m_salesbyordertype;
        public DataTable SalesByOrderType
        {
            get { return m_salesbyordertype; }
            set
            {
                m_salesbyordertype = value;
                NotifyPropertyChanged("SalesByOrderType");
            }
        }


        private decimal m_salesbyitemtotal;
        public decimal SalesByItemTotal
        {
            get { return m_salesbyitemtotal; }
            set
            {
                m_salesbyitemtotal = value;
                NotifyPropertyChanged("SalesByItemTotal");
            }
        }

        private decimal m_salesbymodifiertotal;
        public decimal SalesByModifierTotal
        {
            get { return m_salesbymodifiertotal; }
            set
            {
                m_salesbymodifiertotal = value;
                NotifyPropertyChanged("SalesByModifierTotal");
            }
        }


        private DataTable m_salesbyitem;
        public DataTable SalesByItem
        {
            get { return m_salesbyitem; }
            set
            {
                m_salesbyitem = value;
                NotifyPropertyChanged("SalesByItem");
            }
        }

        private DataTable m_salesbymodifier;
        public DataTable SalesByModifier
        {
            get { return m_salesbymodifier; }
            set
            {
                m_salesbymodifier = value;
                NotifyPropertyChanged("SalesByModifier");
            }
        }

        private DataTable m_ticketvoids;
        public DataTable TicketVoids
        {
            get { return m_ticketvoids; }
            set
            {
                m_ticketvoids = value;
                NotifyPropertyChanged("TicketVoids");
            }
        }


        private DataTable m_ticketitemvoids;
        public DataTable TicketItemVoids
        {
            get { return m_ticketitemvoids; }
            set
            {
                m_ticketitemvoids = value;
                NotifyPropertyChanged("TicketItemVoids");
            }
        }

        private DataTable m_ticketpaymentvoids;
        public DataTable TicketPaymentVoids
        {
            get { return m_ticketpaymentvoids; }
            set
            {
                m_ticketpaymentvoids = value;
                NotifyPropertyChanged("TicketPaymentVoids");
            }
        }

        private DataTable m_ticketpaymentrefunds;
        public DataTable TicketPaymentRefunds
        {
            get { return m_ticketpaymentrefunds; }
            set
            {
                m_ticketpaymentrefunds = value;
                NotifyPropertyChanged("TicketPaymentRefunds");
            }
        }

        private DataTable m_ticketdiscounts;
        public DataTable TicketDiscounts
        {
            get { return m_ticketdiscounts; }
            set
            {
                m_ticketdiscounts = value;
                NotifyPropertyChanged("TicketDiscounts");
            }
        }

        private DataTable m_ticketitemdiscounts;
        public DataTable TicketItemDiscounts
        {
            get { return m_ticketitemdiscounts; }
            set
            {
                m_ticketitemdiscounts = value;
                NotifyPropertyChanged("TicketItemDiscounts");
            }
        }





        public SalesReportVM(Window parent, int employeeid)
        {
            m_parent = parent;
            m_employeeid = employeeid;



            BackClicked = new RelayCommand(ExecuteBackClicked, param => this.CanExecute);


            PreviousDayClicked = new RelayCommand(ExecutePreviousDayClicked, param => this.CanExecute);
            NextDayClicked = new RelayCommand(ExecuteNextDayClicked, param => this.CanExecute);
            CustomDayClicked = new RelayCommand(ExecuteCustomDayClicked, param => this.CanExecute);

            PreviousWeekClicked = new RelayCommand(ExecutePreviousWeekClicked, param => this.CanExecute);
            NextWeekClicked = new RelayCommand(ExecuteNextWeekClicked, param => this.CanExecute);
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

            //By Order Type
            PrintByOrderTypeClicked = new RelayCommand(ExecutePrintByOrderTypeClicked, param => this.CanExecute);
            ExportByOrderTypeClicked = new RelayCommand(ExecuteExportByOrderTypeClicked, param => this.CanExecute);

            //by item type
            PrintByItemClicked = new RelayCommand(ExecutePrintByItemClicked, param => this.CanExecute);
            ExportByItemClicked = new RelayCommand(ExecuteExportByItemClicked, param => this.CanExecute);

            RunReportClicked = new RelayCommand(ExecuteRunReportClicked, param => this.CanExecute);

            m_report = new QSReports();
            m_revenuelist = new ObservableCollection<ReportCat>();
            



            RevenueList = m_report.GetRevenueList();
            SettlementList = m_report.GetSettlementList();

            DailyStartDate = DateTime.Now;
           // RunDailyReport();


            int daynum = (int)DateTime.Now.DayOfWeek;
            WeeklyStartDate = DateTime.Now.AddDays(-daynum);
            WeeklyEndDate = WeeklyStartDate.AddDays(6);


                MonthlyStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);

            CustomStartDate = DateTime.Now;
            CustomEndDate = DateTime.Now;
        }

        private void ClearDailyReport()
        {
            DailyRevenueSalesReport = null;
            DailySettlementSalesReport = null;
            DailyDifferenceSalesReport = null;
            DailySales = null;


            DailyDiscount = 0;
            DailySalesTax = 0;
            DailyTotal = 0;
            DailyProductSubTotal = 0;
            DailyNetPayment = 0;
        }
        private void RunDailyReport()
        {




            DailyRevenueSalesReport = m_report.GetDailyRevenue(DailyStartDate, DailyStartDate); //tips etc.. is calculated here


         


         

            DailySettlementSalesReport = m_report.GetDailySettlement(DailyStartDate, DailyStartDate);

      


            m_dailydifferencesalesreport = new Difference();
            m_dailydifferencesalesreport.NetRevenue = DailyRevenueSalesReport.NetRevenue;
            m_dailydifferencesalesreport.NetPayment = DailySettlementSalesReport.TotalPayment;

            DailyDifferenceSalesReport = m_dailydifferencesalesreport;
            DailySales = m_report.GetDailySales(DailyStartDate);

            //total the above report


            DailyDiscount = 0;
            DailySalesTax = 0;
            DailyTotal = 0;
            DailyProductSubTotal = 0;
            DailyNetPayment = 0;
       


            foreach (DataRow row in DailySales.Rows)
            {
                DailyDiscount = DailyDiscount + (row["adjustment"].ToString() != "" ? (decimal)row["adjustment"] : 0);
                DailySalesTax = DailySalesTax + (row["salestax"].ToString() != "" ? (decimal)row["salestax"] : 0);
                DailyTotal = DailyTotal + (row["total"].ToString() != "" ? (decimal)row["total"] : 0);
                DailyNetPayment = DailyNetPayment + (row["netpayment"].ToString() != "" ? (decimal)row["netpayment"] : 0);
               
            }








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
            // NotifyPropertyChanged("WeeklyDifferenceSalesReport");
        }



        private void RunMonthlyReport()
        {
            MonthlyRevenueSalesReport = m_report.GetMonthlyRevenue(MonthlyStartDate);
           // MonthlySettlementSalesReport = m_report.GetMonthlySettlement(MonthlyStartDate);
           // CalculateMonthlyDifference();
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
            // NotifyPropertyChanged("WeeklyDifferenceSalesReport");
        }

        private void RunCustomReport()
        {

            CustomRevenueSalesReport = m_report.GetCustomRevenue(CustomStartDate, CustomEndDate);
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

        public decimal DailyDiscount
        {
            get { return m_dailydiscount; }
            set { m_dailydiscount = value;
            NotifyPropertyChanged("DailyDiscount");
            }
        }

        public decimal DailySalesTax
        {
            get { return m_dailysalestax; }
            set
            {
                m_dailysalestax = value;
                NotifyPropertyChanged("DailySalesTax");
            }
        }

        public decimal DailyTotal
        {
            get { return m_dailytotal; }
            set
            {
                m_dailytotal = value;
                NotifyPropertyChanged("DailyTotal");
            }
        }

        public decimal DailyShopFee
        {
            get { return m_dailyshopfee; }
            set
            {
                m_dailyshopfee = value;
                NotifyPropertyChanged("DailyShopFee");
            }
        }

        public decimal DailyLaborSubTotal
        {
            get { return m_dailylaborsubtotal; }
            set
            {
                m_dailylaborsubtotal = value;
                NotifyPropertyChanged("DailyLaborSubTotal");
            }
        }

        public decimal DailyProductSubTotal
        {
            get { return m_dailyproductsubtotal; }
            set
            {
                m_dailyproductsubtotal = value;
                NotifyPropertyChanged("DailyProductSubTotal");
            }
        }

        public decimal DailyNetPayment
        {
            get { return m_dailynetpayment; }
            set
            {
                m_dailynetpayment = value;
                NotifyPropertyChanged("DailyNetPayment");
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
        /// MonthlyReport Arrays
        /// </summary>
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

             
                NotifyPropertyChanged("SelectedIndex"); }
        }

        private string _dow;
        public string DOW
        {
            get { return _dow; }
            set
            {
                _dow = value;
                NotifyPropertyChanged("DOW");
            }
        }

        public DateTime DailyStartDate
        {
            get { return m_dailystartdate; }
            set {

                m_dailystartdate = value;
                DOW = m_dailystartdate.DayOfWeek.ToString().Substring(0, 3).ToUpper();
                NotifyPropertyChanged("DailyStartDate"); }
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

        /// <summary>
        ///  Monthly Dates
        /// </summary>
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


        public void ExecuteRunReportClicked(object obj)
        {
            switch (m_selectedindex)
            {
                case 0: //daily
                    RunDailyReport();
                    break;
                case 1: //weekly
                    RunWeeklyReport();
                    break;
                case 2: // monthly
                    RunMonthlyReport();
                    break;

                case 3: // custom
                    RunCustomReport();

                    break;

                case 4:
                    SalesByOrderType = m_report.GetSalesByOrderType(CustomStartDate, CustomEndDate);
                    break;

                case 5:
                    SalesByItem = m_report.GetSalesByItem(CustomStartDate, CustomEndDate);
                    SalesByModifier = m_report.GetSalesByModifier(CustomStartDate, CustomEndDate);
                    decimal totalitem = 0;
                    decimal totalmodifier = 0;

                    foreach(DataRow row in SalesByItem.Rows)
                    {
                        decimal total = decimal.Parse(row["totalamount"].ToString());

                        totalitem += total;
                    }

                    SalesByItemTotal = totalitem;

                    foreach (DataRow row in SalesByModifier.Rows)
                    {
                        decimal total = decimal.Parse(row["totalamount"].ToString());

                        totalmodifier += total;
                    }

                    SalesByModifierTotal = totalmodifier;

                    break;

                case 6:
                    TicketVoids = m_report.GetTicketVoids(CustomStartDate, CustomEndDate);
                    TicketItemVoids = m_report.GetTicketItemVoids(CustomStartDate, CustomEndDate);
                    TicketPaymentVoids = m_report.GetTicketPaymentVoids(CustomStartDate, CustomEndDate);
                    TicketPaymentRefunds = m_report.GetTicketPaymentRefunds(CustomStartDate, CustomEndDate);
                    break;

                case 7:
                    TicketDiscounts = m_report.GetTicketDiscounts(CustomStartDate, CustomEndDate);
                    TicketItemDiscounts = m_report.GetTicketItemDiscounts(CustomStartDate, CustomEndDate);
                    break;
            }
        }

        public void ExecutePreviousDayClicked(object tagstr)
        {

            DailyStartDate = DailyStartDate.AddDays(-1);
            ClearDailyReport();

        }
        public void ExecuteNextDayClicked(object tagstr)
        {
            DailyStartDate = DailyStartDate.AddDays(1);
            ClearDailyReport();
        }

        public void ExecuteCustomDayClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Hidden) { Topmost = true };
            cd.ShowDialog();
            //Utility.OpenModal(this, cd);

            DailyStartDate = cd.StartDate;
            //RunDailyReport();
        }

        /// <summary>
        /// Weekly Report Command Functions
        /// </summary>
        /// <param name="tagstr"></param>
        public void ExecutePreviousWeekClicked(object tagstr)
        {

           
            WeeklyStartDate = WeeklyStartDate.AddDays(-7);
            WeeklyEndDate = WeeklyStartDate.AddDays(6);
            //RunWeeklyReport();

        }
        public void ExecuteNextWeekClicked(object tagstr)
        {
            WeeklyStartDate = WeeklyStartDate.AddDays(7);
            WeeklyEndDate = WeeklyStartDate.AddDays(6);
           // RunWeeklyReport();
        }

        public void ExecuteCustomWeekClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Hidden) { Topmost = true };
            cd.ShowDialog();
            //Utility.OpenModal(this, cd);

            
            int daynum = (int)cd.StartDate.DayOfWeek;
            WeeklyStartDate = cd.StartDate.AddDays(-daynum);
            WeeklyEndDate = WeeklyStartDate.AddDays(6);
            //RunWeeklyReport();
        }

        /// <summary>
        /// -Monthly Report Dates
        /// </summary>
        /// <param name="tagstr"></param>
        public void ExecutePreviousMonthlyClicked(object tagstr)
        {


        
                //go to second half of previous month
                MonthlyStartDate = MonthlyStartDate.AddMonths(-1);
                MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);

     

        }
        public void ExecuteNextMonthlyClicked(object tagstr)
        {

            //go to second half of current month
            MonthlyStartDate = MonthlyStartDate.AddMonths(1);
                MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);
      
        }

        public void ExecuteCustomMonthlyClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Hidden) { Topmost = true };
         
            cd.ShowDialog();
            //Utility.OpenModal(this, cd);


      
                //First Half of month
                MonthlyStartDate = new DateTime(cd.StartDate.Year, cd.StartDate.Month, 1);
            MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);

        }


        public void ExecuteCustomDateClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Visible) { Topmost = true };

            cd.ShowDialog();
            //Utility.OpenModal(this, cd);


            CustomStartDate = cd.StartDate;
            CustomEndDate = cd.EndDate;

          
           

        }

        public void ExecuteBackClicked(object tagstr)
        {

            m_parent.Close();

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

        public void ExecuteExportCSVDayClicked(object tagstr)
        {
            m_report.ExportSalesCSV(DailyStartDate, DailyStartDate);
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



        public void ExecutePrintByOrderTypeClicked(object obj)
        {
            m_report.PrintByOrderType(CustomStartDate, CustomEndDate);
        }

        public void ExecuteExportByOrderTypeClicked(object obj)
        {
            m_report.ExportByOrderType(CustomStartDate, CustomEndDate);
        }


        public void ExecutePrintByItemClicked(object obj)
        {
            m_report.PrintByItem(CustomStartDate, CustomEndDate);
        }

        public void ExecuteExportByItemClicked(object obj)
        {
            m_report.ExportByItem(CustomStartDate, CustomEndDate);
        }
    }
}
