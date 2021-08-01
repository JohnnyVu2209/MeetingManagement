﻿using System;
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
        private const string MEETING_TIME = "Thời gian diễn ra cuộc họp: ";
        private const string MEETING_LOCATION = "Địa điểm diễn ra cuộc họp: ";
        private const string MEETING_CANCEL = "Cuộc họp đã bị huỷ vì lý do: ";
        private const string File_Path_Attachment = "~/Upload/Attachments/";
        private const string File_Path_Report = "~/Upload/Reports/";
        private string Old_location;
        private MEETING meetingSS = null;
        private MEETING meetingEdit = null;
        private void GetMeeting()
        {
            if (Session["Meeting"] != null)
                meetingSS = Session["Meeting"] as MEETING;
            else
            {
                meetingSS = new MEETING();
                Session["Meeting"] = meetingSS;
            }
            if (Session["MeetingEdit"] != null)
                meetingEdit = Session["MeetingEdit"] as MEETING;
            else
            {
                meetingEdit = new MEETING();
                Session["MeetingEdit"] = meetingEdit;
            }
        }
        private void GetEditMeeting(int id)
        {

            if (Session["MeetingEdit"] != null)
                meetingEdit = Session["MeetingEdit"] as MEETING;
            else
            {
                meetingEdit = db.MEETINGs.Find(id);
                Session["MeetingEdit"] = meetingEdit;
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
            GetEditMeeting(id);
            ViewBag.modify = modify;
            ViewBag.user_identity = User.Identity.GetUserId();
            CATEGORY category = db.CATEGORies.Find(meetingEdit.Category_id);
            AspNetUser user = db.AspNetUsers.Find(meetingEdit.Create_by);
            meetingEdit.AspNetUser = user;
            meetingEdit.CATEGORY = category;
            return View(meetingEdit);
        }

        [HttpGet]
        public ActionResult CancelModel(MEETING model, int model_type)
        {
            GetMeeting();
            ViewBag.model_type = model_type;
            model.MEMBERs = meetingEdit.MEMBERs;
            Session["MeetingEdit"] = model;
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult CancelModel(int meeting_id, MEETING model, int model_type)
        {
            var meeting = db.MEETINGs.Find(meeting_id);
            if (model_type == 1)
            {
                meeting.Status = 7;
                meeting.Feedback = model.Feedback;
            }
            else
            {
                meeting.Feedback = model.Feedback;

            }

            db.Entry(meeting).State = EntityState.Modified;
            db.SaveChanges();
            sendCancelModel(meeting);
            return RedirectToAction("Index");
        }

        private void sendCancelModel(MEETING meeting)
        {
            string Receiver = db.AspNetUsers.Find(meeting.Verify_by).Email;
            string Subject = meeting.Meeting_name;
            string Body = MEETING_CANCEL + meeting.Feedback;
            Outlook mail = new Outlook(Receiver, Subject, Body);
            mail.SendMail();
        }

        public ActionResult StatusChanges(int id)
        {
            var meeting = db.MEETINGs.Find(id);
            meeting.Status = 3;
            db.Entry(meeting).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("MeetingDetail", "Meetings", new { id = meeting.Meeting_id, modify = false });
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
            GetEditMeeting(id);
            var current = User.Identity.GetUserId();
            var userList = db.AspNetUsers.Where(x => x.Id != current).Select(selector: x => x.Email).ToList();
            ViewBag.userList = userList;
            return PartialView(meetingEdit);
        }

        [HttpPost]
        public ActionResult MeetingEdit(int meeting_id, MEETING model, HttpPostedFileBase? fileBase)
        {
            var meeting = model;
            if (fileBase != null)
            {
                meeting = updateMeeting(meeting_id, model);

                var filename = Path.GetFileName(fileBase.FileName);
                ATTACHMENT attchment = new ATTACHMENT();
                attchment.Meeting_id = meeting.Meeting_id;
                attchment.Attachment_path = File_Path_Attachment + meeting.Meeting_id + filename;
                attchment.Attachment_name = fileBase.FileName;
                attchment.Attachment_binary = Math.Round(((Double)fileBase.ContentLength / 1024), 2).ToString() + "KB";
                db.ATTACHMENTs.Add(attchment);
                db.SaveChanges();

                var path = Server.MapPath(File_Path_Attachment);
                fileBase.SaveAs(path + meeting.Meeting_id + filename);


            }
            meeting = updateMeeting(meeting_id, model);

            return RedirectToAction("MeetingDetail", new { id = meetingEdit.Meeting_id, modify = true });
        }

        private MEETING updateMeeting(int meeting_id, MEETING model)
        {
            GetMeeting();
            var meeting = db.MEETINGs.Find(meeting_id);

            meeting.Meeting_name = model.Meeting_name;
            if (meeting.Date_Start != model.Date_Start || meeting.Time_Start != model.Time_Start)
            {
                meeting.Date_Start = model.Date_Start;
                meeting.Time_Start = model.Time_Start;
            }
            if (meeting.Location != model.Location)
            {
                meeting.Location = model.Location;
            }
            meeting.Meeting_content = model.Meeting_content;

            var updateMemberList = meetingEdit.MEMBERs.ToList();
            var currentMemeberList = meeting.MEMBERs.ToList();

            //member user
            if (currentMemeberList.Count > updateMemberList.Count)
            {
                for (int i = 0; i < currentMemeberList.Count; i++)
                {
                    var exists = updateMemberList.Exists(x => x.Member_id == currentMemeberList[i].Member_id);
                    if (!exists)
                    {
                        if (currentMemeberList[i].TASKs.Count != 0)
                        {
                            foreach (var task in currentMemeberList[i].TASKs.ToList())
                            {
                                db.TASKs.Remove(task);
                            }
                        }
                        db.MEMBERs.Remove(currentMemeberList[i]);
                    }
                }
            } //Add member
            else if (currentMemeberList.Count < updateMemberList.Count)
            {
                for (int i = 0; i < updateMemberList.Count; i++)
                {
                    var exists = currentMemeberList.Exists(x => x.Member_id == updateMemberList[i].Member_id);
                    if (!exists)
                    {
                        updateMemberList[i].Meeting_id = meeting_id;
                        db.MEMBERs.Add(updateMemberList[i]);
                        var email = db.AspNetUsers.Find(updateMemberList[i].Member_id).Email;
                        sendMailToMember(model, email);
                    }
                }
            }//Case User add member and remove member at the same time
            else
            {
                for (int i = 0; i < currentMemeberList.Count; i++)
                {
                    for (int j = 0; j < updateMemberList.Count; j++)
                    {
                        var currExists = currentMemeberList.Exists(x => x.Member_id == updateMemberList[j].Member_id);
                        var updateExists = updateMemberList.Exists(x => x.Member_id == currentMemeberList[i].Member_id);
                        if (!currExists && !updateExists)
                        {
                            if (currentMemeberList[i].TASKs.Count != 0)
                            {
                                foreach (var task in currentMemeberList[i].TASKs.ToList())
                                {
                                    db.TASKs.Remove(task);
                                }
                            }
                            db.MEMBERs.Remove(currentMemeberList[i]);

                            updateMemberList[i].Meeting_id = meeting_id;
                            db.MEMBERs.Add(updateMemberList[j]);

                            var email = db.AspNetUsers.Find(updateMemberList[i].Member_id).Email;
                            sendMailToMember(model, email);
                        }
                    }
                }
            }

            db.Entry(meeting).State = EntityState.Modified;
            db.SaveChanges();
            Session["MeetingEdit"] = meeting;
            if (meeting.Date_Start != model.Date_Start || meeting.Time_Start != model.Time_Start)
            {
                sendMailToAllMembers(meeting);
            }
            if (meeting.Location != model.Location)
            {
                sendMailToAllMembers(model);
            }
            return meeting;
        }

        private void sendMailToMember(MEETING meeting, string email)
        {
            string Receiver = email;
            string Subject = meeting.Meeting_name;
            string DateTime = meeting.Date_Start.ToString("dd/MM/yyyy") + " " + meeting.Time_Start;
            string Body = MEETING_TIME + DateTime + Environment.NewLine + MEETING_LOCATION + meeting.Location;
            Outlook mail = new Outlook(Receiver, Subject, Body);
            mail.SendMail();
        }
        private void sendMailToAllMembers(MEETING meeting)
        {
            var member = from m in db.MEMBERs.Where(m => m.Meeting_id == meeting.Meeting_id)
                         from u in db.AspNetUsers
                         where u.Id == m.Member_id
                         select u;
            foreach (var item in member)
            {
                string Receiver = item.Email;
                string Subject = meeting.Meeting_name;
                string DateTime = meeting.Date_Start.ToString("dd/MM/yyyy") + " " + meeting.Time_Start;
                string Body = MEETING_TIME + DateTime + Environment.NewLine + MEETING_LOCATION + meeting.Location;
                Outlook mail = new Outlook(Receiver, Subject, Body);
                mail.SendMail();
            }
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
            bool check;
            if (current_user == create_by)
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
            if (ReportFile != null)
            {
                using (var scope = new TransactionScope())
                {
                    if (ValidateFile(ReportFile))
                    {
                        REPORT report = new REPORT();
                        report.Meeting_id = Meeting_id;
                        report.Report_name = ReportFile.FileName;
                        report.Report_binary = Math.Round(((Double)ReportFile.ContentLength / 1024), 2).ToString() + "KB";
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
            return View("MeetingDetail", db.MEETINGs.Find(Meeting_id));
        }
        /*----------Meeting Report-------------*/

        public ActionResult Delete_Attachment(int id)
        {
            ATTACHMENT attachment = db.ATTACHMENTs.Find(id);
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
            var notpending_meetings = meetings.Where(n => n.Status > 1);
            return PartialView("JoinedMeetingGridView", notpending_meetings);
        }

        public ActionResult PendingMeeting()
        {
            currentUser = User.Identity.GetUserId();
            ViewBag.user_identity = currentUser;
            var meetings = db.MEETINGs.ToList();
            meetings = meetings.Where(m => m.Create_by.ToLower().Contains(currentUser)).ToList();
            return PartialView(meetings);
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
            var userList = db.AspNetUsers.Where(x => x.Id != current).Select(selector: x => x.Email).ToList();

            meetingSS.Category_id = id;
            ViewBag.userList = userList;
            return View(meetingSS);
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
                            newAtt.Attachment_binary = Math.Round(((Double)Files.ContentLength / 1024), 2).ToString() + "KB";
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
            model.MEMBERs = meetingSS.MEMBERs;
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
                                                && x.Date_Start == newMeet.Date_Start
                                                && x.Time_Start == newMeet.Time_Start
                                                && x.Date_Create == newMeet.Date_Create).FirstOrDefault();
            foreach (var member in meetingSS.MEMBERs.ToList())
            {
                member.Meeting_id = newMeeting.Meeting_id;
                db.MEMBERs.Add(member);
                db.SaveChanges();
            }
            meetingSS = new MEETING();
            Session["Meeting"] = meetingSS;
        }
        private bool ValidateFile(HttpPostedFileBase files)
        {
            var filesize = files.ContentLength;
            if (filesize > 5 * 1024 * 1024)
                return false;
            return true;
        }



        [HttpGet]
        public ActionResult AddUser(MEETING model, string email)
        {
            GetMeeting();
            bool checkExist = false;
            AspNetUser user = db.AspNetUsers.SingleOrDefault(x => x.Email == email);
            foreach (var m in meetingSS.MEMBERs.ToList())
            {
                if (m.Member_id == user.Id)
                    checkExist = true;
            }
            if (checkExist != true)
            {
                MEMBER member = new MEMBER();
                member.Member_id = user.Id;

                if (meetingSS.MEMBERs.Count != 0)
                {
                    model.MEMBERs = meetingSS.MEMBERs;
                }
                model.MEMBERs.Add(member);

            }
            meetingSS = model;
            Session["Meeting"] = meetingSS;
            return RedirectToAction("MeetingForm", new { id = model.Category_id });
        }
        public ActionResult AddEditUser(MEETING model, string email)
        {
            GetMeeting();
            bool checkExist = false;
            AspNetUser user = db.AspNetUsers.SingleOrDefault(x => x.Email == email);
            foreach (var m in meetingEdit.MEMBERs.ToList())
            {
                if (m.Member_id == user.Id)
                {
                    checkExist = true;
                    model.MEMBERs = meetingEdit.MEMBERs;
                }
            }
            if (checkExist != true)
            {
                MEMBER member = new MEMBER();
                member.Member_id = user.Id;

                if (meetingEdit.MEMBERs.Count != 0)
                {
                    model.MEMBERs = meetingEdit.MEMBERs;
                }
                model.MEMBERs.Add(member);

            }
            CATEGORY category = db.CATEGORies.Find(model.Category_id);
            model.CATEGORY = category;
            MEETING_STATUS status = db.MEETING_STATUS.Find(model.Status);
            model.MEETING_STATUS = status;
            meetingEdit = model;
            Session["MeetingEdit"] = meetingEdit;
            return RedirectToAction("MeetingDetail", new { id = meetingEdit.Meeting_id, modify = true });
        }
        public ActionResult RemoveEditUser(string userId)
        {
            GetMeeting();
            foreach (var member in meetingEdit.MEMBERs.ToList())
            {
                if (member.Member_id == userId)
                {
                    meetingEdit.MEMBERs.Remove(member);
                }
            }
            Session["MeetingEdit"] = meetingEdit;
            return RedirectToAction("MeetingDetail", new { id = meetingEdit.Meeting_id, modify = true });
        }
        public ActionResult RemoveUser(string userId)
        {
            GetMeeting();
            foreach (var member in meetingSS.MEMBERs.ToList())
            {
                if (member.Member_id == userId)
                {
                    meetingSS.MEMBERs.Remove(member);
                }
            }
            Session["Meeting"] = meetingSS;
            return RedirectToAction("MeetingForm", new { id = meetingSS.Category_id });
        }
        public ActionResult CreateUser2()
        {
            return View();
        }


    }
}
