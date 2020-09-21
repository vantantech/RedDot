using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot.DataManager
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
				+ startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and sales.status = 'Closed' and  salesitem.employeeid > 0  order by employeeid";
			return m_dbconnect.GetData(query);
		}



        
		public DataTable GetSalonSalesCommissionByID(int employeeid, DateTime startdate, DateTime enddate)
		{

            //the payment type is optional for commission report .. it is to display on the ticket details in the commission report
            string query = "select salesitem.*, sales.saledate, pay.paymenttype from salesitem " +
							" inner join sales on salesitem.salesid = sales.id" +
							" inner join (select salesid, group_concat(cardgroup) as paymenttype from payment group by salesid ) as pay on salesitem.salesid = pay.salesid "; //optiona line



            if (employeeid >= 900)
				query = query + " where  status='Closed' ";
			else
				query = query + " where salesitem.employeeid =  " + employeeid + " and status='Closed' ";


			query = query + " and   DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' ";
			query = query + "  order by salesitem.salesid, salesitem.id";


			return m_dbconnect.GetData(query);
		}


		public DataTable GetSalonSalesCommissionExportByID(int employeeid, DateTime startdate, DateTime enddate, string type)
		{

			string query = "";


            string detail= " select name,ticketno,date,sales_amount,commission,supplyfee,amount as tip from (select employee.displayname as name, employee.id as employeeid, sales.id as ticketno,DATE(sales.saledate) as date,sum(salesitem.price) as sales_amount, sum((salesitem.price - salesitem.supplyfee) * salesitem.quantity * employee.commission / 100) as commission,   sum(salesitem.supplyfee) as supplyfee  from salesitem " +
                        " inner join sales on salesitem.salesid = sales.id " +
                        " inner join employee on salesitem.employeeid = employee.id";

            if (employeeid >= 900)
                detail = detail + " where  status='Closed' ";
            else
                detail = detail + " where salesitem.employeeid =  " + employeeid + " and status='Closed' ";


            detail = detail + " and   DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' and salesitem.commissiontype != 'none' ";
            detail = detail + " group by displayname, DATE(sales.saledate), sales.id ) as output left outer join gratuity on output.ticketno = gratuity.salesid and output.employeeid = gratuity.employeeid ";
            detail= detail + " order by name, ticketno";




            string daily = "select name,date,sum(sales_amount) as sales_amount,sum(commission) as commission , sum(supplyfee) as supplyfee, sum(tip) as tip from (" + detail + ") as daily group by name,date";



            string summary = "select name,sum(sales_amount) as sales_amount,sum(commission) as commission , sum(supplyfee) as supplyfee , sum(tip) as tip from (" + detail + ") as daily group by name";


			switch(type)
			{
				case "detail":
                    
                 query = detail;

					break;



				case "daily":

                    query = daily;
                   
					break;


				case "summary":

                    query = summary;
					break;


				default:
					query = "error";
					break;


			}



		


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


        //need to exclude any payment that are voided 6/14/2018
        //
		public DataTable GetDailySales(DateTime startdate, DateTime enddate, int employeeid =0)
		{

            string employeefilter = "";

            if (employeeid >= 0) employeefilter = " where salesresult.employeeid = " + employeeid;


            string query = "select salesresult.*, paymentresult.netpayment, grat.tips , (total-coalesce(netpayment,0)) as balance, paymentresult.netpayment + grat.tips as payment_plus_tips " +
		" from (select sales.id,sales.employeeid, sales.saledate, sales.adjustment, sum(salesitem.discount) as totaldiscount,sales.salestax, sales.total, sales.subtotal, " +
		" sum((salesitem.price + salesitem.surcharge) * salesitem.quantity) as  itemprice   from sales  inner join salesitem  " +
		" on sales.id = salesitem.salesid    where status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by sales.id) as salesresult " +
		" left outer join ( select sales.id , sum(payment.netamount) as netpayment from sales inner join payment on sales.id = payment.salesid where void = 0 and  status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by sales.id) as paymentresult on salesresult.id = paymentresult.id  " +
		" left outer join (select salesid, sum( gratuity.amount) as tips from sales inner join gratuity on sales.id = gratuity.salesid where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by salesid ) as grat  on salesresult.id = grat.salesid " + employeefilter;


			return m_dbconnect.GetData(query);


		}

        public DataTable GetDailySalesSummary(DateTime startdate, DateTime enddate)
        {

           

            string query = "select salesresult.*,employee.displayname, paymentresult.netpayment, (total-coalesce(netpayment,0)) as balance, grat.tips ,paymentresult.netpayment + grat.tips as payment_plus_tips " +
        " from (select sales.id,sales.employeeid, sales.saledate, sales.adjustment, sum(salesitem.discount) as totaldiscount,sales.salestax, sales.total, sales.subtotal, " +
        " sum((salesitem.price + salesitem.surcharge) * salesitem.quantity) as  itemprice   from sales  inner join salesitem  " +
        " on sales.id = salesitem.salesid    where status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by sales.id) as salesresult " +
        "  left outer join employee on salesresult.employeeid = employee.id " +
        " left outer join ( select sales.id , sum(payment.netamount) as netpayment from sales inner join payment on sales.id = payment.salesid where void = 0 and  status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by sales.id) as paymentresult on salesresult.id = paymentresult.id  " +
        " left outer join (select salesid, sum( gratuity.amount) as tips from sales inner join gratuity on sales.id = gratuity.salesid where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by salesid ) as grat  on salesresult.id = grat.salesid " ;

            string final = "Select displayname as cashier, employeeid , sum(itemprice) as itemprice,sum(balance) as balance, sum(totaldiscount) as itemdiscount, sum(coalesce(salestax,0)) as tax, sum(total) as total, sum(netpayment) as payment, sum(coalesce(tips,0)) as tips from(" +
                query + ") as final group by cashier,employeeid";

            return m_dbconnect.GetData(final);


        }

        //need to exclude any payment that are voided 6/14/2018
        public DataTable GetSalesSummary(DateTime startdate, DateTime enddate)
		{

			string query = "select saledate,sum(service_sold) as service_sold,sum(giftcard_sold) as giftcard_sold, sum(total_sold) as total_sold, sum(discount) as discount, sum(adjustment) as adjustment, sum(nettotal) as nettotal, sum(cash_paid) as cash_paid, sum(credit_paid) as credit_paid, sum(giftcard_redeemed) as giftcard_redeemed, sum(total_payment) as total_payment, sum(coalesce(tips,0)) as totaltips, sum(total_payment) + sum(coalesce(tips,0)) as payment_plus_tips  from (" +
                            " select salesresult.*,cash_paid,credit_paid,giftcard_redeemed, total_payment, grat.tips  from  (select sales.id, date(sales.saledate) as saledate,sum(case when salesitem.Type = 'Service' then (salesitem.price + salesitem.surcharge) else 0 end) as service_sold,sum(case when substr(salesitem.Type,1,4) = 'Gift' then (salesitem.price + salesitem.surcharge) else 0 end) as giftcard_sold,sum((salesitem.price + salesitem.surcharge) * salesitem.quantity) as  total_sold , sum(salesitem.discount) as discount,sales.adjustment,  sum(((salesitem.price + salesitem.surcharge)  - coalesce(salesitem.discount,0)) * salesitem.quantity) + sales.adjustment as nettotal  from sales " +
							" inner join salesitem   on sales.id = salesitem.salesid    where status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'   group by sales.id) as salesresult  inner join ( select sales.id ,sum(case when payment.cardgroup = 'Cash' then payment.netamount else 0 end) as cash_paid, sum(case when payment.cardgroup = 'Credit' then payment.netamount else 0 end) as credit_paid,sum(case when substr(payment.cardgroup,1,4) = 'Gift' then payment.netamount else 0 end) as giftcard_redeemed,  sum(payment.netamount) as total_payment from sales inner join payment on sales.id = payment.salesid " +
							" where payment.void = 0 and  status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by sales.id) as paymentresult  on salesresult.id = paymentresult.id   " +
							" left outer join (select salesid, sum( gratuity.amount) as tips from sales inner join gratuity on sales.id = gratuity.salesid where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by salesid ) as grat  on salesresult.id = grat.salesid ) as summary group by summary.saledate order by saledate";

			return m_dbconnect.GetData(query);


		}



        //exclude voided payments   06/15/2018
		public DataTable GetSalesTickets(DateTime startdate, DateTime enddate)
		{

			string detailquery = "select salesresult.*,cash_paid,credit_paid,giftcard_redeemed, total_payment, coalesce(tips,0), (total_payment + coalesce(tips,0)) as payment_plus_tips  from " +

                " (select sales.id as ticket_no, sales.saledate,sum(case when salesitem.Type = 'Service' then (salesitem.price + salesitem.surcharge) else 0 end) as service_sold,sum(case when substr(salesitem.Type,1,4) = 'Gift' then (salesitem.price + salesitem.surcharge) else 0 end) as giftcard_sold,sum((salesitem.price + salesitem.surcharge) * salesitem.quantity) as  total_sold , sum(salesitem.discount) as discount,sales.adjustment,  sum(((salesitem.price + salesitem.surcharge) - coalesce(salesitem.discount,0)) * salesitem.quantity) - sales.adjustment as net_total  from sales " +
				" inner join salesitem   on sales.id = salesitem.salesid    where status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'   group by sales.id) as salesresult " +

				" inner join ( select sales.id ,sum(case when payment.cardgroup = 'Cash' then payment.netamount else 0 end) as cash_paid, sum(case when payment.cardgroup = 'Credit' then payment.netamount else 0 end) as credit_paid,sum(case when substr(payment.cardgroup,1,4) = 'Gift' then payment.netamount else 0 end) as giftcard_redeemed,  sum(payment.netamount) as total_payment from sales inner join payment on sales.id = payment.salesid " +
				" where payment.void = 0 and status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by sales.id) as paymentresult " +
				" on salesresult.ticket_no = paymentresult.id   " +

				" left outer join (select salesid, sum( gratuity.amount) as tips from sales inner join gratuity on sales.id = gratuity.salesid where status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  group by salesid ) as grat  on salesresult.ticket_no = grat.salesid ";

			return m_dbconnect.GetData(detailquery);


		}




		public DataTable GetSalesRevenue(DateTime startdate, DateTime enddate)
		{

        

            string query = "select fieldvalue as reportcategory, sum(revenue) as revenue from reportsetup " +
							" left outer join (select reportcategory,sum((salesitem.price + salesitem.surcharge) * salesitem.quantity) as revenue " +
							" from sales inner join salesitem on sales.id = salesitem.salesid    where  sales.status = 'Closed' and  DATE(sales.saledate) between " +
							" '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by  reportcategory ) as netsales on UPPER(reportsetup.fieldcode) = UPPER(netsales.reportcategory) " +
							" where fieldname = 'RevenueCategory' group by fieldvalue order by sortorder"; 
			return m_dbconnect.GetData(query);
		}


		//these payments have been settled
        //ticket status must be 'Closed'
        //exclude voided payment  06/15/18
		public DataTable GetSalesSettlement(DateTime startdate, DateTime enddate)
		{
		   // string query = "select fieldvalue as reportcategory, payments.netamount from reportsetup left outer join (select cardgroup ,sum(payment.netamount) as netamount  from sales " +
						  //  " inner join payment  on sales.id = payment.salesid  where sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by  cardgroup ) as payments on reportsetup.fieldvalue = payments.cardgroup  where fieldname = 'SettlementCategory' order by reportcategory";
			string query;
			string payment;
         

            payment = "select cardgroup , sum(payment.netamount) as netamount from sales " +
                        " inner join payment  on sales.id = payment.salesid    where payment.void = 0 and (payment.TransType='Sale' or payment.TransType='SALE' or payment.TransType='POSTAUTH' ) and  sales.status = 'Closed'  and " + 
						" DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by payment.cardgroup " +
						" union " +
                        "select CONCAT(cardgroup,'REFUND') as cardgroup , sum(payment.netamount) as netamount from sales " +
                        " inner join payment  on sales.id = payment.salesid    where payment.void = 0 and (payment.TransType='Refund' or payment.TransType='RETURN') and  sales.status = 'Closed'  and " + 
                        " DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by payment.cardgroup " +
                        " union " +
                        " select 'Tips', sum(amount) as netamount from gratuity " +
						" inner join sales on sales.id = gratuity.salesid " +
						" where sales.status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' "  ;

				query = "select fieldvalue as reportcategory, sum(payments.netamount) as netamount, sortorder from reportsetup " +
				 " left outer join ( " + payment + " ) as payments " +
                 " on UPPER(reportsetup.fieldcode) = UPPER(payments.cardgroup)   where fieldname = 'SettlementCategory' group by fieldvalue,sortorder order by sortorder";


			return m_dbconnect.GetData(query);
		}






        //07/26/2018 VOID=0 to exclude voided payments
        //these are ALL payments that are not voided and from tickets that also are not voided 06/14/2018
        //tickets must be in 'Closed' status 06/14/2018
        public DataTable GetDailyPayments(DateTime startdate, DateTime enddate, int employeeid)
		{
			string query;

            /*  query = "select sales.id, paymentdate,cashier, if(cardgroup = 'Cash',netamount,0) as cash, if(cardgroup = 'Credit' OR cardgroup = 'CREDIT' ,netamount,0) as credit,  if(cardgroup = 'Gift Card',netamount,0) as giftcard,if(cardgroup = 'Gift Certificate',netamount,0) as giftcertificate, if(cardgroup = 'Reward',netamount,0) as reward, if(cardgroup = 'Stamp Card',netamount,0) as stampcard,if(cardgroup = 'Tips',netamount,0) as tips, netamount as allpayments " +
                  "from (select DATE(sales.saledate)as paymentdate,cardgroup,netamount, displayname as cashier from sales inner join payment on sales.id = payment.salesid  left outer join employee on sales.employeeid = employee.id where sales.status='Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  " +
           " union select DATE(sales.saledate)as paymentdate,'Tips' as cardgroup,amount as netamount, displayname as cashier from sales inner join gratuity on sales.id = gratuity.salesid left outer join employee on sales.employeeid = employee.id where sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  ) as comb ";
           */
            string employeefilter = "";

            if (employeeid >= 0) employeefilter = " and sales.employeeid = " + employeeid;

            query = "select sales.saledate as paymentdate, displayname as cashier,  sales.id as ticketnumber, " +
" payment.*, coalesce(gratuity.tips,0) as tips, (payment.netpayment + coalesce(gratuity.tips,0)) as allpayments " +
" from sales inner join ( select salesid, sum(if (cardgroup = 'Cash',netamount,0)) as cash, " +
" sum(if (cardgroup = 'Credit' OR cardgroup = 'CREDIT', netamount,0)) as credit, " +
" sum(if (cardgroup = 'DEBIT', netamount,0)) as debit, " +
" sum(if (cardgroup = 'Gift Card',netamount,0)) as giftcard, " +
" sum(if (cardgroup = 'Gift Certificate',netamount,0)) as giftcertificate," +
" sum(if (cardgroup = 'Reward',netamount,0)) as reward, " +
" sum(if (cardgroup = 'Stamp Card',netamount,0)) as stampcard, " +
 " sum(netamount) as netpayment  from payment where void=0 group by salesid  ) as payment on sales.id = payment.salesid " +
"  left outer join employee on sales.employeeid = employee.id " +
 "  left join(select salesid, sum(amount) as tips from gratuity group by salesid) as gratuity on sales.id = gratuity.salesid " +
    "   where  sales.status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' " + employeefilter + 
    " order by cashier, paymentdate";




            return m_dbconnect.GetData(query);
		}



        //07/26/2018 VOID=0 to exclude voided payments
        public DataTable GetDailyPaymentSummary(DateTime startdate, DateTime enddate, int employeeid)
        {
            string query;
            string filter = "";

            if(employeeid >= 0)
            {
                filter = " sales.employeeid =  " + employeeid + " and ";
            }

            //UNION removes duplicates so need to select sales id incase there are two payment with same amount
            query = "select  paymentdate,cashier,employeeid, sum(if(cardgroup = 'Cash',netamount,0)) as cash, sum(if(cardgroup = 'Credit' OR cardgroup = 'CREDIT' ,netamount,0)) as credit, sum(if( cardgroup = 'DEBIT' ,netamount,0)) as debit, sum(if(cardgroup = 'Gift Card',netamount,0)) as giftcard, sum(if(cardgroup = 'Gift Certificate',netamount,0)) as giftcertificate, sum(if(cardgroup = 'Reward',netamount,0)) as reward, sum(if(cardgroup = 'Stamp Card',netamount,0)) as stampcard,sum(if(cardgroup = 'Tips',netamount,0)) as tips, sum(netamount) as allpayments " +
                "from (select sales.id as salesid, DATE(sales.saledate)as paymentdate,cardgroup,netamount, displayname as cashier, sales.employeeid as employeeid from sales inner join payment on sales.id = payment.salesid  left outer join employee on sales.employeeid = employee.id where " +  filter + " payment.void =0 and  sales.status='Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'  " +
         " union select sales.id as salesid, DATE(sales.saledate)as paymentdate,'Tips' as cardgroup,sum(amount) as netamount, displayname as cashier, sales.employeeid as employeeid from sales inner join gratuity on sales.id = gratuity.salesid left outer join employee on sales.employeeid = employee.id where " + filter + "  sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' group by employee.id ) as comb " +
            " group by paymentdate, cashier order by cashier, paymentdate";





            return m_dbconnect.GetData(query);
        }


        public DataTable GetRevenueList()
		{
			string query = "select distinct fieldvalue as revenuecategory,sortorder from reportsetup where fieldname = 'RevenueCategory' order by sortorder";
			return m_dbconnect.GetData(query);

		}
		public DataTable GetSettlementList()
		{
			string query = "select distinct fieldvalue as settlementcategory,sortorder from reportsetup where fieldname = 'SettlementCategory' order by sortorder";
			return m_dbconnect.GetData(query);

		}
		public decimal GetDailySalesTax(DateTime startdate, DateTime enddate)
		{

         


            DataTable dt;
			string query = "select sum(salestax) as salestax from sales where  sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
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


			string query = "select sum( gratuity.amount) as gratuity from sales inner join gratuity on sales.id = gratuity.salesid where  sales.status = 'Closed' and DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				if (dt.Rows[0]["gratuity"].ToString() != "")
					return decimal.Parse(dt.Rows[0]["gratuity"].ToString());
				else return 0;
			}
			else return 0;
		}



        //Total of all Ticket discounts .., not item discounts
		public decimal GetDailySalesAdjustments(DateTime startdate, DateTime enddate)
		{
          

            DataTable dt;


			string query = "select sum( sales.adjustment) as adjustment from sales where  sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				if (dt.Rows[0]["adjustment"].ToString() != "")
					return decimal.Parse(dt.Rows[0]["adjustment"].ToString());
				else return 0;
			}
			else return 0;


		}

        //Discounts from actual individual sales item
		public decimal GetDailySalesDiscounts(DateTime startdate, DateTime enddate)
		{
         
            DataTable dt;


			string query = "select sum( salesitem.discount * salesitem.quantity) as discount from sales " +
				" inner join salesitem on sales.id = salesitem.salesid " +
				" where  sales.status = 'Closed' and  DATE(sales.saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "'";
			dt = m_dbconnect.GetData(query);

			if (dt.Rows.Count > 0)
			{
				if (dt.Rows[0]["discount"].ToString() != "")
					return decimal.Parse(dt.Rows[0]["discount"].ToString());
				else return 0;
			}
			else return 0;


		}


        //exclude voided Reward Payment
        //exclude voided reward payment  06/15/18
		public DataTable GetRewardSummary(DateTime startdate, DateTime enddate)
		{
			//get summary of Reward from start until now

			string query = "  select firstname,lastname,phone1, rewardearned.customerid, total as totalsales,(coalesce(rewardtotal,0) + coalesce(credit,0)) as salesreward, coalesce(netamount,0) as totalusedreward " +
  " from( select customerid,sum(tickettotal) as total,  sum(amount) as rewardtotal from customerrewards where transtype = 'ADD'  group by customerid) as rewardearned " +
 " left outer join (select customerid, sum(amount) as netamount from customerrewards where transtype = 'REDEEM'  group by customerid) as rewardpayment on rewardearned.customerid = rewardpayment.customerid " +

    " inner join customer on rewardearned.customerid = customer.id " +

     " left outer join(select customerid, sum(cash) as credit from credit group by customerid) as credit on rewardearned.customerid = credit.customerid ";

            //usedreward = reward that is used within reported date
            string query2 = " select customerid,  coalesce(sum(amount),0) as usedreward from customerrewards " +
     			  " where transtype='REDEEM' and DATE(saledate) between '" + startdate.ToString("yyyy-MM-dd") + "' and '" + enddate.ToString("yyyy-MM-dd") + "' " +
			  " group by customerid ";


            
            //total used reward = all used reward since customer signed up
			string query3 = "select firstname,lastname,phone1, summary.customerid, totalsales, salesreward,coalesce( usedreward,0) as usedreward,totalusedreward, (salesreward - totalusedreward ) as   balance " +
								" from (" + query + ") as summary left outer join (" + query2 + ") as used on summary.customerid = used.customerid order by balance desc";

			return m_dbconnect.GetData(query3);

		}

	}



}
