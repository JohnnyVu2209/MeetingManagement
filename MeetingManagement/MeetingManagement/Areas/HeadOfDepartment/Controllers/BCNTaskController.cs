using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;

namespace MeetingManagement.Areas.HeadOfDepartment.Controllers
{
   
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
            var userid = "a5d77817-16aa-4cc3-bbb2-de403c65bb4e";
            var mEMBER = db.MEMBERs.FirstOrDefault(x => x.Member_id == userid);
            ViewBag.meeting = db.MEETINGs.Where(x => x.Meeting_id == mEMBER.Meeting_id).ToList();
            ViewBag.task = db.TASKs.Where(x => x.Meeting_id == mEMBER.Meeting_id && x.Assignee == mEMBER.Member_id).ToList();
            return View();
        }

        public ActionResult indexTask()
        {
            var task = db.TASKs.ToList();
            return View(task);
        }
        
       
    }
}