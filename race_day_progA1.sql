CREATE DATABASE RaceDay;

USE RaceDay;

CREATE TABLE [User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Participant',
    CONSTRAINT CK_User_Role CHECK (Role IN ('Organiser', 'Participant'))
);

CREATE TABLE Event (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    EventName NVARCHAR(150) NOT NULL,
    EventDate DATE NOT NULL,
    Location NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    CONSTRAINT FK_Event_Organiser
        FOREIGN KEY (OrganiserID) REFERENCES [User](UserID)
);

CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    DistanceKm DECIMAL(5,2) NOT NULL,
    EntryFee DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT FK_Category_Event
        FOREIGN KEY (EventID) REFERENCES Event(EventID),
    CONSTRAINT CK_Category_Distance
        CHECK (DistanceKm > 0),
    CONSTRAINT CK_Category_EntryFee
        CHECK (EntryFee >= 0),
    CONSTRAINT UQ_Category_Event_Name
        UNIQUE (EventID, CategoryName)
);

CREATE TABLE Enrollment (
    EnrollmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrollmentDate DATE NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(20) NOT NULL DEFAULT 'Confirmed',
    CONSTRAINT FK_Enrollment_Participant
        FOREIGN KEY (ParticipantID) REFERENCES [User](UserID),
    CONSTRAINT FK_Enrollment_Category
        FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID),
    CONSTRAINT CK_Enrollment_Status
        CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled')),
    CONSTRAINT UQ_Enrollment_Participant_Category
        UNIQUE (ParticipantID, CategoryID)
);

CREATE TABLE Result (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentID INT NOT NULL UNIQUE,
    FinishTime TIME NULL,
    Position INT NULL,
    ResultDate DATE NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Result_Enrollment
        FOREIGN KEY (EnrollmentID) REFERENCES Enrollment(EnrollmentID),
    CONSTRAINT CK_Result_Position
        CHECK (Position IS NULL OR Position > 0)
);

CREATE TABLE Route (
    RouteID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL UNIQUE,
    RouteName NVARCHAR(150) NOT NULL,
    StartPoint NVARCHAR(150) NOT NULL,
    FinishPoint NVARCHAR(150) NOT NULL,
    RouteDistanceKm DECIMAL(5,2) NOT NULL,
    CONSTRAINT FK_Route_Event
        FOREIGN KEY (EventID) REFERENCES Event(EventID),
    CONSTRAINT CK_Route_Distance
        CHECK (RouteDistanceKm > 0)
);

INSERT INTO [User]
    (FirstName, LastName, Email, PasswordHash, Role)
VALUES
    ('Thabo', 'Mokoena', 'thabo.organiser@raceday.co.za', 'Password123!', 'Organiser'),
    ('Lerato', 'Dlamini', 'lerato.organiser@raceday.co.za', 'Password123!', 'Organiser'),
    ('Sipho', 'Nkosi', 'sipho.participant@raceday.co.za', 'Password123!', 'Participant'),
    ('Amahle', 'Ndlovu', 'amahle.participant@raceday.co.za', 'Password123!', 'Participant');

    INSERT INTO Event
    (OrganiserID, EventName, EventDate, Location, Description)
VALUES
    (1, 'Johannesburg City Run', '2026-10-10', 'Johannesburg, Gauteng',
     'A city road running event for different age groups and distances.'),
    (1, 'Cape Town Coastal Cycle', '2026-11-15', 'Cape Town, Western Cape',
     'A road cycling event along the Cape Town coastline.'),
    (2, 'Soweto Community Walk', '2026-12-05', 'Soweto, Gauteng',
     'A community walking event promoting health and participation.');

     INSERT INTO Category
    (EventID, CategoryName, DistanceKm, EntryFee)
VALUES
    (1, '10 KM Run', 10.00, 150.00),
    (1, '21 KM Half Marathon', 21.10, 250.00),
    (2, '40 KM Cycle', 40.00, 300.00),
    (2, '80 KM Cycle', 80.00, 450.00),
    (3, '5 KM Walk', 5.00, 80.00),
    (3, '10 KM Walk', 10.00, 120.00);

    INSERT INTO Enrollment
    (ParticipantID, CategoryID, EnrollmentDate, Status)
VALUES
    (3, 1, '2026-09-01', 'Confirmed'),
    (3, 3, '2026-09-01', 'Confirmed'),
    (4, 2, '2026-09-02', 'Confirmed'),
    (4, 5, '2026-09-02', 'Confirmed');

    INSERT INTO Result
    (EnrollmentID, FinishTime, Position, ResultDate)
VALUES
    (1, '00:52:34', 18, '2026-10-10'),
    (2, '01:48:20', 7, '2026-11-15');

    INSERT INTO Route
    (EventID, RouteName, StartPoint, FinishPoint, RouteDistanceKm)
VALUES
    (1, 'Johannesburg City Route', 'Mary Fitzgerald Square', 'Ellis Park',
     21.10),
    (2, 'Cape Town Coastal Route', 'Sea Point', 'Hout Bay',
     40.00),
    (3, 'Soweto Community Route', 'Vilakazi Street', 'Walter Sisulu Square',
     10.00);

     SELECT * FROM [User];
     SELECT * FROM Event;
     SELECT * FROM Category;
     SELECT * FROM Enrollment;
     SELECT * FROM Result;
     SELECT * FROM Route;