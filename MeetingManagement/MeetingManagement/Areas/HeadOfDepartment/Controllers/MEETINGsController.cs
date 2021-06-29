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

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    public class MEETINGsController : Controller
    {
        private int cateid = 0;
        private SEP24Team7Entities db = new SEP24Team7Entities();
        public ActionResult MeetingList(int id)
        {
            var all = db.MEETINGs.Where(x => x.Category_id == id).ToList();
            return PartialView(all);
        }
        public ActionResult MeetingForm(int id)
        {
            MEETING newMeet = new MEETING();
            newMeet.Category_id = id;
            cateid = id;
            return View(newMeet);
        }
        public ActionResult MeetingFormUser(List<AspNetUser> list_user)
        {
            MEETING newMeet = new MEETING();
            newMeet.Category_id = cateid;
            List<MEMBER> member = new List<MEMBER>();
            foreach (var u in list_user)
            {
                MEMBER mem = new MEMBER();
                mem.Member_id = u.Id;
                member.Add(mem);
            }
            ViewBag.Member = member;
            return View("MeetingForm", newMeet);
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
                        newMeet.Status = 2;
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
                        newAtt.Attachment_path = path + Files.FileName;
                        db.ATTACHMENTs.Add(newAtt);
                        db.SaveChanges();
                        //}
                        string[] users = model.AspNetUsers.Split(',');
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
            return View();
        }
        [HttpPost]
        public ActionResult CreateUser(string[] listuser)
        {
            List<AspNetUser> model = new List<AspNetUser>();
            foreach (var users in listuser)
            {
                AspNetUser user = db.AspNetUsers.Where(x => x.Email == users).FirstOrDefault();
                model.Add(user);
            }
            MEETING mEETING = new MEETING();
            return PartialView("pickedUser", mEETING);
        }
        public PartialViewResult pickedUser()
        {
            return PartialView();
        }
        public ActionResult CreateUser2()
        {
            return View();
        }
    }
}
