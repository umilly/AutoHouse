
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/05/2016 11:14:15
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
IF OBJECT_ID(N'[dbo].[FK_SensorController]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sensors] DROP CONSTRAINT [FK_SensorController];
GO
IF OBJECT_ID(N'[dbo].[FK_SensorTypeSensor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sensors] DROP CONSTRAINT [FK_SensorTypeSensor];
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
IF OBJECT_ID(N'[dbo].[Controllers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Controllers];
GO
IF OBJECT_ID(N'[dbo].[Sensors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sensors];
GO
IF OBJECT_ID(N'[dbo].[SensorTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SensorTypes];
GO
IF OBJECT_ID(N'[dbo].[DBVersions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DBVersions];
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

-- Creating table 'Controllers'
CREATE TABLE [dbo].[Controllers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IP] nvarchar(max)  NOT NULL,
    [Port] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Sensors'
CREATE TABLE [dbo].[Sensors] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ControllerId] int  NOT NULL,
    [SensorTypeId] int  NOT NULL
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

-- Creating primary key on [Id] in table 'Controllers'
ALTER TABLE [dbo].[Controllers]
ADD CONSTRAINT [PK_Controllers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Sensors'
ALTER TABLE [dbo].[Sensors]
ADD CONSTRAINT [PK_Sensors]
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

-- Creating foreign key on [ControllerId] in table 'Sensors'
ALTER TABLE [dbo].[Sensors]
ADD CONSTRAINT [FK_SensorController]
    FOREIGN KEY ([ControllerId])
    REFERENCES [dbo].[Controllers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SensorController'
CREATE INDEX [IX_FK_SensorController]
ON [dbo].[Sensors]
    ([ControllerId]);
GO

-- Creating foreign key on [SensorTypeId] in table 'Sensors'
ALTER TABLE [dbo].[Sensors]
ADD CONSTRAINT [FK_SensorTypeSensor]
    FOREIGN KEY ([SensorTypeId])
    REFERENCES [dbo].[SensorTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SensorTypeSensor'
CREATE INDEX [IX_FK_SensorTypeSensor]
ON [dbo].[Sensors]
    ([SensorTypeId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------