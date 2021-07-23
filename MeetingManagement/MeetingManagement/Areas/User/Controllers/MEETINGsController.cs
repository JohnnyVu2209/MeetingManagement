using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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

        public ActionResult MeetingDetail(int id, bool modify)
        {
            ViewBag.modify = modify;
            ViewBag.user_identity = User.Identity.GetUserId();
            var meeting = db.MEETINGs.Find(id);
            return View(meeting);
        }

        public ActionResult CancelModel(int id)
        {
            var meeting = db.MEETINGs.Find(id);
            return PartialView(meeting);
        }

        [HttpPost]
        public ActionResult CancelModel(int Meeting_id, string Feedback)
        {
            var meeting = db.MEETINGs.Find(Meeting_id);
            meeting.Feedback = Feedback;
            meeting.Status = 7;

            db.Entry(meeting).State = EntityState.Modified;
            db.SaveChanges();

            //string To = db.AspNetUsers.Find(meeting.Create_by).Email;
            //string Subject = meeting.Meeting_name;
            //string Body = REFUSE_MEETING + feedback;
            //Outlook mail = new Outlook(To, Subject, Body);
            //mail.SendMail();
            return RedirectToAction("Index");
        }

        public ActionResult DeleteAttachment(int id)
        {
            ATTACHMENT attachment = db.ATTACHMENTs.Find(id);
            return PartialView(attachment);
        }

        [HttpPost]
        public ActionResult DeleteAttachmentConfirm(int id)
        {
            ATTACHMENT att = db.ATTACHMENTs.Find(id);
            if (att != null)
            {
                db.ATTACHMENTs.Remove(att);
                db.SaveChanges();
            }
            //link huong dan https://www.youtube.com/watch?v=YQnCkMAYDsQ
            return RedirectToAction("Index");
        }

        public ActionResult MeetingEdit(int id)
        {
            var meeting = db.MEETINGs.Find(id);
            return PartialView(meeting);
        }

        [HttpPost]
        public ActionResult MeetingEdit(MEETING meeting)
        {
            db.Entry(meeting).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "Meetings");
        }

        public PartialViewResult MeetingInfo(int id)
        {
            ViewBag.user_identity = User.Identity.GetUserId();
            var meeting = db.MEETINGs.Find(id);
            return PartialView(meeting);
        }

        public PartialViewResult MeetingMember(int id)
        {
            var meeting = db.MEETINGs.Find(id);
            ViewBag.meeting_host = meeting.Create_by;
            ViewBag.meeting_create = meeting.AspNetUser.Full_name;
            ViewBag.meeting_user = meeting.AspNetUser.Email;
            var member = db.MEMBERs.Where(x => x.Meeting_id == id).ToList();
            return PartialView(member);
        }
        public ActionResult MeetingTask(int id)
        {
            ViewBag.create_by = db.MEETINGs.Find(id).Create_by;
            ViewBag.user_identity = User.Identity.GetUserId();
            ViewBag.meeting_status = db.MEETINGs.Find(id).Status;
            var task = db.TASKs.Where(x => x.Meeting_id == id).ToList();
            ViewBag.Task = task;
            ViewBag.Meeting = id;
            ViewBag.IsCreateBy = isCreateBy(id);
            return PartialView(task);
        }
        private bool isCreateBy(int id)
        {
            var create_by = db.MEETINGs.Find(id).Create_by.ToString();
            var current_user = User.Identity.GetUserId().ToString();
            if (currentUser == create_by)
            {
                return true;
            }
            return false;
        }

        /*----------Meeting Report-------------*/
        [HttpGet]
        public ActionResult MeetingReport(int id)
        {
            ViewBag.Meeting_id = id;
            return PartialView();
        }
        [HttpPost]
        public ActionResult MeetingReport(int Meeting_id, HttpPostedFileBase ReportFile)
        {
            if(ReportFile != null)
            {
                using (var scope = new TransactionScope())
                {
                    if (ValidateFile(ReportFile))
                    {
                        var path = Server.MapPath(File_Path_Report);
                        string extension = Path.GetExtension(ReportFile.FileName);
                        ReportFile.SaveAs(path + ReportFile.FileName);

                        REPORT report = new REPORT();
                        report.MEETING = db.MEETINGs.Find(Meeting_id);
                        report.Report_name = ReportFile.FileName;
                        report.Report_binary = Math.Round(((Double)ReportFile.ContentLength / 1024),2).ToString() + "KB";
                        report.Report_type = ReportFile.ContentType;
                        report.Report_link = File_Path_Report + ReportFile.FileName;
                        db.REPORTs.Add(report);

                        var meeting = db.MEETINGs.Find(Meeting_id);
                        meeting.Check_report = true;
                        meeting.Status = 4;
                        db.Entry(meeting).State = EntityState.Modified;
                        db.SaveChanges();

                        scope.Complete();
                        return View("MeetingDetail", db.MEETINGs.Find(Meeting_id));
                    }
                    ModelState.AddModelError("File", "Dung lượng tối đa cho phép là 5MB");
                }
            }
            ModelState.AddModelError("FileAttach", "Chưa nộp báo cáo!");
            return View();
        }
        /*----------Meeting Report-------------*/

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

        public ActionResult Delete_Attachment(int id)
        {
            ATTACHMENT attachment  = db.ATTACHMENTs.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            db.ATTACHMENTs.Remove(attachment);
            db.SaveChanges();
            return RedirectToAction("Index");
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
            ViewBag.user_identity = currentUser;
            var result = db.MEETINGs.ToList();
            result = result.Where(m => m.Create_by.ToLower().Contains(currentUser)).ToList();
            var notpending_result = result.Where(n => n.Status > 1);
            return PartialView("MyMeetingGridView", notpending_result);
        }

        public ActionResult JoinedMeeting()
        {
            currentUser = User.Identity.GetUserId();
            ViewBag.user_identity = currentUser;
            var meetings = from u in db.MEMBERs.Where(m => m.Member_id.ToLower().Contains(currentUser))
                           from m in db.MEETINGs
                           where u.Meeting_id == m.Meeting_id
                           select m;
            var notpending_meetings = meetings.Where(n => n.Status > 1);
            return PartialView("JoinedMeetingGridView", notpending_meetings);
        }

        public ActionResult PendingMeeting()
        {
            currentUser = User.Identity.GetUserId();
            ViewBag.user_identity = currentUser;
            var meetings = db.MEETINGs.ToList();
            meetings = meetings.Where(m => m.Create_by.ToLower().Contains(currentUser)).ToList();
            var meetings_1 = meetings.Where(n => n.Status == 1);
            return PartialView(meetings_1);
        }

        public ActionResult MeetingList(int id)
        {
            var all = db.MEETINGs.Where(x => x.Category_id == id).ToList();
            return PartialView(all);
        }
        public ActionResult MeetingForm(int id)
        {
            ViewBag.newMeet = new MEETING();
            ViewBag.newMeet.Category_id = id;
            ViewBag.userList = db.AspNetUsers.ToList();
            return View();
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
                        if (ValidateFile(Files))
                        {
                            AddMeeting(model);

                            //store file

                            MEETING meetings = db.MEETINGs.Where(x => x.Meeting_name == model.Meeting_name).FirstOrDefault();

                            ATTACHMENT newAtt = new ATTACHMENT();
                            newAtt.Meeting_id = meetings.Meeting_id;
                            newAtt.Attachment_path = File_Path_Attachment + meetings.Meeting_id;
                            newAtt.Attachment_name = Files.FileName;
                            newAtt.Attachment_binary = ((Double)Files.ContentLength / 1024).ToString() + "KB";
                            db.ATTACHMENTs.Add(newAtt);
                            db.SaveChanges();

                            //store link file to db
                            var path = Server.MapPath(File_Path_Attachment);
                            string extension = Path.GetExtension(Files.FileName);
                            Files.SaveAs(path + meetings.Meeting_id + extension);
                            scope.Complete();
                            return RedirectToAction("Details", "Categories", new { id = model.Category_id });
                        }
                        ModelState.AddModelError("File", "Dung lượng tối đa cho phép là 5MB");
                    }
                }
                else
                {
                    using (var scope = new TransactionScope())
                    {
                        AddMeeting(model);
                        scope.Complete();
                        return RedirectToAction("Details", "Categories", new { id = model.Category_id });
                    }
                }
            }
            return View(model);
        }
        private void AddMeeting(MEETING model)
        {
            MEETING newMeet = new MEETING();
            newMeet.Category_id = model.Category_id;
            newMeet.Meeting_name = model.Meeting_name;
            newMeet.Meeting_content = model.Meeting_content;
            newMeet.Date_Start = model.Date_Start;
            newMeet.Time_Start = model.Time_Start;
            newMeet.Location = model.Location;
            newMeet.Status = 2;
            newMeet.Date_Create = DateTime.Today;
            newMeet.Create_by = User.Identity.GetUserId();
            db.MEETINGs.Add(newMeet);
            db.SaveChanges();

            var meeting = db.MEETINGs.Where(x => x.Meeting_name == newMeet.Meeting_name).FirstOrDefault();
            string[] users = model.AspNetUsers.Split(',');
            foreach (string user in users)
            {
                AspNetUser account = db.AspNetUsers.Where(x => x.Email == user).FirstOrDefault();
                MEMBER member = new MEMBER();
                member.Meeting_id = meeting.Meeting_id;
                member.Member_id = account.Id;
                db.MEMBERs.Add(member);
                db.SaveChanges();
            }
        }
        private bool ValidateFile(HttpPostedFileBase files)
        {
            var filesize = files.ContentLength;
            if (filesize > 5 * 1024 * 1024)
                return false;
            return true;
        }

        private const string File_Path_Attachment = "~/Upload/Attachments/";
        private const string File_Path_Report = "~/Upload/Reports/";
        
        [HttpGet]
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
