using MeetingManagement.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace MeetingManagement.Areas.User.Controllers
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
            newMeet.Category_id = id;
            return View(newMeet);
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
                        newAtt.Attachment_path =File_Path + Files.FileName;
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
        public ActionResult CreateUser2()
        {
            return View();
        }

        //matt meeting list controller roi .- ?
    }
}