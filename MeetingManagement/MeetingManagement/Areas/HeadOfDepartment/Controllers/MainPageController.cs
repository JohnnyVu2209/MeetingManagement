using MeetingManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
    [Authorize]
    public class MainPageController : Controller
    {
        private string VERIFY_MEETING = "Cuộc họp của bạn đã được duyệt.";
        private string REFUSE_MEETING = "Cuộc họp của bạn đã bị từ chối. Lý do: ";
        private string MEETING_DATE = "Cuộc họp sẽ diễn ra lúc: ";
        private string MEETING_ADDRESS = "Nơi diễn ra cuộc họp: ";

        private const string MEETING_TIME = "Thời gian diễn ra cuộc họp: ";
        private const string MEETING_LOCATION = "Địa điểm diễn ra cuộc họp: ";
        private const string MEETING_CANCEL = "Cuộc họp đã bị huỷ vì lý do: ";
        private const string MEETING_REPORT = "Nhắc nhở nộp báo cáo cho cuộc họp.";

        private SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: HeadOfDepartment/Home
        public ActionResult Index()
        {
            ViewBag.user_identity = User.Identity.GetUserId();
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
        public ActionResult VerifyMeeting(MEETING meeting, int meeting_id)
        {
            if (ModelState.IsValid)
            {
                meeting = db.MEETINGs.Find(meeting_id);
                meeting.Status = 2;
                meeting.Verify_by = User.Identity.GetUserId();
                db.Entry(meeting).State = EntityState.Modified;
                db.SaveChanges();

                string To = db.AspNetUsers.Find(meeting.Create_by).Email;
                string Subject = meeting.Meeting_name;
                string DateTime = meeting.Date_Start.ToString("dd/MM/yyyy") + " " + meeting.Time_Start;
                string Body = VERIFY_MEETING + Environment.NewLine +
                                MEETING_DATE + DateTime + Environment.NewLine +
                                MEETING_ADDRESS + meeting.Location ;

                Outlook mail = new Outlook(To, Subject, Body);
                mail.SendMail();
                sendMailToMembers(meeting);
                return RedirectToAction("Index");
                
            }
            return null;
        }

        private void sendMailToMembers(MEETING meeting)
        {
            var member = from m in db.MEMBERs.Where(m => m.Meeting_id == meeting.Meeting_id)
                         from u in db.AspNetUsers
                         where u.Id == m.Member_id
                         select u;
            foreach (var item in member)
            {
                string Receiver = item.Email;
                string Subject = meeting.Meeting_name;
                string DateTime = meeting.Date_Start.ToString("dd/MM/yyyy") + " " + meeting.Time_Start;
                string Body = MEETING_DATE + DateTime + Environment.NewLine + MEETING_ADDRESS + meeting.Location;
                Outlook mail = new Outlook(Receiver, Subject, Body);
                mail.SendMail();
            }
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

            string To = db.AspNetUsers.Find(meeting.Create_by).Email;
            string Subject = meeting.Meeting_name;
            string Body = REFUSE_MEETING + feedback;
            Outlook mail = new Outlook(To, Subject, Body);
            mail.SendMail();
            return RedirectToAction("Index");
        }

        public ActionResult Notify(int id)
        {
            var meeting = db.MEETINGs.Find(id);
            meeting.Notify = true;
            db.Entry(meeting).State = EntityState.Modified;
            db.SaveChanges();

            string DateTime = meeting.Date_Start.ToString("dd/MM/yyyy") + " " + meeting.Time_Start;
            var Receiver = db.AspNetUsers.Find(meeting.Create_by).Email;
            var Subject = meeting.Meeting_name;
            var Body = MEETING_REPORT + Environment.NewLine + MEETING_TIME + DateTime;
            Outlook mail = new Outlook(Receiver, Subject, Body);
            mail.SendMail();

            return RedirectToAction("Index","MainPage");
        }

        public ActionResult StaticticBCN() {
            return View();

        }
    }
}