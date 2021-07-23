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
    public class TasksController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        private string ASIGNED_TASK = "Bạn đã được giao việc: ";
        private string REMIND_TASK = "Nhắc nhở hoàn thành công việc: ";
        // GET: User/Tasks
        public ActionResult Index()
        {
            var userid = User.Identity.GetUserId();
            var mEMBER = db.MEMBERs.FirstOrDefault(x => x.Member_id == userid);
            ViewBag.meeting = db.MEETINGs.Where(x => x.Meeting_id == mEMBER.Meeting_id).ToList();
            ViewBag.task = db.TASKs.Where(x => x.Meeting_id == mEMBER.Meeting_id && x.Assignee == mEMBER.Member_id).OrderBy(x => x.Task_Status == true).ToList();
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
        public ActionResult Create(int MeetingID)
        {
            ViewBag.Meeting_id = new SelectList(db.MEMBERs.Where(x => x.Meeting_id == MeetingID), "Member_id", "AspNetUser.Full_name");
            var task = new TASK();
            task.Meeting_id = MeetingID;
            return PartialView(task);
        }

        // POST: User/Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TASK tASK, string Asignee)
        {            
            db.TASKs.Add(new TASK
            {
                Task_name = tASK.Task_name,
                Assignee = Asignee,
                Task_Deadline = tASK.Task_Deadline,
                Task_Status = false,
                Meeting_id = tASK.Meeting_id
            });
            db.SaveChanges();
            var meetingid = db.MEETINGs.Find(tASK.Meeting_id).Meeting_name.ToString();
            var content = ASIGNED_TASK + tASK.Task_name;
            var sender = db.AspNetUsers.Find(Asignee).Email.ToString();
            var mail = new Outlook(sender, meetingid, content);
            mail.SendMail();

            ViewBag.Meeting_id = new SelectList(db.MEETINGs, "Meeting_id", "Meeting_name", tASK.Meeting_id);
            ViewBag.Meeting_id = new SelectList(db.MEMBERs, "Meeting_id", "Member_id", tASK.Meeting_id);
            return RedirectToAction("MeetingDetail", "Meetings", new { id = tASK.Meeting_id });
        }

        // GET: User/Tasks/Edit/5
        public ActionResult Edit(int TaskID)
        {
            var task = db.TASKs.Find(TaskID);
            ViewBag.Member = new SelectList(db.MEMBERs.Where(t => t.Meeting_id == task.Meeting_id), "Member_id", "AspNetUser.Email");
            return PartialView(task);
        }

        // POST: User/Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TASK tASK, string Asignee)
        {
            if (ModelState.IsValid)
            {
                tASK.Assignee = Asignee;
                db.Entry(tASK).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MeetingDetail", "Meetings", new { id = tASK.Meeting_id });
            }
            ViewBag.Meeting_id = new SelectList(db.MEETINGs, "Meeting_id", "Meeting_name", tASK.Meeting_id);
            ViewBag.Meeting_id = new SelectList(db.MEMBERs, "Meeting_id", "Member_id", tASK.Meeting_id);
            return RedirectToAction("MeetingDetail", "Meetings", new { id = tASK.Meeting_id });
        }

        // GET: User/Tasks/Delete/5
        public ActionResult Delete(int TaskID)
        {
            var task = db.TASKs.Find(TaskID);
            var meeting_id = task.Meeting_id;
            db.TASKs.Remove(task);
            db.SaveChanges();
            return RedirectToAction("MeetingDetail", "Meetings", new { id = meeting_id });
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

        [HttpGet]
        public ActionResult RemindTask(string AsigneeID, int MeetingID, string TaskName)
        {
            var receiver = db.AspNetUsers.Find(AsigneeID).Email.ToString();
            var subject = db.MEETINGs.Find(MeetingID).Meeting_name.ToString();
            var body = REMIND_TASK + TaskName;
            var mail = new Outlook(receiver, subject, body);
            mail.SendMail();
            //return RedirectToAction("Index", "Tasks");
            return new EmptyResult();
        }

    }
}
