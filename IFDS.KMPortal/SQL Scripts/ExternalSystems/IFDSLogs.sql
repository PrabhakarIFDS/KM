USE [ExternalSystems]
GO

/****** Object:  Table [dbo].[DWR_Z_IFDSLogs]    Script Date: 1/25/2016 7:34:40 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[DWR_Z_IFDSLogs](
	[Error] [varchar](max) NULL,
	[ID] [int] NULL,
	[ImportDtStamp] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


