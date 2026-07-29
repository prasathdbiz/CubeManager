CREATE TABLE CubeSets
(
    Id INTEGER NOT NULL PRIMARY KEY,
    ProjectId INT NOT NULL,
    SpecId VARCHAR(100) NOT NULL, -- S206 and more
    TestCriteria VARCHAR(100) NOT NULL, -- Production Control or Identity Testing
    ConcreteGrade INT NOT NULL,
    ConcreteType VARCHAR(100) NOT NULL,
    CharacteristicStrength FLOAT, -- defaults to concrete grade
    StdDeviation FLOAT, -- provided by customer
    SupplierId CHAR(10),
    CreateUser TEXT,
    CreateDate TIMESTAMP,
    LastUpdateUser TEXT,
    LastUpdate TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Batches
(
    SocNum INT NOT NULL, 
    Id INT NOT NULL,
    CubeSetId INT NOT NULL,
    CastingDate DATE NOT NULL,
    TestAge INTEGER NOT NULL, -- days
    TargetTestDate DATE, -- calculated
    Dimension INTEGER NOT NULL, -- 100 or 150
    WitnessNum INTEGER NOT NULL, -- 0 if no witness, otherwise 1-99
    CreateUser TEXT,
    CreateDate TIMESTAMP(3),
    LastUpdateUser TEXT,
    LastUpdate TIMESTAMP(3) DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (SocNum, Id)
)

CREATE TABLE Cubes
(
    Barcode INTEGER NOT NULL PRIMARY KEY, -- assigned
    SampleRef VARCHAR(100),
    SocNum INTEGER NOT NULL,
    BatchId INTEGER NOT NULL,
    -- below all filled by IPC
    MeasuredDimX1 FLOAT, -- from line
    MeasuredDimX2 FLOAT, -- from line
    MeasuredDimX3 FLOAT, -- from line
    MeasuredDimX4 FLOAT, -- from line
    MeasuredDimX5 FLOAT, -- from line
    MeasuredDimX6 FLOAT, -- from line
    MeasuredDimY1 FLOAT, -- from line
    MeasuredDimY2 FLOAT, -- from line
    MeasuredDimY3 FLOAT, -- from line
    MeasuredDimY4 FLOAT, -- from line
    MeasuredDimY5 FLOAT, -- from line
    MeasuredDimY6 FLOAT, -- from line
    AvgDimension FLOAT, -- calculated
	MeasuredMaxForce FLOAT, -- from tester
    MeasuredStrength FLOAT, -- MPa, calculated MaxForce/(dim^2)
    MeasuredWeight FLOAT, -- kg
    MeasuredDensity FLOAT, -- kg/m3socnumbers
    TesterId TINYINT, -- tester 1 to 6
    TestResult TINYINT DEFAULT 0, -- 0: Not tested, 1: Pass, 2: Fail, 3: Pending
    FailCode VARCHAR(20), -- A to G, see notes
    Uploaded TINYINT, -- 0: Not uploaded, 1: Uploaded to server
    ActualTestDate TIMESTAMP(3),
    UploadTime TIMESTAMP(3),
    CreateUser TEXT,
    CreateDate TIMESTAMP(3),
    LastUpdateUser TEXT,
    LastUpdate TIMESTAMP(3) DEFAULT CURRENT_TIMESTAMP
)