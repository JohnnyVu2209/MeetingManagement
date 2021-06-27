using MeetingManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    [Authorize]
    public class MainPageController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: HeadOfDepartment/Home
        public ActionResult Index()
        {
            var model = db.MEETINGs.ToList();
            return View(model);
        }
        public PartialViewResult Create()
        {
            var model = db.MEETINGs.ToList();
            return PartialView(model);
        }

        //public PartialViewResult _MeetingInfo(int meetingID)
        //{
        //    MEETING mEETING = db.MEETINGs.Find(meetingID);
        //    return PartialView(mEETING);
        //}

        

        public PartialViewResult GrdMemberTable(int id)
        {
            var member = db.MEMBERs.Where(m => m.Meeting_id == id).ToList();
            return PartialView(member);
        }

        [HttpGet]
        public ActionResult VerifyMeeting(int id)
        {
            MEETING meeting = db.MEETINGs.Find(id);
            return View(meeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyMeeting(MEETING meeting)
        {
            db.Entry(meeting).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //private void AddErrors(object result)
        //{
        //    throw new NotImplementedException();
        //}

        //public PartialViewResult GetMeetings()
        //{
        //    var model = db.MEETINGs.ToList();
        //    return PartialView("grdMeetingView", model);
        //}
    }
}