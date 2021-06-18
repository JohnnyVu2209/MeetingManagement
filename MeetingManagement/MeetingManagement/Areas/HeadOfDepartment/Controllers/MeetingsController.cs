using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    
    public class MeetingsController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: HeadOfDepartment/Meetings
        public ActionResult Index()
        {          
            return View(db.MEETINGs.ToList());
        }

        // GET: HeadOfDepartment/Meetings/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HeadOfDepartment/Meetings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HeadOfDepartment/Meetings/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: HeadOfDepartment/Meetings/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HeadOfDepartment/Meetings/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: HeadOfDepartment/Meetings/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HeadOfDepartment/Meetings/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
