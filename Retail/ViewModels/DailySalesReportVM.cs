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
    public class DailySalesReportVM:INPCBase
    {
        Reports m_report;
        private ObservableCollection<ReportCat> m_revenuelist;
        private ObservableCollection<ReportCat> m_settlementlist;

        private DailyRevenue                    m_dailyrevenuesalesreport;
        private DailySettlement                 m_dailysettlementsalesreport;
        private DailySettlement                 m_dailypaymentsalesreport;

        private Difference                      m_dailydifferencesalesreport;



        private DateTime                        m_dailystartdate;
  

        private bool                            m_CanExecute = true;
        private int                             m_selectedindex;
        private DataTable                       m_dailysales;
        private DataTable                       m_dailypayments;


        private decimal                         m_dailyproductsubtotal=0;
        private decimal                         m_dailylaborsubtotal=0;
        private decimal                         m_dailytotal=0;
        private decimal                         m_dailydiscount=0;
        private decimal                         m_dailyshopfee=0;
        private decimal                         m_dailysalestax=0;
        private decimal                         m_dailynetpayment=0;
        private decimal                         m_dailypaymenttotal = 0;





        public ICommand PreviousDayClicked { get; set; }
        public ICommand NextDayClicked { get; set; }
        public ICommand CustomDayClicked { get; set; }

        public ICommand ViewTicketClicked { get; set; }

        public ICommand ExportCSVDayClicked { get; set; }
        private Window m_parent;
        private Security m_security;

        public DailySalesReportVM(Window parent, Security security)
        {
            m_parent = parent;
            m_security = security;

            PreviousDayClicked = new RelayCommand(ExecutePreviousDayClicked, param => this.m_CanExecute);
            NextDayClicked = new RelayCommand(ExecuteNextDayClicked, param => this.m_CanExecute);
            CustomDayClicked = new RelayCommand(ExecuteCustomDayClicked, param => this.m_CanExecute);

            ExportCSVDayClicked = new RelayCommand(ExecuteExportCSVDayClicked, param => this.m_CanExecute);
            ViewTicketClicked = new RelayCommand(ExecuteViewTicketClicked, param => this.m_CanExecute);

            m_report = new Reports();
 
            RevenueList = m_report.GetRevenueList();
            SettlementList = m_report.GetSettlementList();

            DailyStartDate = DateTime.Now;
            RunDailyReport();


        }

        private void RunDailyReport()
        {
            DailyRevenueSalesReport = m_report.GetDailyRevenue(DailyStartDate, DailyStartDate);
            DailySettlementSalesReport = m_report.GetDailySettlement(DailyStartDate, DailyStartDate);
            DailyPaymentSalesReport = m_report.GetDailyPayment(DailyStartDate, DailyStartDate);


            m_dailydifferencesalesreport = new Difference();
            m_dailydifferencesalesreport.NetRevenue = DailyRevenueSalesReport.TotalRevenue;
            m_dailydifferencesalesreport.NetPayment = DailySettlementSalesReport.TotalPayment;

            DailyDifferenceSalesReport = m_dailydifferencesalesreport;
            DailySales = m_report.GetDailySales(DailyStartDate);
            DailyPayments = m_report.GetPayments(DailyStartDate, DailyStartDate);

            //total the above report


                DailyDiscount = 0;
                DailySalesTax = 0;
                DailyTotal = 0;
                DailyProductSubTotal = 0;
                DailyLaborSubTotal = 0;
                DailyNetPayment = 0;
                DailyShopFee = 0;

                DailyPaymentTotal = 0;


                foreach(DataRow row in DailySales.Rows)
                {
                    DailyDiscount = DailyDiscount + (row["discount"].ToString()!=""? (decimal)row["discount"] : 0);
                    DailySalesTax = DailySalesTax + (row["salestax"].ToString() != "" ? (decimal)row["salestax"] : 0);
                    DailyTotal = DailyTotal + (row["total"].ToString() != "" ? (decimal)row["total"] : 0);
                    DailyProductSubTotal = DailyProductSubTotal + (row["productsubtotal"].ToString() != "" ? (decimal)row["productsubtotal"] : 0);
                    DailyLaborSubTotal = DailyLaborSubTotal+ (row["laborsubtotal"].ToString() != "" ? (decimal)row["laborsubtotal"] : 0);
                    DailyNetPayment = DailyNetPayment + (row["netpayment"].ToString() != "" ? (decimal)row["netpayment"] : 0);
                    DailyShopFee = DailyShopFee + (row["shopfee"].ToString() != "" ? (decimal)row["shopfee"] : 0);
                }

            foreach(DataRow row in DailyPayments.Rows)
            {
                DailyPaymentTotal = DailyPaymentTotal  +(row["netamount"].ToString() != "" ? (decimal)row["netamount"] : 0);

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


        public decimal DailyPaymentTotal
        {
            get { return m_dailypaymenttotal; }
            set
            {
                m_dailypaymenttotal = value;
                NotifyPropertyChanged("DailyPaymentTotal");
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

        public DailySettlement DailyPaymentSalesReport
        {
            get { return m_dailypaymentsalesreport; }
            set
            {
                m_dailypaymentsalesreport = value;
                NotifyPropertyChanged("DailyPaymentSalesReport");
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
                    case 1: //payment
                       
                        break;
                
                }
                NotifyPropertyChanged("SelectedIndex"); }
        }


        public DateTime DailyStartDate
        {
            get { return m_dailystartdate; }
            set { m_dailystartdate = value; NotifyPropertyChanged("DailyStartDate"); }
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
            CustomDate cd = new CustomDate(Visibility.Hidden);
  
            cd.ShowDialog();
            //Utility.OpenModal(this, cd);

            DailyStartDate = cd.StartDate;
            RunDailyReport();
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

            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteEditClicked: " + e.Message);
            }
        }


        public void ExecuteExportCSVDayClicked(object tagstr)
        {
            m_report.ExportDailySalesCSV(DailyStartDate);
        }

   
    }
}
