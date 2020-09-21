using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class InventoryReportVM:INPCBase
    {
        private DataTable m_inventoryreport;
        private Reports m_reports;
        private int m_count = 0;
        private decimal m_value = 0;



        public InventoryReportVM()
        {
            int count = 0;
            decimal value = 0;

            m_reports = new Reports();

            CurrentReport = m_reports.GetInventoryReport(StartDate,EndDate);

            foreach(DataRow row in CurrentReport.Rows)
            {
                if (row["itemcount"].ToString() != "")  count = count + int.Parse(row["itemcount"].ToString());

                if (row["itemvalue"].ToString() != "") value = value + decimal.Parse(row["itemvalue"].ToString());
            }

            TotalCount = count;
            TotalValue = value;

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
            get{ return m_inventoryreport;}
            set {
                m_inventoryreport = value;
                NotifyPropertyChanged("CurrentReport");
            }
        }

        public int TotalCount
        {
            get { return m_count; }
            set { m_count = value;
            NotifyPropertyChanged("TotalCount");
            }
        }

        public decimal TotalValue
        {
            get { return m_value; }
            set { m_value = value;
            NotifyPropertyChanged("TotalValue");
            }
        }
    }
    
}
