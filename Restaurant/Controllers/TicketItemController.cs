
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.Http;

namespace RedDot
{
    public class TicketItemController : ApiController
    {
        private static DBTicket m_dbticket = new DBTicket();
        private static DBProducts m_dbproduct = new DBProducts();
        static TicketModel tkmodel = new TicketModel();


        // gets a list of seats .. id = ticket id
        public IHttpActionResult Get(int id)
        {
            List<Seat> seats = tkmodel.GetTicketItem(id); //pass in ticket id
            return Ok(seats);
        }


        public IHttpActionResult Put([FromBody] NewTicketItem item )
        {
            //Add item to ticket
            try
            {
                DataTable dt = m_dbproduct.GetProductByID(item.productid);

                Product prod = new Product(dt.Rows[0], OrderType.DineIn);
               

                m_dbticket.DBQSAddProductLineItem(prod.ID, 0, 0, item.salesid, item.seatnumber, prod.Description, prod.Description2, prod.Description3, prod.Unit, item.quantity, item.weight, prod.Price, prod.Type,"", prod.ReportCategory, prod.Taxable, prod.MenuPrefix, "", "", "", "", prod.SpecialPrice, prod.AllowPartial, prod.Weighted, 0, false);
              

                Ticket ticket = new Ticket(item.salesid);


                return Ok(ticket);
            }
            catch
            {
                return null;
            }
           

        }







    }
}
