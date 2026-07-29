-- Dummy/seed data for CubeMgr database (CubeServer app)
-- Run AFTER schema.sql, against the CubeMgr database.
USE CubeMgr;
GO

-- ============ Settings ============
IF NOT EXISTS (SELECT 1 FROM Settings WHERE Name = 'CompanyName')
INSERT INTO Settings (Name, Value) VALUES
    ('CompanyName', 'TUV SUD PSB Pte Ltd'),
    ('DailyReportHour', '18'),
    ('MonthlyReportDay', '1'),
    ('DefaultCurrency', 'SGD');
GO

-- ============ Reference/lookup data ============
IF NOT EXISTS (SELECT 1 FROM ConcreteGrades)
INSERT INTO ConcreteGrades (Grade) VALUES (20), (25), (30), (35), (40), (45), (50);

IF NOT EXISTS (SELECT 1 FROM TestCriteria)
INSERT INTO TestCriteria (Criterion) VALUES ('A'), ('B');

IF NOT EXISTS (SELECT 1 FROM ConcreteTypes)
INSERT INTO ConcreteTypes (Type) VALUES ('Normal'), ('High Strength'), ('Lightweight'), ('Self Compacting');

IF NOT EXISTS (SELECT 1 FROM TestSpecs)
INSERT INTO TestSpecs (SpecId, Description, TestStandard, LastUpdateUser, LastUpdate) VALUES
    ('S206', 'Compressive Strength of Concrete Cubes', 'BS EN 12390-3', 'admin', GETDATE()),
    ('S207', 'Density of Hardened Concrete', 'BS EN 12390-7', 'admin', GETDATE());
GO

-- ============ Users ============
-- Passwords hashed with SHA256({128,99,privilege} + ASCII(password)), base64 (matches Util.EncryptPassword)
IF NOT EXISTS (SELECT 1 FROM Users WHERE UserId = 'admin')
INSERT INTO Users (UserId, UserName, WindowsID, Privilege, Password, Enabled, Tel, Mobile, Email, LastUpdate) VALUES
    ('admin',     'System Administrator', '',  100, 'Jc6u3XG4ap4AyTJNe0eipGER2lCgFujskQ472nJy8ow=', 1, '65001000', '91234567', 'admin@cubemgr.local', GETDATE()),
    ('dataentry', 'Data Entry User',       '',  20,  'b2AdJRDxnnwKFiiVnPvotBcsIBCw854eH8TWHkbKtWY=', 1, '65001001', '91234568', 'dataentry@cubemgr.local', GETDATE()),
    ('custsvc',   'Customer Service User', '',  40,  'Zj61xMYoyMGBr9cH2kTxpQUSknn0CKDLcubx/UZMr7U=', 1, '65001002', '91234569', 'custsvc@cubemgr.local', GETDATE()),
    ('webapi',    'Web API Service User',  '',  50,  'IQdFckF2Nj4nqgiCiqiZRDsApDeo2t2/HTMOixIx+Fg=', 1, '65001003', '91234570', 'webapi@cubemgr.local', GETDATE());
-- Login credentials (plaintext, for testing only):
--   admin / Admin@123          (Administrator)
--   dataentry / DataEntry@123  (DataEntry)
--   custsvc / CustSvc@123      (CustomerService)
--   webapi / WebApi@123        (WebApi)
GO

-- ============ Suppliers ============
IF NOT EXISTS (SELECT 1 FROM Suppliers WHERE Id = 'SUP001')
INSERT INTO Suppliers (Id, Name, CreateUser, CreateDate, LastUpdateUser, LastUpdate) VALUES
    ('SUP001', 'ABC Ready Mix Concrete Pte Ltd', 'admin', GETDATE(), 'admin', GETDATE()),
    ('SUP002', 'Pan-Asia Concrete Supplies Pte Ltd', 'admin', GETDATE(), 'admin', GETDATE()),
    ('SUP003', 'Straits Premix Pte Ltd', 'admin', GETDATE(), 'admin', GETDATE());
GO

-- ============ Projects, Quotations, SCONumbers, CubeSets, Batches, Cubes ============
DECLARE @ProjId1 INT, @ProjId2 INT, @ProjId3 INT;
DECLARE @CS1 INT, @CS2 INT, @CS3 INT;

IF NOT EXISTS (SELECT 1 FROM Quotations WHERE QuoNum = 5001)
INSERT INTO Quotations (QuoNum, BillToParty, SoldToParty, QuoDate, CustomerNum, Currency,
    AttnName, AttnTel, AttnMobile, AttnFax, AttnEmail, QuoSubject, Price, PaymentTerms,
    Confirmed, Filename, UploadUser, Uploaded, LastUpdateUser, LastUpdate) VALUES
    (5001, 'Marina Bay Builders Pte Ltd', 'Marina Bay Builders Pte Ltd', DATEADD(day,-60,GETDATE()), 100001, 'SGD',
     'Tan Wei Ming', '65551111', '91112222', '', 'wm.tan@marinabay.example', 'Cube Testing - Marina Bay Tower', 15000.00, '30 Days',
     1, '', 'admin', GETDATE(), 'admin', GETDATE()),
    (5002, 'Orchard Heights Development', 'Orchard Heights Development', DATEADD(day,-45,GETDATE()), 100002, 'SGD',
     'Sarah Lim', '65552222', '91113333', '', 'sarah.lim@orchardheights.example', 'Cube Testing - Orchard Heights Condo', 22000.00, '30 Days',
     1, '', 'admin', GETDATE(), 'admin', GETDATE()),
    (5003, 'Jurong Industrial Estate Pte Ltd', 'Jurong Industrial Estate Pte Ltd', DATEADD(day,-30,GETDATE()), 100003, 'SGD',
     'Ahmad Rizal', '65553333', '91114444', '', 'ahmad.rizal@jurongie.example', 'Cube Testing - Jurong Warehouse Extension', 9800.00, 'Cash',
     1, '', 'admin', GETDATE(), 'admin', GETDATE());

INSERT INTO Projects (PaymentCode, CurSCONum, Quotation, ProjectName, BillToCompany, BillToAddress,
    SoldToCompany, SoldToAddress, ReportsRequired, ApplicantName, ApplicantDesignation,
    EmailAddr, EmailCC, PricePerCube, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES ('P', 0, 5001, 'Marina Bay Tower Construction', 'Marina Bay Builders Pte Ltd', '1 Marina Bay Ave, Singapore 018956',
    'Marina Bay Builders Pte Ltd', '1 Marina Bay Ave, Singapore 018956', 'Daily, Monthly', 'Tan Wei Ming', 'Site Engineer',
    'wm.tan@marinabay.example', 'sarah.lim@orchardheights.example', 45.00, 'admin', GETDATE(), 'admin', GETDATE());
SET @ProjId1 = CAST(SCOPE_IDENTITY() AS INT);

INSERT INTO Projects (PaymentCode, CurSCONum, Quotation, ProjectName, BillToCompany, BillToAddress,
    SoldToCompany, SoldToAddress, ReportsRequired, ApplicantName, ApplicantDesignation,
    EmailAddr, EmailCC, PricePerCube, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES ('P', 0, 5002, 'Orchard Heights Condominium', 'Orchard Heights Development', '88 Orchard Rd, Singapore 238839',
    'Orchard Heights Development', '88 Orchard Rd, Singapore 238839', 'Daily, Statistical', 'Sarah Lim', 'Project Manager',
    'sarah.lim@orchardheights.example', '', 45.00, 'admin', GETDATE(), 'admin', GETDATE());
SET @ProjId2 = CAST(SCOPE_IDENTITY() AS INT);

INSERT INTO Projects (PaymentCode, CurSCONum, Quotation, ProjectName, BillToCompany, BillToAddress,
    SoldToCompany, SoldToAddress, ReportsRequired, ApplicantName, ApplicantDesignation,
    EmailAddr, EmailCC, PricePerCube, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES ('C', 0, 5003, 'Jurong Warehouse Extension', 'Jurong Industrial Estate Pte Ltd', '20 Jurong Port Rd, Singapore 619970',
    'Jurong Industrial Estate Pte Ltd', '20 Jurong Port Rd, Singapore 619970', 'Monthly', 'Ahmad Rizal', 'Site Supervisor',
    'ahmad.rizal@jurongie.example', '', 40.00, 'admin', GETDATE(), 'admin', GETDATE());
SET @ProjId3 = CAST(SCOPE_IDENTITY() AS INT);

-- SCONumbers (one active SCO per project)
INSERT INTO SCONumbers (SCONum, ProjectId, LastUpdateUser, LastUpdate) VALUES
    (10001, @ProjId1, 'admin', GETDATE()),
    (10002, @ProjId2, 'admin', GETDATE()),
    (10003, @ProjId3, 'admin', GETDATE());

UPDATE Projects SET CurSCONum = 10001 WHERE Id = @ProjId1;
UPDATE Projects SET CurSCONum = 10002 WHERE Id = @ProjId2;
UPDATE Projects SET CurSCONum = 10003 WHERE Id = @ProjId3;

-- CubeSets (one per project)
INSERT INTO CubeSets (ProjectId, SpecId, TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength,
    StdDeviation, SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES (@ProjId1, 'S206', 'A', 40, 'Normal', 48.5, 5.2, 'SUP001', 'Tower A - Level 12 Slab', DATEADD(day,-20,GETDATE()), 'admin', GETDATE(), 'admin', GETDATE());
SET @CS1 = CAST(SCOPE_IDENTITY() AS INT);

INSERT INTO CubeSets (ProjectId, SpecId, TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength,
    StdDeviation, SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES (@ProjId2, 'S206', 'A', 35, 'High Strength', 42.0, 4.5, 'SUP002', 'Block B - Foundation', DATEADD(day,-15,GETDATE()), 'admin', GETDATE(), 'admin', GETDATE());
SET @CS2 = CAST(SCOPE_IDENTITY() AS INT);

INSERT INTO CubeSets (ProjectId, SpecId, TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength,
    StdDeviation, SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES (@ProjId3, 'S206', 'B', 30, 'Normal', 36.0, 4.0, 'SUP003', 'Warehouse Ext - Column C4', DATEADD(day,-10,GETDATE()), 'admin', GETDATE(), 'admin', GETDATE());
SET @CS3 = CAST(SCOPE_IDENTITY() AS INT);

-- Batches: (ScoNum, Id) composite PK, two batches per cubeset (7-day and 28-day)
INSERT INTO Batches (ScoNum, Id, CubeSetId, TestAge, TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, CreateUser, CreateDate, LastUpdateUser, LastUpdate) VALUES
    (10001, 1, @CS1, 7,  DATEADD(day,-13,GETDATE()), 150, 1, 32.4, 32.1, 'P', 'P', 'admin', GETDATE(), 'admin', GETDATE()),
    (10001, 2, @CS1, 28, DATEADD(day, 8, GETDATE()), 150, 1, 49.8, 48.9, 'P', 'P', 'admin', GETDATE(), 'admin', GETDATE()),
    (10002, 1, @CS2, 7,  DATEADD(day, -8,GETDATE()), 150, 1, 28.1, 27.6, 'P', 'F', 'admin', GETDATE(), 'admin', GETDATE()),
    (10002, 2, @CS2, 28, DATEADD(day, 13, GETDATE()), 150, 1, 0.0,  0.0,  'P', 'P', 'admin', GETDATE(), 'admin', GETDATE()),
    (10003, 1, @CS3, 7,  DATEADD(day, -3,GETDATE()), 150, 1, 25.0, 24.8, 'P', 'P', 'admin', GETDATE(), 'admin', GETDATE()),
    (10003, 2, @CS3, 28, DATEADD(day, 18, GETDATE()), 150, 1, 0.0,  0.0,  'P', 'P', 'admin', GETDATE(), 'admin', GETDATE());

-- Cubes: 4 per batch (24 total), mixed TestResult (1=Pass,2=Fail,3=Pending,0=Not Tested)
INSERT INTO Cubes (Barcode, SampleRef, ScoNum, BatchId, MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6,
    MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6,
    AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId,
    TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, CreateUser, CreateDate, LastUpdateUser, LastUpdate) VALUES
-- Batch 10001-1 (7-day, tested, mixed pass/fail)
(20000001, 'MB-A-01', 10001, 1, 150.1,150.0,149.9,150.2,150.0,150.1, 150.0,150.1,150.0,149.9,150.2,150.0, 150.05, 729000, 32.4, 8.15, 2415, 1, 1, 'G', 1, DATEADD(day,-13,GETDATE()), DATEADD(day,-13,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000002, 'MB-A-02', 10001, 1, 150.0,150.2,150.1,150.0,149.9,150.1, 150.1,150.0,150.0,150.1,150.0,149.9, 150.03, 715000, 31.8, 8.10, 2402, 1, 1, 'G', 1, DATEADD(day,-13,GETDATE()), DATEADD(day,-13,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000003, 'MB-A-03', 10001, 1, 150.2,150.1,150.0,150.1,150.0,150.2, 150.0,150.1,150.1,150.0,150.0,150.1, 150.08, 743000, 33.0, 8.20, 2421, 2, 1, 'G', 1, DATEADD(day,-13,GETDATE()), DATEADD(day,-13,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000004, 'MB-A-04', 10001, 1, 150.0,150.0,150.1,150.0,150.1,150.0, 150.0,150.0,150.1,150.0,150.0,150.1, 150.03, 675000, 30.0, 8.05, 2390, 2, 2, 'G', 1, DATEADD(day,-13,GETDATE()), DATEADD(day,-13,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
-- Batch 10001-2 (28-day, not yet tested - target date in future)
(20000005, 'MB-A-05', 10001, 2, 150.0,150.1,150.0,150.1,150.0,150.1, 150.0,150.1,150.0,150.1,150.0,150.1, 150.05, 0,0,8.12,0,0, 0, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000006, 'MB-A-06', 10001, 2, 150.1,150.0,150.1,150.0,150.1,150.0, 150.1,150.0,150.1,150.0,150.1,150.0, 150.05, 0,0,8.08,0,0, 0, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000007, 'MB-A-07', 10001, 2, 150.0,150.0,150.0,150.0,150.0,150.0, 150.0,150.0,150.0,150.0,150.0,150.0, 150.00, 0,0,8.11,0,0, 3, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000008, 'MB-A-08', 10001, 2, 150.1,150.1,150.0,150.0,150.1,150.1, 150.1,150.1,150.0,150.0,150.1,150.1, 150.07, 0,0,8.14,0,0, 3, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
-- Batch 10002-1 (7-day, tested, one fail)
(20000009, 'OH-B-01', 10002, 1, 150.0,150.1,150.0,150.1,150.0,150.1, 150.0,150.1,150.0,150.1,150.0,150.1, 150.05, 632000, 28.1, 8.02, 2380, 1, 1, 'G', 1, DATEADD(day,-8,GETDATE()), DATEADD(day,-8,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000010, 'OH-B-02', 10002, 1, 150.1,150.0,150.1,150.0,150.1,150.0, 150.1,150.0,150.1,150.0,150.1,150.0, 150.05, 610000, 27.1, 7.98, 2372, 1, 1, 'G', 1, DATEADD(day,-8,GETDATE()), DATEADD(day,-8,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000011, 'OH-B-03', 10002, 1, 150.0,150.0,150.1,150.1,150.0,150.0, 150.0,150.0,150.1,150.1,150.0,150.0, 150.03, 450000, 20.0, 7.85, 2340, 2, 2, 'G', 1, DATEADD(day,-8,GETDATE()), DATEADD(day,-8,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000012, 'OH-B-04', 10002, 1, 150.1,150.1,150.0,150.0,150.1,150.1, 150.1,150.1,150.0,150.0,150.1,150.1, 150.07, 640000, 28.4, 8.00, 2378, 2, 1, 'G', 1, DATEADD(day,-8,GETDATE()), DATEADD(day,-8,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
-- Batch 10002-2 (28-day, not tested)
(20000013, 'OH-B-05', 10002, 2, 150.0,150.0,150.0,150.1,150.1,150.0, 150.0,150.0,150.0,150.1,150.1,150.0, 150.03, 0,0,7.99,0,0, 0, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000014, 'OH-B-06', 10002, 2, 150.1,150.0,150.1,150.0,150.0,150.1, 150.1,150.0,150.1,150.0,150.0,150.1, 150.05, 0,0,7.97,0,0, 0, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000015, 'OH-B-07', 10002, 2, 150.0,150.1,150.0,150.1,150.0,150.1, 150.0,150.1,150.0,150.1,150.0,150.1, 150.05, 0,0,8.01,0,0, 3, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
(20000016, 'OH-B-08', 10002, 2, 150.1,150.1,150.1,150.0,150.0,150.0, 150.1,150.1,150.1,150.0,150.0,150.0, 150.05, 0,0,7.96,0,0, 0, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
-- Batch 10003-1 (7-day, tested, all pass)
(20000017, 'JW-C-01', 10003, 1, 150.0,150.0,150.1,150.0,150.1,150.0, 150.0,150.0,150.1,150.0,150.1,150.0, 150.03, 570000, 25.3, 7.90, 2350, 1, 1, 'G', 1, DATEADD(day,-3,GETDATE()), DATEADD(day,-3,GETDATE()), 'custsvc', GETDATE(), 'custsvc', GETDATE()),
(20000018, 'JW-C-02', 10003, 1, 150.1,150.0,150.0,150.1,150.0,150.0, 150.1,150.0,150.0,150.1,150.0,150.0, 150.03, 562000, 24.9, 7.88, 2345, 1, 1, 'G', 1, DATEADD(day,-3,GETDATE()), DATEADD(day,-3,GETDATE()), 'custsvc', GETDATE(), 'custsvc', GETDATE()),
(20000019, 'JW-C-03', 10003, 1, 150.0,150.1,150.1,150.0,150.0,150.1, 150.0,150.1,150.1,150.0,150.0,150.1, 150.05, 578000, 25.6, 7.92, 2355, 2, 1, 'G', 1, DATEADD(day,-3,GETDATE()), DATEADD(day,-3,GETDATE()), 'custsvc', GETDATE(), 'custsvc', GETDATE()),
(20000020, 'JW-C-04', 10003, 1, 150.0,150.0,150.0,150.1,150.1,150.0, 150.0,150.0,150.0,150.1,150.1,150.0, 150.03, 555000, 24.6, 7.86, 2342, 2, 1, 'G', 1, DATEADD(day,-3,GETDATE()), DATEADD(day,-3,GETDATE()), 'custsvc', GETDATE(), 'custsvc', GETDATE()),
-- Batch 10003-2 (28-day, not tested)
(20000021, 'JW-C-05', 10003, 2, 150.0,150.1,150.0,150.0,150.1,150.0, 150.0,150.1,150.0,150.0,150.1,150.0, 150.03, 0,0,7.89,0,0, 0, 'A', 0, NULL, NULL, 'custsvc', GETDATE(), 'custsvc', GETDATE()),
(20000022, 'JW-C-06', 10003, 2, 150.1,150.0,150.1,150.1,150.0,150.0, 150.1,150.0,150.1,150.1,150.0,150.0, 150.05, 0,0,7.91,0,0, 0, 'A', 0, NULL, NULL, 'custsvc', GETDATE(), 'custsvc', GETDATE()),
(20000023, 'JW-C-07', 10003, 2, 150.0,150.0,150.1,150.0,150.0,150.1, 150.0,150.0,150.1,150.0,150.0,150.1, 150.03, 0,0,7.87,0,0, 3, 'A', 0, NULL, NULL, 'custsvc', GETDATE(), 'custsvc', GETDATE()),
(20000024, 'JW-C-08', 10003, 2, 150.1,150.1,150.0,150.1,150.1,150.0, 150.1,150.1,150.0,150.1,150.1,150.0, 150.07, 0,0,7.93,0,0, 0, 'A', 0, NULL, NULL, 'custsvc', GETDATE(), 'custsvc', GETDATE());

-- BarcodeAllocation records reflecting the barcode ranges above
INSERT INTO BarcodeAllocation (ProjectId, Qty, BarcodeStart, BarcodeEnd, CreateUser, CreateDate, LastUpdateUser, LastUpdate) VALUES
    (@ProjId1, 8, 20000001, 20000008, 'admin', GETDATE(), 'admin', GETDATE()),
    (@ProjId2, 8, 20000009, 20000016, 'admin', GETDATE(), 'admin', GETDATE()),
    (@ProjId3, 8, 20000017, 20000024, 'admin', GETDATE(), 'admin', GETDATE());

-- Reports (Report History page)
INSERT INTO Reports (ProjectId, ReportType, FileName, ReportDate, StartDate, EndDate, EmailTo, EmailCC, Released, ReleaseUser, ReleaseDate) VALUES
    (@ProjId1, 'Daily',       'MarinaBayTower_Daily_' + FORMAT(DATEADD(day,-5,GETDATE()),'yyyyMMdd') + '.pdf',
        DATEADD(day,-5,GETDATE()), DATEADD(day,-5,GETDATE()), DATEADD(day,-5,GETDATE()),
        'wm.tan@marinabay.example', 'sarah.lim@orchardheights.example', 2, 'admin', DATEADD(day,-5,GETDATE())),
    (@ProjId1, 'Daily',       'MarinaBayTower_Daily_' + FORMAT(DATEADD(day,-1,GETDATE()),'yyyyMMdd') + '.pdf',
        DATEADD(day,-1,GETDATE()), DATEADD(day,-1,GETDATE()), DATEADD(day,-1,GETDATE()),
        'wm.tan@marinabay.example', 'sarah.lim@orchardheights.example', 0, NULL, NULL),
    (@ProjId1, 'Monthly',     'MarinaBayTower_Monthly_' + FORMAT(DATEADD(day,-20,GETDATE()),'yyyyMMdd') + '.pdf',
        DATEADD(day,-20,GETDATE()), DATEADD(day,-DAY(GETDATE())-19,GETDATE()), DATEADD(day,-DAY(GETDATE()),GETDATE()),
        'wm.tan@marinabay.example', 'sarah.lim@orchardheights.example', 3, 'admin', DATEADD(day,-19,GETDATE())),
    (@ProjId2, 'Daily',       'OrchardHeights_Daily_' + FORMAT(DATEADD(day,-3,GETDATE()),'yyyyMMdd') + '.pdf',
        DATEADD(day,-3,GETDATE()), DATEADD(day,-3,GETDATE()), DATEADD(day,-3,GETDATE()),
        'sarah.lim@orchardheights.example', '', 2, 'admin', DATEADD(day,-3,GETDATE())),
    (@ProjId2, 'Daily',       'OrchardHeights_Daily_' + FORMAT(GETDATE(),'yyyyMMdd') + '.pdf',
        GETDATE(), GETDATE(), GETDATE(),
        'sarah.lim@orchardheights.example', '', 0, NULL, NULL),
    (@ProjId2, 'Statistical', 'OrchardHeights_Statistical_' + FORMAT(DATEADD(day,-10,GETDATE()),'yyyyMMdd') + '.pdf',
        DATEADD(day,-10,GETDATE()), DATEADD(day,-40,GETDATE()), DATEADD(day,-10,GETDATE()),
        'sarah.lim@orchardheights.example', '', 1, 'admin', DATEADD(day,-9,GETDATE())),
    (@ProjId3, 'Monthly',     'JurongWarehouse_Monthly_' + FORMAT(DATEADD(day,-25,GETDATE()),'yyyyMMdd') + '.pdf',
        DATEADD(day,-25,GETDATE()), DATEADD(day,-55,GETDATE()), DATEADD(day,-25,GETDATE()),
        'ahmad.rizal@jurongie.example', '', 3, 'admin', DATEADD(day,-24,GETDATE())),
    (@ProjId3, 'Monthly',     'JurongWarehouse_Monthly_' + FORMAT(DATEADD(day,-1,GETDATE()),'yyyyMMdd') + '.pdf',
        DATEADD(day,-1,GETDATE()), DATEADD(day,-DAY(GETDATE())-30,GETDATE()), DATEADD(day,-DAY(GETDATE()),GETDATE()),
        'ahmad.rizal@jurongie.example', '', 0, NULL, NULL),
    (@ProjId3, 'Daily',       'JurongWarehouse_Daily_' + FORMAT(DATEADD(day,-2,GETDATE()),'yyyyMMdd') + '.pdf',
        DATEADD(day,-2,GETDATE()), DATEADD(day,-2,GETDATE()), DATEADD(day,-2,GETDATE()),
        'ahmad.rizal@jurongie.example', '', 2, 'admin', DATEADD(day,-2,GETDATE()));

-- ScheduledTasks (background jobs the app expects to find)
IF NOT EXISTS (SELECT 1 FROM ScheduledTasks)
INSERT INTO ScheduledTasks (Name, DayOfMonth, Hour, Minute, LastRun, NextRun) VALUES
    ('DailyReport', 0, 18, 0, DATEADD(day,-1,GETDATE()), DATEADD(day,1,GETDATE())),
    ('MonthlyReport', 1, 6, 0, DATEADD(month,-1,GETDATE()), DATEADD(month,1,GETDATE()));

-- ActivityLog sample entries
INSERT INTO ActivityLog (Activity, TableName, UserId, LogDateTime) VALUES
    ('Added Project Marina Bay Tower Construction', 'Projects', 'admin', DATEADD(day,-20,GETDATE())),
    ('Added Project Orchard Heights Condominium', 'Projects', 'admin', DATEADD(day,-15,GETDATE())),
    ('Added Project Jurong Warehouse Extension', 'Projects', 'admin', DATEADD(day,-10,GETDATE())),
    ('Logged in', 'Users', 'admin', DATEADD(hour,-2,GETDATE())),
    ('Logged in', 'Users', 'dataentry', DATEADD(hour,-1,GETDATE()));
GO
