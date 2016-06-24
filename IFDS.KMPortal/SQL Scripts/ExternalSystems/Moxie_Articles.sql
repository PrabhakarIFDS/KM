USE [ExternalSystems]
GO

/****** Object:  Table [dbo].[MOX_Z_Articles]    Script Date: 1/25/2016 7:35:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[MOX_Z_Articles](
	[ArticleID] [int] NULL,
	[ArticleName] [varchar](max) NULL,
	[ArticleViewed] [varchar](max) NULL,
	[ArticleSummary] [varchar](max) NULL,
	[Reviewed] [datetime] NULL,
	[Extension] [varchar](max) NULL,
	[Attachments] [varchar](max) NULL,
	[ArticleRelated] [varchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[ArticleContent] [varchar](max) NULL,
	[ArticleURL] [varchar](max) NULL,
	[SystemType] [varchar](50) NULL,
	[ID] [int] NULL,
	[ImportDtStamp] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[MOX_Z_Articles] ADD  CONSTRAINT [DF_Moxie_Articles_SystemType]  DEFAULT ('KnowledgeBase') FOR [SystemType]
GO


