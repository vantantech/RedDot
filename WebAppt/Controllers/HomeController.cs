using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;
using WebAccess.ViewModels;

namespace WebAccess.Controllers
{
    public class HomeController : BaseController
    {
       
    
        // GET: stores1
        public ActionResult Index(string storecode)
        {
            if(storecode != null)
            {
             
                var store = wa.stores.Where(x => x.storecode == storecode).FirstOrDefault();

                if (store.connectionstring is null) return View();

                this.Session["connectionstring"] = store.connectionstring;
                context = new storeEntities(store.connectionstring);
                context.Database.Connection.ConnectionString = store.connectionstring;


                return RedirectToAction("Services");

            }else
            {
                return RedirectToAction("Stores");
            }
           
        }

        private void OpenConnection()
        {
            string connectionstring = this.Session["connectionstring"].ToString();
            context = new storeEntities(connectionstring);
        }

        public ActionResult Services()
        {
            if (this.Session["connectionstring"] is null) return RedirectToAction("Stores");

            OpenConnection();

            if(context != null)
            {
                var rst = context.categories.Select(x => new Menu { CatDescription = x.description,catid = x.id }).ToList();
                foreach(var item in rst)
                {
                   item.Products = (from a in context.cat2prod join b in context.products on a.prodid equals b.id where a.catid == item.catid select b).ToList();
                }
                return View(rst);
            }


            return View("Error");
        }

        public ActionResult Staff()
        {
            if(this.Session["connectionstring"] is null) return RedirectToAction("Stores");

            OpenConnection();


            if (context != null)
            {
                var rst = context.employees.Where(x => x.active).Where(x => x.firstname != "").OrderBy(x => x.firstname)
                    .Select(x => new
                    {
                        firstname = x.firstname,
                        firstletter = x.firstname.Substring(0, 1)
                    }

                    ).ToList();



                var group = (from u in rst
                             group u by new { u.firstletter } into g
                             select new Staff
                             {
                                 firstletter = g.Key.firstletter,
                                 Employees = g.Select(x => new employee { firstname = x.firstname })
                             }).ToList();
            

                return View(group);
            }
                
            else return View("Error");
        }

    public ActionResult Time(int id)
        {
            this.Session["employeeid"] = id;
            var emp = context.employees.Find(id);
       
            return View(emp);
        }

        public ActionResult Stores()
        {
            return View(wa.stores.ToList());
        }


    }
}
