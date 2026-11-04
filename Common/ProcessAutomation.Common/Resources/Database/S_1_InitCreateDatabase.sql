SET ANSI_NULLS ON


SET QUOTED_IDENTIFIER ON



/****** Object:  Table [dbo].[CAT_TOKEN_TYPE] ******/

IF (OBJECT_ID('CAT_TOKEN_TYPE', 'u') IS NULL)
BEGIN

	CREATE TABLE [dbo].[CAT_TOKEN_TYPE](
		[ID_Token_Type] [int] IDENTITY(1,1) NOT NULL,
		[Token_Type] [varchar](50) NOT NULL,
		[Token_Type_Code] [varchar](10) NOT NULL,
		[Alteration_Counter] [int] NOT NULL,
		[ID_User_Created] [int] NOT NULL,
		[Creation_Date] [datetime] NOT NULL,
		[ID_User_Modified] [int] NULL,
		[Modification_Date] [datetime] NULL,
		[Active] [bit] NOT NULL,
	 CONSTRAINT [PK_CAT_TOKEN_TYPE] PRIMARY KEY CLUSTERED 
	(
		[ID_Token_Type] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
END


/****** Object:  Table [dbo].[CAT_DEVICE] ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON


IF OBJECT_ID (N'dbo.CAT_DEVICE', N'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[CAT_DEVICE](
		[ID_Device] [int] IDENTITY(1,1) NOT NULL,
		[Device] [varchar](50) NOT NULL,
		[Device_Code] [varchar](10) NOT NULL,
		[Alteration_Counter] [int] NOT NULL,
		[ID_User_Created] [int] NOT NULL,
		[Creation_Date] [datetime] NOT NULL,
		[ID_User_Modified] [int] NULL,
		[Modification_Date] [datetime] NULL,
		[Active] [bit] NOT NULL,
	 CONSTRAINT [PK_CAT_DEVICE] PRIMARY KEY CLUSTERED 
	(
		[ID_Device] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END


/****** Object:  Table [dbo].[CAT_STATUS] ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON


IF OBJECT_ID (N'dbo.CAT_STATUS', N'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[CAT_STATUS](
		[ID_Status] [int] IDENTITY(1,1) NOT NULL,
		[Status] [varchar](50) NOT NULL,
		[Number] [int] NOT NULL,
		[ID_Status_Type] [int] NOT NULL,
		[Alteration_Counter] [int] NOT NULL,
		[ID_User_Created] [int] NOT NULL,
		[Creation_Date] [datetime] NOT NULL,
		[ID_User_Modified] [int] NULL,
		[Modification_Date] [datetime] NULL,
		[Active] [bit] NOT NULL,
	 CONSTRAINT [PK_CAT_STATUS] PRIMARY KEY CLUSTERED 
	(
		[ID_Status] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END


/****** Object:  Table [dbo].[CAT_STATUS_TYPE] ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON


IF OBJECT_ID (N'dbo.CAT_STATUS_TYPE', N'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[CAT_STATUS_TYPE](
		[ID_Status_Type] [int] IDENTITY(1,1) NOT NULL,
		[Status_Type] [varchar](50) NOT NULL,
		[Status_Type_Code] [varchar](10) NOT NULL,
		[Alteration_Counter] [int] NOT NULL,
		[ID_User_Created] [int] NOT NULL,
		[Creation_Date] [datetime] NOT NULL,
		[ID_User_Modified] [int] NULL,
		[Modification_Date] [datetime] NULL,
		[Active] [bit] NOT NULL,
	 CONSTRAINT [PK_CAT_STATUS_TYPE] PRIMARY KEY CLUSTERED 
	(
		[ID_Status_Type] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END


/****** Object:  Table [dbo].[CAT_USER] ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON


IF OBJECT_ID (N'dbo.CAT_USER', N'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[CAT_USER](
		[ID_User] [int] IDENTITY(1,1) NOT NULL,
		[User_Name] [varchar](50) NULL,
		[Password] [varchar](50) NULL,
		[Name] [varchar](100) NOT NULL,
		[Last_Name] [varchar](100) NULL,
		[Photo] [varchar](100) NULL,
		[Email] [varchar](100) NULL,
		[Birthdate] [datetime] NULL,
		[Alteration_Counter] [int] NOT NULL,
		[ID_User_Created] [int] NOT NULL,
		[Creation_Date] [datetime] NOT NULL,
		[ID_User_Modified] [int] NULL,
		[Modification_Date] [datetime] NULL,
		[Active] [bit] NOT NULL,
	 CONSTRAINT [PK_CAT_USER] PRIMARY KEY CLUSTERED 
	(
		[ID_User] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END


/****** Object:  Table [dbo].[MESSAGE_ADMIN] ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON


IF OBJECT_ID (N'dbo.MESSAGE_ADMIN', N'U') IS NULL
BEGIN

	CREATE TABLE [dbo].[MESSAGE_ADMIN](
		[ID_Message_Admin] [int] IDENTITY(1,1) NOT NULL,
		[ID_User] [int] NOT NULL,
		[Message] [varchar](1000) NOT NULL,
		[Message_Type] [varchar](30) NOT NULL,
		[Creation_Date] [datetime] NOT NULL,
		[Modification_Date] [datetime] NULL,
		[Active] [bit] NOT NULL,
	 CONSTRAINT [PK_MESSAGE_ADMIN] PRIMARY KEY CLUSTERED 
	(
		[ID_Message_Admin] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END


/****** Object:  Table [dbo].[TOKEN] ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON


IF (OBJECT_ID(N'dbo.TOKEN', N'u') IS NULL)
BEGIN
	
	CREATE TABLE [dbo].[TOKEN](
		[ID_Token] [int] IDENTITY(1,1) NOT NULL,
		[Token] [varchar](100) NOT NULL,
		[Expiration_Date] [datetime] NOT NULL,
		[Finish_Date] [datetime] NULL,
		[ID_Token_Type] [int] NOT NULL,
		[ID_User_Created] [int] NOT NULL,
		[Creation_Date] [datetime] NOT NULL,
		[Active] [bit] NOT NULL,
	 CONSTRAINT [PK_TOKEN] PRIMARY KEY CLUSTERED 
	(
		[ID_Token] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
END


IF (OBJECT_ID(N'dbo.USER_LOGIN', N'u') IS NULL)
BEGIN
	
	CREATE TABLE [dbo].[USER_LOGIN](
		[ID_User_Login] [int] IDENTITY(1,1) NOT NULL,
		[ID_User] [int] NOT NULL,
		[ID_Token] [int] NOT NULL,
		[ID_Device] [int] NOT NULL,
		[Login_Date] [datetime] NOT NULL,
		[Login_Latitude] [varchar](30) NOT NULL,
		[Login_Longitude] [varchar](30) NOT NULL,
		[Logout_Date] [datetime] NULL,
		[Logout_Latitude] [varchar](30) NULL,
		[Logout_Longitude] [varchar](30) NULL,
		[Alteration_Counter] [int] NOT NULL,
		[ID_User_Created] [int] NOT NULL,
		[Creation_Date] [datetime] NOT NULL,
		[ID_User_Modified] [int] NULL,
		[Modification_Date] [datetime] NULL,
	 CONSTRAINT [PK_USER_LOGIN] PRIMARY KEY CLUSTERED 
	(
		[ID_User_Login] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
END


IF (OBJECT_ID('FK_MESSAGE_ADMIN_USER', 'F') IS NULL)
BEGIN
	ALTER TABLE dbo.MESSAGE_ADMIN
	ADD CONSTRAINT FK_MESSAGE_ADMIN_USER FOREIGN KEY (ID_User) REFERENCES dbo.CAT_USER(ID_User) ON DELETE NO ACTION ON UPDATE NO ACTION
END


IF (OBJECT_ID('FK_TOKEN_TOKEN_TYPE', 'F') IS NULL)
BEGIN
	ALTER TABLE dbo.TOKEN
	ADD CONSTRAINT FK_TOKEN_TOKEN_TYPE FOREIGN KEY (ID_Token_Type) REFERENCES dbo.CAT_TOKEN_TYPE(ID_Token_Type) ON DELETE NO ACTION ON UPDATE NO ACTION
END

IF (OBJECT_ID('FK_USER_LOGIN_USER', 'F') IS NULL)
BEGIN
	ALTER TABLE dbo.USER_LOGIN
	ADD CONSTRAINT FK_USER_LOGIN_USER FOREIGN KEY (ID_User) REFERENCES dbo.CAT_USER(ID_User) ON DELETE NO ACTION ON UPDATE NO ACTION
END

IF (OBJECT_ID('FK_USER_LOGIN_TOKEN', 'F') IS NULL)
BEGIN
	ALTER TABLE dbo.USER_LOGIN
	ADD CONSTRAINT FK_USER_LOGIN_TOKEN FOREIGN KEY (ID_Token) REFERENCES dbo.TOKEN(ID_Token) ON DELETE NO ACTION ON UPDATE NO ACTION
END

IF (OBJECT_ID('FK_USER_LOGIN_DEVICE', 'F') IS NULL)
BEGIN
	ALTER TABLE dbo.USER_LOGIN
	ADD CONSTRAINT FK_USER_LOGIN_DEVICE FOREIGN KEY (ID_Device) REFERENCES dbo.CAT_DEVICE(ID_Device) ON DELETE NO ACTION ON UPDATE NO ACTION
END


IF NOT EXISTS (SELECT TOP 1 [User_Name] FROM dbo.CAT_USER WHERE [User_Name] = 'PA_Admin')
BEGIN
	INSERT INTO dbo.CAT_USER ([User_Name], [Password], [Name], Last_Name, Email, Photo, BirthDate,Alteration_Counter, ID_User_Created, Creation_Date, Active)
	VALUES ('PA_Admin', '', 'Super', 'User', 'pa_admin@processautomation.com', NULL, NULL, 1, 0, GETDATE(), 1)
END

IF NOT EXISTS (SELECT TOP 1 Status_Type_Code FROM dbo.CAT_STATUS_TYPE WHERE Status_Type_Code IN ('S08EA12N'))
BEGIN
	INSERT INTO dbo.CAT_STATUS_TYPE (Status_Type, Status_Type_Code, Alteration_Counter, ID_User_Created, Creation_Date, Active) VALUES ('AUTHENTICATION', 'S08EA12N', 1, (SELECT TOP 1 ID_User FROM dbo.CAT_USER WHERE [User_Name] = 'PA_Admin'), GETDATE(), 1)
END

IF NOT EXISTS (SELECT TOP 1 Token_Type_Code FROM dbo.CAT_TOKEN_TYPE WHERE Token_Type_Code IN ('T07EA12N'))
BEGIN
	INSERT INTO dbo.CAT_TOKEN_TYPE (Token_Type, Token_Type_Code, Alteration_Counter, ID_User_Created, Creation_Date, Active) VALUES ('AUTHENTICATION','T07EA12N',1,(SELECT TOP 1 ID_User FROM dbo.CAT_USER WHERE [User_Name] = 'PA_Admin'),GETDATE(),1)
END

IF NOT EXISTS (SELECT TOP 1 Status FROM dbo.CAT_STATUS WHERE Status IN ('LOGIN', 'LOGOUT'))
BEGIN
	INSERT INTO dbo.CAT_STATUS(Status,Number,ID_Status_Type, Alteration_Counter, ID_User_Created, Creation_Date,Active) VALUES('LOGIN',1,(SELECT TOP 1 ID_Status_Type FROM CAT_STATUS_TYPE WHERE Status_Type_Code = 'S08EA12N'), 1, (SELECT TOP 1 ID_User FROM dbo.CAT_USER WHERE [User_Name] = 'PA_Admin'), GETDATE(),1)
	INSERT INTO dbo.CAT_STATUS(Status,Number,ID_Status_Type, Alteration_Counter, ID_User_Created, Creation_Date,Active) VALUES('LOGOUT',10,(SELECT TOP 1 ID_Status_Type FROM CAT_STATUS_TYPE WHERE Status_Type_Code = 'S08EA12N'), 1, (SELECT TOP 1 ID_User FROM dbo.CAT_USER WHERE [User_Name] = 'PA_Admin'), GETDATE(),1)
END

IF NOT EXISTS (SELECT TOP 1 Device_Code FROM dbo.CAT_DEVICE WHERE Device_Code IN ('D04CD05P', 'D04CM04E', 'D04CW01B'))
BEGIN
	INSERT INTO dbo.CAT_DEVICE (Device,Device_Code,Alteration_Counter,ID_User_Created,Creation_Date,Active)
	VALUES ('DESKTOP','D04CD05P',1,(SELECT TOP 1 ID_User FROM dbo.CAT_USER WHERE [User_Name] = 'PA_Admin'),GETDATE(),1)

	INSERT INTO dbo.CAT_DEVICE (Device,Device_Code,Alteration_Counter,ID_User_Created,Creation_Date,Active)
	VALUES ('MOBILE','D04CM04E',1,(SELECT TOP 1 ID_User FROM dbo.CAT_USER WHERE [User_Name] = 'PA_Admin'),GETDATE(),1)

	INSERT INTO dbo.CAT_DEVICE (Device,Device_Code,Alteration_Counter,ID_User_Created,Creation_Date,Active)
	VALUES ('WEB','D04CW01B',1,(SELECT TOP 1 ID_User FROM dbo.CAT_USER WHERE [User_Name] = 'PA_Admin'),GETDATE(),1)
END

------ BEGIN: ROLLBACK ------

--DROP TABLE dbo.MESSAGE_ADMIN
--DROP TABLE dbo.USER_LOGIN
--DROP TABLE dbo.TOKEN
--DROP TABLE dbo.CAT_DEVICE
--DROP TABLE dbo.CAT_STATUS
--DROP TABLE dbo.CAT_USER
--DROP TABLE dbo.CAT_TOKEN_TYPE
--DROP TABLE dbo.CAT_STATUS_TYPE

------ END: ROLLBACK ------