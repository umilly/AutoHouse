
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
    
public partial class DeviceParameterTypeLink
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public DeviceParameterTypeLink()
    {

        this.ComandParameterLinks = new HashSet<ComandParameterLink>();

    }


    public int Id { get; set; }

    public string Name { get; set; }

    public int Order { get; set; }



    public virtual CustomDevice CustomDevice { get; set; }

    public virtual ParameterType ParameterType { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<ComandParameterLink> ComandParameterLinks { get; set; }

}

}
