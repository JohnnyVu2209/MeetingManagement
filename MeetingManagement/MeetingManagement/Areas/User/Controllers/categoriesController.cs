﻿using System;
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
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Taocuochop()
        {
            return View();
        }
        public ActionResult CreateUser()
        {


            return View();
        }
        public ActionResult CreateUser2()
        {


            return View();
        }
    }
}