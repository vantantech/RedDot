using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RedDot
{
    public class PromotionsModel
    {
        private DBPromotions m_dbpromotions;

        public PromotionsModel()
        {
            m_dbpromotions = new DBPromotions();
        }

        public DataTable GetPromotions()
        {
            return m_dbpromotions.GetPromotions();
        }

        public Promotion GetProductAutoPrice(int productid)
        {
            DataTable dt = GetPromotionToday(productid, "AUTO PRICE");

            if (dt == null) return null;
            if (dt.Rows.Count == 0) return null;

            Promotion promo = new Promotion(dt.Rows[0],false);

            return promo;
        }


        public Promotion GetProductAutoDiscount(int productid)
        {
            DataTable dt = GetPromotionToday(productid, "AUTO DISCOUNT");

            if (dt == null) return null;
            if (dt.Rows.Count == 0) return null;

            Promotion promo = new Promotion(dt.Rows[0], false);

            return promo;
        }
        public DataTable GetPromotionToday(int product_id,string discounttype)
        {
            return m_dbpromotions.GetPromotionToday(product_id, discounttype);
        }

        public bool AddPromotion()
        {
            return m_dbpromotions.AddPromotion();

        }

        public bool AddPromotionCategory(int promoid, int catid)
        {
            return m_dbpromotions.AddPromotionCategory(promoid, catid);

        }

        public bool AddPromotionProduct(int promoid,int prodid)
        {
            return m_dbpromotions.AddPromotionProduct(promoid, prodid);

        }
        public bool DeletePromotion(int promoid)
        {
            return m_dbpromotions.DeletePromotion(promoid);

        }

        public bool DeletePromotionCategory(int promoid, int catid)
        {
            return m_dbpromotions.DeletePromotionCategory(promoid,catid);

        }

        public bool DeletePromotionProduct(int promoid, int prodid)
        {
            return m_dbpromotions.DeletePromotionProduct(promoid,prodid);

        }
        public DataRow GetPromotionbyID(int id)
        {
            return m_dbpromotions.GetPromotionbyID(id);
        }


        public DataTable GetPromotionCategories(int promoid)
        {
            return m_dbpromotions.GetPromotionCategories(promoid);
        }


         public DataTable GetPromotionProducts(int promoid)
        {
            return m_dbpromotions.GetPromotionProducts(promoid);
        }
    }
}
