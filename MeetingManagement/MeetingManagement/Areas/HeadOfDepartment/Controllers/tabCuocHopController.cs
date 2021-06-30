using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    
    public class tabCuocHopController : Controller
    {
        SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: HeadOfDepartment/tabCuocHop
        public ActionResult Index()
        {           
            return View(db.CATEGORies.ToList());
        }
    }
}