using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;

namespace MeetingManagement.Areas.User.Controllers
{
    public class HomePageController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: User/HomePage
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult MeetingList()
        {
            var meetinglist = db.MEETINGs.ToList();
            return PartialView();
        }
        public ActionResult TaskList()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult CategoriesList()
        {
            var cATEGORies = db.CATEGORies.ToList();
            return PartialView("CategoriesList", cATEGORies);
        }

        public ActionResult IndexML() 
        {
            List<MEETING> meeting = db.MEETINGs.ToList();
            MeetingListVM meetingListVM = new MeetingListVM();
            List<MeetingListVM> meetingListVMList = meeting.Select(x => new MeetingListVM 
            { FullName = x.AspNetUser.Full_name, 
                MeetingName = x.Meeting_name, 
                DateStart = x.Date_Start, 
                Status = x.MEETING_STATUS.Status_id,
                StatusName = x.MEETING_STATUS.Status_name }).ToList();
            return View(meetingListVMList);
        }
    }
}