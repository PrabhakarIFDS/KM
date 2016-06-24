USE [ExternalSystems]
GO

/****** Object:  Table [dbo].[CNR_Z_CourseCatalog]    Script Date: 1/25/2016 7:32:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CNR_Z_CourseCatalog](
	[Title] [varchar](500) NULL,
	[CreatedDate] [datetime] NULL,
	[CatDesc] [varchar](max) NULL,
	[SubjectIDs] [varchar](max) NULL,
	[SubjectTitles] [varchar](max) NULL,
	[Price] [decimal](18, 0) NULL,
	[ProviderName] [varchar](max) NULL,
	[CatType] [varchar](max) NULL,
	[DeepLinkURL] [varchar](max) NULL,
	[CourseDuration] [varchar](max) NULL,
	[OUAvailability] [varchar](max) NULL,
	[LOTitles] [varchar](max) NULL,
	[LOInstructions] [varchar](max) NULL,
	[LOLocations] [varchar](max) NULL,
	[LOInstructors] [varchar](max) NULL,
	[SystemType] [varchar](50) NULL,
	[ID] [int] NULL,
	[ImportDtStamp] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[CNR_Z_CourseCatalog] ADD  CONSTRAINT [DF_CornerStone_CourseCatalog_SystemType]  DEFAULT ('PACEPartner') FOR [SystemType]
GO


