USE [ExternalSystems]
GO

/****** Object:  Table [dbo].[ProcessLists]    Script Date: 1/25/2016 7:39:10 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProcessLists](
	[Listname] [nvarchar](100) NULL,
	[Columnname] [nvarchar](100) NULL,
	[AddtoReputationScore] [bit] NULL
) ON [PRIMARY]

GO


