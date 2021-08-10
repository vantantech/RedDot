using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;

namespace WebAccess.Controllers
{
     public class UserStoreController : Controller
    {
         webaccessEntities context;

         public UserStoreController()
         {
             context = new webaccessEntities();
         }


         public ActionResult Index(string id)
         {
             var user = context.AspNetUsers.Find(id);

             var loc = context.stores
                 .Where(x => !context.stores.Where(y => y.AspNetUsers.Any(z => z.Id == id)).Contains(x))
                 .OrderBy(x => x.storename)
                 .Select(x => new
                 {
                     id = x.id,
                     Name =  x.storename
                 })
                 .ToList();


             ViewBag.Stores = new SelectList(loc, "id", "Name");

       

             return View(user);
         }


         // POST: UserLocation/Create
         [HttpPost]
         public ActionResult Create(string id, int StoreId)
         {
             try
             {
                 // TODO: Add insert logic here
                 var user = context.AspNetUsers.Find(id);
                 var loc = context.stores.Find(StoreId);

                 user.stores.Add(loc);

                 context.SaveChanges();
                 TempData["UserMessage"] = new UserMessage { CssClassName = "alert-success", Title = "Success!", Message = "Location Assigned" };
                 return RedirectToAction("index", new { id = id });
             }
             catch
             {
                 return View();
             }
         }

         // GET: UserLocation/Delete/5
         public ActionResult Delete(int id, string userid)
         {

             var user = context.AspNetUsers.Find(userid);
             var loc = context.stores.Find(id);
             user.stores.Remove(loc);
             context.SaveChanges();
             TempData["UserMessage"] = new UserMessage { CssClassName = "alert-success", Title = "Success!", Message = "Location Removed" };
             return RedirectToAction("index", new { id = userid });
         }
    }
}