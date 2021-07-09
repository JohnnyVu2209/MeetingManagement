using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;
using Microsoft.AspNet.Identity;

namespace MeetingManagement.Areas.User.Controllers
{
    [Authorize]
    public class MeetingsController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        private string currentUser;
        // GET: User/MEETINGs
        public ActionResult Index()
        {

            var mEETINGs = db.MEETINGs.Include(m => m.CATEGORY);
            return View(mEETINGs.ToList());
        }

        public ActionResult MeetingDetail(int id)
        {
            var meeting = db.MEETINGs.Find(id);
            return View(meeting);
        }

        public PartialViewResult MeetingInfo(int id)
        {
            var meeting = db.MEETINGs.Find(id);
            return PartialView(meeting);
        }
        public ActionResult MeetingTask(int id)
        {
            var task = db.TASKs.Where(x => x.Meeting_id == id).ToList();
            return PartialView(task);
        }
        public ActionResult MeetingReport(int id)
        {
            var report = db.MEETINGs.Find(id);
            return PartialView(report);
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
        public ActionResult MeetingList(int id)
        {
            var all = db.MEETINGs.Where(x => x.Category_id == id).ToList();
            return PartialView(all);
        }
        public ActionResult MeetingForm(int id)
        {
            MEETING newMeet = new MEETING();
            newMeet.Category_id = id;
            return View("MeetingForm2", newMeet);
        }

        [HttpPost]
        public ActionResult MeetingForm(MEETING model, HttpPostedFileBase Files)
        {
            if (ModelState.IsValid)
            {
                if (Files != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        MEETING newMeet = new MEETING();
                        newMeet.Category_id = model.Category_id;
                        newMeet.Meeting_name = model.Meeting_name;
                        newMeet.Meeting_content = model.Meeting_content;
                        newMeet.Date_Start = model.Date_Start;
                        newMeet.Time_Start = model.Time_Start;
                        newMeet.Location = model.Location;
                        newMeet.Status = 1;
                        newMeet.Date_Create = DateTime.Today;
                        newMeet.Create_by = User.Identity.GetUserId();
                        db.MEETINGs.Add(newMeet);
                        db.SaveChanges();
                        //store file
                        MEETING meetings = db.MEETINGs.Where(x => x.Meeting_name == newMeet.Meeting_name).FirstOrDefault();
                        /*foreach(HttpPostedFileBase file in Files)
                        {*/
                        var path = Server.MapPath(File_Path);
                        Files.SaveAs(path + Files.FileName);

                        //store link file to db
                        ATTACHMENT newAtt = new ATTACHMENT();
                        newAtt.Meeting_id = meetings.Meeting_id;
                        newAtt.Attachment_path = File_Path + Files.FileName;
                        db.ATTACHMENTs.Add(newAtt);
                        db.SaveChanges();
                        //}
                        string[] users = model.AspNetUser.ToString().Split(',');
                        foreach (string user in users)
                        {
                            AspNetUser account = db.AspNetUsers.Where(x => x.Email == user).FirstOrDefault();
                            MEMBER member = new MEMBER();
                            member.Meeting_id = meetings.Meeting_id;
                            member.Member_id = account.Id;
                            db.MEMBERs.Add(member);
                            db.SaveChanges();
                        }
                        scope.Complete();
                        return RedirectToAction("Details", "Categories", new { id = model.Category_id });
                    }
                }
                else ModelState.AddModelError("", "File not found!");
            }
            return View(model);
        }
        private const string File_Path = "~/Upload/Attachments/";
        public ActionResult CreateUser()
        {
            List<AspNetUser> model = db.AspNetUsers.ToList();
            ViewBag.result = model;
            return PartialView();
        }
        public ActionResult CreateUser2()
        {
            return View();
        }
    }
}
