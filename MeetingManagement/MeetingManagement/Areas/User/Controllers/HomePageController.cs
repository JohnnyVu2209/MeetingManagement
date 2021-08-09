using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;
using Microsoft.AspNet.Identity;

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
            var userid = User.Identity.GetUserId();
            var mEMBER = db.MEMBERs.FirstOrDefault(x => x.Member_id == userid);
            ViewBag.meeting = db.MEETINGs.Where(x => x.Meeting_id == mEMBER.Meeting_id).ToList();
            ViewBag.task = db.TASKs.Where(x => x.Meeting_id == mEMBER.Meeting_id && x.Assignee == mEMBER.Member_id).OrderBy(x => x.Task_Status == true).ToList();
            return PartialView();
        }

        public ActionResult IndexTaskList2()
        {
            var userid = User.Identity.GetUserId();
            var mEMBER = db.MEMBERs.FirstOrDefault(x => x.Member_id == userid);
            ViewBag.meeting = db.MEETINGs.Where(x => x.Meeting_id == mEMBER.Meeting_id).ToList();
            ViewBag.task = db.TASKs.Where(x => x.Meeting_id == mEMBER.Meeting_id && x.Assignee == mEMBER.Member_id).OrderBy(x => x.Task_Status == true).ToList();
            ViewBag.myTask = db.TASKs.ToList();
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
            var userID = User.Identity.GetUserId();
            var meeting = (from u in db.MEMBERs
                            from m in db.MEETINGs
                            where u.Meeting_id == m.Meeting_id && u.Member_id == userID
                           select m) as List<MEETING>;
            if(meeting != null)
            {
                foreach (var myMeet in db.MEETINGs.Where(x => x.Create_by == userID))
                {
                    meeting.Add(myMeet);
                }
            }
            else
            {
                meeting = db.MEETINGs.Where(x => x.Create_by == userID).ToList();
            }
            MeetingListVM meetingListVM = new MeetingListVM();
            List<MeetingListVM> meetingListVMList = meeting.Select(x => new MeetingListVM
            {
                Email = x.AspNetUser.Email,
                FullName = x.AspNetUser.Full_name,
                MeetingName = x.Meeting_name,
                DateStart = x.Date_Start,
                Status = x.MEETING_STATUS.Status_id,
                StatusName = x.MEETING_STATUS.Status_name,
                CateID = x.Meeting_id
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
    
