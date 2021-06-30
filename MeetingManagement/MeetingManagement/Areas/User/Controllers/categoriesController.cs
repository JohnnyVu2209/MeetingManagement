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
    public class CategoriesController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: User/categories
        public ActionResult Index()
        {
            var cATEGORies = db.CATEGORies.ToList();
            return View(cATEGORies);
        }
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CATEGORY cATEGORY = db.CATEGORies.Find(id);
            if (cATEGORY == null)
            {
                return HttpNotFound();
            }
            return View(cATEGORY);

        }

        public ActionResult MeetingList(int id)
        {
            var UserId = User.Identity.GetUserId();
            var meetings = db.MEETINGs.Where(x => x.Category_id == id && x.Create_by == UserId).ToList();
            return PartialView(meetings);
        }
        
    }
}  