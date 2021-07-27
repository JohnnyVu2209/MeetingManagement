using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using MeetingManagement.Models;
using MeetingManagement.Areas.HeadOfDepartment.Controllers;
using System.Security.Principal;
using System.IO;

namespace MeetingManagement.Tests.HeadOfDepartment
{
    [TestClass]
    public class MEETINGsController
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        private MeetingManagement.Areas.HeadOfDepartment.Controllers.MEETINGsController controller = new MeetingManagement.Areas.HeadOfDepartment.Controllers.MEETINGsController();
        [TestMethod]
        public void TestMeetingListWithCategoryIdNotExist()
        {
            var controller = new MeetingManagement.Areas.HeadOfDepartment.Controllers.MEETINGsController();
            //id of category which haven't had any meeting
            var id_category = 1;

            var result = controller.MeetingList(id_category) as PartialViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<MEETING>;
            Assert.IsNotNull(model);

            Assert.AreEqual(db.MEETINGs.Where(x => x.Category_id == id_category).Count(), model.Count);
        }
        [TestMethod]
        public void Test_MeetingList_With_CatgoryId_Exist()
        {
            //id of category which have some meeting
            var id_category = 44;

            var result = controller.MeetingList(id_category) as PartialViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<MEETING>;
            Assert.IsNotNull(model);

            Assert.AreEqual(db.MEETINGs.Where(x => x.Category_id == id_category).Count(), model.Count);
        }
        [TestMethod]
        [ExpectedException(typeof(Exception), "Category Not Exist")]
        public void Test_MeetingForm_Error_Is_Throw_If_Category_Id_Not_Exist()
        {
            //id of category which haven't had any meeting
            var id_category = 1;
            var result = controller.MeetingForm(id_category);
        }

        [TestMethod]
        public void Test_MeetingForm_Get()
        {
            var category_id = 44;

            var result = controller.MeetingForm(category_id) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test_MeetingForm_Post_With_Validate_Meeting()
        {
            var rand = new Random();
            var meeting = new MEETING()
            {
                Meeting_name = "My Meeting",
                Category_id = 44,
                Meeting_content = rand.NextDouble().ToString(),
                Date_Create = DateTime.Today,
                Date_Start = new DateTime(2021, 7, 1),
                Time_Start = new TimeSpan(6, 0, 0),
                Status = 2,
                Location = rand.NextDouble().ToString(),
                Create_by = rand.NextDouble().ToString(),
                AspNetUsers = "hung.187pm13932@vanlanguni.vn,khoi@admin.com,thong.187pm06719@vanlanguni.vn"
            };
            var result = controller.MeetingForm(meeting, null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Không được tạo cuộc họp trong cùng ngày hoặc trước đó", controller.ModelState["Date_Start"].Errors[0].ErrorMessage);
            Assert.AreEqual("Không thể mở cuộc họp vào thời gian đó", controller.ModelState["Time_Start"].Errors[0].ErrorMessage);

        }
        [TestMethod]
        public void Test_MeetingForm_Post_With_Validate_Model_And_File_Is_Null()
        {


            var context = new Mock<HttpContextBase>();
            var identity = new GenericIdentity("vu.187pm14039@vanlanguni.vn");
            var principal = new GenericPrincipal(identity, new[] { "user" });
            context.Setup(s => s.User).Returns(principal);

            controller.ControllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);
            
            var rand = new Random();
            var meeting = new MEETING()
            {
                Meeting_id = 1,
                Meeting_name = "My Meeting",
                Category_id = 44,
                Meeting_content = rand.NextDouble().ToString(),
                Date_Create = DateTime.Today,
                Date_Start = new DateTime(2021, 7, DateTime.Today.Day+1),
                Time_Start = new TimeSpan(8, 0, 0),
                Status = 2,
                Location = rand.NextDouble().ToString(),
                AspNetUsers = "hung.187pm13932@vanlanguni.vn,khoi@admin.com,thong.187pm06719@vanlanguni.vn"
            };
            using (var scope = new TransactionScope())
            {
                var result = controller.MeetingForm(meeting, null) as RedirectToRouteResult;
                Assert.IsNotNull(result);
                Assert.AreEqual("Details", result.RouteValues["action"]);
                Assert.AreEqual("Categories", result.RouteValues["controller"]);

            }
        }

        [TestMethod]
        public void Test_MeetingForm_Test_MeetingForm_Post_With_Validate_File_Size_Large_Than_5MB()
        {
            

            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);

            var identity = new GenericIdentity("vu.187pm14039@vanlanguni.vn");
            var principal = new GenericPrincipal(identity, new[] { "user" });
            context.Setup(s => s.User).Returns(principal);

            var attachment = new Mock<HttpPostedFileBase>();
            attachment.Setup(f => f.ContentLength).Returns(6 * 1024 * 1024);
            attachment.Setup(f => f.FileName).Returns("test.txt");

            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);
            var rand = new Random();
            var meeting = new MEETING()
            {
                Meeting_id = 1,
                Meeting_name = "My Meeting",
                Category_id = 44,
                Meeting_content = rand.NextDouble().ToString(),
                Date_Create = DateTime.Today,
                Date_Start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day+1),
                Time_Start = new TimeSpan(8, 0, 0),
                Status = 2,
                Location = rand.NextDouble().ToString(),
                AspNetUsers = "hung.187pm13932@vanlanguni.vn,khoi@admin.com,thong.187pm06719@vanlanguni.vn"
            };

            using (var scope = new TransactionScope())
            {
                var result = controller.MeetingForm(meeting, attachment.Object) as RedirectToRouteResult;
                 Assert.AreEqual("Dung lượng tối đa cho phép là 5MB", controller.ModelState["File"].Errors[0].ErrorMessage);
            }

        }
        [TestMethod]
        public void Test_MeetingForm_Test_MeetingForm_Post_With_Validate_File_Size_Less_Than_5MB()
        {
            

            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);

            var identity = new GenericIdentity("vu.187pm14039@vanlanguni.vn");
            var principal = new GenericPrincipal(identity, new[] { "user" });
            context.Setup(s => s.User).Returns(principal);

            var attachment = new Mock<HttpPostedFileBase>();
            attachment.Setup(f => f.ContentLength).Returns(4 * 1024 * 1024);
            attachment.Setup(f => f.FileName).Returns("test.txt");

            controller.ControllerContext = new ControllerContext(context.Object,
                new System.Web.Routing.RouteData(), controller);

            var filename = String.Empty;
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => s);
            attachment.Setup(p => p.SaveAs(It.IsAny<string>())).Callback<string>(s => filename = s);
            var rand = new Random();
            var meeting = new MEETING()
            {
                Meeting_name = "My Meeting",
                Category_id = 44,
                Meeting_content = rand.NextDouble().ToString(),
                Date_Create = DateTime.Today,
                Date_Start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day + 1),
                Time_Start = new TimeSpan(8, 0, 0),
                Status = 2,
                Location = rand.NextDouble().ToString(),
                AspNetUsers = "hung.187pm13932@vanlanguni.vn,khoi@admin.com,thong.187pm06719@vanlanguni.vn"
            };

            using (var scope = new TransactionScope())
            {
                var result = controller.MeetingForm(meeting, attachment.Object) as RedirectToRouteResult;
                var entity = db.MEETINGs.SingleOrDefault(x => x.Meeting_name == meeting.Meeting_name);
                string extension = Path.GetExtension(attachment.Object.FileName);
                Assert.IsNotNull(result);

                Assert.IsNotNull(entity);
                Assert.AreEqual(meeting.Meeting_name, entity.Meeting_name);
                Assert.AreEqual(meeting.Category_id, entity.Category_id);
                Assert.AreEqual(meeting.Date_Create, entity.Date_Create);
                Assert.AreEqual(meeting.Date_Start, entity.Date_Start);
                Assert.AreEqual(meeting.Status, entity.Status);
                Assert.AreEqual(meeting.Location, entity.Location);

                Assert.IsTrue(filename.StartsWith("~/Upload/Attachments/"));
                Assert.IsTrue(filename.EndsWith(entity.Meeting_id.ToString()+extension));
            }

        }
        [TestMethod]
        public void Test_Create_User_Get()
        {
            var context = new Mock<HttpContextBase>();
            var identity = new GenericIdentity("vu.187pm14039@vanlanguni.vn");
            var principal = new GenericPrincipal(identity, new[] { "user" });
            context.Setup(s => s.User).Returns(principal);

            controller.ControllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);

            var result = controller.CreateUser2() as ViewResult;
            Assert.IsNotNull(result);
        }
    }
}
