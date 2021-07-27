using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingManagement.Models
{
    public class TasksBCN
    {
        public String FullName { get; set; }
        public String MeetingName { get; set; }
        public DateTime DueDate  { get; set; }
        public Boolean Status { get; set; }
        public String TaskName { get; set; }
        public int Meeting_id { get; set; }
        public int Meeting_idd { get; set; }
    }
}