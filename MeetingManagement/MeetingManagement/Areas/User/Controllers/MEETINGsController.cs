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
            var meeting = db.MEETINGs.Find(id);
            return View(meeting);
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

            return RedirectToAction("MeetingDetail", "Meetings", new { id = meeting.Meeting_id, modify = false });
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
            var task = db.TASKs.Where(x => x.Meeting_id == id).ToList();
            return PartialView(task);
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
                        REPORT report = new REPORT();
                        report.Meeting_id = Meeting_id;
                        report.Report_name = ReportFile.FileName;
                        report.Report_binary = ((Double)ReportFile.ContentLength / 1024).ToString() + "KB";
                        report.Report_type = ReportFile.ContentType;
                        report.Report_link = File_Path_Report + ReportFile;
                        db.REPORTs.Add(report);
                        db.SaveChanges();

                        var meeting = db.MEETINGs.Find(Meeting_id);
                        meeting.Check_report = true;
                        meeting.Status = 4;
                        db.Entry(meeting).State = EntityState.Modified;
                        db.SaveChanges();

                        var path = Server.MapPath(File_Path_Report);
                        string extension = Path.GetExtension(ReportFile.FileName);
                        ReportFile.SaveAs(path + ReportFile);
                    }
                    ModelState.AddModelError("File", "Dung lượng tối đa cho phép là 5MB");
                }
            }
            ModelState.AddModelError("FileAttach", "Chưa nộp báo cáo!");
            return View("MeetingDetail",db.MEETINGs.Find(Meeting_id));
        }
        /*----------Meeting Report-------------*/


        public ActionResult MyMeeting()
        {
            currentUser = User.Identity.GetUserId();
            ViewBag.user_identity = currentUser;
            var result = db.MEETINGs.ToList();
            result = result.Where(m => m.Create_by.ToLower().Contains(currentUser)).ToList();
            return PartialView("MyMeetingGridView", result);
        }

        public ActionResult JoinedMeeting()
        {
            currentUser = User.Identity.GetUserId();
            ViewBag.user_identity = currentUser;
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
            var current = User.Identity.GetUserId();
            var userList = db.AspNetUsers.Where(x => x.Id != current).Select(selector: x => x.Email).ToList();
            ViewBag.userList = userList;
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
            newMeet.Status = 2;
            newMeet.Date_Create = DateTime.Today;
            newMeet.Create_by = User.Identity.GetUserId();
            db.MEETINGs.Add(newMeet);
            db.SaveChanges();

            var newMeeting = db.MEETINGs.Where(x => x.Meeting_name == newMeet.Meeting_name).FirstOrDefault();
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
                meeting = model;
                Session["Meeting"] = meeting;
            }
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
