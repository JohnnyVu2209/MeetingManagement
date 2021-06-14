using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    public class DanhMucController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();

        // GET: HeadOfDepartment/DanhMuc
        public ActionResult Index()
        {
            var cATEGORies = db.CATEGORies.Include(c => c.USER);
            return View(cATEGORies.ToList());
        }

        // GET: HeadOfDepartment/DanhMuc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CATEGORY cATEGORY = db.CATEGORies.Find(id);
            if (cATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(cATEGORY);
        }

        // GET: HeadOfDepartment/DanhMuc/Create
        public ActionResult Create()
        {
            ViewBag.Create_by = new SelectList(db.USERS, "User_id", "Full_name");
            return View();
        }

        // POST: HeadOfDepartment/DanhMuc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Create_by,Category_id,Category_Name,Category_Content")] CATEGORY cATEGORY)
        {
            if (ModelState.IsValid)
            {
                db.CATEGORies.Add(cATEGORY);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Create_by = new SelectList(db.USERS, "User_id", "Full_name", cATEGORY.Create_by);
            return View(cATEGORY);
        }

        // GET: HeadOfDepartment/DanhMuc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CATEGORY cATEGORY = db.CATEGORies.Find(id);
            if (cATEGORY == null)
            {
                return HttpNotFound();
            }
            ViewBag.Create_by = new SelectList(db.USERS, "User_id", "Full_name", cATEGORY.Create_by);
            return View(cATEGORY);
        }

        // POST: HeadOfDepartment/DanhMuc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Create_by,Category_id,Category_Name,Category_Content")] CATEGORY cATEGORY)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cATEGORY).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Create_by = new SelectList(db.USERS, "User_id", "Full_name", cATEGORY.Create_by);
            return View(cATEGORY);
        }

        // GET: HeadOfDepartment/DanhMuc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CATEGORY cATEGORY = db.CATEGORies.Find(id);
            if (cATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(cATEGORY);
        }

        // POST: HeadOfDepartment/DanhMuc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CATEGORY cATEGORY = db.CATEGORies.Find(id);
            db.CATEGORies.Remove(cATEGORY);
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
