using MeetingManagement.Models;
using System;
using System.Collections.Generic;
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
        public PartialViewResult _Create()
        {
            var model = db.MEETINGs.ToList();
            return PartialView(model);
        }

        [HttpGet]
        public PartialViewResult _MeetingInfo(int meetingID)
        {
            MEETING mEETING = db.MEETINGs.Find(meetingID);
            return PartialView(mEETING);
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