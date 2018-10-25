using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Facade;

namespace Model
{

    public partial class Controller : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }

    public partial class DeviceParameterTypeLink : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }

    public partial class SensorType : IHaveID
    {
        public int ID { get=>Id; }
    }
    public partial class ComandParameterLink : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class ParameterType : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class Zone : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class Sensor : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class Mode : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class Scenario : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }

    public partial class Reaction : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }

    public partial class Condition : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class Parameter : IHaveID
    {
        public const int CurrentTimeId = 1;
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class ConditionType : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class Command : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class CustomDevice : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class ParameterCategory : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }
    public partial class ParametrSetCommand : IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }

    public class Update
    {
        public int Order { get; set; }
        public Guid ID { get; set; }
        public string SqlScript { get; set; }
        public Action FillData { get; set; }
    }

    public enum ConditionTypeValue
    {
        And=1,
        Or=2,
        Less=3,
        More=4,
        Equal=5,
        NotEqual=6
    }
    public enum ParameterTypeValue
    {
        [TypeAssociation(typeof(bool))]
        Bool = 1,
        [TypeAssociation(typeof(int))]
        Int = 2,
        [TypeAssociation(typeof(string))]
        String = 3,
        [TypeAssociation(typeof(float))]
        Float = 4,
        [TypeAssociation(typeof(DateTime))]
        Time = 5,
    }

    //public enum Category
    //{
    //    Light=1,
    //    Climat=2,
    //    Door=3,
    //    Media=4,
    //    Control=5
    //}
    public class TypeAssociationAttribute : Attribute
    {
        private readonly Type _type;
        private static readonly Dictionary<ParameterTypeValue,Type> TypeMap=new Dictionary<ParameterTypeValue, Type>();
        public TypeAssociationAttribute(Type type)
        {
            _type = type;
        }

        public static Type GetType(ParameterTypeValue member)
        {
            if (TypeMap.ContainsKey(member))
                return TypeMap[member];
            var t = typeof (ParameterTypeValue);
            var res = t.GetMembers(BindingFlags.Public | BindingFlags.Static).First(a => ((ParameterTypeValue)Enum.Parse(t, a.Name)).Equals(member));
            TypeMap[member] = ((TypeAssociationAttribute)res.GetCustomAttributes(typeof(TypeAssociationAttribute), true).Single())._type;
            return TypeMap[member];
        }
    }

    [DataContract]
    public class ModeProxy
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public bool IsSelected { get; set; }

        public static ModeProxy FromDBMode(Mode mode)
        {
            return new ModeProxy() {ID = mode.ID,Name = mode.Name,IsSelected = mode.IsSelected};
        }
    }
    [DataContract]
    public class ZoneProxy
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ID { get; set; }
        public static ZoneProxy FromDBZone(Zone zone)
        {
            return new ZoneProxy() { ID = zone.ID, Name = zone.Name};
        }
    }

    [DataContract]
    public class CategoryProxy
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ID { get; set; }
    }

    [DataContract]
    public class ParameterProxy
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string ButtonDescription { get; set; }
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int NextParam { get; set; }
        [DataMember]
        public ParameterTypeValue ParamType { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public CategoryProxy Category{ get; set; }
        [DataMember]
        public SensorProxy Sensor { get; set; }
        [DataMember]
        public string ActualValue { get; set; }
        public static ParameterProxy FromDBParameter(Parameter parameter)
        {
            SensorProxy sensor = null;
            if (parameter.Sensor != null)
            {
                sensor=new SensorProxy()
                {
                    ID = parameter.Sensor.ID,
                    Name = parameter.Sensor.Name,
                    ValueType = (ParameterTypeValue)parameter.Sensor.SensorType.Id,
                    Zone = new ZoneProxy() { ID = parameter.Sensor.Zone.ID,Name = parameter.Sensor.Zone.Name},
                    MinValue=parameter.Sensor.SensorType.MinValue,
                    MaxValue = parameter.Sensor.SensorType.MaxValue

                    
                };
            }
            return new ParameterProxy()
            {
                ID = parameter.ID,
                Name = parameter.Name,
                NextParam = parameter.NextParameter?.ID ?? -1,
                ParamType = (ParameterTypeValue) parameter.ParameterTypeId,
                Value = parameter.Value,
                Category = parameter.ParameterCategory == null ? null : new CategoryProxy() {ID =  parameter.ParameterCategory.ID,Name = parameter.ParameterCategory.Name } ,
                Sensor = sensor,
                Description = parameter.Description,
                ButtonDescription= parameter.ButtonDescription
            };
        }
    }

    [DataContract]
    public class SensorProxy
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public ParameterTypeValue ValueType { get; set; }
        [DataMember]
        public int MinValue { get; set; }
        [DataMember]
        public int MaxValue { get; set; }
        [DataMember]
        public ZoneProxy Zone { get; set; }

    }
    [DataContract]
    public class Modes
    {
        [DataMember]
        public List<ModeProxy> ModeList { get; set; }
    }
    [DataContract]
    public class Parameters
    {
        [DataMember]
        public List<ParameterProxy> ParamList { get; set; }
    }

    
}
