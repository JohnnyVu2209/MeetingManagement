using MeetingManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    public class MainPageController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: HeadOfDepartment/Home
        public ActionResult Index()
        {
            var model = db.MEETINGs.ToList();
            return View(model);
        }
    }
}