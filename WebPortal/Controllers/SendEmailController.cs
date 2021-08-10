using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;

namespace WebAccess.Controllers
{
    public class SendEmailController : BaseController
    {
        webaccessEntities context;
        public SendEmailController()
        {
            context = new webaccessEntities();
        }
        // GET: SendEmail
        public ActionResult Index()
        {
            var list = context.AspNetUsers.ToList();


            ViewBag.DistributionLists = new SelectList(list, "Id", "Email");
    
            return View();
        }

     


        // POST: SendEmail/Send
        [HttpPost]
        public ActionResult Send(int? id,  string subject, string message)
        {
            try
            {

                IEnumerable<AspNetUser> users;
                IEnumerable<AspNetUser> users2;
                IEnumerable<AspNetUser> users3;


                string results = "";
             
          
                int usercount = 0;


                var user = context.AspNetUsers.Find(id);


                results = Utility.SendEmailToUser(user.Email, subject, message);
                Utility.SendEmailToUser(CurrentUser.Email, subject, message);

             
                    if(usercount == 0)
                    {
                        TempData["UserMessage"] = new UserMessage { CssClassName = "alert-danger", Title = "Error!", Message = "No Users selected..."  };
                    }
                    else
                    {
                        TempData["UserMessage"] = new UserMessage { CssClassName = "alert-danger", Title = "Error!", Message = "Operation Failed:" + results };
                    }
                    
              


              
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["UserMessage"] = new UserMessage { CssClassName = "alert-danger", Title = "Error!", Message = "Operation Failed:" + ex.Message };
                return RedirectToAction("Index");
            }
        }

       
    }
}
