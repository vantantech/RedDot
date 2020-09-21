using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;


namespace RedDot
{
    public class Product : INPCBase
    {


        //private decimal m_discountamt;

        private DBProducts m_dbproduct;
        private DataRow m_productdata;

        public string LocationName { get; set; }
        public string LocationStr { get { return LocationName.Substring(0,4).ToUpper() + "=>"; } }

        public Product(DataRow row)
        {
            try
            {
                m_dbproduct = new DBProducts();
                m_productdata = row;
                InitData();
            }
            catch (Exception e)
            {
                MessageBox.Show("Product: " + e.Message);
            }
        }

        public Product(int prodid)
        {
            try
            {
                m_dbproduct = new DBProducts();
                m_productdata = LoadProduct(prodid);
                InitData();
            }
            catch (Exception e)
            {
                MessageBox.Show("Product: " + e.Message);
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
            Description = m_productdata["description"].ToString();
            LocationName = m_productdata["locationname"].ToString();
   
            PartNumber = m_productdata["partnumber"].ToString();
            ModelNumber = m_productdata["modelnumber"].ToString();
            if (m_productdata["msrp"].ToString() != "") MSRP = (decimal)m_productdata["msrp"]; else MSRP = 0;
            if (m_productdata["price"].ToString() != "") Price = (decimal)m_productdata["price"]; else Price =0;
            if (m_productdata["qoh"].ToString() != "") QOH = (int)m_productdata["qoh"]; else QOH = 0;
            if (m_productdata["threshold"].ToString() != "") Threshold = (int)m_productdata["threshold"]; else Threshold = 0;
            if (m_productdata["surcharge"].ToString() != "") Surcharge = (decimal)m_productdata["surcharge"]; else Surcharge= 0;
            if (m_productdata["cost"].ToString() != "") Cost= (decimal)m_productdata["cost"];  else Cost= 0;

            if (m_productdata["locationid"].ToString() != "") LocationId = (int)m_productdata["locationid"]; else LocationId = 0;


            Type = m_productdata["type"].ToString();
            CommissionType = m_productdata["commissiontype"].ToString();
            if (m_productdata["commissionamt"].ToString() != "") CommissionAmt = (decimal)m_productdata["commissionamt"];  else CommissionAmt = 0;
            ColorCode = m_productdata["colorcode"].ToString();
            ReportCategory = m_productdata["reportcategory"].ToString();
            BarCode = m_productdata["barcode"].ToString();
            TaxExempt = m_productdata["taxexempt"].ToString().Equals("1");
            m_imagesrc = m_productdata["imagesrc"].ToString();
           
        }

        public void Save()
        {
            m_dbproduct.UpdateProductString(ID, "description", Description);
        
            m_dbproduct.UpdateProductString(ID, "partnumber", PartNumber);
            m_dbproduct.UpdateProductString(ID, "modelnumber", ModelNumber);
            m_dbproduct.UpdateNumeric(ID, "msrp", MSRP);
            m_dbproduct.UpdateNumeric(ID, "price", Price);
            m_dbproduct.UpdateNumeric(ID, "qoh", QOH);
            m_dbproduct.UpdateNumeric(ID, "threshold", Threshold);
            m_dbproduct.UpdateNumeric(ID, "locationid", LocationId);
            m_dbproduct.UpdateNumeric(ID, "surcharge", Surcharge);
            m_dbproduct.UpdateNumeric(ID, "cost", Cost);
            m_dbproduct.UpdateProductString(ID, "type", Type);
            m_dbproduct.UpdateProductString(ID, "commissiontype", CommissionType);
            m_dbproduct.UpdateNumeric(ID, "commissionamt", CommissionAmt);
            m_dbproduct.UpdateProductString(ID, "ColorCode", ColorCode);
            m_dbproduct.UpdateProductString(ID, "ReportCategory", ReportCategory);
            m_dbproduct.UpdateProductString(ID, "barcode", BarCode);
            m_dbproduct.UpdateNumeric(ID, "taxexempt", TaxExempt ? 1 : 0);
            m_dbproduct.UpdateProductString(ID, "imagesrc", m_imagesrc);
         

            m_productdata = LoadProduct(ID);
            InitData();

        }

        public static decimal GetProductDiscount(int id,decimal price)
        {

            decimal promo;
            DBPromotions _dbpromotions = new DBPromotions();
            //check for promotion on the specific item
            promo = _dbpromotions.GetProductToday(id,price);
            if (promo > 0)
            {
                //promotion found so item is not eligible for global promotions
                return promo;

            }
            else
            {
                //if promotion not available for that item , then we can look for global promotion on all item
                return _dbpromotions.GetProductSaleToday(id,price);

            }

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
                NotifyPropertyChanged("Description");
            }
        }



        private string m_partnumber;
        public string PartNumber
        {
            get
            {
                return m_partnumber;
            }
            set
            {
                m_partnumber = value;
                NotifyPropertyChanged("PartNumber");
            }
        }

       
        private string m_modelnumber;
        public string ModelNumber
        {
            get
            {
                return m_modelnumber;
            }
            set
            {
                m_modelnumber = value;
                NotifyPropertyChanged("ModelNumber");
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
                NotifyPropertyChanged("Price");
            }
        }


        private int m_qoh;
        public int QOH
        {
            get
            {
                return m_qoh;
            }
            set
            {
                m_qoh = value;
                NotifyPropertyChanged("QOH");
            }
        }


        private int m_threshold;
        public int Threshold
        {
            get
            {
                return m_threshold;
            }
            set
            {
                m_threshold = value;
                NotifyPropertyChanged("Threshold");
            }
        }

        private int m_locationid;
        public int LocationId
        {
            get
            {
                return m_locationid;
            }
            set
            {
                m_locationid = value;
                NotifyPropertyChanged("LocationId");
            }
        }

        public string StatusColor
        {
            get{
                if (QOH > Threshold) return "AliceBlue";
                if ( QOH > 0) return "Yellow";
                return "LightCoral";
                
            }
          
        }

        private decimal m_surcharge;
        public decimal Surcharge
        {
            get
            {
                return m_surcharge;
            }
            set
            {
                m_surcharge = value;
                NotifyPropertyChanged("Surcharge");
            }
        }

        private decimal m_msrp;
        public decimal MSRP
        {
            get
            {
                return m_msrp;
            }
            set
            {
                m_msrp = value;
                NotifyPropertyChanged("MSRP");
            }
        }


        private decimal m_cost;
        public decimal Cost
        {
            get
            {
                return m_cost;
            }
            set
            {
                m_cost = value;
                NotifyPropertyChanged("Cost");
            }
        }
        private string m_type;
        public string Type
        {
            get { return m_type; }
            set
            {
                m_type = value;
                NotifyPropertyChanged("Type");
            }
        }
        private string m_commissiontype;
        public string CommissionType
        {
            get { return m_commissiontype; }
            set
            {
                m_commissiontype = value;
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
                NotifyPropertyChanged("ReportCategory");
            }
        }
        private string m_barcode;
        public string BarCode
        {
            get { return m_barcode; }
            set
            {
                m_barcode = value;
                m_productdata = LoadProduct(ID);
            }
        }
        public decimal Discount
        {
            get {
                return GetProductDiscount(ID,Price);
            }
        }
        private bool m_taxexempt;
        public bool TaxExempt
        {
            get { return m_taxexempt; }
            set
            {
                m_taxexempt = value;
                NotifyPropertyChanged("TaxExempt");
            }
        }
        public decimal AdjustedPrice
        { 
            get
            {
                return Price - Discount;
            }
        }

        public decimal ItemValue
        {
            get
            {
                return Cost * QOH;
            }
        }


        public string PriceStr
        {
            get
            {
                if (Price < 0) return "$xx.00";

                if (AdjustedPrice < Price)
                {
                    return String.Format("{0:C2}", Price) + " " + String.Format("({0:C2})", AdjustedPrice) + ((Surcharge > 0) ? ", +" + Surcharge : "");
                }
                else
                    return String.Format("{0:C2}", Price) + ((Surcharge > 0) ? ", +" + Surcharge : "");
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
                NotifyPropertyChanged("ImageSrc");
            }
        }

        public string IDTypeStr
        {
            get { return ID + "," + Type; }
        }
    }
}
