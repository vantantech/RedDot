using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAccess.Models;
using System.Net.Mail;
using System.Web.Mvc;




namespace WebAccess.Models
{
    public static class Utility
    {
        public static void SaveLog(string source, string message1)
        {
            storeEntities context = new storeEntities();
            var newlog = new Log();
            newlog.source = source;
            newlog.createdate = DateTime.Now;
            if (message1.Length > 100) newlog.message = message1.Substring(0, 100);
            else  newlog.message = message1;
      

            context.Logs.Add(newlog);
            context.SaveChanges();
        }
   
        public static int GetQuarter(DateTime date)
        {
            return  (date.Month + 2) / 3;
        }
      
        public static string GetUserType()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string userid = HttpContext.Current.User.Identity.GetUserId();
                storeEntities context = new storeEntities();
                var s = context.AspNetUsers.Find(userid);
                return s.SysUserType.TypeName;

            }
            return "error";
        } 

        public static string SendEmail(IEnumerable<AspNetUser> users, string subject, string body)
        {
         
            try
            {

                storeEntities context = new storeEntities();
                var emailinfo = context.SysSettings.Where(x => x.Item == "EmailInfo").FirstOrDefault();

                MailMessage m = new MailMessage();
                SmtpClient sc = new SmtpClient(emailinfo.Value1);
                

                if (users.Count() == 0) return "Fail: No Users";

 
                m.From = new MailAddress(emailinfo.Value2);


                foreach (var user in users)
                {
                    m.To.Add(user.Email);
                }


                m.Subject = subject;

                m.Body = body;


                sc.Port = int.Parse(emailinfo.Value5);
                sc.Credentials = new System.Net.NetworkCredential(emailinfo.Value3,emailinfo.Value4);
               // sc.EnableSsl = true;
                sc.Send(m);
                return "Success";
            }catch(Exception ex)
            {
                return "Error:" + ex.Message;
            }

        }

        public static string SendEmailToUser(string emailaddress, string subject, string body)
        {

            try
            {

                storeEntities context = new storeEntities();
                var emailinfo = context.SysSettings.Where(x => x.Item == "EmailInfo").FirstOrDefault();

                MailMessage m = new MailMessage();
                SmtpClient sc = new SmtpClient(emailinfo.Value1);


                m.From = new MailAddress(emailinfo.Value2);
                m.To.Add(emailaddress);
               
                m.Subject = subject;

                m.Body = body;


                sc.Port = int.Parse(emailinfo.Value5);
                sc.Credentials = new System.Net.NetworkCredential(emailinfo.Value3, emailinfo.Value4);
                // sc.EnableSsl = true;
                sc.Send(m);
                return "Success";
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }

        }


    
 

    }

    public class ListPair
    {
        public int Id { get; set; }
       public string Name { get; set; }
 

    }
}