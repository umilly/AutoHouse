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
    
    public partial class Sensor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sensor()
        {
            this.Id = 0;
            this.ControllerId = 0;
            this.ContollerSlot = 0;
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int ControllerId { get; set; }
        public int SensorTypeId { get; set; }
        public int ContollerSlot { get; set; }
    
        public virtual Controller Controller { get; set; }
        public virtual SensorType SensorType { get; set; }
    }
}