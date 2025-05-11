-- Set the required options
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

-- Drop foreign key constraints if they exist
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'FK_NoticeRecipients_Students_StudentId') AND parent_object_id = OBJECT_ID(N'NoticeRecipients'))
BEGIN
    ALTER TABLE NoticeRecipients DROP CONSTRAINT FK_NoticeRecipients_Students_StudentId;
END

-- Drop index if it exists
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_NoticeRecipients_StudentId' AND object_id = OBJECT_ID(N'NoticeRecipients'))
BEGIN
    DROP INDEX IX_NoticeRecipients_StudentId ON NoticeRecipients;
END

-- Change the StudentId column to NVARCHAR
IF COL_LENGTH('NoticeRecipients', 'StudentId') IS NOT NULL
BEGIN
    -- We need to create a temporary column, move data, drop old column, and rename
    IF COL_LENGTH('NoticeRecipients', 'StudentId_New') IS NULL
    BEGIN
        ALTER TABLE NoticeRecipients ADD StudentId_New NVARCHAR(450) NOT NULL DEFAULT '';
    END

    -- Update the new column with the string values from the old column
    UPDATE NoticeRecipients SET StudentId_New = CAST(StudentId AS NVARCHAR(450));

    -- Now drop the old column and rename the new one
    DECLARE @sql NVARCHAR(MAX) = 'ALTER TABLE NoticeRecipients DROP COLUMN StudentId';
    EXEC sp_executesql @sql;

    EXEC sp_rename 'NoticeRecipients.StudentId_New', 'StudentId', 'COLUMN';
END

-- Recreate the index and foreign key if needed
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Students')
BEGIN
    CREATE INDEX IX_NoticeRecipients_StudentId ON NoticeRecipients(StudentId);
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Students' AND COLUMN_NAME = 'Id')
    BEGIN
        ALTER TABLE NoticeRecipients ADD CONSTRAINT FK_NoticeRecipients_Students_StudentId 
            FOREIGN KEY (StudentId) REFERENCES Students(Id);
    END
END 