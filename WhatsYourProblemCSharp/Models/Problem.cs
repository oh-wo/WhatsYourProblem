//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WhatsYourProblemCSharp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Problem
    {
        public Problem()
        {
            this.ChatComments = new HashSet<ChatComment>();
        }
    
        public System.Guid ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Nullable<System.DateTime> PostedDate { get; set; }
        public Nullable<System.Guid> PUserID { get; set; }
        public Nullable<int> Rating { get; set; }
    
        public virtual ICollection<ChatComment> ChatComments { get; set; }
        public virtual PUser PUser { get; set; }
    }
}