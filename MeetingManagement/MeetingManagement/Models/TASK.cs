//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeetingManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TASK
    {
        public int Task_id { get; set; }
        public int Meeting_id { get; set; }
        public string Task_name { get; set; }
        public string Assignee { get; set; }
        public bool Task_Status { get; set; }
        public Nullable<System.DateTime> Task_Deadline { get; set; }
        public Nullable<bool> Notify { get; set; }
    
        public virtual MEETING MEETING { get; set; }
        public virtual MEMBER MEMBER { get; set; }
    }
}
