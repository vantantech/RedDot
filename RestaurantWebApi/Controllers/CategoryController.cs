using RedDot;
using RedDot.OrderService.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace RedDot.OrderService.Controllers
{
    public class CategoryController : ApiController
    {
        private static DBProducts m_dbproduct = new DBProducts();
        // GET: Category
        public IHttpActionResult Get()
        {
            List<Category> items = new List<Category>();


            DataTable dt = m_dbproduct.GetCategorybyType("menu1",true);

            foreach (DataRow row in dt.Rows)
            {
                var newitem = new Category(row);
                items.Add(newitem);
            }
            return Ok(items);
        }



    

      
    }
}
