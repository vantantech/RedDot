using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RedDot;
using System.Data;

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
                MessageBox.Show("Product: " + e.Message);
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
                MessageBox.Show("Category: " + e.Message);
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

        public int parentid
        {
            get
            {
                if (m_categorydata["parentid"].ToString() != "") return (int)m_categorydata["parentid"];
                else return 0;
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
        public string ImageSrc
        {
            get
            {
                return "pack://siteoforigin:,,,/" + m_categorydata["imagesrc"].ToString();
            }
            set
            {
                m_dbproduct.UpdateCategoryString(ID, "imagesrc", value);
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
