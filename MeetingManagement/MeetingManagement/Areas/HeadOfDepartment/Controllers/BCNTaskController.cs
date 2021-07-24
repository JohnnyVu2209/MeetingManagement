using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;
using Microsoft.AspNet.Identity;


namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{

    [Authorize]

    public class BCNTaskController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        
        // GET: HeadOfDepartment/BCNTask
        /*        public ActionResult Index()
                {
                    List<TASK> taskBCN = db.TASKs.ToList();
                    TasksBCN tasksBCNVM = new TasksBCN();
                    List<TasksBCN> tasksBCNVNList = taskBCN.Select(x => new TasksBCN
                    {
                        FullName = x.MEETING.AspNetUser.Full_name,
                        TaskName = x.Task_name,
                        DueDate = (DateTime)x.Task_Deadline,
                        Status = (bool)x.Task_Status,
                        MeetingName = x.MEETING.Meeting_name,
                        Meeting_id = x.Meeting_id,
                        Meeting_idd = x.MEETING.Meeting_id
                    }).ToList();
                    return View(tasksBCNVNList);

                }*/

        public ActionResult IndexBCNTask()
        {
            var userid = "f28b3bb0-99b7-439e-bc90-4c8c15fac1a2";
            var mEMBER = db.MEMBERs.FirstOrDefault(x => x.Member_id == userid);
            ViewBag.meeting = db.MEETINGs.Where(x => x.Meeting_id == mEMBER.Meeting_id).ToList();
            ViewBag.task = db.TASKs.Where(x => x.Meeting_id == mEMBER.Meeting_id && x.Assignee == mEMBER.Member_id).ToList();
            return View();
        }

        public ActionResult indexTask()
        {           
            
            var userid = "f28b3bb0-99b7-439e-bc90-4c8c15fac1a2";
            var mEMBER = db.MEMBERs.FirstOrDefault(x => x.Member_id == userid);
            ViewBag.allCate = db.CATEGORies.ToList();
            ViewBag.allMeetingStatus = db.MEETING_STATUS.ToList();
            ViewBag.meeting = db.MEETINGs.Where(x => x.Meeting_id == mEMBER.Meeting_id).ToList();
            ViewBag.task = db.TASKs.Where(x => x.Meeting_id == mEMBER.Meeting_id && x.Assignee == mEMBER.Member_id).ToList();
            /*ViewBag.pendingMeeting = db.MEETINGs.Where(x => x.Status == 1).ToList();*/
            ViewBag.allMeeting = db.MEETINGs.ToList();
            /*ViewBag.attendMeeting = from u in db.TASKs
                                    from m in db.MEETINGs
                                    where u.Meeting_id == m.Meeting_id && u.Assignee == userID
                                    select m;*/
            ViewBag.allWork = db.TASKs.ToList();
            return View();
        }
        
       
    }
}