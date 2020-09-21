
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RedDot
{
    public class CreateTicketController : ApiController
    {
        private static DBTicket m_dbticket = new DBTicket();
        private static DBSales m_dbsales = new DBSales();
        // GET: api/CreateTicket
        public IHttpActionResult Get(int id, int tablenumber, int customercount)
        {
           int salesid = m_dbticket.DBCreateTabletTicket(id, tablenumber, OrderType.DineIn,SubOrderType.None, 3);
           var newitem = new Ticket(salesid);
            newitem.UpdateCustomerCount(customercount);
           return Ok(newitem);
   
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
