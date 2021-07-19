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

namespace MeetingManagement.Areas.HeadOfDepartment
{
    public class MEETINGsController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();

        // GET: HeadOfDepartment/MEETINGs
        public ActionResult Index()
        {
            var mEETINGs = db.MEETINGs.Include(m => m.CATEGORY);
            return View(mEETINGs.ToList());
        }

        // GET: HeadOfDepartment/MEETINGs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MEETING mEETING = db.MEETINGs.Find(id);
            if (mEETING == null)
            {
                return HttpNotFound();
            }
            return View(mEETING);
        }

        // GET: HeadOfDepartment/MEETINGs/Create
        public ActionResult Create()
        {
            ViewBag.Category_id = new SelectList(db.CATEGORies, "Category_id", "Create_by");
            return View();
        }

        // POST: HeadOfDepartment/MEETINGs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CreateBy_id,Meeting_name,Date_Start,Date_End,Meeting_Confirmed,Category_id,Meeting_id,Lacation,Status,Meeting_report")] MEETING mEETING)
        {
            if (ModelState.IsValid)
            {
                db.MEETINGs.Add(mEETING);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Category_id = new SelectList(db.CATEGORies, "Category_id", "Create_by", mEETING.Category_id);
            return View(mEETING);
        }

        // GET: HeadOfDepartment/MEETINGs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MEETING mEETING = db.MEETINGs.Find(id);
            if (mEETING == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category_id = new SelectList(db.CATEGORies, "Category_id", "Create_by", mEETING.Category_id);
            return View(mEETING);
        }

        // POST: HeadOfDepartment/MEETINGs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "Category_id,Meeting_id,Meeting_name,Meeting_goal,Meeting_content,Date_Start,Time_Start,Location,Status,Meeting_report,Date_Create,Create_by")] MEETING mEETING)
        {
            if (ModelState.IsValid)
            {   
                db.Entry(mEETING).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.Category_id = new SelectList(db.CATEGORies, "Category_id", "Create_by", mEETING.Category_id);
            return View(mEETING);
        }

        // GET: HeadOfDepartment/MEETINGs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MEETING mEETING = db.MEETINGs.Find(id);
            if (mEETING == null)
            {
                return HttpNotFound();
            }
            return View(mEETING);
        }

        // POST: HeadOfDepartment/MEETINGs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MEETING mEETING = db.MEETINGs.Find(id);
            db.MEETINGs.Remove(mEETING);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}