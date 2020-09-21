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
    public class SalesController : BaseController
    {
  
        // GET: Sales


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
                    webid = (int)context.stores.Find(StoreId).WebUserId;
                    ViewBag.Stores = SelectListHelper.GetUserStores(CurrentUser, StoreId);
                }
                else
                {
                    webid = (int)defaultstore.WebUserId;
                    ViewBag.Stores = SelectListHelper.GetUserStores(CurrentUser, defaultstore.id);
                }

                var sales = (from s in context.sales
                             join empsub in (from emp in context.employees where emp.userid == webid select emp) on s.employeeid equals empsub.employeeid into ps from c in ps.DefaultIfEmpty()
                             join t in context.salesitems on s.id equals t.salesid
                             join e in context.employees on t.employeeid equals e.employeeid
                             where s.userid == webid && e.userid == webid
                             && DbFunctions.TruncateTime(s.saledate) >= DbFunctions.TruncateTime(StartDate)
                             && DbFunctions.TruncateTime(s.saledate) <= DbFunctions.TruncateTime(EndDate)
                             group new { s, t, e } by new { s.id, s.saledate, c.firstname, c.lastname, s.ticketno, s.total, s.subtotal, s.status, s.note, s.adjustment, s.custom1, s.custom2, s.custom3, s.custom4 } into g
                             select new Ticket
                             {
                                 id = g.Key.id,
                                 saledate = g.Key.saledate,
                                 EmployeeName = g.Key.firstname + " " + g.Key.lastname,
                                 ticketno = g.Key.ticketno,
                                 total = g.Key.total,
                                 subtotal = g.Key.subtotal,
                                 status = g.Key.status,
                                 note = g.Key.note,
                                 adjustment = g.Key.adjustment,
                                 custom1 = g.Key.custom1,
                                 custom2 = g.Key.custom2,
                                 custom3 = g.Key.custom3,
                                 custom4 = g.Key.custom4,
                                 TicketItems = g.Select(x => new TicketItem {EmployeeName = x.e.firstname })
                             }).ToList();

            

               // var sales = (from s in context.sales where s.userid == store.WebUserId && DbFunctions.TruncateTime(s.saledate) == DbFunctions.TruncateTime(vDate) select s).ToList();
                return View(sales);
                
            }else
            {
                
                return View("NoStores");
            }
           
               
           
           
        }



        // GET: Sales/Details/5
        public ActionResult Details(int id)
        {
            var sales = context.sales.Find(id);
            int webid = sales.userid;


            var detail = (from s in context.sales
                          join empsub in (from emp in context.employees where emp.userid == webid select emp) on s.employeeid equals empsub.employeeid into ps
                          from c in ps.DefaultIfEmpty()

                          where s.id == id
                          select new Ticket
                          {
                              id = s.id,
                              userid = s.userid,
                              ticketno = s.ticketno,
                              saledate = s.saledate,
                              subtotal = s.subtotal,
                              adjustment = s.adjustment,
                              total = s.total,
                              note = s.note,
                              custom1 = s.custom1,
                              custom2 = s.custom2,
                              custom3 = s.custom3,
                              custom4 = s.custom4,
                              status = s.status,
                              stationno = s.stationno,
                              gratuities = s.gratuities,
                              payments = s.payments,
                              EmployeeName = c.firstname + " " + c.lastname
                            
                         }).FirstOrDefault();

            detail.TicketItems = (from t in context.salesitems
                                  join e in context.employees
                                  on t.employeeid equals e.employeeid
                                  where e.userid == detail.userid && t.salesid == detail.id
                                  select new TicketItem
                                  {
                                      id = t.id,
                                      salesid = t.salesid,
                                      description = t.description,
                                      discount = t.discount,
                                      price = t.price,
                                      quantity = t.quantity,
                                      EmployeeName = e.firstname + " " + e.lastname,
                                      note = t.note

                                  }).ToList();

            detail.TicketTips = (from g in context.gratuities
                                 join e in context.employees
                                 on g.employeeid equals e.employeeid
                                 where e.userid == detail.userid && g.salesid == detail.id
                                 select new TicketGratuity
                                 {
                                     id = g.id,
                                     salesid = g.salesid,
                                     amount = g.amount,
                                     EmployeeName = e.firstname + " " + e.lastname

                                 }).ToList();
          
            return View(detail);
        }

        // GET: Sales/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sales/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Sales/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Sales/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Sales/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Sales/Delete/5
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
