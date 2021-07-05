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
        public ActionResult IndexTaskList()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult IndexCateList()
        {
            var cATEGORies = db.CATEGORies.ToList();
            return PartialView("IndexCateList", cATEGORies);
        }


        public ActionResult IndexCateDetail(int? id)
        {
            var cateDetail = db.CATEGORies.Find(id);
            if (cateDetail == null)
            {
                return HttpNotFound();
            }
            return View(cateDetail);
        }

        public ActionResult IndexMeetingList()
        {
            List<MEETING> meeting = db.MEETINGs.ToList();
            MeetingListVM meetingListVM = new MeetingListVM();
            List<MeetingListVM> meetingListVMList = meeting.Select(x => new MeetingListVM
            {
                FullName = x.AspNetUser.Full_name,
                MeetingName = x.Meeting_name,
                DateStart = x.Date_Start,
                Status = x.MEETING_STATUS.Status_id,
                StatusName = x.MEETING_STATUS.Status_name
            }).ToList();
            return View(meetingListVMList);
        }
/*        public ActionResult MyMeeting()
        {
            currentUser = User.Identity.GetUserId();
            var result = db.MEETINGs.ToList();
            result = result.Where(m => m.Create_by.ToLower().Contains(currentUser)).ToList();
            return PartialView("MyMeetingGridView", result);
        }

        public ActionResult JoinedMeeting()
        {
            currentUser = User.Identity.GetUserId();
            var meetings = from u in db.MEMBERs.Where(m => m.Member_id.ToLower().Contains(currentUser))
                           from m in db.MEETINGs
                           where u.Meeting_id == m.Meeting_id
                           select m;
            return PartialView("JoinedMeetingGridView", meetings);
        }*/
    
    }
}
    
