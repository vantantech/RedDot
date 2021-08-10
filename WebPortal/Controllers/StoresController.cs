using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;

namespace WebAccess.Controllers
{
    public class StoresController : Controller
    {

        webaccessEntities context;

        public StoresController()
        {
            context = new webaccessEntities();
        }
        // GET: Stores
        public ActionResult Index()
        {
            var stores = context.stores.ToList();
            return View(stores);
        }

        // GET: Stores/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Stores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Stores/Create
        [HttpPost]
        public ActionResult Create(store collection)
        {
            try
            {
                context.stores.Add(collection);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Stores/Edit/5
        public ActionResult Edit(int id)
        {
            var target = context.stores.Find(id);
            return View(target);
        }

        // POST: Stores/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, store collection,string storepass)
        {
            try
            {
                var target = context.stores.Find(id);
                target.storename = collection.storename;
                target.WebUserId = collection.WebUserId;
                if (storepass != "") {
                   var salt = HMac.GenerateSalt();
                   var pass = HMac.ComputeHMAC_SHA256(Encoding.Default.GetBytes(storepass), salt);
                    target.hmac = System.Text.Encoding.Default.GetString(pass);
                     target.salt = System.Text.Encoding.Default.GetString(salt);


                    // var md5 = new MD5CryptoServiceProvider();
                    // var md5data = md5.ComputeHash(Encoding.ASCII.GetBytes(storepass));
                    //target.hmac = System.Text.Encoding.ASCII.GetString(md5data);

                    target.hmac = HMac.HashPassword(storepass);

                }
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Stores/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Stores/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var target = context.stores.Find(id);
                context.stores.Remove(target);
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
