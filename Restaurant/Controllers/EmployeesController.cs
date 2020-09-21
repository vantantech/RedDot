
using RedDot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RedDot
{
    public class EmployeesController : ApiController
    {
        private static DBEmployee m_dbemployee = new DBEmployee();
    

        // GET: api/Employees
        public IHttpActionResult Get()
        {
            List<Employee> items;
            items = new List<Employee>();

      
            DataTable dt = m_dbemployee.GetEmployeeAll();

            foreach (DataRow row in dt.Rows)
            {
                var newitem = new Employee(row);
                items.Add(newitem);
            }
            return Ok(items);
        }

        // GET: api/Employees/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Employees
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Employees/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Employees/5
        public void Delete(int id)
        {
        }
    }
}
