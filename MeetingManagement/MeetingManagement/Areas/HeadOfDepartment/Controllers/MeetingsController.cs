using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;
using System.Transactions;
using Microsoft.AspNet.Identity;
using System.IO;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    public class MEETINGsController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
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
        public ActionResult MeetingList(int id)
        {
            var all = db.MEETINGs.Where(x => x.Category_id == id).ToList();
            return PartialView(all);
        }
        public ActionResult MeetingForm(int id)
        {
            GetMeeting();
            var current = User.Identity.GetUserId();
            var userList = db.AspNetUsers.Where(x => x.Id != current).Select(selector: x => x.Email).ToList();

            meeting.Category_id = id;
            ViewBag.userList = userList;
            return View(meeting);
        }

        [HttpPost]
        public ActionResult MeetingForm(MEETING model, HttpPostedFileBase Files)
        {
            GetMeeting();
            ValidateMeeting(model);
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
                            newAtt.Attachment_path = File_Path + meetings.Meeting_id+extension;
                            newAtt.Attachment_name = Files.FileName;
                            newAtt.Attachment_name = Files.FileName;
                            newAtt.Attachment_binary = Math.Round(((Double)Files.ContentLength / 1024), 2).ToString() + "KB";
                            db.ATTACHMENTs.Add(newAtt);
                            db.SaveChanges();

                            //store link file to db
                            var path = Server.MapPath(File_Path);
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
            newMeet.Status = 2;
            newMeet.Date_Create = DateTime.Today;
            newMeet.Create_by = User.Identity.GetUserId();
            db.MEETINGs.Add(newMeet);
            db.SaveChanges();

            var newMeeting = db.MEETINGs.Where(x => x.Meeting_name == newMeet.Meeting_name
                                                && x.Date_Start == newMeet.Date_Start
                                                && x.Time_Start == newMeet.Time_Start).FirstOrDefault();
            foreach (var member in meeting.MEMBERs.ToList())
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

        private const string File_Path = "~/Upload/Attachments/";
        private void ValidateMeeting(MEETING meeting)
        {
            if (meeting.Date_Start <= DateTime.Today)
            {
                ModelState.AddModelError("Date_Start", "Không được tạo cuộc họp trong cùng ngày hoặc trước đó");
            }
            TimeSpan twentyoneHour = new TimeSpan(21, 0, 0);
            TimeSpan seventhHour = new TimeSpan(07, 0, 0);
            if (meeting.Time_Start < seventhHour || meeting.Time_Start >= twentyoneHour)
            {
                ModelState.AddModelError("Time_Start", "Không thể mở cuộc họp vào thời gian đó");
            }
        }
        public ActionResult AddUser(MEETING model, string email)
        {
            GetMeeting();
            bool checkExist = false;
            AspNetUser user = db.AspNetUsers.SingleOrDefault(x => x.Email == email);
            foreach (var m in meeting.MEMBERs.ToList())
            {
                if (m.Member_id == user.Id)
                    checkExist = true;
            }
            if (checkExist != true)
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
            foreach (var member in meeting.MEMBERs.ToList())
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

        public ActionResult ReportList()
        {
            ViewBag.meeting = db.MEETINGs.ToList();
            return View();
        }
        public ActionResult MeetingDetail(int id)
        {
            var meeting = db.MEETINGs.Find(id);
            return View(meeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MeetingDetail(int category_id, int meeting_id, string meeting_name, DateTime date_start, TimeSpan time_start, string location, string content, int status)
        {
            MEETING meeting = db.MEETINGs.Find(meeting_id);
            meeting.Category_id = category_id;
            meeting.Meeting_name = meeting_name;
            meeting.Date_Start = date_start;
            meeting.Time_Start = time_start;
            meeting.Location = location;
            meeting.Meeting_content = content;
            meeting.Status = status;

            db.Entry(meeting).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
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
        public FileResult DownloadFile(int meeting_id)
        {
            var meeting = db.MEETINGs.Find(meeting_id);

            var path = Server.MapPath(meeting.REPORT.Report_link);

            var bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", meeting.REPORT.Report_name);
        }
    }
}