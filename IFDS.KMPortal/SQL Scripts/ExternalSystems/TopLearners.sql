USE [ExternalSystems]
GO

/****** Object:  Table [dbo].[TopLearners]    Script Date: 1/25/2016 7:42:48 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TopLearners](
	[Member] [nvarchar](100) NULL,
	[MemberStatus] [text] NULL,
	[LastActivity] [timestamp] NULL,
	[web] [nvarchar](50) NULL,
	[ReputationScore] [int] NOT NULL,
	[Column1] [int] NOT NULL,
	[Column2] [int] NOT NULL,
	[Column3] [int] NOT NULL,
	[Column4] [int] NOT NULL,
	[Column5] [int] NOT NULL,
	[Column6] [int] NOT NULL,
	[Column7] [int] NOT NULL,
	[Column8] [int] NOT NULL,
	[Column9] [int] NOT NULL,
	[Column10] [int] NOT NULL,
	[Displayname] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [ReputationScore]
GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [Column1]
GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [Column2]
GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [Column3]
GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [Column4]
GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [Column5]
GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [Column6]
GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [Column7]
GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [Column8]
GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [Column9]
GO

ALTER TABLE [dbo].[TopLearners] ADD  DEFAULT ((0)) FOR [Column10]
GO


