DECLARE @ProjId INT = 1;

IF NOT EXISTS (SELECT 1 FROM Projects WHERE Id = @ProjId AND PaymentCode = 'P')
BEGIN
    THROW 50001, 'Project P0001 (PaymentCode=P, Id=1) not found.', 1;
END

IF EXISTS (SELECT 1 FROM CubeSets WHERE ProjectId = @ProjId AND Location = N'Tower A - Level 15 Beam (EXTRA)')
BEGIN
    PRINT 'extra_seed_p0001.sql: Extra CubeSet already exists; skipping.';
    RETURN;
END

DECLARE @ScoNum INT = (SELECT CurSCONum FROM Projects WHERE Id = @ProjId);
DECLARE @SupplierId VARCHAR(50) =
    ISNULL((SELECT TOP 1 SupplierId FROM CubeSets WHERE ProjectId = @ProjId AND SupplierId IS NOT NULL ORDER BY Id), 'SUP001');

DECLARE @NewCubeSetId INT;
INSERT INTO CubeSets (ProjectId, SpecId, TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength,
    StdDeviation, SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES (@ProjId, 'S206', 'A', 40, 'Normal', 48.5,
    5.2, @SupplierId, N'Tower A - Level 15 Beam (EXTRA)', DATEADD(day,-9,GETDATE()),
    'admin', GETDATE(), 'admin', GETDATE());
SET @NewCubeSetId = CAST(SCOPE_IDENTITY() AS INT);

DECLARE @BatchId1 INT = (SELECT ISNULL(MAX(Id), 0) + 1 FROM Batches WHERE ScoNum = @ScoNum);
DECLARE @BatchId2 INT = @BatchId1 + 1;

INSERT INTO Batches (ScoNum, Id, CubeSetId, TestAge, TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES
    (@ScoNum, @BatchId1, @NewCubeSetId, 7,  DATEADD(day,-2,GETDATE()), 150, 2, 34.2, 33.9, 'P', 'P', 'admin', GETDATE(), 'admin', GETDATE()),
    (@ScoNum, @BatchId2, @NewCubeSetId, 28, DATEADD(day,19,GETDATE()), 150, 2, 0.0,  0.0,  'P', 'P', 'admin', GETDATE(), 'admin', GETDATE());

DECLARE @NextBarcode BIGINT = (SELECT ISNULL(MAX(Barcode), 20000000) + 1 FROM Cubes);

INSERT INTO Cubes (Barcode, SampleRef, ScoNum, BatchId, MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6,
    MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6,
    AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId,
    TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES
    (@NextBarcode + 0, 'MB-A-EX01', @ScoNum, @BatchId1, 150.0,150.1,150.0,150.1,150.0,150.1, 150.0,150.1,150.0,150.1,150.0,150.1, 150.05, 770000, 34.2, 8.16, 2420, 1, 1, 'G', 1, DATEADD(day,-2,GETDATE()), DATEADD(day,-2,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
    (@NextBarcode + 1, 'MB-A-EX02', @ScoNum, @BatchId1, 150.1,150.0,150.1,150.0,150.1,150.0, 150.1,150.0,150.1,150.0,150.1,150.0, 150.05, 755000, 33.6, 8.11, 2408, 1, 1, 'G', 1, DATEADD(day,-2,GETDATE()), DATEADD(day,-2,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
    (@NextBarcode + 2, 'MB-A-EX03', @ScoNum, @BatchId1, 150.0,150.0,150.1,150.1,150.0,150.0, 150.0,150.0,150.1,150.1,150.0,150.0, 150.03, 790000, 35.1, 8.21, 2425, 2, 1, 'G', 1, DATEADD(day,-2,GETDATE()), DATEADD(day,-2,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
    (@NextBarcode + 3, 'MB-A-EX04', @ScoNum, @BatchId1, 150.1,150.1,150.0,150.0,150.1,150.1, 150.1,150.1,150.0,150.0,150.1,150.1, 150.07, 610000, 27.1, 8.05, 2388, 2, 2, 'G', 1, DATEADD(day,-2,GETDATE()), DATEADD(day,-2,GETDATE()), 'dataentry', GETDATE(), 'dataentry', GETDATE()),
    (@NextBarcode + 4, 'MB-A-EX05', @ScoNum, @BatchId2, 150.0,150.1,150.0,150.1,150.0,150.1, 150.0,150.1,150.0,150.1,150.0,150.1, 150.05, 0, 0, 8.12, 0, 0, 0, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
    (@NextBarcode + 5, 'MB-A-EX06', @ScoNum, @BatchId2, 150.1,150.0,150.1,150.0,150.1,150.0, 150.1,150.0,150.1,150.0,150.1,150.0, 150.05, 0, 0, 8.08, 0, 0, 0, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
    (@NextBarcode + 6, 'MB-A-EX07', @ScoNum, @BatchId2, 150.0,150.0,150.0,150.0,150.0,150.0, 150.0,150.0,150.0,150.0,150.0,150.0, 150.00, 0, 0, 8.11, 0, 0, 3, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
    (@NextBarcode + 7, 'MB-A-EX08', @ScoNum, @BatchId2, 150.1,150.1,150.0,150.0,150.1,150.1, 150.1,150.1,150.0,150.0,150.1,150.1, 150.07, 0, 0, 8.14, 0, 0, 3, 'A', 0, NULL, NULL, 'dataentry', GETDATE(), 'dataentry', GETDATE());

IF NOT EXISTS (SELECT 1 FROM BarcodeAllocation WHERE ProjectId = @ProjId AND BarcodeStart = @NextBarcode AND BarcodeEnd = @NextBarcode + 7)
BEGIN
    INSERT INTO BarcodeAllocation (ProjectId, Qty, BarcodeStart, BarcodeEnd, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
    VALUES (@ProjId, 8, @NextBarcode, @NextBarcode + 7, 'admin', GETDATE(), 'admin', GETDATE());
END

PRINT CONCAT('extra_seed_p0001.sql: Added CubeSetId=', @NewCubeSetId, ', ScoNum=', @ScoNum, ', Batches=', @BatchId1, ',', @BatchId2, ', Barcodes=', @NextBarcode, '...', @NextBarcode + 7);
