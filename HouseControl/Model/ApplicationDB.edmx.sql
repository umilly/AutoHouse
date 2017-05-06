
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/05/2017 18:14:57
-- Generated from EDMX file: D:\work\#repo\HouseControl\Model\ApplicationDB.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [house];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Users_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_Users_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_ConditionTypeCondition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conditions] DROP CONSTRAINT [FK_ConditionTypeCondition];
GO
IF OBJECT_ID(N'[dbo].[FK_ReactionCondition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conditions] DROP CONSTRAINT [FK_ReactionCondition];
GO
IF OBJECT_ID(N'[dbo].[FK_ParameterTypeParameter]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parameter] DROP CONSTRAINT [FK_ParameterTypeParameter];
GO
IF OBJECT_ID(N'[dbo].[FK_SensorTypeSensor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_Sensor] DROP CONSTRAINT [FK_SensorTypeSensor];
GO
IF OBJECT_ID(N'[dbo].[FK_SensorController]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_Sensor] DROP CONSTRAINT [FK_SensorController];
GO
IF OBJECT_ID(N'[dbo].[FK_ScenarioMode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Scenarios] DROP CONSTRAINT [FK_ScenarioMode];
GO
IF OBJECT_ID(N'[dbo].[FK_ScenarioReaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Reactions] DROP CONSTRAINT [FK_ScenarioReaction];
GO
IF OBJECT_ID(N'[dbo].[FK_ReactionCommand]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Commands] DROP CONSTRAINT [FK_ReactionCommand];
GO
IF OBJECT_ID(N'[dbo].[FK_ZoneScenario_Zone]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ZoneScenario] DROP CONSTRAINT [FK_ZoneScenario_Zone];
GO
IF OBJECT_ID(N'[dbo].[FK_ZoneScenario_Scenario]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ZoneScenario] DROP CONSTRAINT [FK_ZoneScenario_Scenario];
GO
IF OBJECT_ID(N'[dbo].[FK_CommandCustomDevice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Commands] DROP CONSTRAINT [FK_CommandCustomDevice];
GO
IF OBJECT_ID(N'[dbo].[FK_ConditionCondition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conditions] DROP CONSTRAINT [FK_ConditionCondition];
GO
IF OBJECT_ID(N'[dbo].[FK_CustomDeviceController]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_CustomDevice] DROP CONSTRAINT [FK_CustomDeviceController];
GO
IF OBJECT_ID(N'[dbo].[FK_ConditionSensor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conditions] DROP CONSTRAINT [FK_ConditionSensor];
GO
IF OBJECT_ID(N'[dbo].[FK_ConditionParameter1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conditions] DROP CONSTRAINT [FK_ConditionParameter1];
GO
IF OBJECT_ID(N'[dbo].[FK_ConditionParameter2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Conditions] DROP CONSTRAINT [FK_ConditionParameter2];
GO
IF OBJECT_ID(N'[dbo].[FK_ComandParameterLinkCommand]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComandParameterLinks] DROP CONSTRAINT [FK_ComandParameterLinkCommand];
GO
IF OBJECT_ID(N'[dbo].[FK_ComandParameterLinkParameter]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComandParameterLinks] DROP CONSTRAINT [FK_ComandParameterLinkParameter];
GO
IF OBJECT_ID(N'[dbo].[FK_DeviceParameterTypeLinkCustomDevice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeviceParameterTypeLinks] DROP CONSTRAINT [FK_DeviceParameterTypeLinkCustomDevice];
GO
IF OBJECT_ID(N'[dbo].[FK_DeviceParameterTypeLinkParameterType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeviceParameterTypeLinks] DROP CONSTRAINT [FK_DeviceParameterTypeLinkParameterType];
GO
IF OBJECT_ID(N'[dbo].[FK_ComandParameterLinkDeviceParameterTypeLink]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComandParameterLinks] DROP CONSTRAINT [FK_ComandParameterLinkDeviceParameterTypeLink];
GO
IF OBJECT_ID(N'[dbo].[FK_SensorZone]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_Sensor] DROP CONSTRAINT [FK_SensorZone];
GO
IF OBJECT_ID(N'[dbo].[FK_ParameterParameterCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parameter] DROP CONSTRAINT [FK_ParameterParameterCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_SensorParameter]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parameter] DROP CONSTRAINT [FK_SensorParameter];
GO
IF OBJECT_ID(N'[dbo].[FK_ParametrSetCommandParameter]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ParametrSetCommands] DROP CONSTRAINT [FK_ParametrSetCommandParameter];
GO
IF OBJECT_ID(N'[dbo].[FK_ParametrSetCommandParameter1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ParametrSetCommands] DROP CONSTRAINT [FK_ParametrSetCommandParameter1];
GO
IF OBJECT_ID(N'[dbo].[FK_ParametrSetCommandSensor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ParametrSetCommands] DROP CONSTRAINT [FK_ParametrSetCommandSensor];
GO
IF OBJECT_ID(N'[dbo].[FK_ParametrSetCommandParameter2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ParametrSetCommands] DROP CONSTRAINT [FK_ParametrSetCommandParameter2];
GO
IF OBJECT_ID(N'[dbo].[FK_ParametrSetCommandReaction]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ParametrSetCommands] DROP CONSTRAINT [FK_ParametrSetCommandReaction];
GO
IF OBJECT_ID(N'[dbo].[FK_ParameterParameter]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parameter] DROP CONSTRAINT [FK_ParameterParameter];
GO
IF OBJECT_ID(N'[dbo].[FK_Sensor_inherits_Device]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_Sensor] DROP CONSTRAINT [FK_Sensor_inherits_Device];
GO
IF OBJECT_ID(N'[dbo].[FK_Controller_inherits_Device]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_Controller] DROP CONSTRAINT [FK_Controller_inherits_Device];
GO
IF OBJECT_ID(N'[dbo].[FK_CustomDevice_inherits_Device]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_CustomDevice] DROP CONSTRAINT [FK_CustomDevice_inherits_Device];
GO
IF OBJECT_ID(N'[dbo].[FK_ModBusController_inherits_Controller]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_ModBusController] DROP CONSTRAINT [FK_ModBusController_inherits_Controller];
GO
IF OBJECT_ID(N'[dbo].[FK_ModBusDevice_inherits_CustomDevice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_ModBusDevice] DROP CONSTRAINT [FK_ModBusDevice_inherits_CustomDevice];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Contents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Contents];
GO
IF OBJECT_ID(N'[dbo].[Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roles];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[SensorTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SensorTypes];
GO
IF OBJECT_ID(N'[dbo].[DBVersions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DBVersions];
GO
IF OBJECT_ID(N'[dbo].[Modes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Modes];
GO
IF OBJECT_ID(N'[dbo].[Scenarios]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Scenarios];
GO
IF OBJECT_ID(N'[dbo].[Zones]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Zones];
GO
IF OBJECT_ID(N'[dbo].[Reactions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Reactions];
GO
IF OBJECT_ID(N'[dbo].[Conditions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Conditions];
GO
IF OBJECT_ID(N'[dbo].[ConditionTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ConditionTypes];
GO
IF OBJECT_ID(N'[dbo].[Commands]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Commands];
GO
IF OBJECT_ID(N'[dbo].[Devices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Devices];
GO
IF OBJECT_ID(N'[dbo].[Parameter]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parameter];
GO
IF OBJECT_ID(N'[dbo].[ParameterTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ParameterTypes];
GO
IF OBJECT_ID(N'[dbo].[ComandParameterLinks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ComandParameterLinks];
GO
IF OBJECT_ID(N'[dbo].[DeviceParameterTypeLinks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeviceParameterTypeLinks];
GO
IF OBJECT_ID(N'[dbo].[ParameterCategories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ParameterCategories];
GO
IF OBJECT_ID(N'[dbo].[ParametrSetCommands]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ParametrSetCommands];
GO
IF OBJECT_ID(N'[dbo].[Devices_Sensor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Devices_Sensor];
GO
IF OBJECT_ID(N'[dbo].[Devices_Controller]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Devices_Controller];
GO
IF OBJECT_ID(N'[dbo].[Devices_CustomDevice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Devices_CustomDevice];
GO
IF OBJECT_ID(N'[dbo].[Devices_ModBusController]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Devices_ModBusController];
GO
IF OBJECT_ID(N'[dbo].[Devices_ModBusDevice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Devices_ModBusDevice];
GO
IF OBJECT_ID(N'[dbo].[ZoneScenario]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ZoneScenario];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Contents'
CREATE TABLE [dbo].[Contents] (
    [Id] int  NOT NULL,
    [Content1] nvarchar(100)  NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] int  NOT NULL,
    [name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int  NOT NULL,
    [login] nvarchar(50)  NOT NULL,
    [password] nvarchar(50)  NOT NULL,
    [role] int  NULL
);
GO

-- Creating table 'SensorTypes'
CREATE TABLE [dbo].[SensorTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Key] nvarchar(max)  NOT NULL,
    [MinValue] int  NOT NULL,
    [MaxValue] int  NOT NULL
);
GO

-- Creating table 'DBVersions'
CREATE TABLE [dbo].[DBVersions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Version] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Modes'
CREATE TABLE [dbo].[Modes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [IsSelected] bit  NOT NULL
);
GO

-- Creating table 'Scenarios'
CREATE TABLE [dbo].[Scenarios] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Mode_Id] int  NOT NULL
);
GO

-- Creating table 'Zones'
CREATE TABLE [dbo].[Zones] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Reactions'
CREATE TABLE [dbo].[Reactions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ScenarioId] int  NOT NULL
);
GO

-- Creating table 'Conditions'
CREATE TABLE [dbo].[Conditions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ConditionTypeId] int  NOT NULL,
    [ReactionId] int  NULL,
    [ParentConditionId] int  NULL,
    [Sensor_Id] int  NULL,
    [Parameter1_Id] int  NULL,
    [Parameter2_Id] int  NULL
);
GO

-- Creating table 'ConditionTypes'
CREATE TABLE [dbo].[ConditionTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Commands'
CREATE TABLE [dbo].[Commands] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ScenarioId] int  NOT NULL,
    [ReactionId] int  NOT NULL,
    [CustomDevice_Id] int  NOT NULL
);
GO

-- Creating table 'Devices'
CREATE TABLE [dbo].[Devices] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Parameter'
CREATE TABLE [dbo].[Parameter] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Value] nvarchar(max)  NOT NULL,
    [ParameterTypeId] int  NOT NULL,
    [IsPublic] bit  NOT NULL,
    [Image] varbinary(max)  NULL,
    [NextParameterId] int  NULL,
    [Description] nvarchar(max)  NULL,
    [ParameterCategory_Id] int  NULL,
    [Sensor_Id] int  NULL
);
GO

-- Creating table 'ParameterTypes'
CREATE TABLE [dbo].[ParameterTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ComandParameterLinks'
CREATE TABLE [dbo].[ComandParameterLinks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Command_Id] int  NOT NULL,
    [Parameter_Id] int  NOT NULL,
    [DeviceParameterTypeLink_Id] int  NOT NULL
);
GO

-- Creating table 'DeviceParameterTypeLinks'
CREATE TABLE [dbo].[DeviceParameterTypeLinks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Order] int  NOT NULL,
    [CustomDevice_Id] int  NOT NULL,
    [ParameterType_Id] int  NOT NULL
);
GO

-- Creating table 'ParameterCategories'
CREATE TABLE [dbo].[ParameterCategories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ParametrSetCommands'
CREATE TABLE [dbo].[ParametrSetCommands] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Cooldown] int  NOT NULL,
    [DestParameter_Id] int  NOT NULL,
    [SrcParameter1_Id] int  NULL,
    [Sensor_Id] int  NULL,
    [SrcParameter2_Id] int  NULL,
    [Reaction_Id] int  NOT NULL
);
GO

-- Creating table 'Devices_Sensor'
CREATE TABLE [dbo].[Devices_Sensor] (
    [ContollerSlot] int  NOT NULL,
    [SensorTypeId] int  NOT NULL,
    [Id] int  NOT NULL,
    [Controller_Id] int  NOT NULL,
    [Zone_Id] int  NOT NULL
);
GO

-- Creating table 'Devices_Controller'
CREATE TABLE [dbo].[Devices_Controller] (
    [IP] nvarchar(max)  NOT NULL,
    [Port] int  NOT NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'Devices_CustomDevice'
CREATE TABLE [dbo].[Devices_CustomDevice] (
    [CommandPath] nvarchar(max)  NOT NULL,
    [Id] int  NOT NULL,
    [Controller_Id] int  NOT NULL
);
GO

-- Creating table 'Devices_ModBusController'
CREATE TABLE [dbo].[Devices_ModBusController] (
    [ComPort] smallint  NULL,
    [SpeedType] smallint  NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'Devices_ModBusDevice'
CREATE TABLE [dbo].[Devices_ModBusDevice] (
    [IsCoil] bit  NULL,
    [Address] smallint  NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'ZoneScenario'
CREATE TABLE [dbo].[ZoneScenario] (
    [Zones_Id] int  NOT NULL,
    [Scenarios_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Contents'
ALTER TABLE [dbo].[Contents]
ADD CONSTRAINT [PK_Contents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SensorTypes'
ALTER TABLE [dbo].[SensorTypes]
ADD CONSTRAINT [PK_SensorTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DBVersions'
ALTER TABLE [dbo].[DBVersions]
ADD CONSTRAINT [PK_DBVersions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Modes'
ALTER TABLE [dbo].[Modes]
ADD CONSTRAINT [PK_Modes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Scenarios'
ALTER TABLE [dbo].[Scenarios]
ADD CONSTRAINT [PK_Scenarios]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Zones'
ALTER TABLE [dbo].[Zones]
ADD CONSTRAINT [PK_Zones]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Reactions'
ALTER TABLE [dbo].[Reactions]
ADD CONSTRAINT [PK_Reactions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Conditions'
ALTER TABLE [dbo].[Conditions]
ADD CONSTRAINT [PK_Conditions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ConditionTypes'
ALTER TABLE [dbo].[ConditionTypes]
ADD CONSTRAINT [PK_ConditionTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Commands'
ALTER TABLE [dbo].[Commands]
ADD CONSTRAINT [PK_Commands]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Devices'
ALTER TABLE [dbo].[Devices]
ADD CONSTRAINT [PK_Devices]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Parameter'
ALTER TABLE [dbo].[Parameter]
ADD CONSTRAINT [PK_Parameter]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ParameterTypes'
ALTER TABLE [dbo].[ParameterTypes]
ADD CONSTRAINT [PK_ParameterTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ComandParameterLinks'
ALTER TABLE [dbo].[ComandParameterLinks]
ADD CONSTRAINT [PK_ComandParameterLinks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeviceParameterTypeLinks'
ALTER TABLE [dbo].[DeviceParameterTypeLinks]
ADD CONSTRAINT [PK_DeviceParameterTypeLinks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ParameterCategories'
ALTER TABLE [dbo].[ParameterCategories]
ADD CONSTRAINT [PK_ParameterCategories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [PK_ParametrSetCommands]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Devices_Sensor'
ALTER TABLE [dbo].[Devices_Sensor]
ADD CONSTRAINT [PK_Devices_Sensor]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Devices_Controller'
ALTER TABLE [dbo].[Devices_Controller]
ADD CONSTRAINT [PK_Devices_Controller]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Devices_CustomDevice'
ALTER TABLE [dbo].[Devices_CustomDevice]
ADD CONSTRAINT [PK_Devices_CustomDevice]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Devices_ModBusController'
ALTER TABLE [dbo].[Devices_ModBusController]
ADD CONSTRAINT [PK_Devices_ModBusController]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Devices_ModBusDevice'
ALTER TABLE [dbo].[Devices_ModBusDevice]
ADD CONSTRAINT [PK_Devices_ModBusDevice]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Zones_Id], [Scenarios_Id] in table 'ZoneScenario'
ALTER TABLE [dbo].[ZoneScenario]
ADD CONSTRAINT [PK_ZoneScenario]
    PRIMARY KEY CLUSTERED ([Zones_Id], [Scenarios_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [role] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_Users_Role]
    FOREIGN KEY ([role])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Users_Role'
CREATE INDEX [IX_FK_Users_Role]
ON [dbo].[Users]
    ([role]);
GO

-- Creating foreign key on [ConditionTypeId] in table 'Conditions'
ALTER TABLE [dbo].[Conditions]
ADD CONSTRAINT [FK_ConditionTypeCondition]
    FOREIGN KEY ([ConditionTypeId])
    REFERENCES [dbo].[ConditionTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConditionTypeCondition'
CREATE INDEX [IX_FK_ConditionTypeCondition]
ON [dbo].[Conditions]
    ([ConditionTypeId]);
GO

-- Creating foreign key on [ReactionId] in table 'Conditions'
ALTER TABLE [dbo].[Conditions]
ADD CONSTRAINT [FK_ReactionCondition]
    FOREIGN KEY ([ReactionId])
    REFERENCES [dbo].[Reactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReactionCondition'
CREATE INDEX [IX_FK_ReactionCondition]
ON [dbo].[Conditions]
    ([ReactionId]);
GO

-- Creating foreign key on [ParameterTypeId] in table 'Parameter'
ALTER TABLE [dbo].[Parameter]
ADD CONSTRAINT [FK_ParameterTypeParameter]
    FOREIGN KEY ([ParameterTypeId])
    REFERENCES [dbo].[ParameterTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParameterTypeParameter'
CREATE INDEX [IX_FK_ParameterTypeParameter]
ON [dbo].[Parameter]
    ([ParameterTypeId]);
GO

-- Creating foreign key on [SensorTypeId] in table 'Devices_Sensor'
ALTER TABLE [dbo].[Devices_Sensor]
ADD CONSTRAINT [FK_SensorTypeSensor]
    FOREIGN KEY ([SensorTypeId])
    REFERENCES [dbo].[SensorTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SensorTypeSensor'
CREATE INDEX [IX_FK_SensorTypeSensor]
ON [dbo].[Devices_Sensor]
    ([SensorTypeId]);
GO

-- Creating foreign key on [Controller_Id] in table 'Devices_Sensor'
ALTER TABLE [dbo].[Devices_Sensor]
ADD CONSTRAINT [FK_SensorController]
    FOREIGN KEY ([Controller_Id])
    REFERENCES [dbo].[Devices_Controller]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SensorController'
CREATE INDEX [IX_FK_SensorController]
ON [dbo].[Devices_Sensor]
    ([Controller_Id]);
GO

-- Creating foreign key on [Mode_Id] in table 'Scenarios'
ALTER TABLE [dbo].[Scenarios]
ADD CONSTRAINT [FK_ScenarioMode]
    FOREIGN KEY ([Mode_Id])
    REFERENCES [dbo].[Modes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ScenarioMode'
CREATE INDEX [IX_FK_ScenarioMode]
ON [dbo].[Scenarios]
    ([Mode_Id]);
GO

-- Creating foreign key on [ScenarioId] in table 'Reactions'
ALTER TABLE [dbo].[Reactions]
ADD CONSTRAINT [FK_ScenarioReaction]
    FOREIGN KEY ([ScenarioId])
    REFERENCES [dbo].[Scenarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ScenarioReaction'
CREATE INDEX [IX_FK_ScenarioReaction]
ON [dbo].[Reactions]
    ([ScenarioId]);
GO

-- Creating foreign key on [ReactionId] in table 'Commands'
ALTER TABLE [dbo].[Commands]
ADD CONSTRAINT [FK_ReactionCommand]
    FOREIGN KEY ([ReactionId])
    REFERENCES [dbo].[Reactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReactionCommand'
CREATE INDEX [IX_FK_ReactionCommand]
ON [dbo].[Commands]
    ([ReactionId]);
GO

-- Creating foreign key on [Zones_Id] in table 'ZoneScenario'
ALTER TABLE [dbo].[ZoneScenario]
ADD CONSTRAINT [FK_ZoneScenario_Zone]
    FOREIGN KEY ([Zones_Id])
    REFERENCES [dbo].[Zones]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Scenarios_Id] in table 'ZoneScenario'
ALTER TABLE [dbo].[ZoneScenario]
ADD CONSTRAINT [FK_ZoneScenario_Scenario]
    FOREIGN KEY ([Scenarios_Id])
    REFERENCES [dbo].[Scenarios]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ZoneScenario_Scenario'
CREATE INDEX [IX_FK_ZoneScenario_Scenario]
ON [dbo].[ZoneScenario]
    ([Scenarios_Id]);
GO

-- Creating foreign key on [CustomDevice_Id] in table 'Commands'
ALTER TABLE [dbo].[Commands]
ADD CONSTRAINT [FK_CommandCustomDevice]
    FOREIGN KEY ([CustomDevice_Id])
    REFERENCES [dbo].[Devices_CustomDevice]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CommandCustomDevice'
CREATE INDEX [IX_FK_CommandCustomDevice]
ON [dbo].[Commands]
    ([CustomDevice_Id]);
GO

-- Creating foreign key on [ParentConditionId] in table 'Conditions'
ALTER TABLE [dbo].[Conditions]
ADD CONSTRAINT [FK_ConditionCondition]
    FOREIGN KEY ([ParentConditionId])
    REFERENCES [dbo].[Conditions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConditionCondition'
CREATE INDEX [IX_FK_ConditionCondition]
ON [dbo].[Conditions]
    ([ParentConditionId]);
GO

-- Creating foreign key on [Controller_Id] in table 'Devices_CustomDevice'
ALTER TABLE [dbo].[Devices_CustomDevice]
ADD CONSTRAINT [FK_CustomDeviceController]
    FOREIGN KEY ([Controller_Id])
    REFERENCES [dbo].[Devices_Controller]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomDeviceController'
CREATE INDEX [IX_FK_CustomDeviceController]
ON [dbo].[Devices_CustomDevice]
    ([Controller_Id]);
GO

-- Creating foreign key on [Sensor_Id] in table 'Conditions'
ALTER TABLE [dbo].[Conditions]
ADD CONSTRAINT [FK_ConditionSensor]
    FOREIGN KEY ([Sensor_Id])
    REFERENCES [dbo].[Devices_Sensor]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConditionSensor'
CREATE INDEX [IX_FK_ConditionSensor]
ON [dbo].[Conditions]
    ([Sensor_Id]);
GO

-- Creating foreign key on [Parameter1_Id] in table 'Conditions'
ALTER TABLE [dbo].[Conditions]
ADD CONSTRAINT [FK_ConditionParameter1]
    FOREIGN KEY ([Parameter1_Id])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConditionParameter1'
CREATE INDEX [IX_FK_ConditionParameter1]
ON [dbo].[Conditions]
    ([Parameter1_Id]);
GO

-- Creating foreign key on [Parameter2_Id] in table 'Conditions'
ALTER TABLE [dbo].[Conditions]
ADD CONSTRAINT [FK_ConditionParameter2]
    FOREIGN KEY ([Parameter2_Id])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ConditionParameter2'
CREATE INDEX [IX_FK_ConditionParameter2]
ON [dbo].[Conditions]
    ([Parameter2_Id]);
GO

-- Creating foreign key on [Command_Id] in table 'ComandParameterLinks'
ALTER TABLE [dbo].[ComandParameterLinks]
ADD CONSTRAINT [FK_ComandParameterLinkCommand]
    FOREIGN KEY ([Command_Id])
    REFERENCES [dbo].[Commands]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ComandParameterLinkCommand'
CREATE INDEX [IX_FK_ComandParameterLinkCommand]
ON [dbo].[ComandParameterLinks]
    ([Command_Id]);
GO

-- Creating foreign key on [Parameter_Id] in table 'ComandParameterLinks'
ALTER TABLE [dbo].[ComandParameterLinks]
ADD CONSTRAINT [FK_ComandParameterLinkParameter]
    FOREIGN KEY ([Parameter_Id])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ComandParameterLinkParameter'
CREATE INDEX [IX_FK_ComandParameterLinkParameter]
ON [dbo].[ComandParameterLinks]
    ([Parameter_Id]);
GO

-- Creating foreign key on [CustomDevice_Id] in table 'DeviceParameterTypeLinks'
ALTER TABLE [dbo].[DeviceParameterTypeLinks]
ADD CONSTRAINT [FK_DeviceParameterTypeLinkCustomDevice]
    FOREIGN KEY ([CustomDevice_Id])
    REFERENCES [dbo].[Devices_CustomDevice]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DeviceParameterTypeLinkCustomDevice'
CREATE INDEX [IX_FK_DeviceParameterTypeLinkCustomDevice]
ON [dbo].[DeviceParameterTypeLinks]
    ([CustomDevice_Id]);
GO

-- Creating foreign key on [ParameterType_Id] in table 'DeviceParameterTypeLinks'
ALTER TABLE [dbo].[DeviceParameterTypeLinks]
ADD CONSTRAINT [FK_DeviceParameterTypeLinkParameterType]
    FOREIGN KEY ([ParameterType_Id])
    REFERENCES [dbo].[ParameterTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DeviceParameterTypeLinkParameterType'
CREATE INDEX [IX_FK_DeviceParameterTypeLinkParameterType]
ON [dbo].[DeviceParameterTypeLinks]
    ([ParameterType_Id]);
GO

-- Creating foreign key on [DeviceParameterTypeLink_Id] in table 'ComandParameterLinks'
ALTER TABLE [dbo].[ComandParameterLinks]
ADD CONSTRAINT [FK_ComandParameterLinkDeviceParameterTypeLink]
    FOREIGN KEY ([DeviceParameterTypeLink_Id])
    REFERENCES [dbo].[DeviceParameterTypeLinks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ComandParameterLinkDeviceParameterTypeLink'
CREATE INDEX [IX_FK_ComandParameterLinkDeviceParameterTypeLink]
ON [dbo].[ComandParameterLinks]
    ([DeviceParameterTypeLink_Id]);
GO

-- Creating foreign key on [Zone_Id] in table 'Devices_Sensor'
ALTER TABLE [dbo].[Devices_Sensor]
ADD CONSTRAINT [FK_SensorZone]
    FOREIGN KEY ([Zone_Id])
    REFERENCES [dbo].[Zones]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SensorZone'
CREATE INDEX [IX_FK_SensorZone]
ON [dbo].[Devices_Sensor]
    ([Zone_Id]);
GO

-- Creating foreign key on [ParameterCategory_Id] in table 'Parameter'
ALTER TABLE [dbo].[Parameter]
ADD CONSTRAINT [FK_ParameterParameterCategory]
    FOREIGN KEY ([ParameterCategory_Id])
    REFERENCES [dbo].[ParameterCategories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParameterParameterCategory'
CREATE INDEX [IX_FK_ParameterParameterCategory]
ON [dbo].[Parameter]
    ([ParameterCategory_Id]);
GO

-- Creating foreign key on [Sensor_Id] in table 'Parameter'
ALTER TABLE [dbo].[Parameter]
ADD CONSTRAINT [FK_SensorParameter]
    FOREIGN KEY ([Sensor_Id])
    REFERENCES [dbo].[Devices_Sensor]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SensorParameter'
CREATE INDEX [IX_FK_SensorParameter]
ON [dbo].[Parameter]
    ([Sensor_Id]);
GO

-- Creating foreign key on [DestParameter_Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [FK_ParametrSetCommandParameter]
    FOREIGN KEY ([DestParameter_Id])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametrSetCommandParameter'
CREATE INDEX [IX_FK_ParametrSetCommandParameter]
ON [dbo].[ParametrSetCommands]
    ([DestParameter_Id]);
GO

-- Creating foreign key on [SrcParameter1_Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [FK_ParametrSetCommandParameter1]
    FOREIGN KEY ([SrcParameter1_Id])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametrSetCommandParameter1'
CREATE INDEX [IX_FK_ParametrSetCommandParameter1]
ON [dbo].[ParametrSetCommands]
    ([SrcParameter1_Id]);
GO

-- Creating foreign key on [Sensor_Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [FK_ParametrSetCommandSensor]
    FOREIGN KEY ([Sensor_Id])
    REFERENCES [dbo].[Devices_Sensor]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametrSetCommandSensor'
CREATE INDEX [IX_FK_ParametrSetCommandSensor]
ON [dbo].[ParametrSetCommands]
    ([Sensor_Id]);
GO

-- Creating foreign key on [SrcParameter2_Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [FK_ParametrSetCommandParameter2]
    FOREIGN KEY ([SrcParameter2_Id])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametrSetCommandParameter2'
CREATE INDEX [IX_FK_ParametrSetCommandParameter2]
ON [dbo].[ParametrSetCommands]
    ([SrcParameter2_Id]);
GO

-- Creating foreign key on [Reaction_Id] in table 'ParametrSetCommands'
ALTER TABLE [dbo].[ParametrSetCommands]
ADD CONSTRAINT [FK_ParametrSetCommandReaction]
    FOREIGN KEY ([Reaction_Id])
    REFERENCES [dbo].[Reactions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametrSetCommandReaction'
CREATE INDEX [IX_FK_ParametrSetCommandReaction]
ON [dbo].[ParametrSetCommands]
    ([Reaction_Id]);
GO

-- Creating foreign key on [NextParameterId] in table 'Parameter'
ALTER TABLE [dbo].[Parameter]
ADD CONSTRAINT [FK_ParameterParameter]
    FOREIGN KEY ([NextParameterId])
    REFERENCES [dbo].[Parameter]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParameterParameter'
CREATE INDEX [IX_FK_ParameterParameter]
ON [dbo].[Parameter]
    ([NextParameterId]);
GO

-- Creating foreign key on [Id] in table 'Devices_Sensor'
ALTER TABLE [dbo].[Devices_Sensor]
ADD CONSTRAINT [FK_Sensor_inherits_Device]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Devices]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Devices_Controller'
ALTER TABLE [dbo].[Devices_Controller]
ADD CONSTRAINT [FK_Controller_inherits_Device]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Devices]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Devices_CustomDevice'
ALTER TABLE [dbo].[Devices_CustomDevice]
ADD CONSTRAINT [FK_CustomDevice_inherits_Device]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Devices]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Devices_ModBusController'
ALTER TABLE [dbo].[Devices_ModBusController]
ADD CONSTRAINT [FK_ModBusController_inherits_Controller]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Devices_Controller]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'Devices_ModBusDevice'
ALTER TABLE [dbo].[Devices_ModBusDevice]
ADD CONSTRAINT [FK_ModBusDevice_inherits_CustomDevice]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[Devices_CustomDevice]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------