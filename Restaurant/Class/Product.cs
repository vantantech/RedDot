using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDot;
using System.Windows;
using System.Runtime.Serialization;

namespace RedDot
{
    [DataContract]
    public class Product : INPCBase
    {


        //private decimal m_discountamt;

        private DBProducts m_dbproduct;
        private DataRow m_productdata;

        public bool SpecialPrice = false;
        private OrderType m_ordertype;

        public Product(DataRow row, OrderType ordertype)
        {
            try
            {
                m_dbproduct = new DBProducts();
                m_productdata = row;
                m_ordertype = ordertype;
                InitData();
            }
            catch (Exception e)
            {
                MessageBox.Show("Product: " + e.Message);
            }
        }

        public Product(int prodid, OrderType ordertype)
        {
            try
            {
                m_dbproduct = new DBProducts();
                m_ordertype = ordertype;
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
            Description2 = m_productdata["description2"].ToString();
            Description3 = m_productdata["description3"].ToString();
            Unit = m_productdata["unit"].ToString();

            MenuPrefix = m_productdata["menuprefix"].ToString();
            if (m_productdata["price"].ToString() != "") Price = (decimal)m_productdata["price"]; else Price =0;
            if (m_productdata["priceadj1"].ToString() != "") PriceAdj1 = (decimal)m_productdata["priceadj1"]; else PriceAdj1 = 0;
            if (m_productdata["priceadj2"].ToString() != "") PriceAdj2 = (decimal)m_productdata["priceadj2"]; else PriceAdj2 = 0;
            if (m_productdata["priceadj3"].ToString() != "") PriceAdj3 = (decimal)m_productdata["priceadj3"]; else PriceAdj3 = 0;

            if (m_productdata["combogroupid"].ToString() != "") ComboGroupId= (int)m_productdata["combogroupid"]; else ComboGroupId = 0;
  
            Type = m_productdata["type"].ToString();

            ColorCode = m_productdata["colorcode"].ToString();
            ReportCategory = m_productdata["reportcategory"].ToString();
            Taxable= m_productdata["taxable"].ToString().Equals("1");
            AgeRestricted = m_productdata["agerestricted"].ToString().Equals("1");
            AllowPartial = m_productdata["allowpartial"].ToString().Equals("1");
            Weighted = m_productdata["weighted"].ToString().Equals("1");

            m_imagesrc = m_productdata["imagesrc"].ToString();
            if (m_productdata["modprofileid"].ToString() != "") ModProfileID= (int)m_productdata["modprofileid"];    else ModProfileID= 0;

            if (m_productdata["kitchenprinter"].ToString() != "") KitchenPrinter = int.Parse(m_productdata["kitchenprinter"].ToString());

            //see if there is special pricing
             GetSpecialPricing();
        
        }

        public void Save()
        {
            m_dbproduct.UpdateProductString(ID, "description", Description);
            m_dbproduct.UpdateProductString(ID, "description2", Description2);
            m_dbproduct.UpdateProductString(ID, "description3", Description3);
            m_dbproduct.UpdateProductString(ID, "unit", Unit);
            m_dbproduct.UpdateProductString(ID, "menuprefix", MenuPrefix);
            m_dbproduct.UpdateNumeric(ID, "price", Price);
            m_dbproduct.UpdateNumeric(ID, "priceadj1", PriceAdj1);
            m_dbproduct.UpdateNumeric(ID, "priceadj2", PriceAdj2);
            m_dbproduct.UpdateNumeric(ID, "priceadj3", PriceAdj3);


            m_dbproduct.UpdateProductString(ID, "type", Type);
            m_dbproduct.UpdateProductString(ID, "ColorCode", ColorCode);
            m_dbproduct.UpdateProductString(ID, "ReportCategory", ReportCategory);
            m_dbproduct.UpdateNumeric(ID, "taxable", Taxable ? 1 : 0);
            m_dbproduct.UpdateNumeric(ID, "agerestricted", AgeRestricted ? 1 : 0);
            m_dbproduct.UpdateNumeric(ID, "allowpartial", AllowPartial ? 1 : 0);
            m_dbproduct.UpdateNumeric(ID, "weighted", Weighted ? 1 : 0);
            m_dbproduct.UpdateNumeric(ID, "combogroupid", ComboGroupId);


            if(m_imagesrc.Contains("\\\\"))
            {
                m_dbproduct.UpdateProductString(ID, "imagesrc", m_imagesrc);
            }else
            {
                m_dbproduct.UpdateProductString(ID, "imagesrc", m_imagesrc.Replace("\\", "\\\\"));
            }
        
            m_dbproduct.UpdateNumeric(ID, "modprofileid", m_modprofileid);
            m_dbproduct.UpdateNumeric(ID, "kitchenprinter",m_kitchenprinter);

            m_productdata = LoadProduct(ID);
            InitData();

        }

        private void GetSpecialPricing()
        {

            decimal discount=0;

            AdjustedPrice = Price ;

            switch(m_ordertype)
            {
                case OrderType.DineIn:
                    AdjustedPrice = Price + PriceAdj1;
                    break;
                case OrderType.Bar:
                    AdjustedPrice = Price + PriceAdj2;
                    break;

                case OrderType.ToGo:
                    AdjustedPrice = Price + PriceAdj3;
                    break;
                case OrderType.Delivery:
                    AdjustedPrice = Price + PriceAdj4;
                    break;
            }




            PromotionsModel model = new PromotionsModel();
         
     
          Promotion promo = model.GetProductAutoPrice(ID);

            if (promo == null) return;

            switch (promo.DiscountMethod)
            {
                case "PERCENT":
                    discount = Price * promo.DiscountAmount / 100;
                
                    break;

                case "AMOUNT":
                    discount = promo.DiscountAmount;
            
                    break;

            }

            AdjustedPrice = Price - discount;
            Description = Description + "-" + promo.PromoCode;
            SpecialPrice = true;

        }

        public bool HasForcedModifiers()
        {
            DataTable dt = m_dbproduct.GetForcedModGroupsByProfileID(ModProfileID);
            if (dt != null)
            {
                if (dt.Rows.Count > 0) return true;
                else return false;

            }
            else return false;
        }





        //----------------------Properties --------------------------
        [DataMember]
        public int ID
        {
            get
            {
                if (m_productdata["id"].ToString() != "") return (int)m_productdata["id"];
                else return 0;
            }
            
        }

        private int m_kitchenprinter;

        [DataMember]
        public int KitchenPrinter
        {
            get { return m_kitchenprinter; }
            set
            {
                m_kitchenprinter = value;
                NotifyPropertyChanged("KitchenPrinter");
            }
        }



        private int m_modprofileid;

        [DataMember]
        public int ModProfileID
        {
            get
            {
                return m_modprofileid;
            }
            set {
                m_modprofileid = value;
                NotifyPropertyChanged("ModProfileID");
            }
        }


        private string m_description;
        [DataMember]
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
                NotifyPropertyChanged("DescriptionCombined");
            }
        }


        private string m_description2;
        [DataMember]
        public string Description2
        {
            get
            {
                return m_description2;
            }
            set
            {
                m_description2 = value;
                NotifyPropertyChanged("Description2");
                NotifyPropertyChanged("DescriptionCombined");
            }
        }

        private string m_description3;
        [DataMember]
        public string Description3
        {
            get
            {
                return m_description3;
            }
            set
            {
                m_description3 = value;
                NotifyPropertyChanged("Description3");
            }
        }

        private string m_unit;
        [DataMember]
        public string Unit
        {
            get
            {
                return m_unit;
            }
            set
            {
                m_unit = value;
                NotifyPropertyChanged("Unit");
            }
        }


        [DataMember]
        public string DescriptionCombined
        {
            get
            {
                string rtn = Description + (Unit.Trim() == "" ? "" : " (" + Unit.Trim() + ")");
                if (Description2 != "") rtn = rtn + (char)13 + (char)10 + Description2;
                return rtn;
            }
        }

        [DataMember]
        public string DescriptionCombinedPrice
        {
            get
            {
                string rtn = Description + (Unit.Trim() == "" ? "" : " (" + Unit.Trim() + ")");
                if (Description2 != "") rtn = rtn + (char)13 + (char)10 + Description2;
                return rtn + " " + PriceStr;
            }
        }


        private int m_combogroupid;
        [DataMember]
        public int ComboGroupId
        {
            get
            {
                return m_combogroupid;
            }
            set
            {
                m_combogroupid = value;
                NotifyPropertyChanged("ComboGroupId");
            }
        }

        private string m_menuprefix;
        [DataMember]
        public string MenuPrefix
        {
            get
            {
                return m_menuprefix;
            }
            set
            {
                m_menuprefix = value;
                NotifyPropertyChanged("MenuPrefix");
            }
        }


        private decimal m_price;
        [DataMember]
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


        private decimal m_priceadj1;
        public decimal PriceAdj1
        {
            get
            {
                return m_priceadj1;
            }
            set
            {
                m_priceadj1 = value;
                NotifyPropertyChanged("PriceAdj1");
            }
        }


        private decimal m_priceadj2;
        public decimal PriceAdj2
        {
            get
            {
                return m_priceadj2;
            }
            set
            {
                m_priceadj2 = value;
                NotifyPropertyChanged("PriceAdj2");
            }
        }

        private decimal m_priceadj3;
        public decimal PriceAdj3
        {
            get
            {
                return m_priceadj3;
            }
            set
            {
                m_priceadj3 = value;
                NotifyPropertyChanged("PriceAdj3");
            }
        }

        private decimal m_priceadj4;
        public decimal PriceAdj4
        {
            get
            {
                return m_priceadj4;
            }
            set
            {
                m_priceadj4 = value;
                NotifyPropertyChanged("PriceAdj4");
            }
        }

        private decimal m_adjustedprice;
        [DataMember]
        public decimal AdjustedPrice
        {
            get
            {
                return m_adjustedprice;
            }
            set
            {
                m_adjustedprice = value;
                NotifyPropertyChanged("AdjustedPrice");
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


        private string m_colorcode;
        [DataMember]
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
        [DataMember]
        public string ReportCategory
        {
            get { return m_reportcategory; }
            set
            {
                m_reportcategory = value;
                NotifyPropertyChanged("ReportCategory");
            }
        }

    
        private bool m_taxable;
        [DataMember]
        public bool Taxable
        {
            get { return m_taxable; }
            set
            {
                m_taxable = value;
                NotifyPropertyChanged("Taxable");
            }
        }

        private bool m_agerestricted;
        [DataMember]
        public bool AgeRestricted
        {
            get { return m_agerestricted; }
            set
            {
                m_agerestricted = value;
                NotifyPropertyChanged("AgeRestricted");
            }
        }

        private bool m_allowpartial;
        [DataMember]
        public bool AllowPartial
        {
            get { return m_allowpartial; }
            set
            {
                m_allowpartial = value;
                NotifyPropertyChanged("AllowPartial");
            }
        }

        private bool m_weighted;
        [DataMember]
        public bool Weighted
        {
            get { return m_weighted; }
            set
            {
                m_weighted = value;
                NotifyPropertyChanged("Weighted");
            }
        }

        [DataMember]
        public string PriceStr
        {
            get
            {
                if (Price < 0) return "$xx.00";

                if (AdjustedPrice != Price)
                {
                    return  String.Format("<{0:C2}>", AdjustedPrice);
                }
                else
                    return String.Format("{0:C2}", Price) ;
            }

        }
        private string m_imagesrc;
        [DataMember]
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

        public string ImgUrl
        {
            get
            {
                return "pack://siteoforigin:,,,/" + m_imagesrc;
            }
            set
            {
                m_imagesrc = value;
                NotifyPropertyChanged("ImgUrl");
            }
        }

        public string IDTypeStr
        {
            get { return ID + "," + Type; }
        }


   

        [DataMember]
        public bool HasModifiers
        {
            get
            {
                if (ModProfileID > 0) return true; else return false;
            }
        }

    }
}
