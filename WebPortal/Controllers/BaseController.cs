using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebAccess.Helper;
using WebAccess.Models;


namespace WebAccess.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class BaseController : Controller
    {
        public webaccessEntities wa;
        public storeEntities context;
        public BaseController()
        {
            string connectionstring = "";
            context = new storeEntities(connectionstring);
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string cultureName = null;

            // Attempt to read the culture cookie from Request
            HttpCookie cultureCookie = Request.Cookies["_culture"];
            if (cultureCookie != null)
                cultureName = cultureCookie.Value;
            else
                cultureName = Request.UserLanguages != null && Request.UserLanguages.Length > 0 ?
                        Request.UserLanguages[0] :  // obtain it from HTTP header AcceptLanguages
                        null;
            // Validate culture name
            cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            return base.BeginExecuteCore(callback, state);
        }

        public string GetUserName()
        {
            string userid = User.Identity.GetUserId();
            webaccessEntities context = new webaccessEntities();
            var s = context.AspNetUsers.Find(userid);
            return s.FirstName + " " + s.LastName;
        }




        public AspNetUser CurrentUser
        {
            get
            {

                string userid = User.Identity.GetUserId();
                webaccessEntities context = new webaccessEntities();
                return context.AspNetUsers.Find(userid);


            }


        }


        public string GetUserType()
        {
            string userid = User.Identity.GetUserId();
            webaccessEntities context = new webaccessEntities();
            var s = context.AspNetUsers.Find(userid);
            return s.SysUserType.TypeName;
        }
    }
}