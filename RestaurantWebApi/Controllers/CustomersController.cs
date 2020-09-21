

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
    public class CustomersController : ApiController
    {
        private static DBCustomer m_dbcustomer = new DBCustomer();


        // GET: api/Customers
        public IHttpActionResult Get()

        {
            List<CustomerData> items = new List<CustomerData>();


            DataTable dt = m_dbcustomer.GetAllCustomer();

            foreach(DataRow row in dt.Rows)
            {
                var newitem = new CustomerData(row);
                items.Add(newitem);
            }
            return Ok(items);
        }

        // GET: api/Customers/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Customers
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Customers/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Customers/5
        public void Delete(int id)
        {
        }
    }
}
