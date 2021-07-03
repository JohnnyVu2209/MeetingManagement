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
    [Authorize(Roles = "BCN")]
    public class MainPageController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: HeadOfDepartment/Home
        public ActionResult Index()
        {
            var model = db.MEETINGs.ToList();
            return View(model);
        }

        public PartialViewResult GrdMemberTable(int id)
        {
            var member = db.MEMBERs.Where(m => m.Meeting_id == id).ToList();
            return PartialView(member);
        }

        public ActionResult VerifyMeeting(int id)
        {
            MEETING meeting = db.MEETINGs.Find(id);
            return View(meeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyMeeting(int category_id, int meeting_id, string meeting_name, DateTime date_start, TimeSpan time_start, string location, string content, int status)
        {
            MEETING meeting = db.MEETINGs.Find(meeting_id);
            meeting.Category_id = category_id;
            meeting.Meeting_name = meeting_name;
            meeting.Date_Start = date_start;
            meeting.Time_Start = time_start;
            meeting.Location = location;
            meeting.Meeting_content = content;
            meeting.Status = status;

            db.Entry(meeting).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        
        //public PartialViewResult GrdOtherTable()
        //{
        //    var member = db.OTHER_ACCOUNTs.ToList();
        //    return PartialView(member);
        //}

        public PartialViewResult Feedback(int id)
        {
            var meeting = db.MEETINGs.Find(id);
            return PartialView(meeting);
        }

        [HttpPost]
        public ActionResult Feedback(int meeting_id, string feedback, int status)
        {
            var meeting = db.MEETINGs.Find(meeting_id);
            meeting.Status = 6;
            meeting.Feedback = feedback;

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