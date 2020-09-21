using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;


namespace WebAccess.Controllers
{
    public class SystemMessageController : BaseController
    {
        webaccessEntities context;
        public SystemMessageController()
        {
            context = new webaccessEntities();
        }


        // GET: SystemMessage
        [WebAccessAuthorize(Roles = "System")]
        public ActionResult Index()
        {
        
            var messagelist = context.SystemMessages.ToList();
            return View(messagelist);
        }

  
        // GET: SystemMessage/Create
        [WebAccessAuthorize(Roles = "System", Permission = "Create")]
        public ActionResult Create()
        {
            ViewBag.AlertType = new SelectList(
                  new List<SelectListItem>
                    {
                        new SelectListItem {Text="Information",Value="alert-info"},
                        new SelectListItem {Text="Warning",Value="alert-warning"},
                         new SelectListItem {Text="Critical",Value="alert-danger"}
                    }, "Value", "Text");
            return View();
        }

        // POST: SystemMessage/Create
        [WebAccessAuthorize(Roles = "System", Permission = "Create")]
        [HttpPost]
        public ActionResult Create(SystemMessage collection)
        {
            try
            {
                // TODO: Add insert logic here
                collection.CreateDate = DateTime.Now;
                collection.UpdateDate = DateTime.Now;
                collection.Active = true;
            
                collection.CreatedBy = GetUserName();

           

                context.SystemMessages.Add(collection);
                context.SaveChanges();

                ViewBag.AlertType = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem {Text="Information",Value="alert-info"},
                        new SelectListItem {Text="Warning",Value="alert-warning"},
                         new SelectListItem {Text="Critical",Value="alert-danger"}
                    }, "Value", "Text");

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SystemMessage/Edit/5
        [WebAccessAuthorize(Roles = "System", Permission = "Update")]
        public ActionResult Edit(int id)
        {
            ViewBag.AlertType = new SelectList(
                   new List<SelectListItem>
                    {
                        new SelectListItem {Text="Information",Value="alert-info"},
                        new SelectListItem {Text="Warning",Value="alert-warning"},
                         new SelectListItem {Text="Critical",Value="alert-danger"}
                    }, "Value", "Text");

            return View(context.SystemMessages.Find(id));
        }

        // POST: SystemMessage/Edit/5
        [WebAccessAuthorize(Roles = "System", Permission = "Update")]
        [HttpPost]
        public ActionResult Edit(int id, SystemMessage collection)
        {
            try
            {
                // TODO: Add update logic here
                var target = context.SystemMessages.Find(id);

                target.UpdateDate = DateTime.Now;
                target.Active = collection.Active;
                target.Message = collection.Message;
                target.MsgType = collection.MsgType;
                target.StartDate = collection.StartDate;
                target.EndDate = collection.EndDate;

                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: SystemMessage/Delete/5
        [WebAccessAuthorize(Roles = "System", Permission = "Delete")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SystemMessage/Delete/5
        [WebAccessAuthorize(Roles = "System", Permission = "Delete")]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var target = context.SystemMessages.Find(id);
                context.SystemMessages.Remove(target);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
