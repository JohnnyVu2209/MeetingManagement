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
        public ActionResult MeetingList(int id)
        {
            var all = db.MEETINGs.Where(x => x.Category_id == id).ToList();
            return PartialView(all);
        }


        
        public ActionResult MeetingForm(int id)
        {
            MEETING newMeet = new MEETING();
            CATEGORY cate = db.CATEGORies.Find(id);
            if (cate == null)
            {
                throw new Exception("Category Not Exist");
            }
            newMeet.Category_id = id;
            return View(newMeet);
        }

        [HttpPost]
        public ActionResult MeetingForm(MEETING model, HttpPostedFileBase Files)
        {
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
                            newAtt.Meeting_id = meetings.Meeting_id;
                            newAtt.Attachment_path = File_Path + meetings.Meeting_id;
                            newAtt.Attachment_name = Files.FileName;
                            db.ATTACHMENTs.Add(newAtt);
                            db.SaveChanges();

                            //store link file to db
                            var path = Server.MapPath(File_Path);
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
        public ActionResult CreateUser()
        {
            var userId = User.Identity.GetUserId();
            List<AspNetUser> model = db.AspNetUsers.Where(x => x.Id != userId).ToList();
            ViewBag.result = model;
            return View();
        }
        public ActionResult CreateUser2()
        {
            return View();
        }


        
    }
}
