using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RedDot
{
    public class ShippingReportVM : INPCBase
    {
        private DataTable m_inventoryreport;
        private DataTable m_inventoryreport2;
   

        private Reports m_reports;
        private int m_count = 0;
        private decimal m_value = 0;
        Window _parent;
        Security _security;

        public ICommand PreviousClicked { get; set; }
        public ICommand NextClicked { get; set; }
        public ICommand CustomClicked { get; set; }
        public ICommand PrintClicked { get; set; }
        public ICommand ViewClicked { get; set; }

        public ShippingReportVM(Window parent, Security security)
        {

            _security = security;
            _parent = parent;


            PreviousClicked = new RelayCommand(ExecutePreviousClicked, param => true);
            NextClicked = new RelayCommand(ExecuteNextClicked, param => true);
            CustomClicked = new RelayCommand(ExecuteCustomClicked, param => true);
            PrintClicked = new RelayCommand(ExecutePrintClicked, param => true);
            ViewClicked = new RelayCommand(ExecuteViewClicked, param => true);


            int daynum = (int)DateTime.Now.DayOfWeek;
            StartDate = DateTime.Now.AddDays(-daynum);
            EndDate = StartDate.AddDays(6);


            RunReport();
        }

        public void RunReport()
        {

            int count = 0;
            decimal value = 0;

            m_reports = new Reports();



            CurrentReport = m_reports.GetShippingReport(StartDate, EndDate);
            CurrentReport2 = m_reports.GetShippingReportDelayed();
           

        }


        public void ExecutePreviousClicked(object tagstr)
        {


            StartDate = StartDate.AddDays(-7);
            EndDate = StartDate.AddDays(6);
            RunReport();

        }
        public void ExecuteNextClicked(object tagstr)
        {
            StartDate = StartDate.AddDays(7);
            EndDate = StartDate.AddDays(6);
            RunReport();
        }

        public void ExecuteCustomClicked(object tagstr)
        {
            CustomDate cd = new CustomDate(Visibility.Visible, DateTime.Now);

            cd.ShowDialog();
            //Utility.OpenModal(this, cd);


            StartDate = cd.StartDate;
            EndDate = cd.EndDate;
            RunReport();
        }

        public void ExecutePrintClicked(object tagstr)
        {
            //  m_report.PrintSalesSummary(StartDate, EndDate, RevenueSalesReport, WeeklySettlementSalesReport, RevenueList, SettlementList, true);
        }



        public void ExecuteViewClicked(object salesid)
        {

            try
            {
                int id;

                if (salesid == null) return;

                if (salesid.ToString() != "") id = int.Parse(salesid.ToString());
                else id = 0;

                RetailSalesView ord = new RetailSalesView(_security, id);
                Utility.OpenModal(_parent, ord);


            }
            catch (Exception e)
            {
                MessageBox.Show("ExecuteEditClicked: " + e.Message);
            }
        }

        private DateTime m_startdate;
        private DateTime m_enddate;

        public DateTime StartDate
        {
            get { return m_startdate; }
            set { m_startdate = value; NotifyPropertyChanged("StartDate"); }
        }

        public DateTime EndDate
        {
            get { return m_enddate; }
            set { m_enddate = value; NotifyPropertyChanged("EndDate"); }
        }

        public DataTable CurrentReport
        {
            get { return m_inventoryreport; }
            set
            {
                m_inventoryreport = value;
                NotifyPropertyChanged("CurrentReport");
            }
        }

        public DataTable CurrentReport2
        {
            get { return m_inventoryreport2; }
            set
            {
                m_inventoryreport2 = value;
                NotifyPropertyChanged("CurrentReport2");
            }
        }



        public int TotalCount
        {
            get { return m_count; }
            set
            {
                m_count = value;
                NotifyPropertyChanged("TotalCount");
            }
        }

        public decimal TotalValue
        {
            get { return m_value; }
            set
            {
                m_value = value;
                NotifyPropertyChanged("TotalValue");
            }
        }
    }

}
