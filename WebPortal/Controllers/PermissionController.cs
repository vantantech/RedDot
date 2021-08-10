using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;

namespace WebAccess.Controllers
{
    public class PermissionController : BaseController
    {
        webaccessEntities context;

        public PermissionController()
        {
            context = new webaccessEntities();

        }
        // GET: Permission
        public ActionResult Index(string id)
        {
            try
            {
                if (id == null) return RedirectToAction("index", "home");

                //only select items that is not already on user permission list
                var contained = context.AspNetUserRoles.Where(x => x.UserId == id).Select(x => x.AspNetRole.Name).ToList();
                ViewBag.Roles = new SelectList(context.AspNetRoles.Where(y => !contained.Contains(y.Name)).ToList(), "Id", "Name");
                ViewBag.UserId = id;


                return View(context.AspNetUsers.Find(id));

            }catch(Exception ex)
            {
                return RedirectToAction("index", "error", ex);
            }
           
        }



        [HttpPost]
        public ActionResult Index(string Id, List<AspNetUserRole> AspNetUserRoles)
        {
            //Id = User Id ( GUID = unique string id)
            foreach (AspNetUserRole userrole in AspNetUserRoles)
            {
                var current = context.AspNetUserRoles.Where(x => x.UserId == Id).Where(x => x.RoleId == userrole.RoleId).First();
                current.C = userrole.C;
                current.R = userrole.R;
                current.U = userrole.U;
                current.D = userrole.D;
                current.A = userrole.A;
            }

            context.SaveChanges();

            TempData["UserMessage"] = new UserMessage { CssClassName = "alert-success", Title = "Success!", Message = "Data Saved." };
         
            return RedirectToAction("Index","Users",null);
        }

   

        // POST: Permission/Create
        [HttpPost]
        public ActionResult Create(string id, string RoleId)
        {
            try
            {
                if(RoleId!=null)
                {
                    AspNetUserRole collection = new AspNetUserRole();
                    collection.UserId = id;
                    collection.RoleId = RoleId;
                    collection.R = true;
                    context.AspNetUserRoles.Add(collection);
                    context.SaveChanges();
                }

                TempData["UserMessage"] = new UserMessage { CssClassName = "alert-success", Title = "Success!", Message = "New Record Created." };
                return RedirectToAction("Index", new {id= id });
            }
            catch(Exception ex)
            {
                return RedirectToAction("index", "error", ex);
            }
        }





        // GET: Permission/Delete/5
       
        public ActionResult Delete(string UserId, string RoleId)
        {
           // return View(context.AspNetUserRoles.Find(UserId, RoleId));
            return View(context.AspNetUserRoles.Where(x=>x.UserId == UserId).Where(y=>y.RoleId == RoleId).First());
        }

        // POST: Permission/Delete/5
        [HttpPost]
        public ActionResult Delete(int? UserId, AspNetUserRole collection)
        {
            try
            {
                // TODO: Add delete logic here
                var deletetarget = context.AspNetUserRoles.Where(x => x.UserId == collection.UserId).Where(y => y.RoleId == collection.RoleId).First();
                context.AspNetUserRoles.Remove(deletetarget);
                context.SaveChanges();
                TempData["UserMessage"] = new UserMessage { CssClassName = "alert-success", Title = "Success!", Message = "Record Deleted." };
                return RedirectToAction("Index", new { id = collection.UserId });
            }
            catch
            {
                return View(context.AspNetUserRoles.Where(x => x.UserId == collection.UserId).Where(y => y.RoleId == collection.RoleId).First());
            }
        }
    }
}
