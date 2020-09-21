using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAccess.Helper;
using WebAccess.Models;

namespace WebAccess.Controllers
{
    public class LicenseController : BaseController
    {
        // GET: License
        public ActionResult Index()
        {
            var lic = context.Licenses.ToList();
            return View(lic);
        }

        // GET: License/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: License/Create
        public ActionResult Create()
        {
            ViewBag.Customers = SelectListHelper.GetAllUsers();
            return View();
        }

        // POST: License/Create
        [HttpPost]
        public ActionResult Create(License collection)
        {
            try
            {
                // TODO: Add insert logic here
                License lic = new License();
                lic.applevel = collection.applevel;
                lic.application = collection.application;
                lic.customerid = collection.customerid;
                lic.enddate = collection.enddate;
                lic.startdate = collection.startdate;
                lic.comment = collection.comment;

                context.Licenses.Add(lic);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: License/Edit/5
        public ActionResult Edit(int id)
        {
            License lic = context.Licenses.Find(id);

            ViewBag.Customers = SelectListHelper.GetAllUsers();
            return View(lic);
        }

        // POST: License/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, License collection)
        {
            try
            {
                // TODO: Add update logic here
                License lic = context.Licenses.Find(id);
                lic.startdate = collection.startdate;
                lic.enddate = collection.enddate;
                lic.applevel = collection.applevel;
                lic.permission = collection.permission;
                lic.comment = collection.comment;
                lic.customerid = collection.customerid;

                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: License/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: License/Delete/5
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
