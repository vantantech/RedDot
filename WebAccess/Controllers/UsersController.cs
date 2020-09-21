using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using WebAccess.Models;

namespace WebAccess.Controllers
{
    public class UsersController : BaseController
    {

     
        ApplicationDbContext appcontext;

        public UsersController()
        {

        
            appcontext = new ApplicationDbContext();
        }


        // GET: Users

        [WebAccessAuthorize(Roles = "User Administration")]

        public ActionResult Index(string sort, bool? showinactive)
        {
            ViewBag.sort = sort;


            if (showinactive.HasValue)
            {
                if ((bool)showinactive)
                {

                    ViewBag.showinactive = true;
                    return View(context.AspNetUsers.Where(x => !x.Active).ToList());
                }
                else
                {

                    ViewBag.showinactive = true;
                    return View(context.AspNetUsers.Where(x => x.Active).ToList());
                }
            }
            else
            {

                ViewBag.showinactive = false;
                return View(context.AspNetUsers.Where(x => x.Active).ToList());
            }


        }


        [WebAccessAuthorize(Roles = "User Administration")]
        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {

            if (id == null) return RedirectToAction("Index", id);


            // prepopulat roles for the view dropdown
            // var list = context.AspNetRoles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();

            // ViewBag.Roles = new SelectList(appcontext.Roles.Where(u => !u.Name.Contains("Admin")).ToList(), "Name", "Name");

            var model = context.AspNetUsers.Find(id);
            ViewBag.UserTypes = new SelectList(context.SysUserTypes.ToList(), "Id", "Description");
            if (model.LockoutEndDateUtc > DateTime.Now.AddYears(1))
            {
                ViewBag.UserLocked = true;
            }
            else
            {
                ViewBag.UserLocked = false;
            }
            //ViewBag.Locations = new SelectList(context.RefLocations.ToList(), "Id", "Description");

            return View(model);
        }




        [WebAccessAuthorize(Roles = "User Administration")]
        public ActionResult Unlock(string id)
        {
            var currentuser = context.AspNetUsers.Find(id);
            currentuser.LockoutEndDateUtc = null;
            context.SaveChanges();

            Utility.SendEmailToUser(currentuser.Email, "VTT DMS Account Status", "Your account has been unlocked / activated by the Administrator.  Click here to login: http://www.VTTdms.com ");

            TempData["UserMessage"] = new UserMessage { CssClassName = "alert-success", Title = "Success!", Message = "User account has been unlocked." };
            return RedirectToAction("Index");
        }




        [WebAccessAuthorize(Roles = "User Administration")]
        // POST: Users/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, AspNetUser user, bool UserLocked)
        {
            try
            {

                bool sendemail = false;

                // TODO: Add update logic here
                var currentuser = context.AspNetUsers.Find(id);
                currentuser.Email = user.Email;
                currentuser.UserName = user.Email;
                currentuser.EmailConfirmed = user.EmailConfirmed;
                currentuser.FirstName = user.FirstName;
                currentuser.LastName = user.LastName;
                currentuser.UserTypeId = user.UserTypeId;
                currentuser.Title = user.Title;
                currentuser.Active = user.Active;

                if (currentuser.LockoutEndDateUtc != null) //user is locked
                {
                    currentuser.LockoutEndDateUtc = null;
                    //send unlock email but only after it is saved.. 
                    sendemail = true;
                }
                else
                {
                    if (UserLocked)
                    {
                        currentuser.LockoutEndDateUtc = DateTime.Now.AddYears(10);
                    }
                }


                context.SaveChanges();


                TempData["UserMessage"] = new UserMessage { CssClassName = "alert-success", Title = "Success!", Message = "Data Saved." };

                if (sendemail)
                {
                    Utility.SendEmailToUser(currentuser.Email, "VTT DMS Account Status", "Your account has been unlocked / activated by the Administrator.  Click here to login: http://www.VTTdms.com ");
                }

                //returns back to list
                return RedirectToAction("Index");
            }
            catch
            {

                //error occur , bring user back to edit page
                var model = context.AspNetUsers.Find(id);
                ViewBag.UserTypes = new SelectList(context.SysUserTypes.ToList(), "Id", "Description");
                ViewBag.Locations = new SelectList(context.stores.ToList(), "Id", "storename");
                TempData["UserMessage"] = new UserMessage { CssClassName = "alert-danger", Title = "Error!", Message = "Operation Failed." };
                return View(model);
            }
        }


        [WebAccessAuthorize(Roles = "User Administration")]
        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            return View(context.AspNetUsers.Find(id));
        }

        // POST: Users/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var target = context.AspNetUsers.Find(id);
                context.AspNetUsers.Remove(target);
                context.SaveChanges();

                TempData["UserMessage"] = new UserMessage { CssClassName = "alert-success", Title = "Success!", Message = "Record Deleted." };
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Error", new { message = ex.Message, innermessage = ex.InnerException.Message });
            }
        }


        [WebAccessAuthorize(Roles = "User Administration")]
        public ActionResult ChangePassword(string id)
        {
            SetPasswordViewModel model = new SetPasswordViewModel();
            model.Id = id;
            return View(model);
        }


        [WebAccessAuthorize(Roles = "User Administration")]
        [HttpPost]
        public ActionResult ChangePassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appcontext));



                String hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.NewPassword);

                var user = context.AspNetUsers.Find(model.Id);
                user.PasswordHash = hashedNewPassword;
                context.SaveChanges();

                TempData["UserMessage"] = new UserMessage { CssClassName = "alert-success", Title = "Success!", Message = "Password Changed." };


                return RedirectToAction("Index");

                //IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);



            }

            // If we got this far, something failed, redisplay form
            TempData["UserMessage"] = new UserMessage { CssClassName = "alert-danger", Title = "Error!", Message = "Operation Failed." };
            return View(model);

        }



    }
}