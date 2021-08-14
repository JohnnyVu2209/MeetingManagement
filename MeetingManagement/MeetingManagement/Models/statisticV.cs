using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingManagement.Models
{
    public class statisticV
    {
        public String meetingName { get; set; }
        public DateTime DateCre { get; set; }
        public String meetingHost { get; set; }
        public String meetingLocaion { get; set; }
        public int meetingStatus { get; set; }
        public int meetingId { get; set; }
        public String meetingVerifyBy { get; set; }
    }
}