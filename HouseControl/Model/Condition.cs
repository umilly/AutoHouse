//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Condition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ConditionTypeId { get; set; }
        public Nullable<int> ReactionId { get; set; }
    
        public virtual ConditionType ConditionType { get; set; }
        public virtual Reaction Reaction { get; set; }
    }
}
