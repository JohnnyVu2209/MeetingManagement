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
    
    public partial class ATTACHMENT
    {
        public int Meeting_id { get; set; }
        public string Attachment1 { get; set; }
        public string Attachment_Name { get; set; }
    
        public virtual MEETING MEETING { get; set; }
    }
}