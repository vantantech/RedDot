using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RedDot.OrderService.Class
{
    public class Product 
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
            if (m_productdata["price"].ToString() != "") Price = (decimal)m_productdata["price"]; else Price = 0;
            if (m_productdata["priceadj1"].ToString() != "") PriceAdj1 = (decimal)m_productdata["priceadj1"]; else PriceAdj1 = 0;
            if (m_productdata["priceadj2"].ToString() != "") PriceAdj2 = (decimal)m_productdata["priceadj2"]; else PriceAdj2 = 0;
            if (m_productdata["priceadj3"].ToString() != "") PriceAdj3 = (decimal)m_productdata["priceadj3"]; else PriceAdj3 = 0;

            if (m_productdata["combogroupid"].ToString() != "") ComboGroupId = (int)m_productdata["combogroupid"]; else ComboGroupId = 0;

            Type = m_productdata["type"].ToString();

            ColorCode = m_productdata["colorcode"].ToString();
            ReportCategory = m_productdata["reportcategory"].ToString();
            Taxable = m_productdata["taxable"].ToString().Equals("1");
            AgeRestricted = m_productdata["agerestricted"].ToString().Equals("1");
            AllowPartial = m_productdata["allowpartial"].ToString().Equals("1");
            Weighted = m_productdata["weighted"].ToString().Equals("1");

            m_imagesrc = m_productdata["imagesrc"].ToString();
            if (m_productdata["modprofileid"].ToString() != "") ModProfileID = (int)m_productdata["modprofileid"]; else ModProfileID = 0;

            if (m_productdata["kitchenprinter"].ToString() != "") KitchenPrinter = int.Parse(m_productdata["kitchenprinter"].ToString());

            //see if there is special pricing
            GetSpecialPricing();

        }

  

        private void GetSpecialPricing()
        {

            decimal discount = 0;

            AdjustedPrice = Price;

            switch (m_ordertype)
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





            DBPromotions _dbpromotions = new DBPromotions();
            //check for promotion on the specific item

            DataTable dt = _dbpromotions.GetPromotionToday(ID, "AUTO PRICE");

            if (dt == null) return;
            if (dt.Rows.Count == 0) return;


            string discountmethod = dt.Rows[0]["discountmethod"].ToString();
            switch (discountmethod)
            {
                case "PERCENT":
                    discount = Price * ((decimal)dt.Rows[0]["discountamount"]) / 100;
                    AdjustedPrice = Price - discount;
                    Description = Description + "-" + dt.Rows[0]["promocode"].ToString();
                    SpecialPrice = true;
                    break;

                case "AMOUNT":
                    discount = ((decimal)dt.Rows[0]["discountamount"]);
                    AdjustedPrice = Price - discount;
                    Description = Description + "-" + dt.Rows[0]["promocode"].ToString();
                    SpecialPrice = true;
                    break;

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

        private int m_kitchenprinter;
        public int KitchenPrinter
        {
            get { return m_kitchenprinter; }
            set
            {
                m_kitchenprinter = value;
               
            }
        }



        private int m_modprofileid;
        public int ModProfileID
        {
            get
            {
                return m_modprofileid;
            }
            set
            {
                m_modprofileid = value;
              
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
              
            }
        }


        private string m_description2;
        public string Description2
        {
            get
            {
                return m_description2;
            }
            set
            {
                m_description2 = value;
               
            }
        }

        private string m_description3;
        public string Description3
        {
            get
            {
                return m_description3;
            }
            set
            {
                m_description3 = value;
            
            }
        }

        private string m_unit;
        public string Unit
        {
            get
            {
                return m_unit;
            }
            set
            {
                m_unit = value;
              
            }
        }

        public string DescriptionCombined
        {
            get
            {
                string rtn = Description + (Unit.Trim() == "" ? "" : " (" + Unit.Trim() + ")");
                if (Description2 != "") rtn = rtn + (char)13 + (char)10 + Description2;
                return rtn;
            }
        }

        private int m_combogroupid;
        public int ComboGroupId
        {
            get
            {
                return m_combogroupid;
            }
            set
            {
                m_combogroupid = value;
               
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
                
            }
        }

        private decimal m_adjustedprice;
        public decimal AdjustedPrice
        {
            get
            {
                return m_adjustedprice;
            }
            set
            {
                m_adjustedprice = value;
             
            }
        }



        private string m_type;
        public string Type
        {
            get { return m_type; }
            set
            {
                m_type = value;
            
            }
        }


        private string m_colorcode;
        public string ColorCode
        {
            get { return m_colorcode; }
            set
            {
                m_colorcode = value;
            
            }
        }
        private string m_reportcategory;
        public string ReportCategory
        {
            get { return m_reportcategory; }
            set
            {
                m_reportcategory = value;
          
            }
        }


        private bool m_taxable;
        public bool Taxable
        {
            get { return m_taxable; }
            set
            {
                m_taxable = value;
              
            }
        }

        private bool m_agerestricted;
        public bool AgeRestricted
        {
            get { return m_agerestricted; }
            set
            {
                m_agerestricted = value;
             
            }
        }

        private bool m_allowpartial;
        public bool AllowPartial
        {
            get { return m_allowpartial; }
            set
            {
                m_allowpartial = value;
               
            }
        }

        private bool m_weighted;
        public bool Weighted
        {
            get { return m_weighted; }
            set
            {
                m_weighted = value;
               
            }
        }

        public string PriceStr
        {
            get
            {
                if (Price < 0) return "$xx.00";

                if (AdjustedPrice != Price)
                {
                    return String.Format("<{0:C2}>", AdjustedPrice);
                }
                else
                    return String.Format("{0:C2}", Price);
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
               
            }
        }

        public string IDTypeStr
        {
            get { return ID + "," + Type; }
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

        public bool HasModifiers
        {
            get
            {
                if (ModProfileID > 0) return true; else return false;
            }
        }

    }
}