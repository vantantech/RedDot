
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RedDot
{
    public class LoginController : ApiController
    {
        private static DBSecurity m_dbsecurity = new DBSecurity();
        

        public IHttpActionResult Get(string password)
        {
            if (password == null) return null;


            GlobalSettings.Instance.Init();
           int passlen = GlobalSettings.Instance.PinLength;
           int employeeid =  m_dbsecurity.UserAuthenticate("",password.Substring(0, passlen), passlen);  //function returns id

            Employee employee = new Employee(employeeid);
           

            return Ok(employee);
        }

        public int Get()
        {
            return 0;
        }
    }
}
