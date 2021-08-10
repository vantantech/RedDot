using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;

namespace WebAccess.Controllers
{
    public class SettingsController : BaseController
    {
        // GET: Settings
        public ActionResult Index()
        {
            var setting = wa.SysSettings.ToList();

            return View(setting);
        }

        // GET: Settings/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Settings/Create
   
        public ActionResult Create()
        {
            return View();
        }

        // POST: Settings/Create
        [HttpPost,ValidateInput(false)]
        public ActionResult Create(SysSetting collection)
        {
            try
            {
                // TODO: Add insert logic here
              
                wa.SysSettings.Add(collection);
                wa.SaveChanges();
     
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Settings/Edit/5
        [ValidateInput(false)]
        public ActionResult Edit(int id)
        {
            var target = wa.SysSettings.Find(id);
            return View(target);
        }

        // POST: Settings/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, SysSetting collection)
        {
            try
            {
                // TODO: Add update logic here
                var target =wa.SysSettings.Find(id);

                target.Item = collection.Item;
                target.Description = collection.Description;
                target.Value1 = collection.Value1;
                target.Description2 = collection.Description2;
                target.Value2 = collection.Value2;
                target.Description3 = collection.Description3;

                context.SaveChanges();



                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Settings/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Settings/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
