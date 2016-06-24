USE [ExternalSystems]
GO

/****** Object:  Table [dbo].[URLAccessEntries]    Script Date: 1/25/2016 7:43:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[URLAccessEntries](
	[AccessURL] [text] NULL,
	[List] [nvarchar](100) NULL,
	[Member] [nvarchar](100) NOT NULL,
	[Displayname] [text] NULL,
	[web] [nvarchar](50) NULL,
	[AccessDate] [timestamp] NOT NULL,
	[ProcessFlag] [bit] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[URLAccessEntries] ADD  DEFAULT ((0)) FOR [ProcessFlag]
GO


