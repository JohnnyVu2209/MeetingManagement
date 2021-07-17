using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    public class StatisticController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: HeadOfDepartment/Statistic
        public ActionResult Index()
        {
            var task = db.TASKs.ToList();
            return View(task);
        }

        public ActionResult statisticTask()
        {
            var task = db.TASKs.ToList();
            return View(task);
        }
    }
}