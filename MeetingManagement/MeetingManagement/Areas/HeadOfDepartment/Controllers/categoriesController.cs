using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;
using EntityState = System.Data.Entity.EntityState;
using Microsoft.AspNet.Identity;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();

        // GET: HeadOfDepartment/categories
        public ActionResult Index()
        {
            var categories = db.CATEGORies.ToList();
            return View(categories);
        }
        [HandleError]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                throw new Exception();
            }
            CATEGORY cATEGORY = db.CATEGORies.Find(id);
            if (cATEGORY == null)
            {
                throw new Exception();
            }
            return View(cATEGORY);

        }
        public ActionResult MeetingList(int id)
        {
            var meetings = db.MEETINGs.Where(x => x.Category_id == id).ToList(); 
            return PartialView(meetings) ;
        }

        [HttpGet]
       // GET: HeadOfDepartment/categories/Create
        public ActionResult Create()
        {
            return PartialView("Create", new Models.CATEGORY());
        }
       // POST: HeadOfDepartment/categories/Create
        //To protect from overposting attacks, enable the specific properties you want to bind to, for 
         //more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult Create(CATEGORY cg)
        {
            //link huong dan de lam phan nay https://www.youtube.com/watch?v=2ktFobQ4VmM 
            SEP24Team7Entities db = new SEP24Team7Entities();
            cg.Create_by = HttpContext.User.Identity.GetUserId();
            db.CATEGORies.Add(cg);
            db.SaveChanges();
            return Json(cg, JsonRequestBehavior.AllowGet);
        }

         //GET: HeadOfDepartment/categories/Edit/5
        [HttpGet]
        public PartialViewResult Edit(int categoryID)
        {
            CATEGORY cate = db.CATEGORies.Find(categoryID);
            CATEGORY category = new CATEGORY();
            category.Category_id = cate.Category_id;
            category.Category_Name = cate.Category_Name;
            category.Category_Content = cate.Category_Content;
            category.Create_by = cate.Create_by;
            return PartialView(category);
        }
        [HttpPost]
        public ActionResult Edit(CATEGORY cg)
        {
           CATEGORY old_cg = db.CATEGORies.Where(x => x.Category_id == cg.Category_id).FirstOrDefault();
            db.Entry(cg).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            CATEGORY cATEGORY = db.CATEGORies.Where(x => x.Category_id == id).FirstOrDefault();
            return PartialView(cATEGORY);

        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            CATEGORY cat = db.CATEGORies.Find(id);
            if (cat != null)
            {
                db.CATEGORies.Remove(cat);
                db.SaveChanges();
            }
            //link huong dan https://www.youtube.com/watch?v=YQnCkMAYDsQ
            return RedirectToAction("Index");

        }


        public ActionResult statistic()
        {
            var userid = "f28b3bb0-99b7-439e-bc90-4c8c15fac1a2";
            var mEMBER = db.MEMBERs.FirstOrDefault(x => x.Member_id == userid);
            ViewBag.meeting = db.MEETINGs.Where(x => x.Meeting_id == mEMBER.Meeting_id).ToList();
            ViewBag.task = db.TASKs.Where(x => x.Meeting_id == mEMBER.Meeting_id && x.Assignee == mEMBER.Member_id).ToList();
            ViewBag.allWork = db.TASKs.ToList();
            ViewBag.allMeeting = db.MEETINGs.ToList();
            ViewBag.allCate = db.CATEGORies.ToList();
            ViewBag.allMeetingStatus = db.MEETING_STATUS.ToList();
            return View();
        } 

        public ActionResult statisticChart()
        {
            int ms1 = db.MEETINGs.Where(x => x.Status == 1).Count();
            int ms2 = db.MEETINGs.Where(x => x.Status == 2).Count();
            int ms3 = db.MEETINGs.Where(x => x.Status == 3).Count();
            int ms4 = db.MEETINGs.Where(x => x.Status == 4).Count();
            int ms5 = db.MEETINGs.Where(x => x.Status == 5).Count();
            int ms6 = db.MEETINGs.Where(x => x.Status == 6).Count();
            int ms7 = db.MEETINGs.Where(x => x.Status == 7).Count();
            PieChart pie = new PieChart();
            pie.createM = ms1;
            pie.passM = ms2;
            pie.doneM = ms3;
            pie.reportM = ms4;
            pie.compM = ms5;
            pie.nopassM = ms6;
            pie.cancelM = ms7;
            return Json(pie, JsonRequestBehavior.AllowGet);
        }
        public class PieChart
        {
            public int createM { get; set; }
            public int passM { get; set; }
            public int doneM { get; set; }
            public int reportM { get; set; }
            public int compM { get; set; }
            public int nopassM { get; set; }
            public int cancelM { get; set; }
        }

    }
}