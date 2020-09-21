
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RedDot
{
    public class TicketController : ApiController
    {
         private static DBTicket m_dbticket= new DBTicket();
        private static DBSales m_dbsales = new DBSales();

        // gets a list of tickets .. just pass employeeid
        public IHttpActionResult Get(int id)
        {

            Ticket ticket = new Ticket(id);
            if (ticket.Status == "Closed") return null;
            else          
                return Ok(ticket);
        }

 
        // POST: api/Ticket
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Ticket/5
        public IHttpActionResult Put(int id, [FromBody]string value)
        {
            Ticket ticket = new Ticket(id);
            if(ticket.Status != null)
            {
                ReceiptPrinterModel.SendKitchen(ticket, true);
                ticket.Reload();
            }
      

            return Ok(ticket);


        }

        // DELETE: api/Ticket/5
        public IHttpActionResult Delete(int id)
        {
            Ticket CurrentTicket = new Ticket(id);
        

            if (CurrentTicket.SentToKitchen == false)
            {
                CurrentTicket.VoidTicket("ticket cancelled");
 
            }
            else
            {
                // some item has been sent .. so just need to loop thru and void unsent items only
                foreach (Seat seat in CurrentTicket.Seats)
                {
                    foreach (LineItem line in seat.LineItems)
                    {
                        if (!line.Sent)
                        {
                            CurrentTicket.DeleteLineItem(line.ID);
                        }
                    }
                }

               
            }

            CurrentTicket.Reload();
            return Ok(CurrentTicket);

        }
    }
}
