using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAccess.Controllers
{
    public class StaticContentController : BaseController
    {
        // GET: StaticContent
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult UserNotFound()
        {
            return View();
        }

        public ActionResult RegisterThankYou()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }
    }
}