using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;
using Microsoft.AspNet.Identity;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    [Authorize(Roles = "BCN")]
    public class tabCuocHopController : Controller
    {
        SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: HeadOfDepartment/tabCuocHop
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            ViewBag.pendingMeeting = db.MEETINGs.Where(x => x.Status == 1).ToList();
            ViewBag.attendMeeting = from u in db.MEMBERs
                                    from m in db.MEETINGs
                                    where u.Meeting_id == m.Meeting_id && u.Member_id == userID
                                    select m;
            ViewBag.myMeeting = db.MEETINGs.Where(x => x.Create_by == userID).ToList();
            return View();
        }

    }
}