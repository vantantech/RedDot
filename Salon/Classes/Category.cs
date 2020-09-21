using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;
using System.Data;
using RedDot.DataManager;

namespace RedDot
{
    public class Category : INPCBase
    {


        private DBProducts m_dbproduct;
        private DataRow m_categorydata;
        //private Visibility _catimagevisible;


        public Category(DataRow row)
        {
            try
            {
                m_dbproduct = new DBProducts();
                m_categorydata = row;
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Category: " + e.Message);
            }
        }
        public Category(int catid)
        {
            try
            {
                m_dbproduct = new DBProducts();
                m_categorydata = LoadCategory(catid);
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Category: " + e.Message);
            }
        }
        DataRow LoadCategory(int catid)
        {
            DataTable dt = m_dbproduct.GetCategoryByID(catid);
            if (dt.Rows.Count > 0) return dt.Rows[0];
            else return null;

        }
        public int ID
        {
            get
            {
                if (m_categorydata["id"].ToString() != "") return (int)m_categorydata["id"];
                else return 0;
            }

        }

        public string LetterCode
        {
            get
            {
                return m_categorydata["lettercode"].ToString();
            }
            set
            {
                m_dbproduct.UpdateCategoryString(ID, "lettercode", value);
                m_categorydata = LoadCategory(ID);
                NotifyPropertyChanged("LetterCode");
            }
        }

        public string CatType
        {
            get
            {
                return m_categorydata["cattype"].ToString();
            }
            set
            {
                m_dbproduct.UpdateCategoryString(ID, "cattype", value);
                m_categorydata = LoadCategory(ID);
                NotifyPropertyChanged("CatType");
            }
        }



        public string Description
        {
            get
            {
                return m_categorydata["description"].ToString();
            }
            set
            {
                m_dbproduct.UpdateCategoryString(ID, "description", value);
                m_categorydata = LoadCategory(ID);
                NotifyPropertyChanged("Description");
            }
        }

        public int SortOrder
        {
            get
            {
                return int.Parse(m_categorydata["sortorder"].ToString());
            }
            set
            {
                m_dbproduct.UpdateCategoryInt(ID, "sortorder", value);
                m_categorydata = LoadCategory(ID);
                NotifyPropertyChanged("SortOrder");
            }
        }
        public string ImageSrc
        {
            get
            {
                return "pack://siteoforigin:,,,/" + m_categorydata["imagesrc"].ToString();
            }
            set
            {
                m_dbproduct.UpdateCategoryString(ID, "imagesrc", value);
                m_categorydata = LoadCategory(ID);
                NotifyPropertyChanged("ImageSrc");
            }
        }
        public string CategoryColorCode
        {
            get { return m_categorydata["colorcode"].ToString(); }
            set
            {


                m_dbproduct.UpdateCategoryString(ID, "ColorCode", value);
                m_categorydata = LoadCategory(ID);
                NotifyPropertyChanged("CategoryColorCode");
            }
        }

        public string ID_Type
        {
            get { return ID.ToString() + "," + CatType; }
        }

 

    }
}
