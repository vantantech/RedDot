using RedDot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;




namespace RedDot.OrderService.Controllers
{
    public class ProductController : ApiController
    {
        // GET: Product
        private static DBProducts m_dbproduct = new DBProducts();
   

        public IHttpActionResult Get(int id)
        {
            List<Product> items = new List<Product>();

 

            DataTable dt = m_dbproduct.GetProductsByCat(id);

            foreach (DataRow row in dt.Rows)
            {
                var newitem = new Product(row,OrderType.DineIn);
                items.Add(newitem);
            }
            return Ok(items);
        }

    

    }
}
