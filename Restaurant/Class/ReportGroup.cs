using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class ReportGroup:INPCBase
    {
        private DBReports m_dbreports;


        public ReportGroup(DataRow row)
        {
            m_dbreports = new DBReports();

            m_grouptype = row["grouptype"].ToString();
            m_reportgroup = row["reportgroup"].ToString();
   
            m_active = row["active"].ToString().Equals("1");
            if (row["sortorder"].ToString() != "") m_sortorder = int.Parse(row["sortorder"].ToString());
            else m_sortorder = 0;

            if (row["id"].ToString() != "") ID = int.Parse(row["id"].ToString());
            else ID = 0;

            LoadCategories();
        }


        public void LoadCategories()
        {

            ReportCategory = m_dbreports.GetReportCatList(ID);
        }

        public int ID { get; set; }

        private bool m_selected;
        public bool Selected
        {
            get { return m_selected; }
            set
            {
                m_selected = value;
                NotifyPropertyChanged("Selected");
            }
        }


        private string m_grouptype;
        public string GroupType
        {
            get
            {
                return m_grouptype;
            }
            set
            {
                m_grouptype = value;
                NotifyPropertyChanged("GroupType");
            
            }
        }

        private string m_reportgroup;
        public string ReportGroupDescription
        {
            get
            {
                return m_reportgroup;
            }
            set
            {
                m_reportgroup = value;
                NotifyPropertyChanged("ReportGroupDescription");
                m_dbreports.UpdateReportSetupString(ID, "reportgroup", m_reportgroup);
            }
        }

        private DataTable m_reportcategory;
        public DataTable ReportCategory
        {
            get
            {
                return m_reportcategory;
            }
            set
            {
                m_reportcategory = value;
                NotifyPropertyChanged("ReportCategory");
             
            }
        }

        private int m_sortorder;
        public int SortOrder
        {
            get
            {
                return m_sortorder;
            }
            set
            {
                m_sortorder = value;
                NotifyPropertyChanged("SortOrder");
                m_dbreports.UpdateReportSetupInt(ID, "sortorder", m_sortorder);
            }
        }

        private bool m_active;
        public bool Active
        {
            get
            {
                return m_active;
            }
            set
            {
                m_active = value;
                NotifyPropertyChanged("Active");
                m_dbreports.UpdateReportSetupInt(ID, "active", m_active?1:0);
            }
        }

    }
}
