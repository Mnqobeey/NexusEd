/*
Run this from the project root with sqlcmd variables for the MDF/LDF paths, for example:

sqlcmd -S "(LocalDB)\MSSQLLocalDB" -E -v DatabaseName="NexusEdFixedRoslynLocal" MdfPath="C:\path\to\NexusEd\App_Data\NexusEdLocal.mdf" LdfPath="C:\path\to\NexusEd\App_Data\NexusEdLocal_log.ldf" -i DatabaseSetup.sql
*/

IF DB_ID(N'$(DatabaseName)') IS NULL
BEGIN
    CREATE DATABASE [$(DatabaseName)]
    ON PRIMARY
    (
        NAME = N'$(DatabaseName)',
        FILENAME = N'$(MdfPath)'
    )
    LOG ON
    (
        NAME = N'$(DatabaseName)_log',
        FILENAME = N'$(LdfPath)'
    );
END
GO

USE [$(DatabaseName)];
GO

IF OBJECT_ID(N'dbo.STUDENT', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.STUDENT
    (
        StudentID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_STUDENT PRIMARY KEY,
        StudentName NVARCHAR(50) NULL,
        StudentSurname NVARCHAR(50) NULL,
        StudentUsername NVARCHAR(50) NOT NULL CONSTRAINT UQ_STUDENT_StudentUsername UNIQUE,
        StudentPassword NVARCHAR(50) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.ADMIN_COORDINATOR', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ADMIN_COORDINATOR
    (
        AdminID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ADMIN_COORDINATOR PRIMARY KEY,
        AdminName NVARCHAR(50) NULL,
        AdminSurname NVARCHAR(50) NULL,
        AdminUsername NVARCHAR(50) NOT NULL CONSTRAINT UQ_ADMIN_COORDINATOR_AdminUsername UNIQUE,
        AdminPassword NVARCHAR(50) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.CATEGORY', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CATEGORY
    (
        CategoryID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CATEGORY PRIMARY KEY,
        CategoryType NVARCHAR(50) NOT NULL CONSTRAINT UQ_CATEGORY_CategoryType UNIQUE
    );
END
GO

IF OBJECT_ID(N'dbo.FEEDBACK_QUESTIONS', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FEEDBACK_QUESTIONS
    (
        QuestionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FEEDBACK_QUESTIONS PRIMARY KEY,
        Question NVARCHAR(255) NOT NULL,
        CategoryID INT NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.MODULES', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MODULES
    (
        ModuleID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MODULES PRIMARY KEY,
        ModuleCode NVARCHAR(20) NOT NULL,
        ModuleName NVARCHAR(100) NOT NULL,
        ModuleYear INT NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.STUDENT_MODULES', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.STUDENT_MODULES
    (
        StudentID INT NOT NULL,
        ModuleID INT NOT NULL,
        CONSTRAINT PK_STUDENT_MODULES PRIMARY KEY (StudentID, ModuleID)
    );
END
GO

IF OBJECT_ID(N'dbo.LECTURER', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LECTURER
    (
        LecturerID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LECTURER PRIMARY KEY,
        LecturerName NVARCHAR(50) NOT NULL,
        LecturerSurname NVARCHAR(50) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.MODULE_LECTURER', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MODULE_LECTURER
    (
        ModuleID INT NOT NULL,
        LecturerID INT NOT NULL,
        CONSTRAINT PK_MODULE_LECTURER PRIMARY KEY (ModuleID, LecturerID)
    );
END
GO

IF OBJECT_ID(N'dbo.TUTOR', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TUTOR
    (
        TutorID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TUTOR PRIMARY KEY,
        TutorName NVARCHAR(50) NOT NULL,
        TutorSurname NVARCHAR(50) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.TUTOR_MODULES', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TUTOR_MODULES
    (
        TutorID INT NOT NULL,
        ModuleID INT NOT NULL,
        CONSTRAINT PK_TUTOR_MODULES PRIMARY KEY (TutorID, ModuleID)
    );
END
GO

IF OBJECT_ID(N'dbo.FACILITY', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FACILITY
    (
        FacilityID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FACILITY PRIMARY KEY,
        FaciltyName NVARCHAR(100) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.FEEDBACK', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FEEDBACK
    (
        FeedbackID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FEEDBACK PRIMARY KEY,
        [FeedbackRating Line1] INT NOT NULL,
        [FeedbackRating Line2] INT NOT NULL,
        [FeedbackRating Line3] INT NOT NULL,
        [Comment] NVARCHAR(MAX) NULL,
        FeedbackDate DATETIME NOT NULL,
        StudentID INT NOT NULL,
        CategoryID INT NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.CK_FEEDBACK_RatingLine1_Range', N'C') IS NULL
BEGIN
    ALTER TABLE dbo.FEEDBACK
    ADD CONSTRAINT CK_FEEDBACK_RatingLine1_Range CHECK ([FeedbackRating Line1] BETWEEN 1 AND 5);
END
GO

IF OBJECT_ID(N'dbo.CK_FEEDBACK_RatingLine2_Range', N'C') IS NULL
BEGIN
    ALTER TABLE dbo.FEEDBACK
    ADD CONSTRAINT CK_FEEDBACK_RatingLine2_Range CHECK ([FeedbackRating Line2] BETWEEN 1 AND 5);
END
GO

IF OBJECT_ID(N'dbo.CK_FEEDBACK_RatingLine3_Range', N'C') IS NULL
BEGIN
    ALTER TABLE dbo.FEEDBACK
    ADD CONSTRAINT CK_FEEDBACK_RatingLine3_Range CHECK ([FeedbackRating Line3] BETWEEN 1 AND 5);
END
GO

IF OBJECT_ID(N'dbo.MODULE_FEEDBACK', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.MODULE_FEEDBACK
    (
        FeedbackID INT NOT NULL,
        ModuleID INT NOT NULL,
        CONSTRAINT PK_MODULE_FEEDBACK PRIMARY KEY (FeedbackID, ModuleID)
    );
END
GO

IF OBJECT_ID(N'dbo.LECTURER_FEEDBACK', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LECTURER_FEEDBACK
    (
        FeedbackID INT NOT NULL,
        LecturerID INT NOT NULL,
        CONSTRAINT PK_LECTURER_FEEDBACK PRIMARY KEY (FeedbackID, LecturerID)
    );
END
GO

IF OBJECT_ID(N'dbo.TUTOR_FEEDBACK', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TUTOR_FEEDBACK
    (
        FeedbackID INT NOT NULL,
        TutorID INT NOT NULL,
        CONSTRAINT PK_TUTOR_FEEDBACK PRIMARY KEY (FeedbackID, TutorID)
    );
END
GO

IF OBJECT_ID(N'dbo.ADMIN_FEEDBACK', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ADMIN_FEEDBACK
    (
        FeedbackID INT NOT NULL,
        AdminID INT NOT NULL,
        CONSTRAINT PK_ADMIN_FEEDBACK PRIMARY KEY (FeedbackID, AdminID)
    );
END
GO

IF OBJECT_ID(N'dbo.FACILITY_FEEDBACK', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.FACILITY_FEEDBACK
    (
        FeedbackID INT NOT NULL,
        FacilityID INT NOT NULL,
        CONSTRAINT PK_FACILITY_FEEDBACK PRIMARY KEY (FeedbackID, FacilityID)
    );
END
GO

IF EXISTS (SELECT 1 FROM dbo.STUDENT WHERE StudentUsername = N'Thando')
BEGIN
    UPDATE dbo.STUDENT SET StudentPassword = N'123' WHERE StudentUsername = N'Thando';
END
ELSE
BEGIN
    INSERT INTO dbo.STUDENT (StudentName, StudentSurname, StudentUsername, StudentPassword)
    VALUES (N'Thando', N'', N'Thando', N'123');
END

IF EXISTS (SELECT 1 FROM dbo.ADMIN_COORDINATOR WHERE AdminUsername = N'VanCarol')
BEGIN
    UPDATE dbo.ADMIN_COORDINATOR SET AdminPassword = N'124' WHERE AdminUsername = N'VanCarol';
END
ELSE
BEGIN
    INSERT INTO dbo.ADMIN_COORDINATOR (AdminName, AdminSurname, AdminUsername, AdminPassword)
    VALUES (N'Van', N'Carol', N'VanCarol', N'124');
END

DECLARE @Categories TABLE (CategoryType NVARCHAR(50) NOT NULL);
INSERT INTO @Categories (CategoryType)
VALUES (N'Administration'), (N'Lecturers'), (N'Modules'), (N'Tutors'), (N'Facilities');

INSERT INTO dbo.CATEGORY (CategoryType)
SELECT c.CategoryType
FROM @Categories c
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.CATEGORY existing
    WHERE existing.CategoryType = c.CategoryType
);

IF NOT EXISTS (SELECT 1 FROM dbo.MODULES WHERE ModuleCode = N'PRG101')
BEGIN
    INSERT INTO dbo.MODULES (ModuleCode, ModuleName, ModuleYear)
    VALUES (N'PRG101', N'Programming Fundamentals', 2026);
END

IF NOT EXISTS (SELECT 1 FROM dbo.MODULES WHERE ModuleCode = N'DBS101')
BEGIN
    INSERT INTO dbo.MODULES (ModuleCode, ModuleName, ModuleYear)
    VALUES (N'DBS101', N'Database Systems', 2026);
END

IF NOT EXISTS (SELECT 1 FROM dbo.LECTURER WHERE LecturerName = N'Carol' AND LecturerSurname = N'Van Wyk')
BEGIN
    INSERT INTO dbo.LECTURER (LecturerName, LecturerSurname)
    VALUES (N'Carol', N'Van Wyk');
END

IF NOT EXISTS (SELECT 1 FROM dbo.TUTOR WHERE TutorName = N'Anele' AND TutorSurname = N'Mokoena')
BEGIN
    INSERT INTO dbo.TUTOR (TutorName, TutorSurname)
    VALUES (N'Anele', N'Mokoena');
END

IF NOT EXISTS (SELECT 1 FROM dbo.FACILITY WHERE FaciltyName = N'Library')
BEGIN
    INSERT INTO dbo.FACILITY (FaciltyName)
    VALUES (N'Library');
END

IF NOT EXISTS (SELECT 1 FROM dbo.FACILITY WHERE FaciltyName = N'Computer Lab')
BEGIN
    INSERT INTO dbo.FACILITY (FaciltyName)
    VALUES (N'Computer Lab');
END

DECLARE @ThandoID INT = (SELECT StudentID FROM dbo.STUDENT WHERE StudentUsername = N'Thando');

INSERT INTO dbo.STUDENT_MODULES (StudentID, ModuleID)
SELECT @ThandoID, m.ModuleID
FROM dbo.MODULES m
WHERE m.ModuleCode IN (N'PRG101', N'DBS101')
AND NOT EXISTS
(
    SELECT 1
    FROM dbo.STUDENT_MODULES sm
    WHERE sm.StudentID = @ThandoID
    AND sm.ModuleID = m.ModuleID
);

INSERT INTO dbo.MODULE_LECTURER (ModuleID, LecturerID)
SELECT m.ModuleID, l.LecturerID
FROM dbo.MODULES m
CROSS JOIN dbo.LECTURER l
WHERE m.ModuleCode = N'PRG101'
AND l.LecturerName = N'Carol'
AND l.LecturerSurname = N'Van Wyk'
AND NOT EXISTS
(
    SELECT 1
    FROM dbo.MODULE_LECTURER ml
    WHERE ml.ModuleID = m.ModuleID
    AND ml.LecturerID = l.LecturerID
);

INSERT INTO dbo.TUTOR_MODULES (TutorID, ModuleID)
SELECT t.TutorID, m.ModuleID
FROM dbo.TUTOR t
CROSS JOIN dbo.MODULES m
WHERE t.TutorName = N'Anele'
AND t.TutorSurname = N'Mokoena'
AND m.ModuleCode = N'PRG101'
AND NOT EXISTS
(
    SELECT 1
    FROM dbo.TUTOR_MODULES tm
    WHERE tm.TutorID = t.TutorID
    AND tm.ModuleID = m.ModuleID
);

DECLARE @Questions TABLE
(
    CategoryType NVARCHAR(50) NOT NULL,
    Question NVARCHAR(255) NOT NULL
);

INSERT INTO @Questions (CategoryType, Question)
VALUES
(N'Administration', N'The administration team responded to my request in good time.'),
(N'Administration', N'The administration team communicated clearly.'),
(N'Administration', N'The administration team was helpful.'),
(N'Lecturers', N'The lecturer explained concepts clearly.'),
(N'Lecturers', N'The lecturer was prepared for class.'),
(N'Lecturers', N'The lecturer supported student learning.'),
(N'Modules', N'The module content was clear.'),
(N'Modules', N'The module assessments were fair.'),
(N'Modules', N'The module resources supported my learning.'),
(N'Tutors', N'The tutor explained the work clearly.'),
(N'Tutors', N'The tutor was available when needed.'),
(N'Tutors', N'The tutor sessions helped my understanding.'),
(N'Facilities', N'The facility was available when needed.'),
(N'Facilities', N'The facility was clean and usable.'),
(N'Facilities', N'The facility supported my learning needs.');

INSERT INTO dbo.FEEDBACK_QUESTIONS (Question, CategoryID)
SELECT q.Question, c.CategoryID
FROM @Questions q
INNER JOIN dbo.CATEGORY c ON c.CategoryType = q.CategoryType
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.FEEDBACK_QUESTIONS fq
    WHERE fq.Question = q.Question
    AND fq.CategoryID = c.CategoryID
);
GO
