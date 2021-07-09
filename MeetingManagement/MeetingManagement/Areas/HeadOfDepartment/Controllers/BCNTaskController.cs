﻿using System;
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
        public ActionResult Index()
        {
            List<TASK> taskBCN = db.TASKs.ToList();
            TasksBCN taskBCNVM = new TasksBCN();
            List<TasksBCN> taskBCNVMList = taskBCN.Select(x => new TasksBCN
            {
                FullName = x.MEETING.AspNetUser.Full_name,
                MeetingName = x.MEETING.Meeting_name,
                DueDate = (DateTime)x.Task_Deadline,
                Status = (bool)x.Task_Status,
                TaskName = x.Task_name
            }).ToList();
            return View(taskBCNVMList);
        }
    }
}


/*public ActionResult IndexTasks()
{

    var meeting = db.MEETINGs.ToList();
    foreach (var m in meeting)
    {
        m.Meeting_name
                foreach (var t in m.TASKs)
        {
            t.Task_name
                }
    }
    return View();
}
*/    