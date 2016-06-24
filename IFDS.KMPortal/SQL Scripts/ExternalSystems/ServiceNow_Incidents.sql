USE [ExternalSystems]
GO

/****** Object:  Table [dbo].[SRN_Z_Incidents]    Script Date: 1/25/2016 7:40:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SRN_Z_Incidents](
	[Number] [varchar](50) NULL,
	[Title] [varchar](50) NULL,
	[TicketDesc] [varchar](max) NULL,
	[TicketState] [varchar](50) NULL,
	[Resolution] [varchar](max) NULL,
	[CloseCode] [varchar](100) NULL,
	[SysID] [varchar](50) NULL,
	[Category] [varchar](50) NULL,
	[ExternalURL] [varchar](100) NULL,
	[Active] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[TicketType] [varchar](20) NULL,
	[SystemType] [varchar](50) NULL,
	[ID] [int] NULL,
	[ImportDtStamp] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[SRN_Z_Incidents] ADD  CONSTRAINT [DF_Snow_Incidents_SystemType]  DEFAULT ('ServiceNow') FOR [SystemType]
GO


