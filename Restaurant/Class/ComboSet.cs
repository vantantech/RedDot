using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;

namespace RedDot
{
    public class ComboSet : INPCBase
    {
        DBProducts m_dbproducts;
        public ComboSet(int id)
        {
            m_dbproducts = new DBProducts();
            DataTable dt = m_dbproducts.GetComboSetByID(id);
            InitData(dt.Rows[0]);
        }
        public ComboSet(DataRow row)
        {
            m_dbproducts = new DBProducts();
            InitData(row);
        }

        private ObservableCollection<Product> m_products;
        public ObservableCollection<Product> Products { get { return m_products; } set { m_products = value; NotifyPropertyChanged("Products"); } }


        public void InitData(DataRow row)
        {

            ID = (int)row["ID"];
            if (row["combogroupid"].ToString() != "") ComboGroupID = int.Parse(row["combogroupid"].ToString());
            else ComboGroupID = 0;
            Description = row["Description"].ToString();
            if (row["maxprice"].ToString() != "") MaxPrice = decimal.Parse(row["maxprice"].ToString());

            if (row["min"].ToString() != "") Min = int.Parse(row["min"].ToString());
            else Min = 0;
            if (row["max"].ToString() != "") Max = int.Parse(row["max"].ToString());
            else Max = 0;

            if (row["sortorder"].ToString() != "") SortOrder = int.Parse(row["sortorder"].ToString());
            else SortOrder = 0;


            AllowDuplicate = (row["allowduplicate"].ToString() == "1");

        }

        public void Save()
        {

            m_dbproducts.UpdateComboSetString(ID, "description", Description);

            m_dbproducts.UpdateComboSetPrice(ComboGroupID,ID, MaxPrice);
            m_dbproducts.UpdateComboSetInt(ComboGroupID, ID,  "min", Min);
            m_dbproducts.UpdateComboSetInt(ComboGroupID, ID,  "max", Max);
            m_dbproducts.UpdateComboSetInt(ComboGroupID, ID, "sortorder", SortOrder);
            m_dbproducts.UpdateComboSetInt(ComboGroupID, ID, "allowduplicate", (AllowDuplicate ? 1 : 0));
        }

        public int ID { get; set; }

        public int ComboGroupID { get; set; }


        private string m_description;
        public string Description
        {
            get { return m_description; }
            set
            {
                m_description = value;
                NotifyPropertyChanged("Description");
            }
        }

        private int m_sortorder;
        public int SortOrder
        {
            get { return m_sortorder; }
            set
            {
                m_sortorder = value;
          
                NotifyPropertyChanged("SortOrder");
            }
        }



        private int m_min;
        private int m_max;

        public int Min
        {
            get { return m_min; }

            set
            {
                m_min = value;
                NotifyPropertyChanged("Min");
            }

        }

        public int Max
        {
            get { return m_max; }

            set
            {
                m_max = value;
                NotifyPropertyChanged("Max");
            }

        }

        private decimal m_maxprice;
        public decimal MaxPrice
        {
            get { return m_maxprice; }
            set
            {
                m_maxprice = value;
                NotifyPropertyChanged("MaxPrice");
                NotifyPropertyChanged("MaxPriceStr");
            }
        }

        public string MaxPriceStr
        {
            get
            {
                return String.Format("{0:C2}", MaxPrice);
            }

        }

        private bool m_allowduplicate;
        public bool AllowDuplicate
        {
            get
            {
                return m_allowduplicate;
            }

            set
            {
                m_allowduplicate = value;
                NotifyPropertyChanged("AllowDuplicate");
            }
        }
    }


}
