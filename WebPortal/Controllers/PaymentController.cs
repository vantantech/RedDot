using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAccess.Helper;
using WebAccess.Models;
using WebAccess.ViewModels;

namespace WebAccess.Controllers
{
    public class PaymentController : BaseController
    {
     
        // GET: Payment
        public ActionResult Index(int? reporttype, int? StoreId, string startdate, string enddate)
        {

            int webid = 0;

            if (startdate == null) startdate = DateTime.Now.ToShortDateString();
            if (enddate == null) enddate = DateTime.Now.ToShortDateString();


            ViewBag.SalesType = SelectListHelper.GetUserStores(CurrentUser, reporttype);
            ViewBag.startdate = startdate;
            ViewBag.enddate = enddate;

            DateTime StartDate = Convert.ToDateTime(startdate);
            DateTime EndDate = Convert.ToDateTime(enddate);

            var defaultstore = CurrentUser.stores.FirstOrDefault();
            if (defaultstore != null)
            {

                if (StoreId.HasValue)
                {
                    webid = (int)wa.stores.Find(StoreId).WebUserId;
                    ViewBag.Stores = SelectListHelper.GetUserStores(CurrentUser, StoreId);
                }
                else
                {
                    webid = (int)defaultstore.WebUserId;
                    ViewBag.Stores = SelectListHelper.GetUserStores(CurrentUser, defaultstore.id);
                }

                var tips = (from s in context.sales
                             join t in context.gratuities on s.id equals t.salesid
                             join empsub in (from emp in context.employees  select emp ) on s.employeeid equals empsub.id into ps from e in ps.DefaultIfEmpty()
                             where s.userid == webid
                             && DbFunctions.TruncateTime(s.saledate) >= DbFunctions.TruncateTime(StartDate)
                             && DbFunctions.TruncateTime(s.saledate) <= DbFunctions.TruncateTime(EndDate)
                             select new PaymentVM
                             {
                                 saledate = DbFunctions.TruncateTime(s.saledate),
                                 description = "Tips",
                                 netamount = t.amount,
                                 Cashier = e.firstname + " " + e.lastname

                             }).ToList();

               

                var sales = (from s in context.sales
                             join p in context.payments on s.id equals p.salesid
                             join empsub in (from emp in context.employees  select emp) on s.employeeid equals empsub.id into ps from e in ps.DefaultIfEmpty()
                             where s.userid == webid && p.voided == 0 && p.authorcode != "VOID"
                             && DbFunctions.TruncateTime(s.saledate) >= DbFunctions.TruncateTime(StartDate)
                             && DbFunctions.TruncateTime(s.saledate) <= DbFunctions.TruncateTime(EndDate)
                             select new PaymentVM {
                                 saledate = DbFunctions.TruncateTime(s.saledate),
                             description = p.description,
                             netamount = p.netamount,
                                 Cashier = e.firstname + " " + e.lastname

                             }).ToList();


                var tips_payments = tips.Union(sales).ToList();

                var pivot = tips_payments.GroupBy(f => new { f.saledate, f.Cashier })
                    .Select(g => new PaymentPivot
                    {
                        saledate = g.Key.saledate,
                        Cashier = g.Key.Cashier,
                        Cash = g.Where(x => x.description == "Cash").Sum(x=>x.netamount),
                        Credit = g.Where(x => x.description.ToUpper() == "CREDIT").Sum(x => x.netamount),
                        Debit = g.Where(x => x.description.ToUpper() == "DEBIT").Sum(x => x.netamount),
                        GiftCard = g.Where(x => x.description.ToUpper() == "GIFT CARD").Sum(x => x.netamount),
                        GiftCertificate = g.Where(x => x.description == "Gift Certificate").Sum(x => x.netamount),
                        Reward = g.Where(x => x.description == "Reward").Sum(x => x.netamount),
                        StampCard = g.Where(x => x.description == "StampCard").Sum(x => x.netamount),
                        Tips = g.Where(x => x.description == "Tips").Sum(x => x.netamount),
                        AllPayments = g.Sum(x=>x.netamount)
                    }).OrderBy(x=>x.saledate).ThenBy(x=>x.Cashier).ToList();

                // var sales = (from s in context.sales where s.userid == store.WebUserId && DbFunctions.TruncateTime(s.saledate) == DbFunctions.TruncateTime(vDate) select s).ToList();
                return View(pivot);

            }
            else
            {

                return View("NoStores");
            }


            

        }
    }
}