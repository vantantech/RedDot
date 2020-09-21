using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RedDot;

namespace RedDot
{
    [DataContract]
    public class SalesModifier:INPCBase
    {
        DBProducts m_products;

        public SalesModifier(DataRow row)
        {
            m_products = new DBProducts();
            InitData(row);
        }

        public void InitData(DataRow row)
        {

            ID = (int)row["ID"];
            ModifierID = (int)row["modifierid"];
            SortOrder = (int)row["sortorder"];
            Description = row["Description"].ToString();
            ColorCode = row["colorcode"].ToString();
            if (row["price"].ToString() != "") Price = decimal.Parse(row["price"].ToString());
            if (row["quantity"].ToString() != "") Quantity = decimal.Parse(row["quantity"].ToString());
            Quantifiable = row["quantifiable"].ToString().Equals("1");
        }

        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int ModifierID { get; set; }
        public int SortOrder { get; set; }
        [DataMember]
        public bool Quantifiable { get; set; }
      
        private string m_description;
        [DataMember]
        public string Description { get { return m_description; } set { m_description = value; NotifyPropertyChanged("Description"); NotifyPropertyChanged("ReceiptStr"); } }

        private string m_colorcode;
        [DataMember]
        public string ColorCode { get { return m_colorcode; } set { m_colorcode = value; NotifyPropertyChanged("ColorCode"); } }


        private decimal m_price;
        [DataMember]
        public decimal Price { get { return m_price; } set { m_price = value; NotifyPropertyChanged("Price"); NotifyPropertyChanged("PriceStr"); NotifyPropertyChanged("ReceiptStr"); } }


        public bool Selected { get; set; }
        [DataMember]
        public decimal Quantity { get; set; }

        private bool m_showmodprice;
        public bool ShowModPrice { get { return m_showmodprice; } set { m_showmodprice = value; NotifyPropertyChanged("ShowModPrice"); NotifyPropertyChanged("ReceiptStr"); } }

        [DataMember]
        public decimal TotalPrice
        {
            get
            {
                return Price * Quantity;
            }
        }

        [DataMember]
        public string PriceStr
        {
            get
            {
                if (Quantifiable)
                    return String.Format("{0:C2}", TotalPrice);
                else return "";
            }

        }

        [DataMember]
        public string ReceiptStr
        {
            get
            {
                if (ID > 0)
                {
                    if (ShowModPrice) return Utility.FormatPrintRow(" * " + DoFormat(Quantity) + "x " + Description, " " + String.Format("({0:0.00})", TotalPrice), 30);
                    else return " * " + DoFormat(Quantity) + "x " + Description;

                }
                else return Description;
            }

        }

        [DataMember]
        public string QuantityStr
        {
            get
            {
                if (Quantifiable)
                    return DoFormat(Quantity);
                else return "";
            }
        }

        public string DoFormat(decimal myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int)myNumber).ToString();
            }
            else
            {
                return s;
            }
        }
    }

  
}
