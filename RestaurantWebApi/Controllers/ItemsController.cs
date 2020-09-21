using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RedDot;
using RedDot.OrderService.Class;

namespace RedDot.OrderService.Controllers
{
    public class ItemsController : ApiController
    {
        private static DBEmployee m_dbemployee;
        List<Item> items;


        // GET: api/Items
        public IHttpActionResult Get()
        {
            items = new List<Item>();
            var mockItems = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." }
            };

            m_dbemployee = new DBEmployee();
            DataTable dt = m_dbemployee.GetEmployeeAll();

            foreach (DataRow row in dt.Rows)
            {
                var newitem = new Item { Id = Guid.NewGuid().ToString(), Text = row["firstname"].ToString(), Description = "redot pos salong." };
                items.Add(newitem);
            }
            return Ok(items);
        }

  

        // GET: api/Items/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Items
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Items/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Items/5
        public void Delete(int id)
        {
        }
    }
}
