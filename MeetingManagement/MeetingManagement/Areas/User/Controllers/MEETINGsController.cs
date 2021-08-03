using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
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
        private MEETING meeting = null;
        private void GetMeeting()
        {
            if (Session["Meeting"] != null)
                meeting = Session["Meeting"] as MEETING;
            else
            {
                meeting = new MEETING();
                Session["Meeting"] = meeting;
            }
        }
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


        public ActionResult CancelModel(int meeting_id, int model_type)
        {
            ViewBag.model_type = model_type;
            var meeting = db.MEETINGs.Find(meeting_id);
            meeting.Feedback = "";
            return PartialView(meeting);
        }

        [HttpPost]
        public ActionResult CancelModel(int meeting_id, string Feedback, int model_type, string? meeting_name, DateTime meeting_datestart, TimeSpan meeting_timestart, string meeting_location, string meeting_content)
            {
            var meeting = db.MEETINGs.Find(meeting_id);
            if(model_type == 1)
            {
                meeting.Status = 7;
                meeting.Feedback = Feedback;
            }
            else
            {
                meeting.Feedback = Feedback;
                meeting.Meeting_name = meeting_name;
                meeting.Date_Start = meeting_datestart;
                meeting.Time_Start = meeting_timestart;
                meeting.Location = meeting_location;
                meeting.Meeting_content = meeting_content;
            }

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
            ViewBag.date = DateTime.Now.ToShortDateString();
            ViewBag.time = DateTime.Now.ToShortTimeString();
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
        [HttpGet]
        public ActionResult MeetingForm(int id)
        {
            GetMeeting();
            var current = User.Identity.GetUserId();
            var userList = db.AspNetUsers.Where(x=> x.Id != current).Select(selector: x => x.Email).ToList();
       
            meeting.Category_id = id;
            ViewBag.userList = userList;
            return View(meeting);
        }

        [HttpPost]
        public ActionResult MeetingForm(MEETING model, HttpPostedFileBase Files)
        {
            GetMeeting();
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

                            string extension = Path.GetExtension(Files.FileName);
                            newAtt.Meeting_id = meetings.Meeting_id;
                            newAtt.Attachment_path = File_Path_Attachment + meetings.Meeting_id + extension;
                            newAtt.Attachment_name = Files.FileName;
                            newAtt.Attachment_binary = Math.Round(((Double)Files.ContentLength / 1024),2).ToString() + "KB";
                            db.ATTACHMENTs.Add(newAtt);
                            db.SaveChanges();

                            //store link file to db
                            var path = Server.MapPath(File_Path_Attachment);
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
            var current = User.Identity.GetUserId();
            var userList = db.AspNetUsers.Where(x => x.Id != current).Select(selector: x => x.Email).ToList();
            ViewBag.userList = userList;
            model.MEMBERs = meeting.MEMBERs;
            return View(model);
        }
        private void AddMeeting(MEETING model)
        {
            GetMeeting();
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

            var newMeeting = db.MEETINGs.Where(x => x.Meeting_name == newMeet.Meeting_name 
                                                &&  x.Date_Start   == newMeet.Date_Start
                                                &&  x.Time_Start   == newMeet.Time_Start).FirstOrDefault();
            foreach(var member in meeting.MEMBERs.ToList())
            {
                member.Meeting_id = newMeeting.Meeting_id;
                db.MEMBERs.Add(member);
                db.SaveChanges();
            }
            meeting = new MEETING();
            Session["Meeting"] = meeting;
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
        public ActionResult AddUser(MEETING model, string email)
        {
            GetMeeting();
            bool checkExist = false;
            AspNetUser user = db.AspNetUsers.SingleOrDefault(x => x.Email == email);
            foreach(var m in meeting.MEMBERs.ToList())
            {
                if (m.Member_id == user.Id)
                    checkExist = true;
            }
            if(checkExist != true)
            {
                MEMBER member = new MEMBER();
                member.Member_id = user.Id;

                if (meeting.MEMBERs.Count != 0)
                {
                    model.MEMBERs = meeting.MEMBERs;
                }
                model.MEMBERs.Add(member);
                
            }
            meeting = model;
            Session["Meeting"] = meeting;
            return RedirectToAction("MeetingForm", new { id = model.Category_id });
        }
        public ActionResult RemoveUser(string userId)
        {
            GetMeeting();
            foreach(var member in meeting.MEMBERs.ToList())
            {
                if (member.Member_id == userId)
                    meeting.MEMBERs.Remove(member);
            }
            Session["Meeting"] = meeting;
            return RedirectToAction("MeetingForm", new { id = meeting.Category_id });
        }
        public ActionResult CreateUser2()
        {
            return View();
        }
    }
}
