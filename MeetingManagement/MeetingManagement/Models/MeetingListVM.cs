using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingManagement.Models
{
    public class MeetingListVM
    {
        public String FullName { get; set; }
        public String MeetingName { get; set; }
        public DateTime DateStart  { get; set; }
        public int Status  { get; set; }
        public String StatusName  { get; set; }
        public string Email { get; internal set; }
        public int CateID { get; set; }
    }
}