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



		public DataTable GetRetailWorkedEmployees(DateTime startdate, DateTime enddate)
		{


			string query = " select distinct sales.employeeid  from sales  where  DATE(sales.closedate) between '"
				+ startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and sales.employeeid > 0  order by employeeid";
			return m_dbconnect.GetData(query);
		}


		//this gives commission for sales item only




		public DataTable GetEmployeeSalesCommissionByID(int employeeid, DateTime startdate, DateTime enddate)
		{


			string query = "select employee.firstname, employee.lastname, salesitem.*, (price + coalesce(surcharge,0) - coalesce(salesitem.discount,0)) as pricesurcharge, sales.saledate,sales.discount as adjustment,sales.employeeid as salesemployeeid, sales.closedate, pay.paymenttype from salesitem " +
							" inner join sales on salesitem.salesid = sales.id" +
							" inner join employee on sales.employeeid = employee.id" +
							" inner join (select salesid, group_concat(description) as paymenttype from payment group by salesid ) as pay on salesitem.salesid = pay.salesid ";


			if (employeeid >= 900 )
				query = query + " where  status='Closed' ";
			else
				query = query + " where sales.employeeid =  " + employeeid + " and status='Closed' ";


			query = query + " and   DATE(sales.closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and salesitem.commissiontype != 'none' ";
			query = query + "  order by salesitem.salesid, salesitem.id";


			return m_dbconnect.GetData(query);
		}
	


	



		public DataTable GetRetailSales(DateTime startdate, DateTime enddate)
		{

			//  string query = "select sales.*, paymentresult.netpayment  from  sales  inner join ( select sales.id , sum(payment.netamount) as netpayment from sales inner join payment on sales.id = payment.salesid where sales.status = 'Closed' and DATE(sales.closedate) between " +
			//  " '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by sales.id) as paymentresult  on sales.id = paymentresult.id";


			string query = "select location.description as location,station.description as station, sales.*, employee.displayname as salesperson, paymentresult.paymenttype, paymentresult.netpayment  from  sales " +
							" inner join station on sales.stationno = station.id " +
							 " inner join location on station.locationid = location.id " +
							" inner join employee on sales.employeeid = employee.id " + 
						  " inner join ( select id,group_concat(description,'=',netamount) as paymenttype, sum(netamount) as netpayment from (select sales.id , sum(netamount) as netamount, payment.description " +
						  " from sales inner join payment on sales.id = payment.salesid " +
						  " where payment.void = 0 and sales.status = 'Closed' and DATE(sales.closedate) between   '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by sales.id, payment.description) as temp group by id) as paymentresult " +
							 " on sales.id = paymentresult.id   ";



			return m_dbconnect.GetData(query);


		}









		public DataTable GetPayments(DateTime startdate, DateTime enddate)
		{

			string query = "select location.description as location, sales.parentid,station.description as station, employee.displayname as salesperson, payment.* from payment " +
				 " inner join sales on sales.id = payment.salesid " +
				" inner join station on sales.stationno = station.id " +
							 " inner join location on station.locationid = location.id " +
							" inner join employee on sales.employeeid = employee.id " +
							 
							" where void=0 and date(paymentdate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' order by paymentdate";
			return m_dbconnect.GetData(query);


		}
		
	   // public DataTable GetRetailDailySales(DateTime reportdate)
	   // {

		  //  string query = "select sales.*, paymentresult.netpayment  from  sales  inner join ( select sales.id , sum(payment.netamount) as netpayment from sales inner join payment on sales.id = payment.salesid where DATE(sales.closedate) = '" + reportdate.ToString("yyyy-MM-dd") + "' group by sales.id) as paymentresult  on sales.id = paymentresult.id";
		  //  return m_dbconnect.GetData(query);


	  //  }



		public DataTable GetRetailDailyRevenue(DateTime startdate, DateTime enddate)
		{
			string query = "select fieldvalue as reportcategory, revenue from reportsetup " +
							" left outer join (select reportcategory,sum((salesitem.price +  coalesce(salesitem.surcharge,0) - salesitem.discount) * salesitem.quantity) as revenue " +
							" from sales inner join salesitem on sales.id = salesitem.salesid    where sales.status = 'Closed' and  DATE(sales.closedate) between " +
							" '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by  reportcategory ) as netsales on reportsetup.fieldvalue = netsales.reportcategory " +
							" where fieldname = 'RevenueCategory' order by reportcategory";
			return m_dbconnect.GetData(query);
		}





		//these payments have been settled
		public DataTable GetDailySettlement(DateTime startdate, DateTime enddate)
		{
		   // string query = "select fieldvalue as reportcategory, payments.netamount from reportsetup left outer join (select description ,sum(payment.netamount) as netamount  from sales " +
						  //  " inner join payment  on sales.id = payment.salesid  where sales.status = 'Closed' and  DATE(sales.closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by  description ) as payments on reportsetup.fieldvalue = payments.description  where fieldname = 'SettlementCategory' order by reportcategory";
			string query;
			string payment;


			payment = "select description , sum(payment.netamount) as netamount from sales " +
						" inner join payment  on sales.id = payment.salesid    where payment.void =0 and  sales.status = 'Closed' and " +
						" DATE(sales.closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by description ";


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
   " left outer join ( select description, sum(payment.netamount) as netamount  from payment where payment.void=0 and   DATE(payment.paymentdate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by description) as payments " +
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
			string query = "select sum(salestax) as salestax from sales where sales.status = 'Closed' and  DATE(sales.closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				if (dt.Rows[0]["salestax"].ToString() != "")
					return decimal.Parse(dt.Rows[0]["salestax"].ToString());
				else return 0;
			}
			else return 0;
		}

		public decimal GetDailyShopFee(DateTime startdate, DateTime enddate)
		{
			DataTable dt;
			string query = "select sum(shopfee) as shopfee from sales where sales.status = 'Closed' and  DATE(sales.closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				if (dt.Rows[0]["shopfee"].ToString() != "")
					return decimal.Parse(dt.Rows[0]["shopfee"].ToString());
				else return 0;
			}
			else return 0;
		}






		public decimal GetDailySalesDiscount(DateTime startdate, DateTime enddate)
		{

			DataTable dt;


			string query = "select sum( sales.discount) as discount from sales where sales.status = 'Closed' and  DATE(sales.closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
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
			  " where customerid is not null and DATE(sales.closedate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  " +
			  " group by customerid ";


			string query3 = "select firstname,lastname,phone1, summary.customerid, totalsales, salesreward,coalesce( usedreward,0) as usedreward,totalusedreward,  balance " +
								" from (" + query + ") as summary left outer join (" + query2 + ") as used on summary.customerid = used.customerid";

			return m_dbconnect.GetData(query3);

		}




		public DataTable GetInventory(DateTime startdate, DateTime enddate)
		{
			string query;

			query = "select category.description as category, sum(product.qoh) as itemcount, sum(coalesce(cost,0)*coalesce(qoh,0)) as itemvalue  from category inner join cat2prod on category.id = cat2prod.catid inner join product on cat2prod.prodid = product.id where product.type='product' group by category.description " ;


			return m_dbconnect.GetData(query);
		}

        public DataTable GetShipping(DateTime startdate, DateTime enddate)
        {
            string query;

   

            query = "select   if(trim(shipdate) = '','maroon',if(shipdate is null ,'maroon','lightgreen')) as statuscolor , sales.saledate,sales.closedate,replace(shiporder.shipdate,'12:00:00 AM','') as shipdate, shiporder.* from shiporder inner join (select sales.* from sales  where  closedate between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  ) as sales on shiporder.salesid = sales.id ";


            return m_dbconnect.GetData(query + " order by shipdate,closedate");
        }


        public DataTable GetShippingDelayed()
        {
            string query;

  

            query = "select   if(trim(shipdate) = '','maroon',if(shipdate is null ,'maroon','lightgreen')) as statuscolor , sales.saledate,sales.closedate,replace(shiporder.shipdate,'12:00:00 AM','') as shipdate, shiporder.* from shiporder inner join sales on shiporder.salesid = sales.id ";

            query = query + " where (trim(shipdate) = '' or shipdate is null) and status='Closed'";
            return m_dbconnect.GetData(query + " order by saledate");
        }

        public DataTable GetProductSales(DateTime startdate, DateTime enddate)
        {
            string query;

            query = "select salesitem.description , count(*) as cnt, sum(salesitem.price) as total from sales inner join salesitem on sales.id = salesitem.salesid where sales.saledate between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by salesitem.productid,salesitem.description order by cnt desc, total desc";

            return m_dbconnect.GetData(query, "Product");
        }

    }



}
