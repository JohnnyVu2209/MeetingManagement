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

        public ActionResult IndexBCNTask()
        {
            var userid = "f28b3bb0-99b7-439e-bc90-4c8c15fac1a2";
            var mEMBER = db.MEMBERs.FirstOrDefault(x => x.Member_id == userid);
            ViewBag.meeting = db.MEETINGs.Where(x => x.Meeting_id == mEMBER.Meeting_id).ToList();
            ViewBag.task = db.TASKs.Where(x => x.Meeting_id == mEMBER.Meeting_id && x.Assignee == mEMBER.Member_id).ToList();
            return View();
        }

        public ActionResult indexPieChart() {
            int ms1 = db.MEETINGs.Where(x => x.Status == 1).Count();
            int ms2 = db.MEETINGs.Where(x => x.Status == 2).Count();
            int ms3 = db.MEETINGs.Where(x => x.Status == 3).Count();
            int ms4 = db.MEETINGs.Where(x => x.Status == 4).Count();
            int ms5 = db.MEETINGs.Where(x => x.Status == 5).Count();
            int ms6 = db.MEETINGs.Where(x => x.Status == 6).Count();
            int ms7 = db.MEETINGs.Where(x => x.Status == 7).Count();
            PieChart pie = new PieChart();
            pie.createM = ms1;
            pie.passM = ms2;
            pie.doneM = ms3;
            pie.reportM = ms4;
            pie.compM = ms5;
            pie.nopassM = ms6;
            pie.cancelM = ms7;
            return Json(pie,JsonRequestBehavior.AllowGet);
        }
        public class PieChart
        {
            public int createM { get; set; }
            public int passM { get; set; }
            public int doneM { get; set; }
            public int reportM { get; set; }
            public int compM { get; set; }
            public int nopassM { get; set; }
            public int cancelM { get; set; }
        }

        public ActionResult indexTask()
        {
            var userid = "f28b3bb0-99b7-439e-bc90-4c8c15fac1a2";
            var mEMBER = db.MEMBERs.FirstOrDefault(x => x.Member_id == userid);
            ViewBag.meeting = db.MEETINGs.Where(x => x.Meeting_id == mEMBER.Meeting_id).ToList();
            ViewBag.task = db.TASKs.Where(x => x.Meeting_id == mEMBER.Meeting_id && x.Assignee == mEMBER.Member_id).ToList();
            ViewBag.allWork = db.TASKs.ToList();
/*            DateTime month07 = Convert.ToDateTime("07/dd/yyyy");*/
            ViewBag.allMeeting = db.MEETINGs.ToList();
            ViewBag.allCate = db.CATEGORies.ToList();
            ViewBag.allMeetingStatus = db.MEETING_STATUS.ToList();
            return View();
        }


       
    }
}