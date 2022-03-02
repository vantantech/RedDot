using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;
using RedDot.DataManager;
using RedDotBase;

namespace RedDot
{
    public class MenuItem : INPCBase
    {


       // private decimal m_discountamt;

        private DBProducts m_dbproduct;
        private DataRow m_productdata;

        private bool m_productshowdiscount;

        private bool m_savetodb=false;

        public MenuItem(DataRow row,bool savetodb)
        {
            try
            {
                m_savetodb = savetodb;
                m_dbproduct = new DBProducts();
                m_productdata = row;
                InitData();
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Product: " + e.Message);
            }
        }

        public MenuItem(int prodid,bool savetodb)
        {
            try
            {
                m_savetodb = savetodb;
                m_dbproduct = new DBProducts();
                m_productdata = LoadProduct(prodid);
                InitData();
            }
            catch (Exception e)
            {
                TouchMessageBox.Show("Product: " + e.Message);
            }
        }

        DataRow LoadProduct(int prodid)
        {
            DataTable dt = m_dbproduct.GetProductByID(prodid);
            if (dt.Rows.Count > 0) return dt.Rows[0];
            else return null;

        }


        private void InitData()
        {

            m_productshowdiscount = GlobalSettings.Instance.ProductShowDiscount;

            m_description         = m_productdata["description"].ToString();
            m_menuprefix          = m_productdata["menuprefix"].ToString();
            m_barcode             = m_productdata["barcode"].ToString();
            m_type                = m_productdata["type"].ToString();
            m_commissiontype      = m_productdata["commissiontype"].ToString();
            m_colorcode           = m_productdata["colorcode"].ToString();
            m_reportcategory      = m_productdata["reportcategory"].ToString();
            m_imagesrc            = m_productdata["imagesrc"].ToString();

            if (m_productdata["price"].ToString() != "") m_price                 = (decimal)m_productdata["price"];             else m_price = 0;
            if (m_productdata["turnvalue"].ToString() != "") m_turnvalue         = (decimal)m_productdata["turnvalue"];         else m_turnvalue = 0;
            if (m_productdata["supplyfee"].ToString() != "") m_supplyfee         = (decimal)m_productdata["supplyfee"];         else m_supplyfee = 0;
            if (m_productdata["commissionamt"].ToString() != "") m_commissionamt = (decimal)m_productdata["commissionamt"];     else m_commissionamt = 0;
            Taxable= Convert.ToBoolean(m_productdata["taxable"]);

       
        }


        private decimal  GetDiscount()
        {

            decimal discount = 0;

  

     


            DBPromotions _dbpromotions = new DBPromotions();
            //check for promotion on the specific item

            DataTable dt = _dbpromotions.GetPromotionToday(ID, "AUTO PRICE");

            if (dt == null) return 0;
            if (dt.Rows.Count == 0) return 0;


            string discountmethod = dt.Rows[0]["discountmethod"].ToString();
            switch (discountmethod)
            {
                case "PERCENT":
                    discount = Price * ((decimal)dt.Rows[0]["discountamount"]) / 100;
                 
                    break;

                case "AMOUNT":
                    discount = ((decimal)dt.Rows[0]["discountamount"]);
                
                    break;

            }

            return discount;
        }


        public int ID
        {
            get
            {
                if (m_productdata["id"].ToString() != "") return (int)m_productdata["id"];
                else return 0;
            }
            
        }

  
        private string m_description;
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
                if(m_savetodb) m_dbproduct.UpdateProductString(ID, "description", m_description);
          
                NotifyPropertyChanged("Description");
            }
        }


        private string m_menuprefix;
        public string MenuPrefix
        {
            get
            {
                return m_menuprefix;
            }
            set
            {
                m_menuprefix = value;
                if (m_savetodb) m_dbproduct.UpdateProductString(ID, "menuprefix", m_menuprefix);
                NotifyPropertyChanged("MenuPrefix");
            }
        }

 
        private decimal m_price;
        public decimal Price
        {
            get
            {
                return m_price;
            }
            set
            {
                m_price = value;
                if (m_savetodb) m_dbproduct.UpdateNumeric(ID, "price", m_price);
                NotifyPropertyChanged("Price");
            }
        }


        private decimal m_turnvalue;
        public decimal TurnValue
        {
            get
            {
                return m_turnvalue;
            }
            set
            {
                m_turnvalue = value;
                if (m_savetodb) m_dbproduct.UpdateNumeric(ID, "turnvalue", TurnValue);
                NotifyPropertyChanged("TurnValue");
            }
        }


        private decimal m_supplyfee;
        public decimal SupplyFee
        {
            get
            {
                return m_supplyfee;
            }
            set
            {
                m_supplyfee = value;
                if (m_savetodb) m_dbproduct.UpdateNumeric(ID, "supplyfee", SupplyFee);
                NotifyPropertyChanged("SupplyFee");
            }
        }

        private string m_barcode;
        public string BarCode
        {
            get { return m_barcode; }
            set
            {
                m_barcode = value;
                if (m_savetodb) m_dbproduct.UpdateProductString(ID, "barcode",m_barcode);
                NotifyPropertyChanged("BarCode");
            }
        }

        private bool m_taxable;
        public bool Taxable
        {
            get { return m_taxable; }
            set
            {
                m_taxable = value;
                if (m_savetodb) m_dbproduct.UpdateProductBool(ID, "taxable", m_taxable);
                NotifyPropertyChanged("Taxable");
            }
        }

        private string m_type;
        public string Type
        {
            get { return m_type; }
            set
            {
                m_type = value;
                if (m_savetodb) m_dbproduct.UpdateProductString(ID, "type", Type);
                NotifyPropertyChanged("Type");
            }
        }

        public ProductType ItemEnumType
        {
            get
            {
                switch (m_type.Replace(" ", "").ToUpper())
                {
                    case "GIFTCARD": return ProductType.GiftCard;
                    case "GIFTCERTIFICATE": return ProductType.GiftCertificate;
                    case "PRODUCT": return ProductType.Product;
                    case "SERVICE": return ProductType.Service;


                    default: return ProductType.Service;
                }
            }
        }


        private string m_commissiontype;
        public string CommissionType
        {
            get { return m_commissiontype; }
            set
            {
                m_commissiontype = value;
                if (m_savetodb) m_dbproduct.UpdateProductString(ID, "commissiontype", CommissionType);
                NotifyPropertyChanged("CommissionType");
            }
        }
        private decimal m_commissionamt;
        public decimal CommissionAmt
        {
            get
            {
                return m_commissionamt;
            }
            set
            {
                m_commissionamt = value;
                if (m_savetodb) m_dbproduct.UpdateNumeric(ID, "commissionamt", CommissionAmt);
                NotifyPropertyChanged("CommissionAmt");
            }
        }
        private string m_colorcode;
        public string ColorCode
        {
            get { return m_colorcode; }
            set
            {
                m_colorcode = value;
                if (m_savetodb) m_dbproduct.UpdateProductString(ID, "ColorCode", ColorCode);
                NotifyPropertyChanged("ColorCode");
            }
        }
        private string m_reportcategory;
        public string ReportCategory
        {
            get { return m_reportcategory; }
            set
            {
                m_reportcategory = value;
                if (m_savetodb) m_dbproduct.UpdateProductString(ID, "ReportCategory", ReportCategory);
                NotifyPropertyChanged("ReportCategory");
            }
        }

        private string m_imagesrc;
        public string ImageSrc
        {
            get
            {
                return "pack://siteoforigin:,,,/" + m_imagesrc;
            }
            set
            {
                m_imagesrc = value;
                if (m_savetodb) m_dbproduct.UpdateProductString(ID, "imagesrc", m_imagesrc);
                NotifyPropertyChanged("ImageSrc");
            }
        }




        public decimal Discount
        {
            get
            {
                if (Price <= 0) return 0;
                return GetDiscount();
            }
        }

        public decimal AdjustedPrice
        {
            get
            {
                return Price - Discount;
            }
        }

        public string PriceStr
        {
            get
            {
                if (Price < 0) return "xx.00";

                if (Discount > 0 && m_productshowdiscount)
                {
                    return String.Format("{0:f2}", Price) + " " + String.Format("({0:f2})", Discount);
                }
                else
                    return String.Format("{0:f2}", Price);
            }

        }



        public string IDTypeStr
        {
            get { return ID + "," + Type; }
        }
    }
}
