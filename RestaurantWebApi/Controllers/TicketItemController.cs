using RedDot.OrderService.Class;
using RedDot.OrderService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.Http;

namespace RedDot.OrderService.Controllers
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

            DataTable dt = m_dbproduct.GetProductByID(item.productid);

            Product prod = new Product(dt.Rows[0],OrderType.DineIn);
            int seatnumber = 1;
            string note = "";
            int quantity = 1;
            decimal weight = 1;
 
            m_dbticket.DBQSAddProductLineItem(prod.ID, 0, 0, item.salesid, seatnumber, prod.Description, prod.Description2, prod.Description3, prod.Unit, quantity, weight, prod.Price, prod.Type, note, prod.ReportCategory, prod.Taxable, prod.MenuPrefix, "", "", "", "", prod.SpecialPrice, prod.AllowPartial, prod.Weighted, 0, false);

            List<Seat> seats = tkmodel.GetTicketItem(item.salesid);

            return Ok(seats);

        }







    }
}
