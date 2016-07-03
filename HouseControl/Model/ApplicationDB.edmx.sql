
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 07/03/2016 19:46:24
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
IF OBJECT_ID(N'[dbo].[FK_CommandDevice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Commands] DROP CONSTRAINT [FK_CommandDevice];
GO
IF OBJECT_ID(N'[dbo].[FK_ParametеrCommand]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parametеr] DROP CONSTRAINT [FK_ParametеrCommand];
GO
IF OBJECT_ID(N'[dbo].[FK_ParameterTypeParametеr]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parametеr] DROP CONSTRAINT [FK_ParameterTypeParametеr];
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
IF OBJECT_ID(N'[dbo].[FK_Sensor_inherits_Device]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_Sensor] DROP CONSTRAINT [FK_Sensor_inherits_Device];
GO
IF OBJECT_ID(N'[dbo].[FK_Controller_inherits_Device]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices_Controller] DROP CONSTRAINT [FK_Controller_inherits_Device];
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
IF OBJECT_ID(N'[dbo].[Parametеr]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parametеr];
GO
IF OBJECT_ID(N'[dbo].[ParameterTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ParameterTypes];
GO
IF OBJECT_ID(N'[dbo].[Devices_Sensor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Devices_Sensor];
GO
IF OBJECT_ID(N'[dbo].[Devices_Controller]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Devices_Controller];
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
    [Description] nvarchar(max)  NOT NULL
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
    [ReactionId] int  NULL
);
GO

-- Creating table 'ConditionTypes'
CREATE TABLE [dbo].[ConditionTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Value] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Commands'
CREATE TABLE [dbo].[Commands] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ScenarioId] int  NOT NULL,
    [ReactionId] int  NOT NULL,
    [Device_Id] int  NOT NULL
);
GO

-- Creating table 'Devices'
CREATE TABLE [dbo].[Devices] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Parametеr'
CREATE TABLE [dbo].[Parametеr] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Value] nvarchar(max)  NOT NULL,
    [ParameterTypeId] int  NOT NULL,
    [Command_Id] int  NOT NULL
);
GO

-- Creating table 'ParameterTypes'
CREATE TABLE [dbo].[ParameterTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Devices_Sensor'
CREATE TABLE [dbo].[Devices_Sensor] (
    [ContollerSlot] int  NOT NULL,
    [SensorTypeId] int  NOT NULL,
    [Id] int  NOT NULL,
    [Controller_Id] int  NOT NULL
);
GO

-- Creating table 'Devices_Controller'
CREATE TABLE [dbo].[Devices_Controller] (
    [IP] nvarchar(max)  NOT NULL,
    [Port] int  NOT NULL,
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

-- Creating primary key on [Id] in table 'Parametеr'
ALTER TABLE [dbo].[Parametеr]
ADD CONSTRAINT [PK_Parametеr]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ParameterTypes'
ALTER TABLE [dbo].[ParameterTypes]
ADD CONSTRAINT [PK_ParameterTypes]
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

-- Creating foreign key on [Device_Id] in table 'Commands'
ALTER TABLE [dbo].[Commands]
ADD CONSTRAINT [FK_CommandDevice]
    FOREIGN KEY ([Device_Id])
    REFERENCES [dbo].[Devices]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CommandDevice'
CREATE INDEX [IX_FK_CommandDevice]
ON [dbo].[Commands]
    ([Device_Id]);
GO

-- Creating foreign key on [Command_Id] in table 'Parametеr'
ALTER TABLE [dbo].[Parametеr]
ADD CONSTRAINT [FK_ParametеrCommand]
    FOREIGN KEY ([Command_Id])
    REFERENCES [dbo].[Commands]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParametеrCommand'
CREATE INDEX [IX_FK_ParametеrCommand]
ON [dbo].[Parametеr]
    ([Command_Id]);
GO

-- Creating foreign key on [ParameterTypeId] in table 'Parametеr'
ALTER TABLE [dbo].[Parametеr]
ADD CONSTRAINT [FK_ParameterTypeParametеr]
    FOREIGN KEY ([ParameterTypeId])
    REFERENCES [dbo].[ParameterTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ParameterTypeParametеr'
CREATE INDEX [IX_FK_ParameterTypeParametеr]
ON [dbo].[Parametеr]
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

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------