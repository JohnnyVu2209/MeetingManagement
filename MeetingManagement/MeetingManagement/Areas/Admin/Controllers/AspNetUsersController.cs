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
    [Authorize(Roles = "Admin")]
    public class AspNetUsersController : Controller
    {

        private ApplicationUserManager _userManager;
        private SEP24Team7Entities db = new SEP24Team7Entities();
        //private ApplicationSignInManager _signInManager;

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

        // GET: Admin/AspNetUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
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
            /* if (ModelState.IsValid)
             {
                 db.AspNetUsers.Add(aspNetUser);
                 db.SaveChanges();
                 return RedirectToAction("Index");
             }
             S
             return View(aspNetUser);*/

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                AddOtherAccount(user.Id);
                return RedirectToAction("Index", "AspNetUsers");
            }
            else
            {
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        private void AddOtherAccount(string id)
        {
            var user = db.AspNetUsers.Find(id);
            db.OTHER_ACCOUNTs.Add(new OTHER_ACCOUNT
            {
                othUser_id = user.Id,
                othUser_office = user.Email
            }) ;
            db.SaveChanges();
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        // GET: Admin/AspNetUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Admin/AspNetUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: Admin/AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Admin/AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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
            /*Password EncryptData = new Password();
            AspNetUser aspNetUser = db.AspNetUsers.Find(Id);
            aspNetUser.PasswordHash = EncryptData.Encode(password);
            db.Entry(aspNetUser).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");*/
            if (ModelState.IsValid)
            {
                var user = db.AspNetUsers.Find(Id);
                var result = await _userManager.AddPasswordAsync(user.Id, password);
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
    }
}
