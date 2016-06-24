USE [ExternalSystems]
GO

/****** Object:  Table [dbo].[MOX_Z_GlossaryTerms]    Script Date: 1/25/2016 7:36:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[MOX_Z_GlossaryTerms](
	[ID] [int] NULL,
	[Term] [varchar](max) NULL,
	[GlossText] [varchar](max) NULL,
	[SystemType] [varchar](50) NULL,
	[ImportDtStamp] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[MOX_Z_GlossaryTerms] ADD  CONSTRAINT [DF_Moxie_GlossaryTerms_SystemType]  DEFAULT ('KnowledgeBase') FOR [SystemType]
GO


