using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;
using Microsoft.AspNet.Identity;

namespace MeetingManagement.Areas.User.Controllers
{
    [Authorize]
    public class MEETINGsController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        private string currentUser;
        // GET: User/MEETINGs
        public ActionResult Index()
        {

            var mEETINGs = db.MEETINGs.Include(m => m.CATEGORY);
            return View(mEETINGs.ToList());
        }

        // GET: User/MEETINGs/Details/5
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

        // GET: User/MEETINGs/Create
        public ActionResult Create()
        {
            ViewBag.Category_id = new SelectList(db.CATEGORies, "Category_id", "Create_by");
            return View();
        }

        // POST: User/MEETINGs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MEETING mEETING)
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

        // GET: User/MEETINGs/Edit/5
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

        // POST: User/MEETINGs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CreateBy_id,Meeting_name,Date_Start,Date_End,Meeting_Confirmed,Category_id,Meeting_id,Lacation,Status,Meeting_report")] MEETING mEETING)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mEETING).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category_id = new SelectList(db.CATEGORies, "Category_id", "Create_by", mEETING.Category_id);
            return View(mEETING);
        }

        // GET: User/MEETINGs/Delete/5
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

        // POST: User/MEETINGs/Delete/5
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

        public ActionResult MyMeeting()
        {
            currentUser = User.Identity.GetUserId();
            var result = db.MEETINGs.ToList();
            result = result.Where(m => m.Create_by.ToLower().Contains(currentUser)).ToList();
            return PartialView("MyMeetingGridView", result);
        }

        public ActionResult JoinedMeeting()
        {
            currentUser = User.Identity.GetUserId();
            var meetings = from u in db.MEMBERs.Where(m => m.Member_id.ToLower().Contains(currentUser))
                           from m in db.MEETINGs
                           where u.Meeting_id == m.Meeting_id
                           select m;
            return PartialView("JoinedMeetingGridView", meetings);
        }
    }
}
