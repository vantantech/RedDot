using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedDot
{
	public class DBSalonReports: DBReports
	{

		private DBConnect m_dbconnect;

		public DBSalonReports()
		{
			m_dbconnect = new DBConnect();
		}




	

		//this gives commission for sales item only

		public DataTable GetSalonSalesCommissionByID(int employeeid, DateTime startdate, DateTime enddate)
		{


			string query = "select salesitem.*, sales.saledate,sales.saledate, pay.paymenttype from salesitem " +
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























	}



}
