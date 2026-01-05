USE WasmDB 
GO

BEGIN TRANSACTION;

BEGIN TRY 


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Properties')
BEGIN 

    ALTER TABLE Properties ADD 
    SourceType NVARCHAR(50) NULL,
    VerifiedStoreName NVARCHAR(100) NULL,
    SourceIdentifier NVARCHAR(100) NULL,
    IsVerified BIT DEFAULT 0,
    ExternalReferenceId NVARCHAR(255) NULL, 
    SyncTimestamp DATETIME DEFAULT GETDATE();

END


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TrustedStores')
BEGIN 

    CREATE TABLE TrustedStores (
        Id INT PRIMARY KEY IDENTITY(1,1), 
        SenderId NVARCHAR(50) NOT NULL UNIQUE,
        DisplayName NVARCHAR(100) NOT NULL,
        Category NVARCHAR(50) NULL,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME DEFAULT GETDATE()
    );

    INSERT INTO TrustedStores (SenderId, DisplayName, Category)
    VALUES  
    ('JARIR', 'Jarir Bookstore', 'Electronics'),
    ('AMAZON', 'Amazon SA', 'General Retail'),
    ('NOON', 'Noon.com', 'General Retail'),
    ('EXTRA', 'Extra', 'Electronics'),
    ('M_KORS', 'Michael Kors', 'Fashion');
END


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PendingSync')
BEGIN
    CREATE TABLE PendingSync (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserPhone NVARCHAR(20) NOT NULL,        
        DetectedItemName NVARCHAR(MAX) NOT NULL, 
        SenderId NVARCHAR(50) NOT NULL,          
        SourceType NVARCHAR(50) NOT NULL,        
        Status NVARCHAR(50) DEFAULT 'Pending',   
        RawContent NVARCHAR(MAX) NULL,          
        CreatedAt DATETIME DEFAULT GETDATE(),
        
        CONSTRAINT CHK_Status CHECK (Status IN ('Pending', 'Confirmed', 'Rejected'))
    );
END


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SyncLogs')
BEGIN
    CREATE TABLE SyncLogs (
        Id INT PRIMARY KEY IDENTITY(1,1),
        LogTimestamp DATETIME DEFAULT GETDATE(),
        ActionType NVARCHAR(50) NULL,           
        Status NVARCHAR(50) NULL,               
        Details NVARCHAR(MAX) NULL              
    );
END

COMMIT TRANSACTION;
PRINT 'Wasm DB Updated Successfully.';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT 'An error occurred during the update: ' + ERROR_MESSAGE();
END CATCH