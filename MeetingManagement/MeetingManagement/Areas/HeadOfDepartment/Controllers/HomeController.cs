using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    public class HomeController : Controller
    {
        // GET: HeadOfDepartment/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}