using RedDot.OrderService.Class;
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
    public class CreateTicketController : ApiController
    {
        private static DBTicket m_dbticket = new DBTicket();
        private static DBSales m_dbsales = new DBSales();
        // GET: api/CreateTicket
        public IHttpActionResult Get(int id, int tablenumber)
        {
           int salesid = m_dbticket.DBCreateTicketQS(id, tablenumber, OrderType.DineIn,SubOrderType.None, 1);

            DataTable dt = m_dbticket.GetSalesTicket(salesid);
            if(dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                var newitem = new Ticket(int.Parse(row["id"].ToString()));


                return Ok(newitem);
            }
            return null;
        }

        // GET: api/CreateTicket/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CreateTicket
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/CreateTicket/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CreateTicket/5
        public void Delete(int id)
        {
        }
    }
}
