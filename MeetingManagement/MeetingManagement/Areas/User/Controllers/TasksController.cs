using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;

namespace MeetingManagement.Areas.User.Controllers
{
    public class TasksController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();

        // GET: User/Tasks
        public ActionResult Index()
        {
            return View();
        }

        // GET: User/Tasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TASK tASK = db.TASKs.Find(id);
            if (tASK == null)
            {
                return HttpNotFound();
            }
            return View(tASK);
        }

        // GET: User/Tasks/Create
        public ActionResult Create()
        {
            ViewBag.Meeting_id = new SelectList(db.MEMBERs, "Meeting_id", "Member_id");
            return View();
        }

        // POST: User/Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Task_id,Meeting_id,Task_name,Assignee,Task_Status,Task_Deadline,Notify")] TASK tASK)
        {
            if (ModelState.IsValid)
            {
                db.TASKs.Add(tASK);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Meeting_id = new SelectList(db.MEETINGs, "Meeting_id", "Meeting_name", tASK.Meeting_id);
            ViewBag.Meeting_id = new SelectList(db.MEMBERs, "Meeting_id", "Member_id", tASK.Meeting_id);
            return View(tASK);
        }

        // GET: User/Tasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TASK tASK = db.TASKs.Find(id);
            if (tASK == null)
            {
                return HttpNotFound();
            }
            ViewBag.Meeting_id = new SelectList(db.MEETINGs, "Meeting_id", "Meeting_name", tASK.Meeting_id);
            ViewBag.Meeting_id = new SelectList(db.MEMBERs, "Meeting_id", "Member_id", tASK.Meeting_id);
            return View(tASK);
        }

        // POST: User/Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Task_id,Meeting_id,Task_name,Assignee,Task_Status,Task_Deadline,Notify")] TASK tASK)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tASK).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Meeting_id = new SelectList(db.MEETINGs, "Meeting_id", "Meeting_name", tASK.Meeting_id);
            ViewBag.Meeting_id = new SelectList(db.MEMBERs, "Meeting_id", "Member_id", tASK.Meeting_id);
            return View(tASK);
        }

        // GET: User/Tasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TASK tASK = db.TASKs.Find(id);
            if (tASK == null)
            {
                return HttpNotFound();
            }
            return View(tASK);
        }

        // POST: User/Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TASK tASK = db.TASKs.Find(id);
            db.TASKs.Remove(tASK);
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
