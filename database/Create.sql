/*    ==Scripting Parameters==

    Source Database Engine Edition : Microsoft Azure SQL Database Edition
    Source Database Engine Type : Microsoft Azure SQL Database

    Target Database Engine Edition : Microsoft Azure SQL Database Edition
    Target Database Engine Type : Microsoft Azure SQL Database
*/

/****** Object:  Table [dbo].[SantaUsers]    Script Date: 24/09/2017 16:25:23 ******/
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
	CONSTRAINT AK_Email UNIQUE(Email) 
)
GO


