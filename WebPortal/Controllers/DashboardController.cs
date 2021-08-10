using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAccess.Models;
using WebAccess.Helper;


namespace WebAccess.Controllers
{
    public class DashboardController : BaseController
    {

     
        // GET: Dashboard
        public ActionResult Index()
        {
            if (CurrentUser == null) return RedirectToAction("Index", "Home", null);

            var messages = wa.SystemMessages.Where(x => x.Active == true).Where(y => y.StartDate < DateTime.Now).Where(z => z.EndDate > DateTime.Now).ToList();

            ViewBag.ServerTime = "<" + DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString() + ">";
            return View(messages);
        }



        public ActionResult GetDailySales(int? StoreId, string reportdate, string move)
        {

            int webid = 0;
            DateTime querydate;

            if(reportdate == null)
            {
               
                querydate = DateTime.Now;
            }else
            {
                
                querydate = Convert.ToDateTime(reportdate);
                if (move == "previous") querydate = querydate.AddDays(-1);
                if (move == "next") querydate = querydate.AddDays(1);
                if (querydate > DateTime.Now) querydate = DateTime.Now;


               
            }

            ViewBag.ReportDate = querydate.ToShortDateString();
            

            var defaultstore = CurrentUser.stores.FirstOrDefault();
            if(defaultstore != null)
            {
                if (CurrentUser.stores.Count > 1)
                    ViewBag.MultipleStores = true;
                else
                    ViewBag.MultipleStores = false;


                if (StoreId.HasValue)
                {
                    webid = (int)wa.stores.Find(StoreId).WebUserId;
                    ViewBag.Stores = SelectListHelper.GetUserStores(CurrentUser, StoreId);

                }else
                {
                    webid = (int)defaultstore.WebUserId;
                    ViewBag.Stores = SelectListHelper.GetUserStores(CurrentUser, defaultstore.id);

                }
            
                //var sales = context.sales.Where(x => x.userid == store.WebUserId).Where(x=> dbfunctions. ).ToList();
               

             
                var sales = (from s in context.sales
                             where s.userid == webid 
                             && DbFunctions.TruncateTime(s.saledate) == DbFunctions.TruncateTime(querydate)
                             select s).ToList();

                return PartialView("_todaysales", sales);
            }else
                return PartialView("_nostores");
          
        }





        public ActionResult GetWeeklySales(int? StoreId, string reportdate, string move)
        {
            int webid = 0;
            DateTime firstday = Convert.ToDateTime("01/01/2017");
            DateTime WeeklyStartDate;
            DateTime WeeklyEndDate;
            DateTime ThisWeek;

            int diff = (int)DateTime.Now.DayOfWeek - (int)firstday.DayOfWeek;


            if (diff >= 0)
            {
                //positive or equal ..that means the start
                ThisWeek = DateTime.Now.AddDays(-diff);

            }
            else
            {
                ThisWeek = DateTime.Now.AddDays((diff * -1) - 7);
            }




            var defaultstore = CurrentUser.stores.FirstOrDefault();
            if (defaultstore != null)
            {
                if (CurrentUser.stores.Count > 1)
                    ViewBag.MultipleStores = true;
                else
                    ViewBag.MultipleStores = false;


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
            

                if (reportdate == null)
                {
                    WeeklyStartDate = ThisWeek;
                 
                }
                else
                {

                    WeeklyStartDate = Convert.ToDateTime(reportdate);

                    if (move == "previous") WeeklyStartDate = WeeklyStartDate.AddDays(-7);
                    if (move == "next") WeeklyStartDate = WeeklyStartDate.AddDays(7);


                    if (WeeklyStartDate > ThisWeek) WeeklyStartDate = ThisWeek;  //revert back if week is in future



                }




                ViewBag.ReportDate = WeeklyStartDate.ToShortDateString();

                WeeklyEndDate = WeeklyStartDate.AddDays(6);



                //var sales = context.sales.Where(x => x.userid == store.WebUserId).Where(x=> dbfunctions. ).ToList();
              

                var sales = (from s in context.sales
                             where s.userid == webid
                             && DbFunctions.TruncateTime(s.saledate) >= DbFunctions.TruncateTime(WeeklyStartDate) && DbFunctions.TruncateTime(s.saledate) <= DbFunctions.TruncateTime(WeeklyEndDate)
                             select s).ToList();

                return PartialView("_weeklysales", sales);
            }
            else
                return PartialView("_nostores");

        }



        public ActionResult GetMonthlySales(int? StoreId, string reportdate, string move)
        {
            int webid = 0;
          
            DateTime MonthlyStartDate;
            DateTime MonthlyEndDate;
            DateTime ThisMonth;

            ThisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var defaultstore = CurrentUser.stores.FirstOrDefault();
            if (defaultstore != null)
            {

                if (CurrentUser.stores.Count > 1)
                    ViewBag.MultipleStores = true;
                else
                    ViewBag.MultipleStores = false;


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
            


                if (reportdate == null)
                {
                    MonthlyStartDate = ThisMonth;
                  

                }
                else
                {

                    MonthlyStartDate = Convert.ToDateTime(reportdate);

                    if (move == "previous") MonthlyStartDate = MonthlyStartDate.AddMonths(-1);
                    if (move == "next") MonthlyStartDate = MonthlyStartDate.AddMonths(1);


                    if (MonthlyStartDate > ThisMonth) MonthlyStartDate = ThisMonth;  //revert back if week is in future



                }


                MonthlyEndDate = MonthlyStartDate.AddMonths(1).AddDays(-1);
                ViewBag.ReportDate = MonthlyStartDate.ToShortDateString();

                //var sales = context.sales.Where(x => x.userid == store.WebUserId).Where(x=> dbfunctions. ).ToList();
             

                var sales = (from s in context.sales
                             where s.userid == webid
                             && DbFunctions.TruncateTime(s.saledate) >= DbFunctions.TruncateTime(MonthlyStartDate) && DbFunctions.TruncateTime(s.saledate) <= DbFunctions.TruncateTime(MonthlyEndDate)
                             select s).ToList();

                return PartialView("_monthlysales", sales);
            }
            else
                return PartialView("_nostores");

        }


        // GET: Dashboard/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Dashboard/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Create
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

        // GET: Dashboard/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Dashboard/Edit/5
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

        // GET: Dashboard/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Dashboard/Delete/5
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
