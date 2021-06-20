using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeetingManagement.Areas.User.Controllers
{
    public class HomePageController : Controller
    {
        // GET: User/HomePage
        public ActionResult Index()
        {
            return View();
        }
    }
}