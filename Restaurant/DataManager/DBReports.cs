using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
	public class DBReports
	{

		private DBConnect m_dbconnect;

		public DBReports()
		{
			m_dbconnect = new DBConnect();
		}


	


		public DataTable GetDailySales(DateTime reportdate)
		{

			string query = "select (adjustment + itemdiscount) as discount, salesresult.*, (itemprice + coalesce(salesmodifierprice,0))  as itemtotal,coalesce(paymentresult.tips,0) as tips, coalesce(paymentresult.netpayment,0) as netpayment,( salesresult.total - coalesce(paymentresult.netpayment,0)) as balance from (select sales.id, sales.saledate, (sales.adjustment + sales.cashdiscount) as adjustment, sales.total, sales.subtotal,   (sum(salesitemcombined.discount * salesitemcombined.quantity * salesitemcombined.weight) * (-1)) as itemdiscount, sum(salesitemcombined.price * salesitemcombined.quantity * salesitemcombined.weight) as  itemprice , sum(salesmodifierprice * salesitemcombined.quantity) as salesmodifierprice,sales.salestax from sales " +
            " inner join (select salesitem.*,sum( salesmodifiers.price) as salesmodifierprice from sales inner join salesitem  on sales.id = salesitem.salesid  left outer join salesmodifiers on salesitem.id = salesmodifiers.salesitemid where status = 'Closed' and  DATE(sales.saledate) =  '" + reportdate.ToString("yyyy-MM-dd") + "' and salesitem.void =0 group by salesitem.id) as salesitemcombined on sales.id = salesitemcombined.salesid  " +
                          "  where  status = 'Closed' and  DATE(sales.saledate) =  '" + reportdate.ToString("yyyy-MM-dd") + "' group by sales.id) as salesresult left join ( select sales.id , sum(payment.netamount) as netpayment, sum(tipamount) as tips from sales inner join payment on sales.id = payment.salesid where payment.void=0 and DATE(sales.saledate) =  '" + reportdate.ToString("yyyy-MM-dd") + "' group by sales.id) as paymentresult " +
						  " on salesresult.id = paymentresult.id " ;
			return m_dbconnect.GetData(query);


		}


        public DataTable GetSalesByOrderType(DateTime startdate, DateTime enddate)
        {
            string query = "select ordertype,sum(total) as total, count(*) as ticketcount, ROUND(sum(total)/count(*),2) as avgticketamount, ROUND(count(*)/s.ticketcount * 100,2) as percentage   from sales  ";

            string sum = "select  count(*) as ticketcount, sum(total)/count(*) as avgticketamount from sales  where status='Closed' and total > 0 and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";


            string query3 = query + " CROSS JOIN (" + sum + ") s  where total > 0 and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'   group by sales.ordertype";

            return m_dbconnect.GetData(query3);
        }

        public DataTable GetSalesByItem(DateTime startdate, DateTime enddate)
        {
            string query = "select salesitem.description, sum(quantity * weight) as totalquantity,  sum(quantity * weight * (price - discount)) as totalamount " +
                            " from sales inner join salesitem on sales.id = salesitem.salesid where sales.status = 'Closed' and salesitem.void =0 and  total > 0 and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' " + 
                            " group by productid order by description";


            return m_dbconnect.GetData(query);
        }

        public DataTable GetSalesByModifier(DateTime startdate, DateTime enddate)
        {
            string query = "select salesmodifiers.description, sum(salesmodifiers.quantity ) as totalquantity,  sum(salesmodifiers.quantity *  salesmodifiers.price ) as totalamount " +
                            " from sales inner join salesitem on sales.id = salesitem.salesid inner join salesmodifiers on salesitem.id = salesmodifiers.salesitemid " +
                            " where sales.status = 'Closed' and salesitem.void =0 and  total > 0 and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' " +
                            " group by modifierid order by salesmodifiers.description";


            return m_dbconnect.GetData(query);
        }


        public DataTable GetTicketVoids(DateTime startdate, DateTime enddate)
        {
            string query = "select * from sales where status='Voided' and  DATE(sales.voiddate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' ";

            return m_dbconnect.GetData(query);
        }

        public DataTable GetTicketItemVoids(DateTime startdate, DateTime enddate)
        {
         
            string query = "select ordertype,sales.id,saledate, salesitem.voiddate, salesitem.description, (salesitem.price- salesitem.discount) * quantity as total, salesitem.custom4  from sales " + 
                            " inner join salesitem on sales.id = salesitem.salesid " +
                            " where salesitem.void = 1 and  DATE(salesitem.voiddate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' order by sales.id, salesitem.id ";
            return m_dbconnect.GetData(query);
        }


        public DataTable GetTicketPaymentVoids(DateTime startdate, DateTime enddate)
        {

            string query = "select ordertype,sales.id,saledate,payment.voiddate, concat( payment.cardgroup,payment.cardtype) as description, netamount as total, payment.custom4 from sales " +
                            " inner join payment on sales.id = payment.salesid " +
                            " where payment.void = 1 and  DATE(payment.voiddate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' ";
            return m_dbconnect.GetData(query);
        }


        public DataTable GetTicketPaymentRefunds(DateTime startdate, DateTime enddate)
        {

            string query = "select ordertype,sales.id,saledate,paymentdate, concat( payment.cardgroup,payment.cardtype) as description, (netamount + tipamount) as total, payment.custom4 from sales " +
                            " inner join payment on sales.id = payment.salesid " +
                            " where payment.void = 0 and  (transtype='RETURN' or transtype='REFUND') and  DATE(paymentdate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' ";
            return m_dbconnect.GetData(query);
        }

        public DataTable GetTicketDiscounts(DateTime startdate, DateTime enddate)
        {

            string query = "select * from sales where status='Closed' and  adjustment != 0 and  DATE(saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' ";
            return m_dbconnect.GetData(query);
        }

        public DataTable GetTicketItemDiscounts(DateTime startdate, DateTime enddate)
        {

            string query = "select * from sales inner join salesitem on sales.id = salesitem.salesid where salesitem.void=0 and  discount != 0 and  DATE(saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' ";
            return m_dbconnect.GetData(query);
        }

        public DataTable GetSalesSummary(DateTime startdate, DateTime enddate)
        {
            string salesresult = "select sales.id, date(sales.saledate) as saledate," +
                            "sum(case when salesitemcombined.Type = 'product' then (salesitemcombined.price + salesmodifierprice) * salesitemcombined.quantity  else 0 end) as food_sold," +
                            "sum(case when substr(salesitemcombined.Type, 1, 4) = 'Gift' then salesitemcombined.price   else 0 end) as giftcard_sold," +
                            "sum(salesitemcombined.discount * salesitemcombined.quantity) as discount,sales.adjustment,sales.salestax, " +
                            "sum(((salesitemcombined.price + salesmodifierprice) - coalesce(salesitemcombined.discount, 0)) * salesitemcombined.quantity) + sales.adjustment + sales.salestax as nettotal " +
                            " from sales inner join (select salesitem.*,sum( coalesce(salesmodifiers.price,0)) as salesmodifierprice from salesitem  left outer join salesmodifiers on salesitem.id = salesmodifiers.salesitemid group by salesitem.id) as salesitemcombined  on sales.id = salesitemcombined.salesid" +
                            " where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'" +
                            " group by sales.id";

            //payment is joined with sales so that we only get the payment during the sales date
            string paymentresult = "select sales.id ," + 
                                "sum(case when payment.cardgroup = 'Cash' then payment.netamount else 0 end) as cash_paid," + 
                                "sum(case when payment.cardgroup = 'Credit' then payment.netamount else 0 end) as credit_paid," +
                                 "sum(case when payment.cardgroup = 'Debit' then payment.netamount else 0 end) as debit_paid," +
                                "sum(case when substr(payment.cardgroup, 1, 4) = 'Gift' then payment.netamount else 0 end) as giftcard_redeemed," +
                                "sum(payment.netamount) as total_payment, sum(payment.tipamount) as tips " +
                                " from sales inner join payment on sales.id = payment.salesid" +
                                " where payment.void = 0 and status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by sales.id";

        


            // join the sales and payment together
            string subquery = " select salesresult.*,cash_paid,credit_paid,debit_paid,giftcard_redeemed, total_payment,tips from(" + salesresult + 			 
			                    ") as salesresult  inner join(" + paymentresult + ") as paymentresult on salesresult.id = paymentresult.id ";

            //sums the individual total into daily total
            string query = "select saledate,sum(food_sold) as food_sold," +
                          "sum(giftcard_sold) as giftcard_sold, sum(salestax) as salestax," +
                          "sum(discount) as discount, sum(adjustment) as adjustment," +
                          "sum(nettotal) as nettotal, sum(cash_paid) as cash_paid," +
                          "sum(credit_paid) as credit_paid,sum(debit_paid) as debit_paid, sum(giftcard_redeemed) as giftcard_redeemed," +
                          "sum(total_payment) as total_payment,  sum(coalesce(tips, 0)) as totaltips," +
                          "sum(total_payment) + sum(coalesce(tips, 0)) as payment_plus_tips from(" + subquery +
                          ") as summary group by summary.saledate order by saledate ";


            return m_dbconnect.GetData(query);


        }



        //exclude voided payments   06/15/2018
        public DataTable GetSalesTickets(DateTime startdate, DateTime enddate)
        {
            /*
            string salesresult_old = "select sales.id, sales.saledate," +
                        "sum(case when salesitemcombined.Type = 'product' or salesitemcombined.Type = 'combo'  then (salesitemcombined.price + salesmodifierprice) * salesitemcombined.quantity  else 0 end) as food_sold," +
                        "sum(case when substr(salesitemcombined.Type, 1, 4) = 'Gift' then salesitemcombined.price   else 0 end) as giftcard_sold," +
                        "sum(salesitemcombined.discount * salesitemcombined.quantity) as discount,sales.adjustment,sales.salestax, " +
                        "sum(((salesitemcombined.price + salesmodifierprice) - coalesce(salesitemcombined.discount, 0)) * salesitemcombined.quantity) + sales.adjustment + sales.salestax as nettotal " +
                        " from sales inner join (select salesitem.*,sum( coalesce(salesmodifiers.price,0)) as salesmodifierprice from salesitem  left outer join salesmodifiers on salesitem.id = salesmodifiers.salesitemid group by salesitem.id) as salesitemcombined  on sales.id = salesitemcombined.salesid" +
                        " where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'" +
                        " group by sales.id";
                        */

            string salesresult = "select sales.id, sales.saledate," +
                        "sum(case when allsalesitem.Type = 'product' or allsalesitem.Type = 'combo'  then(allsalesitem.price * allsalesitem.weight  + coalesce(salesmodifierprice,0)    + allsalesitem.diff ) * allsalesitem.quantity  else 0 end) as food_sold, " +
                        "sum(case when substr(allsalesitem.Type, 1, 4) = 'Gift' then allsalesitem.price   else 0 end) as giftcard_sold," +
                        "sum(allsalesitem.discount * allsalesitem.quantity) as discount,sales.cashdiscount,sales.adjustment,sales.salestax, " +
                        "sum(((allsalesitem.price* allsalesitem.weight + coalesce(salesmodifierprice,0) + allsalesitem.diff) - coalesce(allsalesitem.discount, 0)) * allsalesitem.quantity)  + sales.cashdiscount +  sales.adjustment + sales.salestax as nettotal " +
                        " from sales inner join " +
                        "( select salesitem.*,0 as diff from(select salesitem.*, sum(coalesce(salesmodifiers.price, 0) * salesmodifiers.quantity) as salesmodifierprice from salesitem " +
                        " left outer join salesmodifiers on salesitem.id = salesmodifiers.salesitemid where salesitem.type = 'product' and salesitem.void=0 group by salesitem.id) as salesitem " +
                        " union " +
                        " select  combo.*, innercombo2.diff from(select salesitem.*, coalesce(sum(coalesce(salesmodifiers.price, 0) * salesmodifiers.quantity),0) as salesmodifierprice from salesitem " +
                        " left outer join salesmodifiers on salesitem.id = salesmodifiers.salesitemid where salesitem.type = 'combo' group by salesitem.id) as combo " +
                        " inner join (select comboid, sum(diff) as diff from(select comboid, if ((sum(coalesce(salesmodifiers.price, 0)) + salesitem.price - combomaxprice) > 0,(sum(coalesce(salesmodifiers.price, 0)) + salesitem.price - combomaxprice) , 0) as diff from salesitem " +
                        " left outer join salesmodifiers on salesitem.id = salesmodifiers.salesitemid group by salesitem.id ) as innercombo  group by comboid ) as innercombo2 " +
                        " on combo.id = innercombo2.comboid ) as allsalesitem   on sales.id = allsalesitem.salesid " +
                        "  where status = 'Closed' and DATE(sales.saledate)  between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' " +
                        " group by sales.id ";


            //payment is joined with sales so that we only get the payment during the sales date
            string paymentresult = "select sales.id ," +
                                "sum(case when payment.cardgroup = 'Cash' then payment.netamount else 0 end) as cash_paid," +
                                "sum(case when payment.cardgroup = 'Credit' then payment.netamount else 0 end) as credit_paid," +
                                "sum(case when payment.cardgroup = 'Debit' then payment.netamount else 0 end) as debit_paid," +
                                "sum(case when substr(payment.cardgroup, 1, 4) = 'Gift' then payment.netamount else 0 end) as giftcard_redeemed," +
                                "sum(payment.netamount) as total_payment, sum(payment.tipamount) as tips " +
                                " from sales inner join payment on sales.id = payment.salesid" +
                                " where payment.void = 0 and status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by sales.id";




            // join the sales and payment together
            string summary = " select salesresult.*,cash_paid,credit_paid,debit_paid,giftcard_redeemed, total_payment,tips from(" + salesresult +
                                ") as salesresult  inner join(" + paymentresult + ") as paymentresult on salesresult.id = paymentresult.id ";

          
            string query = "select saledate,id as receipt_no, food_sold as food_drinks," +
                          "giftcard_sold, salestax," +
                          "discount, adjustment,cashdiscount," +
                          "nettotal, cash_paid," +
                          "credit_paid,debit_paid, giftcard_redeemed," +
                          "total_payment,  coalesce(tips, 0) as totaltips," +
                          "total_payment + coalesce(tips, 0) as payment_plus_tips from(" + summary +
                          ") as summary  order by saledate, id ";
            try
            {
                return m_dbconnect.GetData(query);
            }catch(Exception ex)
            {
                TouchMessageBox.Show(ex.Message);
                TouchMessageBox.Show(ex.InnerException.Message);
                return null;
            }

          


        }




        public DataTable GetPayments(DateTime startdate, DateTime enddate)
		{

			string query = "select * from payment where date(paymentdate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			return m_dbconnect.GetData(query);


		}

        public decimal GetStationPaymentTotal(int stationno, string cardgroup, DateTime startdate, DateTime enddate)
        {

            string query = "select sum(netamount) as total from payment where stationno=" + stationno + " and  UPPER(cardgroup) = '" + cardgroup.Trim().ToUpper() + "' and date(paymentdate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
            DataTable dt = m_dbconnect.GetData(query);

            decimal total = 0;
            
            if(dt.Rows.Count > 0)
            {
                string amtstr = dt.Rows[0]["total"].ToString();
                    if (amtstr != null && amtstr != "")
                    total = decimal.Parse(amtstr); else return 0;

            }

            return total;
        }


        public DataTable GetSalesRevenue(DateTime startdate, DateTime enddate)
		{
			/*string query = "select reportgroup as reportcategory, sum(netrevenue) as netrevenue, sum(grossrevenue) as grossrevenue from reportsetup " +
                       " left outer join (select reportcategory,sum((salesitem.price + salesitem.modprice  - salesitem.discount) * salesitem.quantity) as netrevenue,sum((salesitem.price + salesitem.modprice) * salesitem.quantity) as grossrevenue " +
					   " from sales inner join (select salesitem.*, sum(coalesce(salesmodifiers.price,0)) as modprice from salesitem left outer join salesmodifiers on salesitem.id = salesmodifiers.salesitemid group by salesitem.id) as salesitem on sales.id = salesitem.salesid    where sales.status = 'Closed' and  DATE(sales.saledate) between " +
					   " '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by  reportcategory ) as netsales on reportsetup.reportcategory = netsales.reportcategory " +
					   " where fieldname = 'RevenueCategory' group by reportgroup order by sortorder";
                       */


            string query = "select sum(coalesce(cnt,0)) as qty,  reportgroup.reportgroup, sum(netrevenue) as netrevenue, sum(grossrevenue) as grossrevenue from reportgroup " +
                            " inner join reportsetup on reportgroup.id = reportsetup.reportgroupid  " +
                           " left outer join (" +



                           " select sum(coalesce(salesitem.quantity * salesitem.weight,0)) as cnt, salesitem.reportcategory,sum((salesitem.price * salesitem.weight + salesitem.modprice - salesitem.discount) * salesitem.quantity) as netrevenue, " +
                            " sum((salesitem.price * salesitem.weight + salesitem.modprice) * salesitem.quantity) as grossrevenue " +
                            " from sales inner join (select salesitem.*, coalesce(sum(coalesce(salesmodifiers.price, 0) * salesmodifiers.quantity),0) as modprice from sales inner join salesitem on sales.id = salesitem.salesid " +
                            " left outer join salesmodifiers on salesitem.id = salesmodifiers.salesitemid where sales.status = 'Closed' and DATE(sales.saledate) between  '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and salesitem.void = 0 and salesitem.type != 'combo' " +
                             " group by salesitem.id ) as salesitem on sales.id = salesitem.salesid " +
                            " where sales.status = 'Closed' and DATE(sales.saledate) between  '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by  salesitem.reportcategory " +
                           
                            
                            
                            "  union " +


                            " select sum(coalesce(salesitem.quantity * salesitem.weight,0)) as cnt, salesitem.reportcategory, sum((salesitem.price * salesitem.weight + salesitem.modprice + salesitem.diff - salesitem.discount) * salesitem.quantity) as netrevenue, " +
                            "  sum((salesitem.price * salesitem.weight + salesitem.modprice + salesitem.diff) * salesitem.quantity) as grossrevenue from sales " +
                            " inner join ( select weight, combo.salesid,combo.id,price,discount,quantity,reportcategory,modprice , sum(diff) as diff from(select salesitem.*, coalesce(sum(coalesce(salesmodifiers.price, 0) * salesmodifiers.quantity),0) as modprice from sales inner join salesitem on sales.id = salesitem.salesid " +
                            " left outer join salesmodifiers on salesitem.id = salesmodifiers.salesitemid where sales.status = 'Closed' and DATE(sales.saledate) between  '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and salesitem.void = 0 and salesitem.type = 'combo' " +
                            " group by salesitem.id) as combo inner join " +
                            " (select  comboid, if ((sum(coalesce(salesmodifiers.price, 0)) + salesitem.price - combomaxprice) > 0,(sum(coalesce(salesmodifiers.price, 0)) + salesitem.price - combomaxprice) , 0) as diff from salesitem " +
                            " left outer join salesmodifiers on salesitem.id = salesmodifiers.salesitemid where comboid != 0 group by salesitem.id ) as innercombo on combo.id = innercombo.comboid " +
                            " group by combo.salesid,combo.id,price, discount,quantity,reportcategory,modprice ) as salesitem on sales.id = salesitem.salesid " +
                            " where sales.status = 'Closed' and DATE(sales.saledate) between   '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'   group by  salesitem.reportcategory " +
                            
                            
                            ") as netsales  on reportsetup.reportcategory = netsales.reportcategory where active=1 and grouptype = 'RevenueCategory' group by reportgroup.reportgroup order by sortorder";



            return m_dbconnect.GetData(query);


		}

		//these payments have been settled
	

		public DataTable GetSalesSettlement(DateTime startdate, DateTime enddate)
		{
			// string query = "select reportgroup as reportcategory, payments.netamount from reportsetup left outer join (select cardgroup ,sum(payment.netamount) as netamount  from sales " +
			//  " inner join payment  on sales.id = payment.salesid  where sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by  cardgroup ) as payments on reportsetup.reportgroup = payments.cardgroup  where grouptype = 'SettlementCategory' order by reportcategory";
			string query;

			query = "select reportgroup, sum(payments.netamount) as netamount from reportgroup inner join reportsetup on reportgroup.id = reportsetup.reportgroupid" +
			 " left outer join (select cardgroup ,sum(payment.netamount + coalesce(payment.tipamount,0)) as netamount from sales " +
			 "  inner join payment  on sales.id = payment.salesid " +
			 "  where sales.status = 'Closed' and payment.void =0  and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by  cardgroup ) as payments " +
             " on UPPER(reportsetup.reportcategory) = UPPER(payments.cardgroup)   where active=1 and grouptype = 'SettlementCategory' group by reportgroup.reportgroup order by sortorder";


			return m_dbconnect.GetData(query);
		}



        public DataTable GetReportGroupList(string grouptype)
        {

            string query = "select * from  reportgroup where grouptype='" + grouptype + "'  order by  sortorder";
            return m_dbconnect.GetData(query);

        }

        public DataTable GetReportCatList(int groupid)
        {

            string query = "select * from  reportsetup where reportgroupid=" +  groupid ;
            return m_dbconnect.GetData(query);

        }


        public int GetMaxSortOrder(string grouptype, string reportgroup)
        {
            try
            {
                string query = "select max(sortorder) as id from reportgroup where grouptype='" + grouptype + "' and reportgroup='" + reportgroup + "'";
                DataTable dt = m_dbconnect.GetData(query);
                if (dt.Rows.Count > 0)
                {
                    return int.Parse(dt.Rows[0]["id"].ToString());
                }
                else
                    return 0;
            }catch
            {
                return 0;
            }
        
        }
        public int AddNewGroupList(string grouptype, string reportgroup)
        {

            int max = GetMaxSortOrder(grouptype, reportgroup) + 1;
           
            string query = "insert into reportgroup (grouptype,reportgroup,sortorder,active) values ('" + grouptype + "','" + reportgroup + "'," + max + ", 1)";
            m_dbconnect.Command(query);

            query = "select max(id) as id from reportgroup where grouptype='" + grouptype + "' and reportgroup='" + reportgroup + "'";
            DataTable dt = m_dbconnect.GetData(query);
            if (dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["id"].ToString());
            }
            else
                return 0;

        }

        public bool AddNewCatList(int reportgroupid, string reportcategory)
        {
            string query = "insert into reportsetup (reportgroupid,reportcategory) values (" + reportgroupid + ",'" + reportcategory + "')";
            return m_dbconnect.Command(query);
        }

        public bool DeleteGroupList(int id)
        {
            string query = "delete from reportgroup where id=" + id;
            return m_dbconnect.Command(query);
        }

        public bool DeleteCatList(int id)
        {
            string query = "delete from reportsetup where id=" + id;
            return m_dbconnect.Command(query);
        }

        public DataTable GetRevenueList()
        {
            string query = "select distinct reportgroup from reportgroup inner join reportsetup on reportgroup.id = reportsetup.reportgroupid where active=1 and grouptype = 'RevenueCategory' order by sortorder";
            return m_dbconnect.GetData(query);

        }
        public DataTable GetSettlementList()
        {
            string query = "select distinct reportgroup  from reportgroup inner join reportsetup on reportgroup.id = reportsetup.reportgroupid  where active=1 and  grouptype = 'SettlementCategory' order by sortorder";
            return m_dbconnect.GetData(query);

        }
        public decimal GetDailySalesTax(DateTime startdate, DateTime enddate)
		{
			DataTable dt;
			string query = "select sum(salestax) as salestax from sales where sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				if (dt.Rows[0]["salestax"].ToString() != "")
					return decimal.Parse(dt.Rows[0]["salestax"].ToString());
				else return 0;
			}
			else return 0;
		}






        public decimal GetTipsWithHeld(DateTime startdate, DateTime enddate)
		{
			DataTable dt;


			string query = "select sum( payment.tipamount) as gratuity from sales inner join payment on sales.id = payment.salesid where payment.void=0 and sales.status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				if (dt.Rows[0]["gratuity"].ToString() != "")
					return decimal.Parse(dt.Rows[0]["gratuity"].ToString());
				else return 0;
			}
			else return 0;
		}

        public decimal GetAutoTips(DateTime startdate, DateTime enddate)
        {
            DataTable dt;


            string query = "select sum( autotip) as gratuity from sales  where sales.status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
            dt = m_dbconnect.GetData(query);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["gratuity"].ToString() != "")
                    return decimal.Parse(dt.Rows[0]["gratuity"].ToString());
                else return 0;
            }
            else return 0;
        }

        public decimal GetDailySalesAdjustments(DateTime startdate, DateTime enddate)
		{

			DataTable dt;


			string query = "select sum( sales.adjustment + sales.cashdiscount) as adjustment from sales where sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				if (dt.Rows[0]["adjustment"].ToString() != "")
					return decimal.Parse(dt.Rows[0]["adjustment"].ToString());
				else return 0;
			}
			else return 0;


		}

        public decimal GetDailySalesDiscounts(DateTime startdate, DateTime enddate)
        {

            DataTable dt;


            string query = "select sum( salesitem.discount * salesitem.quantity) as discount from sales " +
                " inner join salesitem on sales.id = salesitem.salesid " +
                " where salesitem.void = 0  and sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
            dt = m_dbconnect.GetData(query);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["discount"].ToString() != "")
                    return decimal.Parse(dt.Rows[0]["discount"].ToString());
                else return 0;
            }
            else return 0;


        }

        public DataTable GetMonthlySummary(DateTime startdate)
		{

			int days = DateTime.DaysInMonth(startdate.Year, startdate.Month);
			DateTime enddate = startdate.AddDays(days-1);

			string query = " select  sum(productsubtotal) as product,round(sum(salestax * productsubtotal/subtotal),2) as producttax, sum(laborsubtotal) as labor,round(sum(laborsubtotal/subtotal * salestax),2) as labortax, sum(shopfee) as shopfee,round(sum(shopfee/subtotal*salestax),2) as shopfeetax, sum(subtotal) as subtotal, sum(salestax) as taxpaid,  sum(total) as monthlytotal " +
							" from sales where date(closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and taxexempt =0 and status='Closed' " +
							" union select  sum(productsubtotal) as product,round(sum(salestax * productsubtotal/subtotal),2) as producttax, sum(laborsubtotal) as labor,round(sum(laborsubtotal/subtotal * salestax),2) as labortax, sum(shopfee) as shopfee,round(sum(shopfee/subtotal*salestax),2) as shopfeetax, sum(subtotal) as subtotal, sum(salestax) as taxpaid,  sum(total) as monthlytotal " +
							" from sales where date(closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and taxexempt =1 and status='Closed' " +
							" union select  sum(productsubtotal) as product,round(sum(salestax * productsubtotal/subtotal),2) as producttax, sum(laborsubtotal) as labor,round(sum(laborsubtotal/subtotal * salestax),2) as labortax, sum(shopfee) as shopfee,round(sum(shopfee/subtotal*salestax),2) as shopfeetax, sum(subtotal) as subtotal, sum(salestax) as taxpaid,  sum(total) as monthlytotal " +
							" from sales where date(closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  and status='Closed'";

			return m_dbconnect.GetData(query);

		}



		public DataTable GetRewardSummary(DateTime startdate, DateTime enddate, decimal rewardpercent)
		{
			//get summary of Reward from start until now

			string query = " select firstname,lastname,phone1, sales.customerid, sum(total) as totalsales,( sum(total) * " + rewardpercent / 100 + " + coalesce(credit,0)) as salesreward, coalesce(sum(amount),0) as totalusedreward, ( sum(total) * " + rewardpercent / 100 + " + coalesce(credit,0) - coalesce(sum(amount),0)) as balance  from sales " +
						 " left outer join (select * from payment where description='Reward') as rewardpayment on sales.id = rewardpayment.salesid " +
						 " inner join customer on sales.customerid = customer.id " +
						 " left outer join (select customerid, sum(cash) as credit from credit group by customerid) as credit on sales.customerid = credit.customerid " +
						 " where sales.customerid is not null  " +
						 " group by sales.customerid ";


			string query2 = " select customerid,  coalesce(sum(amount),0) as usedreward from sales " +
			  " left outer join (select * from payment  where description='Reward' ) as rewardpayment on sales.id = rewardpayment.salesid " +
			  " where customerid is not null and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  " +
			  " group by customerid ";


			string query3 = "select firstname,lastname,phone1, summary.customerid, totalsales, salesreward,coalesce( usedreward,0) as usedreward,totalusedreward,  balance " +
								" from (" + query + ") as summary left outer join (" + query2 + ") as used on summary.customerid = used.customerid";

			return m_dbconnect.GetData(query3);

		}


        public bool UpdateReportSetupString(int ID, string field, string fieldvalue)
        {
            string query = "update reportgroup set " + field + " = '" + fieldvalue + "' where id=" + ID;
            return m_dbconnect.Command(query);
        }

        public bool UpdateReportSetupInt(int ID, string field, int fieldvalue)
        {
            string query = "update reportgroup set " + field + " = " + fieldvalue + " where id=" + ID;
            return m_dbconnect.Command(query);
        }

    }



}
