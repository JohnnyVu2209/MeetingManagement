using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;
using EntityState = System.Data.Entity.EntityState;
using Microsoft.AspNet.Identity;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();

        // GET: HeadOfDepartment/categories
        public ActionResult Index()
        {
            var categories = db.CATEGORies.ToList();
            return View(categories);
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
                throw new Exception();
            }
            return View(cATEGORY);

        }

        public ActionResult MeetingList(int id)
        {
            var meetings = db.MEETINGs.Where(x => x.Category_id == id).ToList(); 
            return PartialView(meetings) ;
        }




        // GET: HeadOfDepartment/categories/Create
        //public PartialViewResult Create()
        //{
        //    return PartialView("Create", new Models.CATEGORY());
        //}
        // POST: HeadOfDepartment/categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //public JsonResult Create(CATEGORY cg)
        //{
        //    /*link huong dan de lam phan nay https://www.youtube.com/watch?v=2ktFobQ4VmM */
        //    /*SEP24Team7Entities db = new SEP24Team7Entities();*/
        //    cg.Create_by = HttpContext.User.Identity.GetUserId();
        //    db.CATEGORies.Add(cg);
        //    db.SaveChanges();
        //    return Json(cg, JsonRequestBehavior.AllowGet);
        //}

        // GET: HeadOfDepartment/categories/Edit/5
        //[HttpGet]
        //public PartialViewResult Edit(int categoryID)
        //{
        //    CATEGORY cate = db.CATEGORies.Find(categoryID);
        //    CATEGORY category = new CATEGORY();
        //    category.Category_id = cate.Category_id;
        //    category.Category_Name = cate.Category_Name;
        //    category.Category_Content = cate.Category_Content;
        //    category.Create_by = cate.Create_by;
        //    return PartialView(category);
        //}
        //[HttpPost]
        //public ActionResult Edit(CATEGORY cg)
        //{
        //   /* CATEGORY old_cg = db.CATEGORies.Where(x => x.Category_id == cg.Category_id).FirstOrDefault();*/
        //    db.Entry(cg).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //public ActionResult Delete(int id)
        //{
        //    CATEGORY cATEGORY = db.CATEGORies.Where(x => x.Category_id == id).FirstOrDefault();
        //    return PartialView(cATEGORY);

        //}
        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirm(int id)
        //{
        //    CATEGORY cat = db.CATEGORies.Find(id);
        //    if (cat != null) {
        //        db.CATEGORies.Remove(cat);
        //        db.SaveChanges();
        //    }
        //    //link huong dan https://www.youtube.com/watch?v=YQnCkMAYDsQ
        //    return RedirectToAction("Index");

        //}

    }
}