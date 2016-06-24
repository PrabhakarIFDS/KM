USE [ExternalSystems]
GO

/****** Object:  Table [dbo].[MOX_Z_SolutionFinders]    Script Date: 1/25/2016 7:37:18 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[MOX_Z_SolutionFinders](
	[SolutionFinderID] [int] NULL,
	[SolutionFinderName] [varchar](max) NULL,
	[SFstatement] [varchar](max) NULL,
	[SFChoiceID] [int] NULL,
	[SFChoiceName] [varchar](max) NULL,
	[SystemType] [varchar](50) NULL,
	[ID] [int] NULL,
	[ImportDtStamp] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[MOX_Z_SolutionFinders] ADD  CONSTRAINT [DF_Moxie_SolutionFinders_SystemType]  DEFAULT ('KnowledgeBase') FOR [SystemType]
GO


