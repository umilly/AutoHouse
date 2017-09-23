using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Mapping;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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

    public partial class Models
    {
        private static Dictionary<string, string> _connectinsStrings = new Dictionary<string, string>();
        static Models()
        {
        }

        public Models(string connectionString) : base(BuildEntityConnectionStringFromAppSettings(connectionString))
        {

            var migration = new MigrationService(this);
            migration.Migrate();
        }

        public static string BuildEntityConnectionStringFromAppSettings(string nameOfConnectionString)
        {
            if(File.Exists("constr"))
                _connectinsStrings["1"] = File.ReadAllText("constr");
            _connectinsStrings["vlad"] = @"Data Source=UMILLY;Initial Catalog=house;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;App=EntityFramework";
            _connectinsStrings["locFile"] = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\hs\db\house.mdf;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;App=EntityFramework";
            if (!_connectinsStrings.ContainsKey(nameOfConnectionString))
                throw new ArgumentException($"expected '{nameOfConnectionString}' but not found, strings count = {_connectinsStrings.Count} ");
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
                RegiterUpdate(Guid.Parse("DE8C2E66-7A2B-477F-9424-96731DB6C102"), @"", FillParameterTypes);
                RegiterUpdate(Guid.Parse("AABAB4FA-1155-474B-855F-828A4C7C59A7"), @"", FillParameters);
                RegiterUpdate(Guid.Parse("9974EEE0-E905-4BCF-B2C7-AAD0F5F7A3CD"), ParamsCommandUpdate, FillParameterCategories);
                RegiterUpdate(Guid.Parse("9A8F722D-1E46-4924-962B-CCCF1CD8D10F"), AddFields);
                RegiterUpdate(Guid.Parse("AD4874E8-FD62-4F90-BA16-225E10356ABB"), AddParameterChainLink,AdditionalParamCategory);
                RegiterUpdate(Guid.Parse("D8649F0B-A473-4846-9786-3FE1FB993C11"), CreateModbusControllerAndDevices);
                RegiterUpdate(Guid.Parse("08E7A232-440A-451D-B88A-03E3B9F100C0"), CreateParamsDescription);
                RegiterUpdate(Guid.Parse("4C6BFC34-A0DB-4526-AAB2-1FFB285ACE08"), AddInvertField);
            }

            public const string AddInvertField = @"
ALTER TABLE [dbo].[ParametrSetCommands]
ADD  [Invert] bit  NOT NULL DEFAULT(0);
";

            public const string CreateParamsDescription= @"
SET QUOTED_IDENTIFIER OFF;

ALTER TABLE [dbo].[Parameter]
ADD [Description] nvarchar(max)  NULL;
";


            public const string CreateModbusControllerAndDevices = @"
SET QUOTED_IDENTIFIER OFF;
-- Creating table 'Devices_ModBusController'
CREATE TABLE [dbo].[Devices_ModBusController] (
    [ComPort] smallint  NULL,
    [SpeedType] smallint  NULL,
    [Id] int  NOT NULL
);

-- Creating table 'Devices_ModBusDevice'
CREATE TABLE [dbo].[Devices_ModBusDevice] (
    [IsCoil] bit  NULL,
    [Address] smallint  NULL,
    [Id] int  NOT NULL
);

-- Creating primary key on [Id] in table 'Devices_ModBusController'
ALTER TABLE [dbo].[Devices_ModBusController]
ADD CONSTRAINT [PK_Devices_ModBusController]
    PRIMARY KEY CLUSTERED ([Id] ASC);

-- Creating primary key on [Id] in table 'Devices_ModBusDevice'
ALTER TABLE [dbo].[Devices_ModBusDevice]
ADD CONSTRAINT [PK_Devices_ModBusDevice]
    PRIMARY KEY CLUSTERED ([Id] ASC);

-- Creating foreign key on [Id] in table 'Devices_ModBusController'
ALTER TABLE [dbo].[Devices_ModBusController]
ADD CONSTRAINT [FK_ModBusController_inherits_Controller]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Devices_Controller]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;


-- Creating foreign key on [Id] in table 'Devices_ModBusDevice'
ALTER TABLE [dbo].[Devices_ModBusDevice]
ADD CONSTRAINT [FK_ModBusDevice_inherits_CustomDevice]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Devices_CustomDevice]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;


";

            public const string AddParameterChainLink =
                @"
SET QUOTED_IDENTIFIER OFF;

ALTER TABLE [dbo].[Parameter]
ADD [NextParameterId] int  NULL;

ALTER TABLE [dbo].[Parameter]
ADD CONSTRAINT [FK_ParameterParameter]
    FOREIGN KEY ([NextParameterId])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

CREATE INDEX [IX_FK_ParameterParameter]
ON [dbo].[Parameter]
    ([NextParameterId]);
";

            private void AdditionalParamCategory()
            {
                _context.ParameterCategories.Add(new ParameterCategory() { Id = 5, Name = "Учёт" });
            }

            public const string AddFields = @"
SET QUOTED_IDENTIFIER OFF;
USE [house];

ALTER TABLE [dbo].[ParametrSetCommands]
ADD  [Reaction_Id] int NOT NULL;

ALTER TABLE [dbo].[Modes]
ADD [IsSelected] bit  NOT NULL DEFAULT(0);

-- Creating foreign key on [Reaction_Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [FK_ParametrSetCommandReaction]
    FOREIGN KEY ([Reaction_Id])
    REFERENCES [dbo].[Reactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametrSetCommandReaction'
CREATE INDEX [IX_FK_ParametrSetCommandReaction]
ON [dbo].[ParametrSetCommands]
    ([Reaction_Id]);

";

            private void FillParameterCategories()
            {
                _context.ParameterCategories.Add(new ParameterCategory() {Id = 1, Name = "Свет"});
                _context.ParameterCategories.Add(new ParameterCategory() { Id = 2, Name = "Климат" });
                _context.ParameterCategories.Add(new ParameterCategory() { Id = 3, Name = "СКУД" });
                _context.ParameterCategories.Add(new ParameterCategory() { Id = 4, Name = "Медиа" });
            }

            public const string ParamsCommandUpdate = @"
SET QUOTED_IDENTIFIER OFF;
USE [house];
CREATE TABLE [dbo].[ParameterCategories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
CREATE TABLE [dbo].[ParametrSetCommands] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Cooldown] int  NOT NULL,
    [DestParameter_Id] int  NOT NULL,
    [SrcParameter1_Id] int  NULL,
    [Sensor_Id] int  NULL,
    [SrcParameter2_Id] int  NULL
);

ALTER TABLE [dbo].[Parameter]
ADD  [ParameterCategory_Id] int  NULL;

ALTER TABLE [dbo].[Parameter]
ADD  [Sensor_Id] int  NULL;

ALTER TABLE [dbo].[ParameterCategories]
ADD CONSTRAINT [PK_ParameterCategories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [PK_ParametrSetCommands]
    PRIMARY KEY CLUSTERED ([Id] ASC);
-- Creating foreign key on [ParameterCategory_Id] in table 'Parameter'
ALTER TABLE [dbo].[Parameter]
ADD CONSTRAINT [FK_ParameterParameterCategory]
    FOREIGN KEY ([ParameterCategory_Id])
    REFERENCES [dbo].[ParameterCategories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ParameterParameterCategory'
CREATE INDEX [IX_FK_ParameterParameterCategory]
ON [dbo].[Parameter]
    ([ParameterCategory_Id]);

-- Creating foreign key on [Sensor_Id] in table 'Parameter'
ALTER TABLE [dbo].[Parameter]
ADD CONSTRAINT [FK_SensorParameter]
    FOREIGN KEY ([Sensor_Id])
    REFERENCES [dbo].[Devices_Sensor]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SensorParameter'
CREATE INDEX [IX_FK_SensorParameter]
ON [dbo].[Parameter]
    ([Sensor_Id]);

-- Creating foreign key on [DestParameter_Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [FK_ParametrSetCommandParameter]
    FOREIGN KEY ([DestParameter_Id])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametrSetCommandParameter'
CREATE INDEX [IX_FK_ParametrSetCommandParameter]
ON [dbo].[ParametrSetCommands]
    ([DestParameter_Id]);

-- Creating foreign key on [SrcParameter1_Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [FK_ParametrSetCommandParameter1]
    FOREIGN KEY ([SrcParameter1_Id])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametrSetCommandParameter1'
CREATE INDEX [IX_FK_ParametrSetCommandParameter1]
ON [dbo].[ParametrSetCommands]
    ([SrcParameter1_Id]);

-- Creating foreign key on [Sensor_Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [FK_ParametrSetCommandSensor]
    FOREIGN KEY ([Sensor_Id])
    REFERENCES [dbo].[Devices_Sensor]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametrSetCommandSensor'
CREATE INDEX [IX_FK_ParametrSetCommandSensor]
ON [dbo].[ParametrSetCommands]
    ([Sensor_Id]);

-- Creating foreign key on [SrcParameter2_Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [FK_ParametrSetCommandParameter2]
    FOREIGN KEY ([SrcParameter2_Id])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametrSetCommandParameter2'
CREATE INDEX [IX_FK_ParametrSetCommandParameter2]
ON [dbo].[ParametrSetCommands]
    ([SrcParameter2_Id]);


";

            private void FillParameters()
            {
                _context.ParameterTypes.Add(new ParameterType() { Id = (int)ParameterTypeValue.Time, Name = "Время" });
                var param = _context.Parameter.Find(Model.Parameter.CurrentTimeId);
                if (param == null)
                {
                    param=new Parameter();
                    _context.Parameter.Add(param);
                }
                param.Id = Model.Parameter.CurrentTimeId;
                param.Name = "Текущее время";
                param.ParameterTypeId = (int) ParameterTypeValue.Time;
                param.Value = string.Empty;

            }

            private void FillParameterTypes()
            {
                _context.ParameterTypes.Add(new ParameterType() { Id = (int) ParameterTypeValue.Bool, Name = "bool"});
                _context.ParameterTypes.Add(new ParameterType() { Id = (int)ParameterTypeValue.Int, Name = "Целое" });
                _context.ParameterTypes.Add(new ParameterType() { Id = (int)ParameterTypeValue.String, Name = "Строка" });
                _context.ParameterTypes.Add(new ParameterType() { Id = (int)ParameterTypeValue.Float, Name = "Десятичное" });
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
                    Key = "button",
                    Name = "тактовая или сенсорная кнопка",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_mq",
                    Name = "датчик газа/дыма",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_pir",
                    Name = "датчик движения",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_flame",
                    Name = "датчик огня",
                    MinValue = 0,
                    MaxValue = 1
                });


                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_light",
                    Name = "датчик света",
                    MinValue = 0,
                    MaxValue = 1
                });


                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_temp",
                    Name = "датчик температуры",
                    MinValue = -40,
                    MaxValue = 125
                });


                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_hum",
                    Name = "датчик влажности",
                    MinValue = 0,
                    MaxValue = 100
                });


                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_lum",
                    Name = "датчик уровня освещенности",
                    MinValue = 0,
                    MaxValue = 65535
                });

                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_sound",
                    Name = "датчик звука",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_water",
                    Name = "датчик протечки",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_power",
                    Name = "датчик тока",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_bar",
                    Name = "датчик атмосферного давления",
                    MinValue = 300,
                    MaxValue = 1100
                });
                //___________________
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "rele_clim",
                    Name = "реле управления контурами отопления ",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "rele_vent",
                    Name = "реле управления вент. Контурами",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "shim_ligth",
                    Name = "значение заданного уровня света шим",
                    MinValue = 0,
                    MaxValue = 255
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_door",
                    Name = "датчик открытия двери",
                    MinValue = 0,
                    MaxValue = 1
                });
                _context.SensorTypes.Add(new SensorType()
                {
                    Key = "sens_wind",
                    Name = "датчик открытия окна",
                    MinValue = 0,
                    MaxValue = 1
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
                Description = parameter.Description
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
