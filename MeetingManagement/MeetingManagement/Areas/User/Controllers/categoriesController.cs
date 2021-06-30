using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingManagement.Models;

namespace MeetingManagement.Areas.User.Controllers
{
    public class CategoriesController : Controller
    {
        private SEP24Team7Entities db = new SEP24Team7Entities();
        // GET: User/categories
        public ActionResult Index()
        {
            var cATEGORies = db.CATEGORies.ToList();
            return View(cATEGORies);
        }
        public ActionResult CategoriesListDetail(int id)
        {
            List<CATEGORY> cate = db.CATEGORies.ToList();
            CategoryListVM categoryListVM = new CategoryListVM();
            List<CategoryListVM> categoryListVMList = cate.Select(x => new CategoryListVM
            {
                CateContent = x.Category_Content,
                CateId = x.Category_id,
                CateName = x.Category_Name,
                CateCreateBy = x.Create_by
            }).ToList();
            return View(categoryListVMList);
        }

    }

}