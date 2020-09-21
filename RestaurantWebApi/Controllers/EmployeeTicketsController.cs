using RedDot.OrderService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RedDot.OrderService.Controllers
{
    public class EmployeeTicketsController : ApiController
    {

        private static DBTicket m_dbticket = new DBTicket();
        private static DBSales m_dbsales = new DBSales();

        // gets a list of tickets .. just pass employeeid
        public IHttpActionResult Get(int id)
        {
            List<Ticket> items = new List<Ticket>();



            DataTable dt = m_dbsales.GetOpenSalesbyEmployee(id);

            foreach (DataRow row in dt.Rows)
            {
                var newitem = new Ticket(int.Parse(row["id"].ToString()));
                items.Add(newitem);
            }
            return Ok(items);
        }


    }
}
