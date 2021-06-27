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

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    public class MEETINGsController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();

        public ActionResult MeetingForm()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MeetingForm(MEETING model, HttpPostedFileBase[] files)
        {
            if (ModelState.IsValid)
            {
                if (files != null)
                {
                    using (var scope = new TransactionScope())
                    {

                        db.MEETINGs.Add(model);
                        db.SaveChanges();

                        //store file
                        foreach(HttpPostedFileBase file in files)
                        {
                            var path = Server.MapPath(File_Path);
                            file.SaveAs(path + file.FileName);
                        }
                    }

                    return RedirectToAction("Index");
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
    }
}
