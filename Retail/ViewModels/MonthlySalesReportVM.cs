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
    public class MonthlySalesReportVM:INPCBase
    {
        Reports m_report;
        private ObservableCollection<ReportCat> m_revenuelist;
        private ObservableCollection<ReportCat> m_settlementlist;

   


        private ObservableCollection<DailyRevenue> m_monthlyrevenuesalesreport;
        private ObservableCollection<DailySettlement> m_monthlysettlementsalesreport;

        private ObservableCollection<DailySettlement> m_monthlypaymentsalesreport;


        private ObservableCollection<Difference> m_monthlydifferencesalesreport;

        private DailyRevenue m_monthlydetailrevenuesalesreport;
        private DailySettlement m_monthlydetailsettlementsalesreport;
        private Difference m_monthlydetaildifferencesalesreport;

     
        
      


      


        private DateTime m_monthlystartdate;
        private DateTime m_monthlyenddate;



        private bool      m_CanExecute = true;
        private bool      m_includeopen = true;
    
    
        private DataTable m_monthlydetailsales;
        private DataTable m_monthlysummary;




        private decimal   m_monthlydetailproductsubtotal = 0;
        private decimal   m_monthlydetaillaborsubtotal = 0;
        private decimal   m_monthlydetailtotal = 0;
        private decimal   m_monthlydetaildiscount = 0;
        private decimal   m_monthlydetailshopfee = 0;
        private decimal   m_monthlydetailsalestax = 0;
        private decimal   m_monthlydetailnetpayment = 0;




        public ICommand PreviousMonthClicked { get; set; }
        public ICommand NextMonthClicked { get; set; }
        public ICommand CustomMonthClicked { get; set; }


     
        public ICommand ExportCSVMonthlyClicked { get; set; }

        public MonthlySalesReportVM()
        {
            



            PreviousMonthClicked = new RelayCommand(ExecutePreviousMonthClicked, param => this.m_CanExecute);
            NextMonthClicked = new RelayCommand(ExecuteNextMonthClicked, param => this.m_CanExecute);
            CustomMonthClicked = new RelayCommand(ExecuteCustomMonthClicked, param => this.m_CanExecute);

      
            ExportCSVMonthlyClicked = new RelayCommand(ExecuteExportCSVMonthClicked, param => this.m_CanExecute);

            m_report = new Reports();
            m_revenuelist = new ObservableCollection<ReportCat>();
            



            RevenueList = m_report.GetRevenueList();
            SettlementList = m_report.GetSettlementList();

    


     
            MonthlyStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);


            RunMonthlyReport();
            RunMonthlyDetailReport();

        }

    





        private void RunMonthlyReport()
        {
            MonthlySummary = m_report.GetMonthlySummary(MonthlyStartDate);

            MonthlyRevenueSalesReport = m_report.GetMonthlyRevenue(MonthlyStartDate);
            MonthlySettlementSalesReport = m_report.GetMonthlySettlement(MonthlyStartDate);
            MonthlyPaymentSalesReport = m_report.GetMonthlyPayment(MonthlyStartDate);


            CalculateMonthlyDifference();
        }


        private void CalculateMonthlyDifference()
        {
            MonthlyDifferenceSalesReport = new ObservableCollection<Difference>();
            Difference dff;
            foreach (DailyRevenue daily in MonthlyRevenueSalesReport)
            {
                dff = new Difference();
                dff.NetRevenue = daily.TotalRevenue;
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


        private void RunMonthlyDetailReport()
        {

            int days = DateTime.DaysInMonth(MonthlyStartDate.Year, MonthlyStartDate.Month);
            DateTime enddate = MonthlyStartDate.AddDays(days - 1);


            MonthlyDetailRevenueSalesReport = m_report.GetDailyRevenue(MonthlyStartDate, enddate);
            MonthlyDetailSettlementSalesReport = m_report.GetDailySettlement(MonthlyStartDate, enddate);

            m_monthlydetaildifferencesalesreport = new Difference();
            m_monthlydetaildifferencesalesreport.NetRevenue = MonthlyDetailRevenueSalesReport.TotalRevenue;
            m_monthlydetaildifferencesalesreport.NetPayment = MonthlyDetailSettlementSalesReport.TotalPayment;

            MonthlyDetailDifferenceSalesReport = m_monthlydetaildifferencesalesreport;
            MonthlyDetailSales = m_report.GetMonthlySales(MonthlyStartDate);

            //total the above report


            MonthlyDetailDiscount = 0;
            MonthlyDetailSalesTax = 0;
            MonthlyDetailTotal = 0;
            MonthlyDetailProductSubTotal = 0;
            MonthlyDetailLaborSubTotal = 0;
            MonthlyDetailNetPayment = 0;
            MonthlyDetailShopFee = 0;


            foreach (DataRow row in MonthlyDetailSales.Rows)
            {
                MonthlyDetailDiscount = MonthlyDetailDiscount + (row["discount"].ToString() != "" ? (decimal)row["discount"] : 0);
                MonthlyDetailSalesTax = MonthlyDetailSalesTax + (row["salestax"].ToString() != "" ? (decimal)row["salestax"] : 0);
                MonthlyDetailTotal = MonthlyDetailTotal + (row["total"].ToString() != "" ? (decimal)row["total"] : 0);
                MonthlyDetailProductSubTotal = MonthlyDetailProductSubTotal + (row["productsubtotal"].ToString() != "" ? (decimal)row["productsubtotal"] : 0);
                MonthlyDetailLaborSubTotal = MonthlyDetailLaborSubTotal + (row["laborsubtotal"].ToString() != "" ? (decimal)row["laborsubtotal"] : 0);
                MonthlyDetailNetPayment = MonthlyDetailNetPayment + (row["netpayment"].ToString() != "" ? (decimal)row["netpayment"] : 0);
                MonthlyDetailShopFee = MonthlyDetailShopFee + (row["shopfee"].ToString() != "" ? (decimal)row["shopfee"] : 0);
            }


        }


        public bool IncludeOpen
        {
            get { return m_includeopen; }
            set { m_includeopen = value;
            NotifyPropertyChanged("IncludeOpen");
            
            }
        }


        /// <summary>
        /// Daily Report Arrays
        /// </summary>



        public DataTable MonthlySummary
        {
            get { return m_monthlysummary; }
            set
            {
                m_monthlysummary = value;
                NotifyPropertyChanged("MonthlySummary");
            }

        }







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


        public ObservableCollection<DailySettlement> MonthlyPaymentSalesReport
        {
            get { return m_monthlypaymentsalesreport; }
            set
            {
                m_monthlypaymentsalesreport = value;
                NotifyPropertyChanged("MonthlyPaymentSalesReport");
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

        public DataTable MonthlyDetailSales
        {
            get { return m_monthlydetailsales; }
            set
            {
                m_monthlydetailsales = value;
                NotifyPropertyChanged("MonthlyDetailSales");
            }

        }




        public decimal MonthlyDetailDiscount
        {
            get { return m_monthlydetaildiscount; }
            set
            {
                m_monthlydetaildiscount = value;
                NotifyPropertyChanged("MonthlyDetailDiscount");
            }
        }

        public decimal MonthlyDetailSalesTax
        {
            get { return m_monthlydetailsalestax; }
            set
            {
                m_monthlydetailsalestax = value;
                NotifyPropertyChanged("MonthlyDetailSalesTax");
            }
        }

        public decimal MonthlyDetailTotal
        {
            get { return m_monthlydetailtotal; }
            set
            {
                m_monthlydetailtotal = value;
                NotifyPropertyChanged("MonthlyDetailTotal");
            }
        }

        public decimal MonthlyDetailShopFee
        {
            get { return m_monthlydetailshopfee; }
            set
            {
                m_monthlydetailshopfee = value;
                NotifyPropertyChanged("MonthlyDetailShopFee");
            }
        }

        public decimal MonthlyDetailLaborSubTotal
        {
            get { return m_monthlydetaillaborsubtotal; }
            set
            {
                m_monthlydetaillaborsubtotal = value;
                NotifyPropertyChanged("MonthlyDetailLaborSubTotal");
            }
        }

        public decimal MonthlyDetailProductSubTotal
        {
            get { return m_monthlydetailproductsubtotal; }
            set
            {
                m_monthlydetailproductsubtotal = value;
                NotifyPropertyChanged("MonthlyDetailProductSubTotal");
            }
        }

        public decimal MonthlyDetailNetPayment
        {
            get { return m_monthlydetailnetpayment; }
            set
            {
                m_monthlydetailnetpayment = value;
                NotifyPropertyChanged("MonthlyDetailNetPayment");
            }
        }


        public DailyRevenue MonthlyDetailRevenueSalesReport
        {
            get { return m_monthlydetailrevenuesalesreport; }
            set
            {
                m_monthlydetailrevenuesalesreport = value;
                NotifyPropertyChanged("MonthlyDetailRevenueSalesReport");
            }
        }

        public DailySettlement MonthlyDetailSettlementSalesReport
        {
            get { return m_monthlydetailsettlementsalesreport; }
            set
            {
                m_monthlydetailsettlementsalesreport = value;
                NotifyPropertyChanged("MonthlyDetailSettlementSalesReport");
            }
        }

        public Difference MonthlyDetailDifferenceSalesReport
        {
            get { return m_monthlydetaildifferencesalesreport; }
            set
            {
                m_monthlydetaildifferencesalesreport = value;
                NotifyPropertyChanged("MonthlyDetailDifferenceSalesReport");
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





   


        public void ExecutePreviousMonthClicked(object tagstr)
        {


            MonthlyStartDate = MonthlyStartDate.AddMonths(-1);
            MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);

            RunMonthlyReport();
            RunMonthlyDetailReport();

        }
        public void ExecuteNextMonthClicked(object tagstr)
        {
            MonthlyStartDate = MonthlyStartDate.AddMonths(1);
            MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);

            RunMonthlyReport();
            RunMonthlyDetailReport();
        }

        public void ExecuteCustomMonthClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Hidden, DateTime.Now);

            cd.ShowDialog();
            //Utility.OpenModal(this, cd);


           MonthlyStartDate = new DateTime(cd.StartDate.Year, cd.StartDate.Month, 1);
           MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);

            RunMonthlyReport();
            RunMonthlyDetailReport();
        }


     

        public void ExecuteExportCSVMonthClicked(object tagstr)
        {

            m_report.ExportMonthlySalesCSV(MonthlyStartDate);
        }
    }
}
