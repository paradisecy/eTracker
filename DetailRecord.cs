//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eTracker
{
    using System;
    using System.Collections.Generic;
    
    public partial class DetailRecord
    {
        public int Id { get; set; }
        public Nullable<int> RecordId { get; set; }
        public string UnknownWord { get; set; }
        public string TranslatedWords { get; set; }
        public string SuggestedSynonisms { get; set; }
        public string SelectedSynonism { get; set; }
        public Nullable<int> CurrentPageNo { get; set; }
        public Nullable<System.DateTime> TimeStamp { get; set; }
    
        public virtual Record Record { get; set; }
    }
}
