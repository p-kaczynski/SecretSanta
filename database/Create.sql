/*    ==Scripting Parameters==

    Source Database Engine Edition : Microsoft Azure SQL Database Edition
    Source Database Engine Type : Microsoft Azure SQL Database

    Target Server Version : SQL Server 2017
    Target Database Engine Edition : Microsoft SQL Server Standard Edition
    Target Database Engine Type : Standalone SQL Server
*/

USE [SantaDB]
GO
/****** Object:  User [SantaAppUser]    Script Date: 05/10/2017 23:34:49 ******/
CREATE USER [SantaAppUser] FOR LOGIN [SantaAppUser] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [SantaAppUser]
GO
/****** Object:  Table [dbo].[SantaAdmins]    Script Date: 05/10/2017 23:34:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SantaAdmins](
	[AdminId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](200) NOT NULL,
	[PasswordHash] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_TransactionHistoryArchive_TransactionID] PRIMARY KEY NONCLUSTERED 
(
	[AdminId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
 CONSTRAINT [AK_UserName] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
/****** Object:  Table [dbo].[SantaSettings]    Script Date: 05/10/2017 23:34:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SantaSettings](
	[Key] [nvarchar](200) NOT NULL,
	[Value] [nvarchar](2048) NULL,
 CONSTRAINT [PK_SantaSettings] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
/****** Object:  Table [dbo].[SantaUsers]    Script Date: 05/10/2017 23:35:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SantaUsers](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](254) NOT NULL,
	[FacebookProfileUrl] [nvarchar](max) NOT NULL,
	[PasswordHash] [varbinary](max) NULL,
	[DisplayName] [nvarchar](200) NOT NULL,
	[FullName] [nvarchar](200) NOT NULL,
	[AddressLine1] [nvarchar](200) NOT NULL,
	[AddressLine2] [nvarchar](200) NULL,
	[PostalCode] [nvarchar](32) NOT NULL,
	[City] [nvarchar](200) NOT NULL,
	[Country] [nvarchar](200) NOT NULL,
	[Note] [nvarchar](max) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[AdminConfirmed] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [AK_Email] PRIMARY KEY NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO


