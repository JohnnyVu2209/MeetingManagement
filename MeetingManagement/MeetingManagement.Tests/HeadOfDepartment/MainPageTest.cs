using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading;
using MeetingManagement.Areas.HeadOfDepartment.Controllers;
using MeetingManagement.Models;

namespace MeetingManagement.Tests.HeadOfDepartment
{
    [TestClass()]
    public class MainPageTest
    {
        [TestMethod()]
        public void IndexTest()
        {
            //Arrange
            var controller = new MainPageController();

            //Act
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<MEETING>;
            Assert.IsNotNull(model);

            var db = new SEP24Team7Entities();
            Assert.AreEqual(db.MEETINGs.Count(), model.Count());

        }

        [TestMethod()]
        public void GrdMemberTableTest()
        {
            var controller = new MainPageController();

            var result = controller.GrdMemberTable(34) as PartialViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<MEMBER>;
            Assert.IsNotNull(model);

            var db = new SEP24Team7Entities();
            var member = db.MEMBERs.Where(m => m.Meeting_id == 34).ToList();
            Assert.AreEqual(member.Count(), model.Count());
        }

        //[TestMethod()]
        //public void VerifyMeetingTest()
        //{
        //    var controller = new MainPageController();

        //    var result = controller.VerifyMeeting(34);
        //    Assert.IsNotNull(result);

        //    //Check old data
        //    var db = new SEP24Team7Entities();
        //    MEETING meeting = db.MEETINGs.Find(34);
        //    Assert.AreEqual(44, meeting.Category_id);
        //    Assert.AreEqual("Dữ liệu test ( đừng thay đổi nha :* )", meeting.Meeting_name);
        //    Assert.AreEqual("Đây là content", meeting.Meeting_content);
        //    Assert.AreEqual("24/06/2021 12:00:00 SA", meeting.Date_Start.ToString());
        //    Assert.AreEqual("08:30:00", meeting.Time_Start.ToString());
        //    Assert.AreEqual("Trường Văn Lang", meeting.Location);
        //    Assert.AreEqual(1,meeting.Status);
        //    Assert.AreEqual("23/05/2021 12:00:00 SA", meeting.Date_Create.ToString());
        //    Assert.AreEqual("7ac6bf6f-7a03-4bea-9e87-d94efc295a62", meeting.Create_by);

        //    //New data 
        //    MEETING new_meeting = new MEETING();
        //    new_meeting.Category_id = 44;
        //    new_meeting.Meeting_id = 34;
        //    new_meeting.Meeting_name = "Đây là dữ liệu Test mới";
        //    new_meeting.Meeting_content = "Đây là content";
        //    new_meeting.Date_Start = DateTime.Parse("24/06/2021");
        //    TimeSpan? Timestart = new TimeSpan(08, 30, 00);
        //    new_meeting.Time_Start = TimeSpan.Zero;
        //    new_meeting.Location = "Trường Văn Lang";
        //    new_meeting.Status = 2;

        //    //Call function
        //    controller.VerifyMeeting(new_meeting.Category_id, new_meeting.Meeting_id,new_meeting.Meeting_name, new_meeting.Date_Start, new_meeting.Time_Start.Value.Add(Timestart.Value), new_meeting.Location, new_meeting.Meeting_content, new_meeting.Status);
        //    db.Entry(meeting).Reload();
        //    //Check new data
        //    Assert.AreEqual("Đây là dữ liệu Test mới", db.MEETINGs.Find(34).Meeting_name);
        //    Assert.AreEqual("Đây là content", db.MEETINGs.Find(34).Meeting_content);
        //    Assert.AreEqual("24/06/2021 12:00:00 SA", db.MEETINGs.Find(34).Date_Start.ToString());
        //    Assert.AreEqual("08:30:00", db.MEETINGs.Find(34).Time_Start.ToString());
        //    Assert.AreEqual("Trường Văn Lang", db.MEETINGs.Find(34).Location);
        //    Assert.AreEqual(2, db.MEETINGs.Find(34).Status);


        ////}
    }
}

