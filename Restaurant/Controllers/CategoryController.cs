using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace RedDot.Controllers
{
    public class CategoryController: ApiController
    {
        private static DBProducts m_dbproduct = new DBProducts();
        // GET: Category
        public IHttpActionResult Get()
        {
            List<Category> items = new List<Category>();


            DataTable dt = m_dbproduct.GetCategorybyType("menu1", true);

            foreach (DataRow row in dt.Rows)
            {
                var newitem = new Category(row);
                items.Add(newitem);
            }

             IHttpActionResult rtn = Ok(items);

            return rtn;
        }
    }
}
