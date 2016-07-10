using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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
    public partial class Parametеr : IHaveID
    {
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

    public partial class Models
    {
        private static Dictionary<string, string> _connectinsStrings = new Dictionary<string, string>();
        static Models()
        {
            _connectinsStrings["vlad"] =
                @"Data Source=UMILLY;Initial Catalog=house;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;App=EntityFramework";
            _connectinsStrings["tima"] =
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\TIMA\Documents\house.mdf;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;App=EntityFramework";
        }

        public Models(string connectionString) : base(BuildEntityConnectionStringFromAppSettings(connectionString))
        {
            var migration=new MigrationService(this);
            migration.Migrate();
        }

        public static string BuildEntityConnectionStringFromAppSettings(string nameOfConnectionString)
        {
            var shortConnectionString = _connectinsStrings[nameOfConnectionString];

            // Specify the provider name, server and database. 
            string providerName = "System.Data.SqlClient";

            // Initialize the connection string builder for the 
            // underlying provider taking the short connection string.
            SqlConnectionStringBuilder sqlBuilder =
                new SqlConnectionStringBuilder(shortConnectionString);

            // Set the properties for the data source.
            sqlBuilder.IntegratedSecurity = true;

            // Build the SqlConnection connection string. 
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
                new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata =
                String.Format(
                    "res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl",
                    "ApplicationDB");
            return entityBuilder.ToString();
        }

        public class MigrationService
        {
            private Models _context;
            private static List<Update> _updates = new List<Update>();
            public MigrationService(Models context)
            {
                _context = context;
                RegiterUpdate(Guid.Parse("6DA1FC41-A02D-40EC-82AF-7489C8E62CE4"), @"", FillSencorTypeDict);
                RegiterUpdate(Guid.Parse("E00785CC-F6D3-4A28-AA04-8B601AAD9015"), @"", FillZone);
                RegiterUpdate(Guid.Parse("4501377E-791E-4827-B50A-0909A3CC4ED2"), @"", FillConditionTypes);
                RegiterUpdate(Guid.Parse("4501377E-791E-4827-B50A-0909A3CC4ED2"), @"", FillParameterTypes);
            }

            private void FillParameterTypes()
            {
                _context.ParameterTypes.Add(new ParameterType() { Id = (int) ParameterTypeValue.Bool, Name = "bool"});
                _context.ParameterTypes.Add(new ParameterType() { Id = (int)ParameterTypeValue.Int, Name = "Целое" });
                _context.ParameterTypes.Add(new ParameterType() { Id = (int)ParameterTypeValue.String, Name = "Строка" });
                _context.ParameterTypes.Add(new ParameterType() { Id = (int)ParameterTypeValue.Double, Name = "Десятичное" });
            }

            private void FillConditionTypes()
            {
                _context.ConditionTypes.Add(new ConditionType() { Id = (int) ConditionTypeValue.And, Name = "И"});
                _context.ConditionTypes.Add(new ConditionType() { Id = (int)ConditionTypeValue.Or, Name = "ИЛИ" });
                _context.ConditionTypes.Add(new ConditionType() { Id = (int)ConditionTypeValue.Equal, Name = "=" });
                _context.ConditionTypes.Add(new ConditionType() { Id = (int)ConditionTypeValue.Less, Name = "<" });
                _context.ConditionTypes.Add(new ConditionType() { Id = (int)ConditionTypeValue.More, Name = ">" });
                _context.ConditionTypes.Add(new ConditionType() { Id = (int)ConditionTypeValue.NotEqual, Name = "<>" });
            }

            private void FillZone()
            {
                _context.Zones.Add(new Zone()
                {
                    Id = 1,
                    Name = "Глобальная зона",
                    Description = "Это зона включет общие параметры и устройства"
                });
            }

            private void FillSencorTypeDict()
            {
                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "button",
                    Name = "тактовая или сенсорная кнопка",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_mq",
                    Name = "датчик газа/дыма",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_pir",
                    Name = "датчик движения",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_flame",
                    Name = "датчик огня",
                    MinValue = 0,
                    MaxValue = 1
                });


                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_light",
                    Name = "датчик света",
                    MinValue = 0,
                    MaxValue = 1
                });


                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_temp",
                    Name = "датчик температуры",
                    MinValue = -40,
                    MaxValue = 125
                });


                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_hum",
                    Name = "датчик влажности",
                    MinValue = 0,
                    MaxValue = 100
                });


                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_lum",
                    Name = "датчик уровня освещенности",
                    MinValue = 0,
                    MaxValue = 65535
                });

                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_sound",
                    Name = "датчик звука",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_water",
                    Name = "датчик протечки",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_power",
                    Name = "датчик тока",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Id = 1,
                    Key = "sens_bar",
                    Name = "датчик атмосферного давления",
                    MinValue = 300,
                    MaxValue = 1100
                });
            }

            private void RegiterUpdate(Guid guid, string sql, Action fillData=null)
            {
                _updates.Add(new Update() {ID = guid,Order = _updates.Count,SqlScript = sql,FillData = fillData});
            }

            public void Migrate()
            {
                if (!_context.DBVersions.Any())
                {
                    var version = _context.DBVersions.Create();
                    version.Version = GetLastUpdateId();
                    _context.DBVersions.Add(version);
                    _updates.OrderBy(a => a.Order).ToList().ForEach(update => { update.FillData?.Invoke(); });
                    _context.SaveChanges();
                    return;
                }
                var currentVersion = _context.DBVersions.Single().Version;
                var lastUpdateOrder = _updates.First(a => a.ID == currentVersion).Order;
                ApplyScripts(lastUpdateOrder);

            }

            private void ApplyScripts(int lastUpdateOrder)
            {
                var scripts = _updates.Where(a => a.Order > lastUpdateOrder).OrderBy(a=>a.Order);

                foreach (var script in scripts)
                {
                    if (!string.IsNullOrEmpty(script.SqlScript))
                    {
                        _context.Database.ExecuteSqlCommand(script.SqlScript);
                    }
                    script.FillData?.Invoke();
                    _context.DBVersions.Single().Version = script.ID;
                    _context.SaveChanges();
                }

            }

            private Guid GetLastUpdateId()
            {
                return _updates.OrderBy(a => a.Order).Last().ID;
            }
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
        [TypeAssociation(typeof(double))]
        Double = 4,
    }

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
}
