using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace WebAccess.Models
{
    public class WebAccessAuthorizeAttribute : AuthorizeAttribute
    {

        public string View { get; set; }
        public string Permission { get; set; }



        public WebAccessAuthorizeAttribute()
        {
            View = "AuthorizeFailed"; //go to this page if failed

        }


        /// <summary>
        /// Check for Authorization
        /// </summary>
        /// <param name="filterContext"></param>
        /// 


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //check on permission since user is authorized ( has atlest Read Access)_
            if (filterContext.Result == null)
            {

                //check user for other access if required
                if (Permission != null && Permission != "")
                {
                    //does user has Create, Update, Delete or Approve access??

                    if (!HasPermission(Roles, Permission))
                    {
                        NotAuthorized(filterContext);
                    }


                }

            }
            else
            {
                //user not authorized to even View ( Read Access)
                NotAuthorized(filterContext);
            }



        }

        /// <summary>
        /// Method to check if the user is Authorized or not
        /// if yes continue to perform the action else redirect to error page
        /// </summary>
        /// <param name="filterContext"></param>
        private void NotAuthorized(AuthorizationContext filterContext)
        {


            //If the user is Un-Authorized then Navigate to Auth Failed View 
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {

                // var result = new ViewResult { ViewName = View };
                var vr = new ViewResult();
                vr.ViewName = View;

                // ViewDataDictionary dict = new ViewDataDictionary();
                // dict.Add("Message", "Sorry you are not Authorized to Perform this Action");

                // vr.ViewData = dict;

                var result = vr;

                filterContext.Result = result;
            }
        }


        private bool HasPermission(string requiredrole, string permission)
        {
            storeEntities context = new storeEntities();

            string userid = HttpContext.Current.User.Identity.GetUserId();
            var role = context.AspNetUserRoles.Where(x => x.AspNetRole.Name == requiredrole).Where(x => x.UserId == userid).ToList();
            if (role != null)
            {
                string foundpermission = (role[0].C ? "Create," : "") + (role[0].R ? "Read," : "") + (role[0].U ? "Update," : "") + (role[0].D ? "Delete," : "") + (role[0].A ? "Approve" : "");

                if (foundpermission.Contains(permission)) return true;
                else return false;
            }
            else return false;


        }
    }
}