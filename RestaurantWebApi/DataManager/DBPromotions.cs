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
        public DataTable GetPromotionToday(int productid,  string discounttype)
        {
            string sql;
            string sql2;
            string sql3;
            string sql4;
            string dow;

            DataTable dt;
          

            try
            {
                dow = DateTime.Now.DayOfWeek.ToString();
                dow = dow.Substring(0, 3).ToUpper(); //Day of Week



                sql = "select promotion.* from promotion " +
                    " inner join promo2cat on promotion.id = promo2cat.promoid " +
                    " inner join category on promo2cat.catid = category.id " +
                    " inner join cat2prod on category.id = cat2prod.catid " +
                    " where discounttype = '" + discounttype + "' and   DATE(startdate) <= DATE(NOW()) and CAST(starttime AS TIME) <= CURTIME() and    DATE(enddate) >= DATE(NOW()) and CAST(endtime AS TIME) >= CURTIME() and " + dow + "=1 and cat2prod.prodid=" + productid + " and active=1 ";

                sql2 =  "select promotion.* from promotion " +
                    " inner join promo2cat on promotion.id = promo2cat.promoid " +
                    " where discounttype = '" + discounttype + "' and   DATE(startdate) <= DATE(NOW()) and CAST(starttime AS TIME) <= CURTIME() and    DATE(enddate) >= DATE(NOW()) and CAST(endtime AS TIME) >= CURTIME()  and " + dow + "=1 and promo2cat.prodid= " + productid + "  and active =1 ";
                sql3 = "select promotion.* from promotion " +
               " where discounttype = '" + discounttype + "' and   DATE(startdate) <= DATE(NOW()) and CAST(starttime AS TIME) <= CURTIME() and    DATE(enddate) >= DATE(NOW()) and CAST(endtime AS TIME) >= CURTIME()  and " + dow + "=1 and  id not in (select promoid from promo2cat )  and active =1 ";


                sql4 = sql + " union " + sql2 + " union " + sql3 + " order by priority desc";

                return  m_dbconnect.GetData(sql3);

        

            }
            catch
            {
                // MessageBox.Show("Promotion Errro:" + ex.Message);
                return null;
            }



        }

        //Promotions for a single product so there are no exceptions
        /*
        public decimal GetProductPromotionToday(int productid, decimal price, string discounttype)
        {
            string sql;
            string dow;
            DataTable dt;
            decimal discount = 0;

            try
            {
                dow = DateTime.Now.DayOfWeek.ToString();
                dow = dow.Substring(0, 3).ToUpper(); //field name .. mon , tue, wed , etc...


                sql = "select * from promotion " +
                     " inner join promo2cat on promotion.id = promo2cat.promoid " +
                    " where substring(discounttype,1,4) = '" + discounttype + "' and     DATE(startdate) <= DATE(NOW()) and CAST(starttime AS TIME) <= CURTIME() and    DATE(enddate) >= DATE(NOW()) and CAST(endtime AS TIME) >= CURTIME()  and " + dow + "=1 and promo2cat.prodid= " + productid + "  and active =1 order by priority";

                dt = m_dbconnect.GetData(sql);
                if (dt.Rows.Count > 0)
                {
                    string discountmethod = dt.Rows[0]["discountmethod"].ToString();
                    switch (discountmethod)
                    {
                        case "PERCENT":
                            discount = price * ((decimal)dt.Rows[0]["discountamount"]) / 100;
                            return discount;

                        case "AMOUNT":
                            discount = ((decimal)dt.Rows[0]["discountamount"]);
                            return discount;

                        case "OVERRIDE":
                            discount = price - ((decimal)dt.Rows[0]["discountamount"]);
                            return discount;
                    }

                    return 0;

                }
                else return 0;
            }
            catch
            {
                // MessageBox.Show("Promotion Errro:" + ex.Message);
                return 0;
            }


        }

  */

        public DataTable GetPromotions()
        {
            string sql;

            sql = "select  0 as selected, promotion.* from promotion order by priority desc ";
            return m_dbconnect.GetData(sql);
        }

        public DataTable GetPromotionCategories(int promoid)
        {
            string sql;



            sql = "select category.* from category " +
                 " inner join promo2cat on promo2cat.catid = category.id " +
                " where promo2cat.promoid=" + promoid + " order by category.sortorder";
            return m_dbconnect.GetData(sql);

        }

        public DataTable GetPromotionProducts(int promoid)
        {
            string sql;



            sql = "select product.* from product " +
                 " inner join promo2cat on promo2cat.prodid = product.id " +
                " where promo2cat.promoid=" + promoid + " order by description";
            return m_dbconnect.GetData(sql);
        }

        public DataTable GetProductIDs(int promoid)
        {

            string sql = "select cat2prod.prodid from promo2cat  " +
          " inner join category on promo2cat.catid = category.id " +
          " inner join cat2prod on category.id = cat2prod.catid " +
          " where promo2cat.promoid=" + promoid ;

           string  sql2 = "select promo2cat.prodid from promo2cat  " +
           " where prodid != 0 and promo2cat.promoid= " + promoid ;

           string sql3 = sql + " union " + sql2 ;
            return m_dbconnect.GetData(sql3);

        }

        public DataRow GetPromotionbyID(int id)
        {
            string sql;



            sql = "select  * from promotion where id=" + id;
            var dt = m_dbconnect.GetData(sql);
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

        public bool DeletePromotionCategory(int promotionid, int catid)
        {
            string query;

            query = "delete from promo2cat where promoid=" + promotionid + " and catid=" + catid;
            return m_dbconnect.Command(query);
        }

        public bool DeletePromotionProduct(int promotionid, int prodid)
        {
            string query;

            query = "delete from promo2cat where promoid=" + promotionid + " and prodid=" + prodid;
            return m_dbconnect.Command(query);
        }

        public bool AddPromotion()
        {
            string query;

            string promodate = DateTime.Now.ToString("yyyy-MM-dd");
            string endtime = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");

            query = "insert into promotion (description,startdate,enddate,starttime,endtime,active,discounttype,discountmethod) values ('New Discount','" + promodate + "','" + promodate + "','" + promodate + "','" + endtime + "',1,'DISCOUNT','PERCENT')";
            return m_dbconnect.Command(query);
        }


        public bool AddPromotionCategory(int promotionid, int catid)
        {
            string query;

            query = "insert into promo2cat (promoid,catid) values (" + promotionid + "," + catid + ")";
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


        public bool DeductUsage(int id)
        {
            string query;

            query = "update promotion set usagenumber = (usagenumber - 1)  where id=" + id;
            return m_dbconnect.Command(query);

        }
    }
}
