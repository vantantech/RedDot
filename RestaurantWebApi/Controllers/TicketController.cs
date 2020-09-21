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
    public class TicketController : ApiController
    {
         private static DBTicket m_dbticket= new DBTicket();
        private static DBSales m_dbsales = new DBSales();

        // gets a list of tickets .. just pass employeeid
        public IHttpActionResult Get(int id)
        {

            Ticket ticket = new Ticket(id);

          
            return Ok(ticket);
        }

        // GET: api/Ticket/5
        public string Get(int id, int ordernumber)
        {
            return "value";
        }

        // POST: api/Ticket
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Ticket/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Ticket/5
        public void Delete(int id)
        {
        }
    }
}
