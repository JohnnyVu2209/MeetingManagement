using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Controllers;
using MeetingManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using EntityState = System.Data.Entity.EntityState;

namespace MeetingManagement.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AspNetUsersController : Controller
    {

        private ApplicationUserManager _userManager;
        private SEP24Team7Entities db = new SEP24Team7Entities();
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Admin/AspNetUsers
        public ActionResult Index()
        {
            ViewBag.Role = GetRoles();
            return View(db.AspNetUsers.ToList());
        }

        // GET: Admin/AspNetUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/AspNetUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                AddOtherAccount(user.Id);
                return RedirectToAction("Index", "AspNetUsers");
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("Email", "User with this email already exists");
            return View(model);

        }

        private void AddOtherAccount(string id)
        {
            var user = db.AspNetUsers.Find(id);
            db.OTHER_ACCOUNTs.Add(new OTHER_ACCOUNT
            {
                othUser_id = user.Id,
                othUser_Email = user.Email
            });
            db.SaveChanges();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public PartialViewResult SearchUser(String searchText)
        {
            List<AspNetUser> model = GetUsers();
            var result = model.Where(a => a.UserName.ToLower().Contains(searchText) || a.Email.ToLower().Contains(searchText)).ToList();
            return PartialView("UserGridView", result);
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

        public ActionResult GetOtherUser()
        {
            var otherUser = from u in db.AspNetUsers
                            from o in db.OTHER_ACCOUNTs
                            where u.Id == o.othUser_id
                            select u;
            return PartialView("OtherUserGridView", otherUser);
        }
        [HttpGet]
        public ActionResult ResetPassword(string userID)
        {
            AspNetUser ac = db.AspNetUsers.Find(userID);
            return PartialView(ac);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        public async Task<ActionResult> ResetPassword(string Id, string password)
        {

            if (ModelState.IsValid)
            {
                var user = db.AspNetUsers.Find(Id);
                var result = await UserManager.AddPasswordAsync(user.Id, password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrors(result);
                }

            }

            // If we got this far, something failed, redisplay form
            return View();

        }
        public ActionResult Send()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Send(Outlook gmail)
        {
            gmail.SendMail();
            return View();
        }
    }
}
