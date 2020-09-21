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


		public DataTable GetWorkedEmployees(DateTime startdate, DateTime enddate)
		{


			string query = " select distinct salesitem.employeeid  from sales inner join  salesitem on sales.id = salesitem.salesid  where  DATE(sales.saledate) between '"
				+ startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and sales.status = 'Closed' and  salesitem.employeeid > 0 and salesitem.commissiontype != 'none' order by employeeid";
			return m_dbconnect.GetData(query);
		}




		public DataTable GetSalonSalesCommissionByID(int employeeid, DateTime startdate, DateTime enddate)
		{


			string query = "select salesitem.*, sales.saledate, pay.paymenttype from salesitem " +
							" inner join sales on salesitem.salesid = sales.id" +
							" inner join (select salesid, group_concat(description) as paymenttype from payment group by salesid ) as pay on salesitem.salesid = pay.salesid ";


			if (employeeid >= 900)
				query = query + " where  status='Closed' ";
			else
				query = query + " where salesitem.employeeid =  " + employeeid + " and status='Closed' ";


			query = query + " and   DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and salesitem.commissiontype != 'none' ";
			query = query + "  order by salesitem.salesid, salesitem.id";


			return m_dbconnect.GetData(query);
		}

		public decimal GetEmployeeGratuitybySalesID(int employeeid, int SalesID)
		{
			DataTable dt;
			string query = "select * from gratuity where employeeid = " + employeeid + " and salesid=" + SalesID;
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				return (decimal)dt.Rows[0]["amount"];
			}
			else return 0;
		}


		public decimal GetGratuitybySalesID(int SalesID)
		{
			DataTable dt;
			string query = "select sum(amount) as amount from gratuity where salesid=" + SalesID + " group by salesid";
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				return (decimal)dt.Rows[0]["amount"];
			}
			else return 0;
		}

		public DataTable GetDailySales(DateTime startdate, DateTime enddate)
		{

			string query = "select salesresult.*, paymentresult.netpayment, grat.tips ,paymentresult.netpayment + grat.tips as payment_plus_tips " +
		" from (select sales.id, sales.saledate, sales.adjustment, sum(salesitem.discount) as totaldiscount, sales.total, sales.subtotal, " +
		" sum(salesitem.price* salesitem.quantity) as  itemprice   from sales  inner join salesitem  " +
		" on sales.id = salesitem.salesid    where status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by sales.id) as salesresult " +
		" inner join ( select sales.id , sum(payment.netamount) as netpayment from sales inner join payment on sales.id = payment.salesid where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by sales.id) as paymentresult on salesresult.id = paymentresult.id  " +
		" left outer join (select salesid, sum( gratuity.amount) as tips from sales inner join gratuity on sales.id = gratuity.salesid where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by salesid ) as grat  on salesresult.id = grat.salesid ";


			return m_dbconnect.GetData(query);


		}

		public DataTable GetSalesSummary(DateTime startdate, DateTime enddate)
		{

            string query = "select saledate,sum(service_sold) as service_sold,sum(giftcard_sold) as giftcard_sold, sum(total_sold) as total_sold, sum(discount) as discount, sum(adjustment) as adjustment, sum(nettotal) as nettotal, sum(cash_paid) as cash_paid, sum(credit_paid) as credit_paid, sum(giftcard_redeemed) as giftcard_redeemed, sum(total_payment) as total_payment, sum(coalesce(tips,0)) as totaltips, sum(total_payment) + sum(coalesce(tips,0)) as payment_plus_tips  from (" +
                            " select salesresult.*,cash_paid,credit_paid,giftcard_redeemed, total_payment, grat.tips  from  (select sales.id, date(sales.saledate) as saledate,sum(case when salesitem.Type = 'Service' then salesitem.price else 0 end) as service_sold,sum(case when substr(salesitem.Type,1,4) = 'Gift' then salesitem.price else 0 end) as giftcard_sold,sum(salesitem.price * salesitem.quantity) as  total_sold , sum(salesitem.discount) as discount,sales.adjustment,  sum((salesitem.price  - coalesce(salesitem.discount,0)) * salesitem.quantity) + sales.adjustment as nettotal  from sales " +
                            " inner join salesitem   on sales.id = salesitem.salesid    where status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'   group by sales.id) as salesresult  inner join ( select sales.id ,sum(case when payment.description = 'Cash' then payment.netamount else 0 end) as cash_paid, sum(case when payment.description = 'Credit' then payment.netamount else 0 end) as credit_paid,sum(case when substr(payment.description,1,4) = 'Gift' then payment.netamount else 0 end) as giftcard_redeemed,  sum(payment.netamount) as total_payment from sales inner join payment on sales.id = payment.salesid " +
                            " where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by sales.id) as paymentresult  on salesresult.id = paymentresult.id   " +
                            " left outer join (select salesid, sum( gratuity.amount) as tips from sales inner join gratuity on sales.id = gratuity.salesid where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by salesid ) as grat  on salesresult.id = grat.salesid ) as summary group by summary.saledate order by saledate";

			return m_dbconnect.GetData(query);


		}




		public DataTable GetSalesTickets(DateTime startdate, DateTime enddate)
		{

            string detailquery = "select salesresult.*,cash_paid,credit_paid,giftcard_redeemed, total_payment, coalesce(tips,0), (total_payment + coalesce(tips,0)) as payment_plus_tips  from " +

				" (select sales.id as ticket_no, sales.saledate,sum(case when salesitem.Type = 'Service' then salesitem.price else 0 end) as service_sold,sum(case when substr(salesitem.Type,1,4) = 'Gift' then salesitem.price else 0 end) as giftcard_sold,sum(salesitem.price * salesitem.quantity) as  total_sold , sum(salesitem.discount) as discount,sales.adjustment,  sum((salesitem.price - coalesce(salesitem.discount,0)) * salesitem.quantity) - sales.adjustment as net_total  from sales " +
				" inner join salesitem   on sales.id = salesitem.salesid    where status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'   group by sales.id) as salesresult " +

				" inner join ( select sales.id ,sum(case when payment.description = 'Cash' then payment.netamount else 0 end) as cash_paid, sum(case when payment.description = 'Credit' then payment.netamount else 0 end) as credit_paid,sum(case when substr(payment.description,1,4) = 'Gift' then payment.netamount else 0 end) as giftcard_redeemed,  sum(payment.netamount) as total_payment from sales inner join payment on sales.id = payment.salesid " +
				" where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by sales.id) as paymentresult " +
				" on salesresult.ticket_no = paymentresult.id   " +

				" left outer join (select salesid, sum( gratuity.amount) as tips from sales inner join gratuity on sales.id = gratuity.salesid where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by salesid ) as grat  on salesresult.ticket_no = grat.salesid ";






			return m_dbconnect.GetData(detailquery);


		}




		public DataTable GetPayments(DateTime startdate, DateTime enddate)
		{

			string query = "select * from payment where date(paymentdate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			return m_dbconnect.GetData(query);


		}
		
	   // public DataTable GetRetailDailySales(DateTime reportdate)
	   // {

		  //  string query = "select sales.*, paymentresult.netpayment  from  sales  inner join ( select sales.id , sum(payment.netamount) as netpayment from sales inner join payment on sales.id = payment.salesid where DATE(sales.saledate) = '" + reportdate.ToString("yyyy-MM-dd") + "' group by sales.id) as paymentresult  on sales.id = paymentresult.id";
		  //  return m_dbconnect.GetData(query);


	  //  }






		public DataTable GetSalesRevenue(DateTime startdate, DateTime enddate)
		{
			string query = "select fieldvalue as reportcategory, revenue from reportsetup " +
							" left outer join (select reportcategory,sum(salesitem.price * salesitem.quantity) as revenue " +
							" from sales inner join salesitem on sales.id = salesitem.salesid    where sales.status = 'Closed' and  DATE(sales.saledate) between " +
							" '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by  reportcategory ) as netsales on reportsetup.fieldvalue = netsales.reportcategory " +
							" where fieldname = 'RevenueCategory' order by reportcategory"; 
			return m_dbconnect.GetData(query);
		}


		//these payments have been settled
		public DataTable GetSalesSettlement(DateTime startdate, DateTime enddate)
		{
		   // string query = "select fieldvalue as reportcategory, payments.netamount from reportsetup left outer join (select description ,sum(payment.netamount) as netamount  from sales " +
						  //  " inner join payment  on sales.id = payment.salesid  where sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by  description ) as payments on reportsetup.fieldvalue = payments.description  where fieldname = 'SettlementCategory' order by reportcategory";
			string query;
			string payment;


			payment = "select description , sum(payment.netamount) as netamount from sales " +
						" inner join payment  on sales.id = payment.salesid    where sales.status = 'Closed' and " +
						" DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by description " +
						" union " +
						" select 'Tips', sum(amount) as netamount from gratuity " +
						" inner join sales on sales.id = gratuity.salesid " +
						" where sales.status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' ";

				query = "select fieldvalue as reportcategory, payments.netamount from reportsetup " +
				 " left outer join ( " + payment + " ) as payments " +
				 " on reportsetup.fieldvalue = payments.description  where fieldname = 'SettlementCategory' order by reportcategory";


			return m_dbconnect.GetData(query);
		}







		//these are ALL payments
		public DataTable GetDailyPayments(DateTime startdate, DateTime enddate)
		{
			string query;

			query = "select fieldvalue as reportcategory, coalesce(payments.netamount,0) as netamount from reportsetup " +
   " left outer join ( select description, sum(payment.netamount) as netamount  from payment where  DATE(payment.paymentdate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by description) as payments " +
	" on reportsetup.fieldvalue = payments.description  where fieldname = 'SettlementCategory'  order by reportcategory";





			return m_dbconnect.GetData(query);
		}
		public DataTable GetRevenueList()
		{
			string query = "select fieldvalue as revenuecategory from reportsetup where fieldname = 'RevenueCategory' order by fieldvalue";
			return m_dbconnect.GetData(query);

		}
		public DataTable GetSettlementList()
		{
			string query = "select fieldvalue as settlementcategory from reportsetup where fieldname = 'SettlementCategory' order by fieldvalue";
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



		public decimal GetDailyTips(DateTime startdate, DateTime enddate)
		{
			DataTable dt;


			string query = "select sum( gratuity.amount) as gratuity from sales inner join gratuity on sales.id = gratuity.salesid where sales.status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
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


			string query = "select sum( sales.adjustment) as adjustment from sales where sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
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
				" where sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				if (dt.Rows[0]["discount"].ToString() != "")
					return decimal.Parse(dt.Rows[0]["discount"].ToString());
				else return 0;
			}
			else return 0;


		}



		public DataTable GetRewardSummary(DateTime startdate, DateTime enddate)
		{
			//get summary of Reward from start until now

			string query = " select firstname,lastname,phone1, sales.customerid, sum(total) as totalsales,sum(coalesce(si.rewardtotal,0) + coalesce(credit,0)) as salesreward, coalesce(sum(amount),0) as totalusedreward  from sales " +
							" inner join (select salesid, sum(rewardamount) as rewardtotal from salesitem group by salesid) as si on sales.id = si.salesid " + 
						 " left outer join (select * from payment where description='Reward') as rewardpayment on sales.id = rewardpayment.salesid " +
						 " inner join customer on sales.customerid = customer.id " +
						 " left outer join (select customerid, sum(cash) as credit from credit group by customerid) as credit on sales.customerid = credit.customerid " +
						 " where sales.customerid is not null  " +
						 " group by sales.customerid ";


			string query2 = " select customerid,  coalesce(sum(amount),0) as usedreward from sales " +
			  " left outer join (select * from payment  where description='Reward' ) as rewardpayment on sales.id = rewardpayment.salesid " +
			  " where customerid is not null and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and sales.status = 'Closed'  " +
			  " group by customerid ";


			string query3 = "select firstname,lastname,phone1, summary.customerid, totalsales, salesreward,coalesce( usedreward,0) as usedreward,totalusedreward, (salesreward - totalusedreward ) as   balance " +
								" from (" + query + ") as summary left outer join (" + query2 + ") as used on summary.customerid = used.customerid";

			return m_dbconnect.GetData(query3);

		}

	}



}
