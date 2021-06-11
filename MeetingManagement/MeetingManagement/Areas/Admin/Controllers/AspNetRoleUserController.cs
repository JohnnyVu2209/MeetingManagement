using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;

namespace MeetingManagement.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AspNetRoleUserController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();

        // GET: Admin/AspNetUsers/Create
        public ActionResult Create(string UserID)
        {
            Console.Write(UserID);
            ViewBag.Users = db.AspNetUsers.Find(UserID);
            ViewBag.Roles = new SelectList(db.AspNetRoles, "Id", "Name");
            return View();
        }

        // POST: Admin/AspNetUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string UserID, string RoleID)
        {
            var role = db.AspNetRoles.Find(RoleID);
            var user = db.AspNetUsers.Find(UserID);

            user.AspNetRoles.Add(role);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "AspNetUsers");
        }

        // GET: Admin/AspNetUsers/Delete/5
        public ActionResult Delete(string roleId, string userId)
        {
            var role = db.AspNetRoles.Find(roleId);
            var user = db.AspNetUsers.Find(userId);

            user.AspNetRoles.Remove(role);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "AspNetUsers");
        }

        // POST: Admin/AspNetUsers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    AspNetUser aspNetUser = db.AspNetUsers.Find(id);
        //    db.AspNetUsers.Remove(aspNetUser);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public List<AspNetUser> GetUsers()
        {
            List<AspNetUser> users = new List<AspNetUser>();
            users = db.AspNetUsers.ToList();
            return users;
        }

        public List<AspNetRole> GetRoles()
        {
            List<AspNetRole> roles = db.AspNetRoles.ToList();
            return roles;
        }
    }
}
