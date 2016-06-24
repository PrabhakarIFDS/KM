USE TestData
GO

CREATE TABLE dbo.KMP_Z_TopLearners
   (Member nvarchar(100) NULL,
    "MemberStatus" text NULL,
	Displayname text NULL,
    "LastActivity" Timestamp NULL,
    web nvarchar(50) NULL,
    "ReputationScore" int NOT NULL DEFAULT 0,
    Column1 int NOT NULL DEFAULT 0,
    Column2 int NOT NULL DEFAULT 0,
    Column3 int NOT NULL DEFAULT 0,
    Column4 int NOT NULL DEFAULT 0,
    Column5 int NOT NULL DEFAULT 0, 
    Column6 int NOT NULL DEFAULT 0,
    Column7 int NOT NULL DEFAULT 0,
    Column8 int NOT NULL DEFAULT 0,
    Column9 int NOT NULL DEFAULT 0,
    Column10 int NOT NULL DEFAULT 0)
GO

CREATE TABLE dbo.KMP_Z_URLAccessEntries
   (AccessURL text NULL,
    List nvarchar(100) NULL,
    Member nvarchar(100) NOT NULL,
    Displayname text NULL,
    web nvarchar(50) NULL,
    AccessDate timestamp,
    ProcessFlag bit NOT NULL DEFAULT 0,
    ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY)
GO

CREATE TABLE dbo.KMP_Z_ProcessLists
   (Listname nvarchar(100) NULL,
    Columnname nvarchar(100) NULL,
    AddtoReputationScore bit)
GO

INSERT INTO [dbo].[KMP_Z_URLAccessEntries]
           ([AccessURL]
           ,[List]
           ,[Member]
           ,[Displayname]
           ,[web]
           ,[ProcessFlag])
     VALUES
	('http://sharepointsite','FAQ','guest137','Rajkumar R','Development','FALSE'),
	('http://sharepointsite','FAQ','guest137','Rajkumar R','Development','FALSE'),
	('http://sharepointsite','FAQ','guest137','Rajkumar R','Development','FALSE'),
	('http://sharepointsite','Site Pages','guest137','Rajkumar R','Development','FALSE'),
	('http://sharepointsite','Site Pages','guest137','Rajkumar R','Development','FALSE'),
	('http://sharepointsite','Site Pages','guest137','Rajkumar R','Development','FALSE'),
	('http://sharepointsite','Discussions','guest137','Rajkumar R','Development','FALSE'),	
	('http://sharepointsite','Discussions','guest137','Rajkumar R','Development','FALSE'),
	('http://sharepointsite','Discussions','guest137','Rajkumar R','Development','FALSE'),
	('http://sharepointsite','Documents','guest137','Rajkumar R','Development','FALSE'),	
	('http://sharepointsite','Documents','guest137','Rajkumar R','Development','FALSE'),
	('http://sharepointsite','Documents','guest137','Rajkumar R','Development','FALSE'),
	('http://sharepointsite','FAQ','guest138','Michael Cardinal','Development','FALSE'),
	('http://sharepointsite','FAQ','guest138','Michael Cardinal','Development','FALSE'),
	('http://sharepointsite','FAQ','guest138','Michael Cardinal','Development','FALSE'),
	('http://sharepointsite','Site Pages','guest138','Michael Cardinal','Development','FALSE'),
	('http://sharepointsite','Site Pages','guest138','Michael Cardinal','Development','FALSE'),
	('http://sharepointsite','Site Pages','guest138','Michael Cardinal','Development','FALSE'),
	('http://sharepointsite','Discussions','guest138','Michael Cardinal','Development','FALSE'),	
	('http://sharepointsite','Discussions','guest138','Michael Cardinal','Development','FALSE'),
	('http://sharepointsite','Discussions','guest138','Michael Cardinal','Development','FALSE'),
	('http://sharepointsite','Documents','guest138','Michael Cardinal','Development','FALSE'),	
	('http://sharepointsite','Documents','guest138','Michael Cardinal','Development','FALSE'),
	('http://sharepointsite','Documents','guest138','Michael Cardinal','Development','FALSE'),
	('http://sharepointsite','FAQ','guest137','Rajkumar R','Application Support','FALSE'),
	('http://sharepointsite','FAQ','guest137','Rajkumar R','Application Support','FALSE'),
	('http://sharepointsite','FAQ','guest137','Rajkumar R','Application Support','FALSE'),
	('http://sharepointsite','Site Pages','guest137','Rajkumar R','Application Support','FALSE'),
	('http://sharepointsite','Site Pages','guest137','Rajkumar R','Application Support','FALSE'),
	('http://sharepointsite','Site Pages','guest137','Rajkumar R','Application Support','FALSE'),
	('http://sharepointsite','Discussions','guest137','Rajkumar R','Application Support','FALSE'),	
	('http://sharepointsite','Discussions','guest137','Rajkumar R','Application Support','FALSE'),
	('http://sharepointsite','Discussions','guest137','Rajkumar R','Application Support','FALSE'),
	('http://sharepointsite','Documents','guest137','Rajkumar R','Application Support','FALSE'),	
	('http://sharepointsite','Documents','guest137','Rajkumar R','Application Support','FALSE'),
	('http://sharepointsite','Documents','guest137','Rajkumar R','Application Support','FALSE'),
	('http://sharepointsite','FAQ','guest138','Michael Cardinal','Application Support','FALSE'),
	('http://sharepointsite','FAQ','guest138','Michael Cardinal','Application Support','FALSE'),
	('http://sharepointsite','FAQ','guest138','Michael Cardinal','Application Support','FALSE'),
	('http://sharepointsite','Site Pages','guest138','Michael Cardinal','Application Support','FALSE'),
	('http://sharepointsite','Site Pages','guest138','Michael CardinalR','Application Support','FALSE'),
	('http://sharepointsite','Site Pages','guest138','Michael Cardinal','Application Support','FALSE'),
	('http://sharepointsite','Discussions','guest138','Michael Cardinal','Application Support','FALSE'),	
	('http://sharepointsite','Discussions','guest138','Michael Cardinal','Application Support','FALSE'),
	('http://sharepointsite','Discussions','guest138','Michael Cardinal','Application Support','FALSE'),
	('http://sharepointsite','Documents','guest138','Michael Cardinal','Application Support','FALSE'),	
	('http://sharepointsite','Documents','guest138','Michael Cardinal','Application Support','FALSE'),
	('http://sharepointsite','Documents','guest138','Michael Cardinal','Application Support','FALSE')
GO

INSERT INTO [dbo].[KMP_Z_ProcessLists]
           ([Listname]
           ,[Columnname]
           ,[AddtoReputationScore])
     VALUES
           ('Discussions List','Column1','TRUE'),
	       ('Site Pages','Column2','TRUE'),
           ('Pages','Column3','TRUE'),
		   ('Community Members','Column4','TRUE')
GO