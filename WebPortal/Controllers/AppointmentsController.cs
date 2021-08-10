using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;

namespace WebAccess.Controllers
{
    public class AppointmentsController : BaseController
    {
       
    
        // GET: stores1
        public ActionResult Index(string storecode)
        {
            if(storecode != null)
            {
             
                var store = wa.stores.Where(x => x.storecode == storecode).FirstOrDefault();

                if (store.connectionstring is null) return View();

                this.Session["connectionstring"] = store.connectionstring;
              

                return RedirectToAction("Staff");

            }else
            {
                return RedirectToAction("Stores");
            }
           
        }

        public ActionResult Services()
        {
            if (this.Session["connectionstring"].ToString() == "") return RedirectToAction("Index");


            return View(context.categories.ToList());
        }

        public ActionResult Staff()
        {
            if(this.Session["connectionstring"] is null) return RedirectToAction("Stores");

            string connectionstring = this.Session["connectionstring"].ToString();
         

            context = new storeEntities(connectionstring);
            if (context != null)
                return View(context.employees.Where(x=>x.active).Where(x=>x.firstname != "").OrderBy(x=>x.firstname).ToList());
            else return View("Error");
        }

    

        public ActionResult Stores()
        {
            return View(wa.stores.ToList());
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(db != null)
                    db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
