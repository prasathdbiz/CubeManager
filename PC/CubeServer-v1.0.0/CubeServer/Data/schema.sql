-- CubeMgr database schema for CubeServer
-- Generated from Data/Database.cs SQL usage. Run against an empty "CubeMgr" database.

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
CREATE TABLE Users (
    UserId          VARCHAR(50)     NOT NULL PRIMARY KEY,
    UserName        NVARCHAR(200)   NULL,
    WindowsID       NVARCHAR(200)   NULL,
    Privilege       INT             NOT NULL DEFAULT 0,
    Password        NVARCHAR(200)   NULL,
    Enabled         TINYINT         NOT NULL DEFAULT 1,
    Tel             NVARCHAR(50)    NULL,
    Mobile          NVARCHAR(50)    NULL,
    Email           NVARCHAR(200)   NULL,
    Secret          NVARCHAR(200)   NULL,
    LastLogin       DATETIME        NULL,
    LastUpdate      DATETIME        NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Settings')
CREATE TABLE Settings (
    Name    VARCHAR(100)    NOT NULL PRIMARY KEY,
    Value   NVARCHAR(MAX)   NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Session')
CREATE TABLE Session (
    SessionId       VARCHAR(100)    NOT NULL PRIMARY KEY,
    UserId          VARCHAR(50)     NULL,
    SessionData     NVARCHAR(MAX)   NULL,
    LastUpdate      DATETIME        NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tokens')
CREATE TABLE Tokens (
    UserId      VARCHAR(50)     NOT NULL,
    Token       VARCHAR(200)    NOT NULL PRIMARY KEY,
    Expiry      DATETIME        NULL,
    LastAccess  DATETIME        NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ActivityLog')
CREATE TABLE ActivityLog (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Activity    NVARCHAR(500)   NULL,
    TableName   VARCHAR(100)    NULL,
    UserId      VARCHAR(50)     NULL,
    LogDateTime DATETIME        NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Quotations')
CREATE TABLE Quotations (
    QuoNum          INT             NOT NULL PRIMARY KEY,
    BillToParty     NVARCHAR(200)   NULL,
    SoldToParty     NVARCHAR(200)   NULL,
    QuoDate         DATETIME        NULL,
    CustomerNum     BIGINT          NULL,
    Currency        VARCHAR(10)     NULL,
    AttnName        NVARCHAR(200)   NULL,
    AttnTel         VARCHAR(50)     NULL,
    AttnMobile      VARCHAR(50)     NULL,
    AttnFax         VARCHAR(50)     NULL,
    AttnEmail       NVARCHAR(200)   NULL,
    QuoSubject      NVARCHAR(500)   NULL,
    Price           FLOAT           NULL,
    PaymentTerms    NVARCHAR(200)   NULL,
    Confirmed       TINYINT         NOT NULL DEFAULT 0,
    Filename        NVARCHAR(300)   NULL,
    UploadUser      VARCHAR(50)     NULL,
    Uploaded        DATETIME        NULL,
    LastUpdateUser  VARCHAR(50)     NULL,
    LastUpdate      DATETIME        NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Projects')
CREATE TABLE Projects (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,
    PaymentCode             VARCHAR(1)      NOT NULL DEFAULT 'C',
    CurSCONum               INT             NOT NULL DEFAULT 0,
    Quotation               INT             NULL,
    ProjectName             NVARCHAR(300)   NOT NULL,
    BillToCompany           NVARCHAR(300)   NULL,
    BillToAddress           NVARCHAR(500)   NULL,
    SoldToCompany           NVARCHAR(300)   NULL,
    SoldToAddress           NVARCHAR(500)   NULL,
    ReportsRequired         NVARCHAR(200)   NULL,
    ApplicantName           NVARCHAR(200)   NULL,
    ApplicantDesignation    NVARCHAR(200)   NULL,
    EmailAddr               NVARCHAR(MAX)   NULL,
    EmailCC                 NVARCHAR(MAX)   NULL,
    PricePerCube            FLOAT           NULL,
    CreateUser              VARCHAR(50)     NULL,
    CreateDate              DATETIME        NULL,
    LastUpdateUser          VARCHAR(50)     NULL,
    LastUpdate              DATETIME        NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SCONumbers')
CREATE TABLE SCONumbers (
    SCONum          INT             NOT NULL PRIMARY KEY,
    ProjectId       INT             NOT NULL,
    LastUpdateUser  VARCHAR(50)     NULL,
    LastUpdate      DATETIME        NULL DEFAULT GETDATE(),
    CONSTRAINT FK_SCONumbers_Projects FOREIGN KEY (ProjectId) REFERENCES Projects(Id)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Suppliers')
CREATE TABLE Suppliers (
    Id              VARCHAR(50)     NOT NULL PRIMARY KEY,
    Name            NVARCHAR(300)   NULL,
    CreateUser      VARCHAR(50)     NULL,
    CreateDate      DATETIME        NULL,
    LastUpdateUser  VARCHAR(50)     NULL,
    LastUpdate      DATETIME        NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CubeSets')
CREATE TABLE CubeSets (
    Id                      INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId               INT             NOT NULL,
    SpecId                  VARCHAR(50)     NULL,
    TestCriteria            VARCHAR(50)     NULL,
    ConcreteGrade           INT             NULL,
    ConcreteType            VARCHAR(50)     NULL,
    CharacteristicStrength  FLOAT           NULL,
    StdDeviation            FLOAT           NULL,
    SupplierId              VARCHAR(50)     NULL,
    Location                NVARCHAR(200)   NULL,
    CastingDate             DATETIME        NULL,
    CreateUser              VARCHAR(50)     NULL,
    CreateDate              DATETIME        NULL,
    LastUpdateUser          VARCHAR(50)     NULL,
    LastUpdate              DATETIME        NULL,
    CONSTRAINT FK_CubeSets_Projects FOREIGN KEY (ProjectId) REFERENCES Projects(Id)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Batches')
CREATE TABLE Batches (
    ScoNum              INT             NOT NULL,
    Id                  INT             NOT NULL,
    CubeSetId           INT             NOT NULL,
    TestAge             INT             NULL,
    TargetTestDate      DATETIME        NULL,
    Dimension           FLOAT           NULL,
    WitnessNum          INT             NULL,
    AvgStrength         FLOAT           NULL,
    RollingAvgStrength  FLOAT           NULL,
    CriterionA          CHAR(1)         NULL,
    CriterionB          CHAR(1)         NULL,
    CreateUser          VARCHAR(50)     NULL,
    CreateDate          DATETIME        NULL,
    LastUpdateUser      VARCHAR(50)     NULL,
    LastUpdate          DATETIME        NULL,
    CONSTRAINT PK_Batches PRIMARY KEY (ScoNum, Id),
    CONSTRAINT FK_Batches_CubeSets FOREIGN KEY (CubeSetId) REFERENCES CubeSets(Id)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cubes')
CREATE TABLE Cubes (
    Id                  BIGINT IDENTITY(1,1) PRIMARY KEY,
    Barcode             BIGINT          NOT NULL,
    SampleRef           NVARCHAR(100)   NULL,
    ScoNum              INT             NOT NULL,
    BatchId             INT             NOT NULL,
    MeasuredDimX1       FLOAT           NULL,
    MeasuredDimX2       FLOAT           NULL,
    MeasuredDimX3       FLOAT           NULL,
    MeasuredDimX4       FLOAT           NULL,
    MeasuredDimX5       FLOAT           NULL,
    MeasuredDimX6       FLOAT           NULL,
    MeasuredDimY1       FLOAT           NULL,
    MeasuredDimY2       FLOAT           NULL,
    MeasuredDimY3       FLOAT           NULL,
    MeasuredDimY4       FLOAT           NULL,
    MeasuredDimY5       FLOAT           NULL,
    MeasuredDimY6       FLOAT           NULL,
    AvgDimension        FLOAT           NULL,
    MeasuredMaxForce    FLOAT           NULL,
    MeasuredStrength    FLOAT           NULL,
    MeasuredWeight      FLOAT           NULL,
    MeasuredDensity     FLOAT           NULL,
    TesterId            TINYINT         NULL,
    TestResult          TINYINT         NOT NULL DEFAULT 0,
    StatusCode          VARCHAR(5)      NULL,
    Uploaded            TINYINT         NOT NULL DEFAULT 0,
    ActualTestDate      DATETIME        NULL,
    UploadTime          DATETIME        NULL,
    CreateUser          VARCHAR(50)     NULL,
    CreateDate          DATETIME        NULL,
    LastUpdateUser      VARCHAR(50)     NULL,
    LastUpdate          DATETIME        NULL,
    CONSTRAINT FK_Cubes_Batches FOREIGN KEY (ScoNum, BatchId) REFERENCES Batches(ScoNum, Id)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BarcodeAllocation')
CREATE TABLE BarcodeAllocation (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId       INT             NOT NULL,
    Qty             INT             NULL,
    BarcodeStart    BIGINT          NULL,
    BarcodeEnd      BIGINT          NULL,
    CreateUser      VARCHAR(50)     NULL,
    CreateDate      DATETIME        NULL,
    LastUpdateUser  VARCHAR(50)     NULL,
    LastUpdate      DATETIME        NULL,
    CONSTRAINT FK_BarcodeAllocation_Projects FOREIGN KEY (ProjectId) REFERENCES Projects(Id)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TestSpecs')
CREATE TABLE TestSpecs (
    SpecId          VARCHAR(50)     NOT NULL PRIMARY KEY,
    Description     NVARCHAR(300)   NULL,
    TestStandard    NVARCHAR(200)   NULL,
    LastUpdateUser  VARCHAR(50)     NULL,
    LastUpdate      DATETIME        NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reports')
CREATE TABLE Reports (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    ProjectId       INT             NOT NULL,
    ReportType      VARCHAR(50)     NULL,
    FileName        NVARCHAR(300)   NULL,
    ReportDate      DATETIME        NULL,
    StartDate       DATETIME        NULL,
    EndDate         DATETIME        NULL,
    EmailTo         NVARCHAR(MAX)   NULL,
    EmailCC         NVARCHAR(MAX)   NULL,
    Released        TINYINT         NOT NULL DEFAULT 0,
    ReleaseUser     VARCHAR(50)     NULL,
    ReleaseDate     DATETIME        NULL,
    CONSTRAINT FK_Reports_Projects FOREIGN KEY (ProjectId) REFERENCES Projects(Id)
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScheduledTasks')
CREATE TABLE ScheduledTasks (
    Name        VARCHAR(100)    NOT NULL PRIMARY KEY,
    DayOfMonth  INT             NULL,
    Hour        INT             NULL,
    Minute      INT             NULL,
    LastRun     DATETIME        NULL,
    NextRun     DATETIME        NULL
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConcreteGrades')
CREATE TABLE ConcreteGrades (
    Grade   INT NOT NULL PRIMARY KEY
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TestCriteria')
CREATE TABLE TestCriteria (
    Criterion   VARCHAR(50) NOT NULL PRIMARY KEY
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConcreteTypes')
CREATE TABLE ConcreteTypes (
    Type    VARCHAR(50) NOT NULL PRIMARY KEY
);
