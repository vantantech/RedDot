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
    public class Modifier:INPCBase
    {
        DBProducts m_products;
        public Modifier(int id)
        {
            m_products = new DBProducts();
            DataTable dt = m_products.GetModifierByID(id);
            InitData(dt.Rows[0]);
        }
        public Modifier(DataRow row)
        {
            m_products = new DBProducts();
            InitData(row);
        }

        public void InitData(DataRow row)
        {

            ID = (int)row["ID"];
            m_description = row["Description"].ToString();
            if (row["price"].ToString() != "") m_price = decimal.Parse(row["price"].ToString());
            m_colorcode = row["colorcode"].ToString();
            isglobal= row["isglobal"].ToString().Equals("1");
            quantifiable = row["quantifiable"].ToString().Equals("1");
        }


        public int ID { get; set; }

        private bool isglobal;
        public bool IsGlobal
        {
            get { return isglobal; }
            set
            {
                isglobal = value;
                NotifyPropertyChanged("IsGlobal");
                m_products.UpdateModifierInt(ID, "isglobal", isglobal ? 1 : 0);
            }
        }

        private bool quantifiable;
        public bool Quantifiable
        {
            get { return quantifiable; }
            set
            {
                quantifiable = value;
                NotifyPropertyChanged("Quantifiable");
                m_products.UpdateModifierInt(ID, "quantifiable", quantifiable ? 1 : 0);
            }
        }


        private string m_description;
        public string Description { 
            get { return m_description; }
            set {
                m_description = value;
                NotifyPropertyChanged("Description");
                m_products.UpdateModifierString(ID, "description", m_description);
            }
        }


        private decimal m_price;
        public decimal Price {
            get { return m_price; }
            set {
                m_price = value; 
                NotifyPropertyChanged("Price");
                NotifyPropertyChanged("PriceStr");
                m_products.UpdateModifierPrice(ID, m_price);
            }
        }


        private string m_colorcode;
        public string ColorCode
        {
            get { return m_colorcode; }
            set
            {
                m_colorcode = value;
                NotifyPropertyChanged("ColorCode");
                m_products.UpdateModifierString(ID, "colorcode", m_colorcode);
            }
        }

        public string PriceStr
        {
            get
            {
                if (Quantifiable) return String.Format("{0:C2}", Price);
                else return "";
            }

        }
  

    }

  
}
