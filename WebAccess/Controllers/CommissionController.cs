using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;
using WebAccess.Helper;
using System.Data.Entity;
using WebAccess.ViewModels;

namespace WebAccess.Controllers
{
    public class CommissionController : BaseController
    {
        // GET: Commission
        public ActionResult Index(int? reporttype, int? StoreId, string startdate, string enddate, int? employeeid)
        {

           
                 int webid = 0;

            if (startdate == null) startdate = DateTime.Now.ToShortDateString();
            if (enddate == null) enddate = DateTime.Now.ToShortDateString();


            if (employeeid == null) employeeid = 0;
           
            ViewBag.startdate = startdate;
            ViewBag.enddate = enddate;

            DateTime StartDate = Convert.ToDateTime(startdate);
            DateTime EndDate = Convert.ToDateTime(enddate);

            var defaultstore = CurrentUser.stores.FirstOrDefault();
            if (defaultstore != null)
            {
                

                if (StoreId.HasValue)
                {
                    webid = (int)context.stores.Find(StoreId).WebUserId;
                    ViewBag.Stores = SelectListHelper.GetUserStores(CurrentUser, StoreId);
                }
                else
                {
                    webid = (int)defaultstore.WebUserId;
                    ViewBag.Stores = SelectListHelper.GetUserStores(CurrentUser, defaultstore.id);
                }


                ViewBag.Employees = SelectListHelper.GetStoreEmployees(webid);



                var sales1 = (from s in context.sales
                             join t in context.salesitems on s.id equals t.salesid
                              where s.userid == webid && t.employeeid == (int)employeeid && t.commissiontype != "none"
                             && DbFunctions.TruncateTime(s.saledate) >= DbFunctions.TruncateTime(StartDate)
                             && DbFunctions.TruncateTime(s.saledate) <= DbFunctions.TruncateTime(EndDate)
                             group new { s, t} by new { s.id, s.saledate, s.ticketno, s.total, s.subtotal, s.status, s.note, s.adjustment, s.custom1, s.custom2, s.custom3, s.custom4 } into g
                             select new Ticket
                             {
                                 id = g.Key.id,
                                 saledate = g.Key.saledate,
                                 ticketno = g.Key.ticketno,
                                 total = g.Sum(x=>x.t.price),
                                 Commission = g.Sum(x => x.t.price) * 0.60m,
                                 status = g.Key.status,
                                 note = g.Key.note,
                                 adjustment = g.Key.adjustment,
                                 custom1 = g.Key.custom1,
                                 custom2 = g.Key.custom2,
                                 custom3 = g.Key.custom3,
                                 custom4 = g.Key.custom4
                                 
                             }).ToList();

                if(sales1 == null)
                {
                    return View(sales1);

                }else
                {
                    var sales = (from s in sales1
                                 join gr in (from grat in context.gratuities where grat.employeeid == (int)employeeid select grat) on s.id equals gr.salesid into ps
                                 from c in ps.DefaultIfEmpty()
                                 select new Ticket
                                 {
                                     id = s.id,
                                     saledate = s.saledate,
                                     ticketno = s.ticketno,
                                     total = s.total,
                                     Commission = s.Commission,
                                     status = s.status,
                                     note = s.note,
                                     adjustment = s.adjustment,
                                     custom1 = s.custom1,
                                     custom2 = s.custom2,
                                     custom3 = s.custom3,
                                     custom4 = s.custom4,
                                     TipTotal =(c==null)?0:c.amount
                                 }).ToList();





                    // var sales = (from s in context.sales where s.userid == store.WebUserId && DbFunctions.TruncateTime(s.saledate) == DbFunctions.TruncateTime(vDate) select s).ToList();
                    return View(sales);
                }
          

            }
            else
            {

                return View("NoStores");
            }


      
           
           
               
        }
    }
}