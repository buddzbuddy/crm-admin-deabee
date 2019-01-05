USE [Dynamic]
GO
/****** Object:  Table [dbo].[SomeTable]    Script Date: 2014-11-08 06:19:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SomeTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Firstname] [nvarchar](50) NULL,
	[Surname] [nvarchar](50) NULL,
	[Gender] [nvarchar](10) NULL,
 CONSTRAINT [PK_SomeTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DynamicAttribute]    Script Date: 2014-11-08 06:19:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DynamicAttribute](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DynamicAttribute] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DynamicTemplate]    Script Date: 2014-11-08 06:19:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DynamicTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DynamicTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DynamicTemplateAttributes]    Script Date: 2014-11-08 06:19:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DynamicTemplateAttributes](
	[TemplateId] [int] NOT NULL,
	[AttributeId] [int] NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[TypeId] [int] NOT NULL,
	[Idx] [int] NOT NULL,
 CONSTRAINT [PK_DynamicTemplateAttributes] PRIMARY KEY CLUSTERED 
(
	[TemplateId] ASC,
	[AttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DynamicType]    Script Date: 2014-11-08 06:19:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DynamicType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DynamicType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[DynamicTemplateAttributes]  WITH CHECK ADD  CONSTRAINT [FK_DynamicTemplateAttributes_DynamicAttribute] FOREIGN KEY([AttributeId])
REFERENCES [dbo].[DynamicAttribute] ([Id])
GO
ALTER TABLE [dbo].[DynamicTemplateAttributes] CHECK CONSTRAINT [FK_DynamicTemplateAttributes_DynamicAttribute]
GO
ALTER TABLE [dbo].[DynamicTemplateAttributes]  WITH CHECK ADD  CONSTRAINT [FK_DynamicTemplateAttributes_DynamicTemplate] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[DynamicTemplate] ([Id])
GO
ALTER TABLE [dbo].[DynamicTemplateAttributes] CHECK CONSTRAINT [FK_DynamicTemplateAttributes_DynamicTemplate]
GO
ALTER TABLE [dbo].[DynamicTemplateAttributes]  WITH CHECK ADD  CONSTRAINT [FK_DynamicTemplateAttributes_DynamicType] FOREIGN KEY([TypeId])
REFERENCES [dbo].[DynamicType] ([Id])
GO
ALTER TABLE [dbo].[DynamicTemplateAttributes] CHECK CONSTRAINT [FK_DynamicTemplateAttributes_DynamicType]
GO
