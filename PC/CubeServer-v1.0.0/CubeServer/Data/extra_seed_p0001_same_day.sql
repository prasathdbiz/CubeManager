DECLARE @ProjId INT = 1;

IF NOT EXISTS (SELECT 1 FROM Projects WHERE Id = @ProjId AND PaymentCode = 'P')
BEGIN
    THROW 50001, 'Project P0001 (PaymentCode=P, Id=1) not found.', 1;
END

IF EXISTS (SELECT 1 FROM CubeSets WHERE ProjectId = @ProjId AND Location = N'Tower A - Level 12 Slab (EXTRA SAME DAY)')
BEGIN
    RETURN;
END

DECLARE @ScoNum INT = (SELECT CurSCONum FROM Projects WHERE Id = @ProjId);
DECLARE @SupplierId VARCHAR(50) =
    ISNULL((SELECT TOP 1 SupplierId FROM CubeSets WHERE ProjectId = @ProjId AND SupplierId IS NOT NULL ORDER BY Id), 'SUP001');

DECLARE @TestDt DATETIME = DATEADD(day,-13,GETDATE());

DECLARE @NewCubeSetId INT;
INSERT INTO CubeSets (ProjectId, SpecId, TestCriteria, ConcreteGrade, ConcreteType, CharacteristicStrength,
    StdDeviation, SupplierId, Location, CastingDate, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES (@ProjId, 'S206', 'A', 40, 'Normal', 48.5,
    5.2, @SupplierId, N'Tower A - Level 12 Slab (EXTRA SAME DAY)', DATEADD(day,-20,GETDATE()),
    'admin', GETDATE(), 'admin', GETDATE());
SET @NewCubeSetId = CAST(SCOPE_IDENTITY() AS INT);

DECLARE @BatchId INT = (SELECT ISNULL(MAX(Id), 0) + 1 FROM Batches WHERE ScoNum = @ScoNum);

INSERT INTO Batches (ScoNum, Id, CubeSetId, TestAge, TargetTestDate, Dimension, WitnessNum, AvgStrength, RollingAvgStrength, CriterionA, CriterionB, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES
    (@ScoNum, @BatchId, @NewCubeSetId, 7, @TestDt, 150, 3, 33.0, 32.8, 'P', 'P', 'admin', GETDATE(), 'admin', GETDATE());

DECLARE @NextBarcode BIGINT = (SELECT ISNULL(MAX(Barcode), 20000000) + 1 FROM Cubes);

INSERT INTO Cubes (Barcode, SampleRef, ScoNum, BatchId, MeasuredDimX1, MeasuredDimX2, MeasuredDimX3, MeasuredDimX4, MeasuredDimX5, MeasuredDimX6,
    MeasuredDimY1, MeasuredDimY2, MeasuredDimY3, MeasuredDimY4, MeasuredDimY5, MeasuredDimY6,
    AvgDimension, MeasuredMaxForce, MeasuredStrength, MeasuredWeight, MeasuredDensity, TesterId,
    TestResult, StatusCode, Uploaded, ActualTestDate, UploadTime, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
VALUES
    (@NextBarcode + 0, 'MB-A-SD01', @ScoNum, @BatchId, 150.0,150.1,150.0,150.1,150.0,150.1, 150.0,150.1,150.0,150.1,150.0,150.1, 150.05, 742000, 33.0, 8.16, 2419, 1, 1, 'G', 1, @TestDt, @TestDt, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
    (@NextBarcode + 1, 'MB-A-SD02', @ScoNum, @BatchId, 150.1,150.0,150.1,150.0,150.1,150.0, 150.1,150.0,150.1,150.0,150.1,150.0, 150.05, 728000, 32.4, 8.11, 2407, 1, 1, 'G', 1, @TestDt, @TestDt, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
    (@NextBarcode + 2, 'MB-A-SD03', @ScoNum, @BatchId, 150.0,150.0,150.1,150.1,150.0,150.0, 150.0,150.0,150.1,150.1,150.0,150.0, 150.03, 760000, 33.8, 8.21, 2424, 2, 1, 'G', 1, @TestDt, @TestDt, 'dataentry', GETDATE(), 'dataentry', GETDATE()),
    (@NextBarcode + 3, 'MB-A-SD04', @ScoNum, @BatchId, 150.1,150.1,150.0,150.0,150.1,150.1, 150.1,150.1,150.0,150.0,150.1,150.1, 150.07, 595000, 26.4, 8.05, 2387, 2, 2, 'G', 1, @TestDt, @TestDt, 'dataentry', GETDATE(), 'dataentry', GETDATE());

IF NOT EXISTS (SELECT 1 FROM BarcodeAllocation WHERE ProjectId = @ProjId AND BarcodeStart = @NextBarcode AND BarcodeEnd = @NextBarcode + 3)
BEGIN
    INSERT INTO BarcodeAllocation (ProjectId, Qty, BarcodeStart, BarcodeEnd, CreateUser, CreateDate, LastUpdateUser, LastUpdate)
    VALUES (@ProjId, 4, @NextBarcode, @NextBarcode + 3, 'admin', GETDATE(), 'admin', GETDATE());
END
