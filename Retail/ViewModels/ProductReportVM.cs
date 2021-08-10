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
    public class ProductReportVM : INPCBase
    {
        private DataTable m_inventoryreport;
        private Reports m_reports;
        private int m_count = 0;
        private decimal m_value = 0;


        public ICommand PreviousClicked { get; set; }
        public ICommand NextClicked { get; set; }
        public ICommand CustomClicked { get; set; }
        public ICommand PrintClicked { get; set; }

        public ProductReportVM()
        {


            PreviousClicked = new RelayCommand(ExecutePreviousClicked, param => true);
            NextClicked = new RelayCommand(ExecuteNextClicked, param => true);
            CustomClicked = new RelayCommand(ExecuteCustomClicked, param => true);
            PrintClicked = new RelayCommand(ExecutePrintClicked, param => true);



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



            CurrentReport = m_reports.GetProductSalesReport(StartDate, EndDate);

            foreach (DataRow row in CurrentReport.Rows)
            {
                if (row["cnt"].ToString() != "") count = count + int.Parse(row["cnt"].ToString());

                if (row["total"].ToString() != "") value = value + decimal.Parse(row["total"].ToString());
            }

            TotalCount = count;
            TotalValue = value;
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
