DROP TABLE [dbo].[GitHubFiles]

CREATE TABLE [dbo].[GitHubFiles](
	[FileId] [int] IDENTITY(1,1) NOT NULL,
	[FileUrl] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

