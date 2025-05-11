-- Set the required options
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

-- Add missing columns to AspNetUsers table, checking if they exist first
IF COL_LENGTH('AspNetUsers', 'Address') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD Address NVARCHAR(100) NULL;
END

IF COL_LENGTH('AspNetUsers', 'City') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD City NVARCHAR(50) NULL;
END

IF COL_LENGTH('AspNetUsers', 'State') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD State NVARCHAR(50) NULL;
END

IF COL_LENGTH('AspNetUsers', 'Country') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD Country NVARCHAR(50) NULL;
END

IF COL_LENGTH('AspNetUsers', 'PostalCode') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD PostalCode NVARCHAR(20) NULL;
END

IF COL_LENGTH('AspNetUsers', 'DateOfBirth') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD DateOfBirth DATETIME2 NULL;
END

IF COL_LENGTH('AspNetUsers', 'Gender') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD Gender INT NULL;
END

IF COL_LENGTH('AspNetUsers', 'LastLoginDate') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD LastLoginDate DATETIME2 NULL;
END

IF COL_LENGTH('AspNetUsers', 'ProfilePicture') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD ProfilePicture NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('AspNetUsers', 'CreatedBy') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD CreatedBy NVARCHAR(450) NULL;
END

IF COL_LENGTH('AspNetUsers', 'UpdatedBy') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD UpdatedBy NVARCHAR(450) NULL;
END

IF COL_LENGTH('AspNetUsers', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE();
END

IF COL_LENGTH('AspNetUsers', 'UpdatedAt') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD UpdatedAt DATETIME2 NULL;
END

-- Student related fields
IF COL_LENGTH('AspNetUsers', 'StudentId') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD StudentId NVARCHAR(20) NULL;
END

IF COL_LENGTH('AspNetUsers', 'University') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD University NVARCHAR(100) NULL;
END

IF COL_LENGTH('AspNetUsers', 'Course') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD Course NVARCHAR(100) NULL;
END

IF COL_LENGTH('AspNetUsers', 'EmergencyContactName') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD EmergencyContactName NVARCHAR(50) NULL;
END

IF COL_LENGTH('AspNetUsers', 'EmergencyContactPhone') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD EmergencyContactPhone NVARCHAR(20) NULL;
END

IF COL_LENGTH('AspNetUsers', 'EmergencyContactRelationship') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD EmergencyContactRelationship NVARCHAR(50) NULL;
END

-- Manager related fields
IF COL_LENGTH('AspNetUsers', 'IsHostelManager') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD IsHostelManager BIT NOT NULL DEFAULT 0;
END

IF COL_LENGTH('AspNetUsers', 'ManagerLicenseNumber') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD ManagerLicenseNumber NVARCHAR(MAX) NULL;
END

IF COL_LENGTH('AspNetUsers', 'LicenseExpiryDate') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD LicenseExpiryDate DATETIME2 NULL;
END

-- Set the Role column with a default value if it doesn't exist
IF COL_LENGTH('AspNetUsers', 'Role') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD Role INT NOT NULL DEFAULT 1; -- Default to Student (assuming that's value 1)
END

-- Make sure FirstName and LastName columns exist
IF COL_LENGTH('AspNetUsers', 'FirstName') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD FirstName NVARCHAR(50) NOT NULL DEFAULT '';
END

IF COL_LENGTH('AspNetUsers', 'LastName') IS NULL
BEGIN
    ALTER TABLE AspNetUsers ADD LastName NVARCHAR(50) NOT NULL DEFAULT '';
END

-- Create a backup of the values
SELECT Id, HostelId, RoomId INTO #TempUsers FROM AspNetUsers;

-- Drop foreign key constraints if exist
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'FK_AspNetUsers_Hostels_HostelId') AND parent_object_id = OBJECT_ID(N'AspNetUsers'))
BEGIN
    ALTER TABLE AspNetUsers DROP CONSTRAINT FK_AspNetUsers_Hostels_HostelId;
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'FK_AspNetUsers_Rooms_RoomId') AND parent_object_id = OBJECT_ID(N'AspNetUsers'))
BEGIN
    ALTER TABLE AspNetUsers DROP CONSTRAINT FK_AspNetUsers_Rooms_RoomId;
END

-- Drop indexes if exist
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_HostelId' AND object_id = OBJECT_ID(N'AspNetUsers'))
BEGIN
    DROP INDEX IX_AspNetUsers_HostelId ON AspNetUsers;
END

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_RoomId' AND object_id = OBJECT_ID(N'AspNetUsers'))
BEGIN
    DROP INDEX IX_AspNetUsers_RoomId ON AspNetUsers;
END

-- Alter the columns to be integers
ALTER TABLE AspNetUsers ALTER COLUMN HostelId INT NULL;
ALTER TABLE AspNetUsers ALTER COLUMN RoomId INT NULL;

-- Re-create indexes
CREATE INDEX IX_AspNetUsers_HostelId ON AspNetUsers(HostelId);
CREATE INDEX IX_AspNetUsers_RoomId ON AspNetUsers(RoomId);

-- Re-add foreign key constraints
ALTER TABLE AspNetUsers ADD CONSTRAINT FK_AspNetUsers_Hostels_HostelId
    FOREIGN KEY (HostelId) REFERENCES Hostels(Id);

ALTER TABLE AspNetUsers ADD CONSTRAINT FK_AspNetUsers_Rooms_RoomId
    FOREIGN KEY (RoomId) REFERENCES Rooms(Id);

-- Drop temporary table
DROP TABLE #TempUsers;

-- Update StudentId in MaintenanceRequests to be INT
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'FK_MaintenanceRequests_Students_StudentId') AND parent_object_id = OBJECT_ID(N'MaintenanceRequests'))
BEGIN
    ALTER TABLE MaintenanceRequests DROP CONSTRAINT FK_MaintenanceRequests_Students_StudentId;
END

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MaintenanceRequests_StudentId' AND object_id = OBJECT_ID(N'MaintenanceRequests'))
BEGIN
    DROP INDEX IX_MaintenanceRequests_StudentId ON MaintenanceRequests;
END

ALTER TABLE MaintenanceRequests ALTER COLUMN StudentId INT NULL;

CREATE INDEX IX_MaintenanceRequests_StudentId ON MaintenanceRequests(StudentId);

ALTER TABLE MaintenanceRequests ADD CONSTRAINT FK_MaintenanceRequests_Students_StudentId
    FOREIGN KEY (StudentId) REFERENCES Students(Id);

-- Update StudentId in NoticeRecipient to be INT
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'FK_NoticeRecipient_Students_StudentId') AND parent_object_id = OBJECT_ID(N'NoticeRecipient'))
BEGIN
    ALTER TABLE NoticeRecipient DROP CONSTRAINT FK_NoticeRecipient_Students_StudentId;
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'FK_NoticeRecipient_Students_StudentId1') AND parent_object_id = OBJECT_ID(N'NoticeRecipient'))
BEGIN
    ALTER TABLE NoticeRecipient DROP CONSTRAINT FK_NoticeRecipient_Students_StudentId1;
END

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_NoticeRecipient_StudentId' AND object_id = OBJECT_ID(N'NoticeRecipient'))
BEGIN
    DROP INDEX IX_NoticeRecipient_StudentId ON NoticeRecipient;
END

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_NoticeRecipient_StudentId1' AND object_id = OBJECT_ID(N'NoticeRecipient'))
BEGIN
    DROP INDEX IX_NoticeRecipient_StudentId1 ON NoticeRecipient;
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NoticeRecipient' AND COLUMN_NAME = 'StudentId1')
BEGIN
    ALTER TABLE NoticeRecipient DROP COLUMN StudentId1;
END

ALTER TABLE NoticeRecipient ALTER COLUMN StudentId INT NOT NULL;

CREATE INDEX IX_NoticeRecipient_StudentId ON NoticeRecipient(StudentId);

ALTER TABLE NoticeRecipient ADD CONSTRAINT FK_NoticeRecipient_Students_StudentId
    FOREIGN KEY (StudentId) REFERENCES Students(Id);

-- Update StudentId in Payments to be INT
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'FK_Payments_Students_StudentId') AND parent_object_id = OBJECT_ID(N'Payments'))
BEGIN
    ALTER TABLE Payments DROP CONSTRAINT FK_Payments_Students_StudentId;
END

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Payments_StudentId' AND object_id = OBJECT_ID(N'Payments'))
BEGIN
    DROP INDEX IX_Payments_StudentId ON Payments;
END

ALTER TABLE Payments ALTER COLUMN StudentId INT NOT NULL;

CREATE INDEX IX_Payments_StudentId ON Payments(StudentId);

ALTER TABLE Payments ADD CONSTRAINT FK_Payments_Students_StudentId
    FOREIGN KEY (StudentId) REFERENCES Students(Id); 