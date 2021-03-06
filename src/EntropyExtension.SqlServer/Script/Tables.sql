﻿
CREATE TABLE [dbo].[EntropyLog]
(
	LogId           UNIQUEIDENTIFIER NOT NULL,
	LogLevel        NVARCHAR(15)     NOT NULL,
	ApplicationName NVARCHAR(100)    NULL,
	Name            NVARCHAR(100)    NULL,
	LocalTime       DATETIME         NOT NULL,
	Exception       NVARCHAR(MAX)    NOT NULL,
	StateInfo       NVARCHAR(MAX)    NULL,
	MachineName     NVARCHAR(100)    NOT NULL,
)
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[EntropyLog] ADD 
CONSTRAINT [PK_EntropyLog_LogId] PRIMARY KEY CLUSTERED (LogId) ON [PRIMARY] 
GO


CREATE TABLE [dbo].[EntropyLog_HttpInfo]
(
	HttpInfoId  UNIQUEIDENTIFIER NOT NULL,
	LogId       UNIQUEIDENTIFIER NOT NULL,
	ContentType NVARCHAR(100)    NULL,
	Cookies     NVARCHAR(MAX)    NULL,
	Headers     NVARCHAR(MAX)    NULL,
	Host        NVARCHAR(50)     NULL,
	Method      NVARCHAR(10)     NULL,
	[Path]      NVARCHAR(MAX)    NULL,
	Protocol    NVARCHAR(MAX)    NULL,
	Query       NVARCHAR(MAX)    NULL,
	RequestID   NVARCHAR(100)    NULL,
	Scheme      NVARCHAR(10)     NULL,
	StatusCode  INT              NOT NULL,
	[User]      NVARCHAR(100)    NULL
)  
ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[EntropyLog_HttpInfo] ADD 
CONSTRAINT [PK_EntropyLog_HttpInfo_HttpInfoId] PRIMARY KEY CLUSTERED (HttpInfoId) ON [PRIMARY] 
GO


ALTER TABLE dbo.EntropyLog_HttpInfo ADD CONSTRAINT
	FK_EntropyLog_HttpInfo_EntropyLog FOREIGN KEY
	(
	LogId
	) REFERENCES dbo.EntropyLog
	(
	LogId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
