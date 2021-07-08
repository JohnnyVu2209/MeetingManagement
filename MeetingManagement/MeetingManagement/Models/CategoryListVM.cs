using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingManagement.Models
{
    public class CategoryListVM
    {
        public int CateId { get; set; }
        public String CateName { get; set; }
        public String CateContent { get; set; }
        public String CateCreateBy { get; set; }
    }
}