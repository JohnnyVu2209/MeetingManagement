using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    public class MEETINGsController : Controller
    {
        // GET: HeadOfDepartment/MEETINGs
        public ActionResult Index()
        {
            return View();
        }
    }
}