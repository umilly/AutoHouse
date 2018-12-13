using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Facade;

namespace Model
{
    
    public partial class Models:IInitable
    {
        public Models() : base(BuildEntityConnectionStringFromAppSettings())
        {
            var migration = new MigrationService(this);
            migration.Migrate();
        }

        public static string BuildEntityConnectionStringFromAppSettings()
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
            var dbFile = @"F:\work\#repo\db\house.mdf";
            string shortConnectionString=string.Empty;
            if (File.Exists("constr"))
                shortConnectionString = File.ReadAllText("constr");
            if (File.Exists(dbFile))
                shortConnectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbFile};Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;App=EntityFramework";
            //_connectinsStrings["vlad"] = @"Data Source=UMILLY;Initial Catalog=house;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;App=EntityFramework";
            

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
                RegiterUpdate(Guid.Parse("5E2E511F-36A7-4394-A02F-ECE1869DCA74"), CreateParamButtonDescription);
                RegiterUpdate(Guid.Parse("F439FC8D-C225-4A0A-8910-CA588A3C48F1"), CreateCustomSensors);
                RegiterUpdate(Guid.Parse("DE4F8F87-CD5C-4115-92A0-19E10AAA9AFF"), CustomSensorInnerName);
                RegiterUpdate(Guid.Parse("0788A4B6-7E6C-4C41-BE04-15913AF16644"), IsActiveForReactions);
            }

          

            public string IsActiveForReactions = @"
ALTER TABLE [dbo].[Reactions]
ADD [IsActive] bit  NOT NULL DEFAULT(1);
";

            public const string CustomSensorInnerName = @"
ALTER TABLE [dbo].[Devices_CustomSensor]
ADD [InnerName] nvarchar(max)  NOT NULL;
";

            public const string CreateCustomSensors = @"
SET QUOTED_IDENTIFIER OFF;
CREATE TABLE [dbo].[Devices_CustomSensor] (
    [LastValue] nvarchar(max)  NULL,
    [ValueChangeDate] datetime  NOT NULL,
    [Id] int  NOT NULL
);

ALTER TABLE [dbo].[Devices_CustomSensor]
ADD CONSTRAINT [PK_Devices_CustomSensor]
    PRIMARY KEY CLUSTERED ([Id] ASC);

ALTER TABLE [dbo].[Devices_CustomSensor]
ADD CONSTRAINT [FK_CustomSensor_inherits_Sensor]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Devices_Sensor]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

";
            public const string CreateParamButtonDescription = @"
SET QUOTED_IDENTIFIER OFF;

ALTER TABLE [dbo].[Parameter]
ADD [ButtonDescription] nvarchar(max)  NULL;
";

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
                Guid currentVersion = Guid.Empty;
                try
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
                    currentVersion = _context.DBVersions.Single().Version;
                    var lastUpdateOrder = _updates.First(a => a.ID == currentVersion).Order;
                    ApplyScripts(lastUpdateOrder);
                }
                catch (Exception e)
                {
                    throw new Exception($"error migrate db from '{currentVersion.ToString()}' to '{_updates.Last().ID}' total: '{_updates.Count}' ",e);
                }

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

        public void Init()
        {
            
        }
    }
}