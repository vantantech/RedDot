using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
    public class DBPromotions
    {
        private DBConnect _dbconnect;

        public DBPromotions()
        {
            _dbconnect = new DBConnect();
        }


        //Get sales on all product except for any under exceptions
        public decimal GetProductSaleToday(int productid, decimal price)
        {
            string sql;
            string dow;
            decimal discountamount;
            DataTable dt;


            dow = DateTime.Now.DayOfWeek.ToString();
            dow = dow.Substring(0, 3).ToUpper(); //Day of Week



            sql = "select  if(discounttype='Amount', discountamount, discountamount/100 * " + price + ") as discountcalc " + 
                " from promotion where startdate < NOW() and enddate > NOW() and dow like '%" + dow + "%' and productid=0 and active=1 and discounttype != 'Exception' ";
            dt= _dbconnect.GetData(sql, "promotion");

            if (dt.Rows.Count > 0)
            {

                if (dt.Rows[0]["discountcalc"].ToString() != "")
                {
                    discountamount = (decimal)dt.Rows[0]["discountcalc"];

                    //Look for exceptions .. exclusions
                    sql = "select  promotion.productid, cat2prod.prodid as prodid from promotion " +
                            " left outer join cat2prod on promotion.categoryid = cat2prod.catid " +
                            " where startdate < NOW() and enddate > NOW() and dow like '%" + dow + "%' and  active=1 and discounttype = 'Exception' and (productid=" + productid + "  or prodid=" + productid + ") ";
                    dt = _dbconnect.GetData(sql, "promotion");

                    if (dt.Rows.Count > 0) return 0;
                    else return discountamount;
                }
                else return 0;

               

            }
            else return 0;

        }

/*
        public decimal GetServiceSaleToday(int serviceid)
        {
            string sql;
            string dow;
            decimal discountamount;
            DataTable dt;


            dow = DateTime.Now.DayOfWeek.ToString();
            dow = dow.Substring(0, 3).ToUpper(); //Day of Week



            sql = "select  if(discounttype='Amount', discountamount, discountamount/100 * (select price from service where id = " + serviceid + ")) as discountcalc " +
                " from promotion where startdate < NOW() and enddate > NOW() and dow like '%" + dow + "%' and serviceid=0 and active=1 and discounttype != 'Exception' ";
            dt = _dbconnect.GetData(sql, "promotion");

            if (dt.Rows.Count > 0)
            {
                discountamount = (decimal)dt.Rows[0]["discountcalc"];

                //Look for exceptions .. exclusions
                sql = "select  promotion.serviceid from promotion " +
                        " left outer join cat2service on promotion.categoryid = cat2service.catid " +
                        " where startdate < NOW() and enddate > NOW() and dow like '%" + dow + "%' and  active=1 and discounttype = 'Exception' and (promotion.serviceid=" + serviceid + "  or cat2service.serviceid=" + serviceid + ") ";
                dt = _dbconnect.GetData(sql, "promotion");

                if (dt.Rows.Count > 0) return 0;
                else return discountamount;


            }
            else return 0;

        } */


        public DataTable GetAllToday()
        {
            string sql;
            string dow;

            dow = DateTime.Now.DayOfWeek.ToString();
            dow = dow.Substring(0, 3).ToUpper();
            sql = "select * from promotion where startdate < NOW() and enddate > NOW() and dow like '%" + dow + "%'  and active=1 ";
            return _dbconnect.GetData(sql, "promotion");

        }


        public decimal GetProductToday(int productid, decimal price)
        {
            string sql;
            string dow;
            DataTable dt;

            dow = DateTime.Now.DayOfWeek.ToString();
            dow = dow.Substring(0, 3).ToUpper();
            sql = "select if(discounttype = 'Amount',discountamount, discountamount/100 * " + price + ") as discountamount from promotion " +
                      " where startdate < NOW() and enddate > NOW() and dow like '%" + dow + "%' and productid = " + productid + "  and active =1";

            dt = _dbconnect.GetData(sql, "promotion");
            if (dt.Rows.Count > 0)
            {
                return (decimal)dt.Rows[0]["discountamount"];

            }
            else return 0;

        }



        /*
        public decimal GetServiceToday(int serviceid)
        {
            string sql;
            string dow;
            DataTable dt;

            dow = DateTime.Now.DayOfWeek.ToString();
            dow = dow.Substring(0, 3).ToUpper();
            sql = "select if(discounttype = 'Amount',discountamount, discountamount/100 * price) as discountamount from promotion " +
                " inner join service on promotion.serviceid = service.id " +
                " where startdate < NOW() and enddate > NOW() and dow like '%" + dow + "%' and serviceid = " + serviceid + "  and active =1";

            dt = _dbconnect.GetData(sql, "promotion");
            if (dt.Rows.Count > 0)
            {
                return (decimal)dt.Rows[0]["discountamount"];

            }
            else return 0;

        }*/

    }
}
