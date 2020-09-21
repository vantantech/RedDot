using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RedDot
{
    public class DBPromotions
    {
        private DBConnect m_dbconnect;

        public DBPromotions()
        {
            m_dbconnect = new DBConnect();
        }


        //Get promotion on all product except for any under exceptions
        public decimal GetCategoryPromotionToday(int productid, decimal price)
        {
            string sql;
            string dow;
            decimal discountamount;
            DataTable dt;

            try
            {
                dow = DateTime.Now.DayOfWeek.ToString();
                dow = dow.Substring(0, 3).ToUpper(); //Day of Week



                sql = "select  if(discounttype='Amount', discountamount, discountamount/100 * " + price + ") as discountcalc  from promotion " +
                    " inner join promo2cat on promotion.id = promo2cat.promoid " +
                    " inner join category on promo2cat.catid = category.id " +
                    " inner join cat2prod on category.id = cat2prod.catid " +
                    " where startdate < NOW() and enddate > NOW() and " + dow + "=1 and cat2prod.prodid=" + productid + " and active=1  ";
                dt = m_dbconnect.GetData(sql, "promotion");

                if (dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["discountcalc"].ToString() != "")
                    {
                        discountamount = (decimal)dt.Rows[0]["discountcalc"];
                        return discountamount;
                    }
                    else return 0;



                }
                else return 0;
            
            }catch
            {
               // MessageBox.Show("Promotion Errro:" + ex.Message);
                return 0;
            }

         

        }





        //Promotions for a single product so there are no exceptions
        public decimal GetProductPromotionToday(int productid, decimal price)
        {
            string sql;
            string dow;
            DataTable dt;

            try
            {
                dow = DateTime.Now.DayOfWeek.ToString();
                dow = dow.Substring(0, 3).ToUpper();


                sql = "select if(discounttype = 'Amount',discountamount, discountamount/100 * " + price + ") as discountamount from promotion " +
                     " inner join promo2cat on promotion.id = promo2cat.promoid " +
                    " where startdate < NOW() and enddate > NOW() and " + dow + "=1 and promo2cat.prodid= " + productid + "  and active =1";

                dt = m_dbconnect.GetData(sql, "promotion");
                if (dt.Rows.Count > 0)
                {
                    return (decimal)dt.Rows[0]["discountamount"];

                }
                else return 0;
            }catch(Exception ex)
            {
               // MessageBox.Show("Promotion Errro:" + ex.Message);
                return 0;
            }
          

        }




        public DataTable GetPromotions()
        {
            string sql;



            sql = "select  0 as selected, promotion.* from promotion ";
            return m_dbconnect.GetData(sql, "promotion");

        }

        public DataTable GetPromotionCategories(int promoid)
        {
            string sql;



            sql = "select category.* from category " +
                 " inner join promo2cat on promo2cat.catid = category.id " +
                " where promo2cat.promoid=" + promoid + " order by category.sortorder";
            return m_dbconnect.GetData(sql, "promotion");

        }

        public DataTable GetPromotionProducts(int promoid)
        {
            string sql;



            sql = "select product.* from product " +
                 " inner join promo2cat on promo2cat.prodid = product.id " +
                " where promo2cat.promoid=" + promoid + " order by menuprefix";
            return m_dbconnect.GetData(sql, "promotion");

        }

        public DataRow GetPromotionbyID(int id)
        {
            string sql;



            sql = "select  * from promotion where id=" + id;
            var dt = m_dbconnect.GetData(sql, "promotion");
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }
            else return null;

        }

        public bool DeletePromotion(int promotionid)
        {
            try
            {
                string query;

                query = "delete from promo2cat where promoid=" + promotionid;
                m_dbconnect.Command(query);


                query = "delete from promotion where id=" + promotionid;
                return m_dbconnect.Command(query);
            }
            catch
            {
                return false;
            }
           
        }

        public bool DeletePromotionCategory(int promotionid,int catid)
        {
            string query;

            query = "delete from promo2cat where promoid=" + promotionid + " and catid=" + catid;
            return m_dbconnect.Command(query);
        }

        public bool DeletePromotionProduct(int promotionid,int prodid)
        {
            string query;

            query = "delete from promo2cat where promoid=" + promotionid + " and prodid=" + prodid;
            return m_dbconnect.Command(query);
        }

        public bool AddPromotion()
        {
            string query;

            query = "insert into promotion (description,startdate,enddate,active,discounttype,promotype) values ('New Discount',NOW(),NOW(),1,'Percent','Discount')";
            return m_dbconnect.Command(query);
        }


        public bool AddPromotionCategory(int promotionid,int catid)
        {
            string query;

            query = "insert into promo2cat (promoid,catid) values (" + promotionid + "," +  catid + ")";
            return m_dbconnect.Command(query);
        }


        public bool AddPromotionProduct(int promotionid, int prodid)
        {
            string query;

            query = "insert into promo2cat (promoid,prodid) values (" + promotionid + "," + prodid + ")";
            return m_dbconnect.Command(query);
        }









        public bool UpdatePromotionString(int id, string field, string fieldvalue)
        {
            string query;

            query = "update promotion set " + field + " ='" + fieldvalue + "' where id=" + id;
            return m_dbconnect.Command(query);

        }


        public bool UpdatePromotionNumeric(int id, string field, string fieldvalue)
        {
            string query;

            query = "update promotion set " + field + " =" + fieldvalue + " where id=" + id;
            return m_dbconnect.Command(query);

        }

    }
}
