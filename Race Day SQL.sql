CREATE DATABASE RaceDayDB;

USE RaceDayDB;

CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    RoleID INT NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Phone VARCHAR(20) NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

CREATE TABLE Events (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserID INT NOT NULL,
    Name VARCHAR(150) NOT NULL,
    Description VARCHAR(MAX) NOT NULL,
    Date DATETIME2 NOT NULL,
    Location VARCHAR(200) NOT NULL,
    Distance DECIMAL(6,2) NOT NULL,
    EventType VARCHAR(20) NOT NULL CHECK (EventType IN ('Run', 'Walk', 'Cycle')),
    CONSTRAINT FK_Events_Users FOREIGN KEY (OrganiserID) REFERENCES Users(UserID)
);

CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName VARCHAR(100) NOT NULL,
    MinAge INT NULL,
    MaxAge INT NULL,
    CONSTRAINT FK_Categories_Events FOREIGN KEY (EventID) REFERENCES Events(EventID) ON DELETE CASCADE
);

CREATE TABLE Enrolments (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    Status VARCHAR(20) NOT NULL DEFAULT 'Confirmed' CHECK (Status IN ('Confirmed', 'Cancelled', 'DNS', 'DNF')),
    CONSTRAINT FK_Enrolments_Users FOREIGN KEY (ParticipantID) REFERENCES Users(UserID),
    CONSTRAINT FK_Enrolments_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT UQ_Participant_Category UNIQUE (ParticipantID, CategoryID)
);

CREATE TABLE Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE,
    FinishTime TIME NOT NULL,
    OverallPosition INT NOT NULL,
    CategoryPosition INT NOT NULL,
    CONSTRAINT FK_Results_Enrolments FOREIGN KEY (EnrolmentID) REFERENCES Enrolments(EnrolmentID) ON DELETE CASCADE
);

INSERT INTO Roles (RoleName) VALUES ('Organiser'), ('Participant');

INSERT INTO Users (RoleID, FirstName, LastName, Email, PasswordHash, Phone) VALUES
(1, 'Tumelo', 'Motsepe', 'tumelomo@gmail.com', 'P@ssword1', '+27821234567'),
(2, 'Herman', 'Hlalele', 'hermanhl@gmail.com', 'P@ssword2', '+27712196222'),
(2, 'Maria', 'Mokoena', 'mariamo@yahoo.com', 'P@ssword3', '+27723236944');

INSERT INTO Events (OrganiserID, Name, Description, Date, Location, Distance, EventType) VALUES
(1, 'Comrades Marathon', 'The ultimate human race between Pietermaritzburg and Durban.', '2027-06-13 06:30:00', 'Pietermaritzburg to Durban', 70.00, 'Run'),
(1, 'Cape Town Cycle Tour', 'The world famous scenic cycle tour around the Cape Peninsula.', '2027-03-14 06:00:00', 'Cape Town', 100.00, 'Cycle'),
(1, 'Soweto 10km Community Walk', 'A community-focused historical walk through Soweto.', '2027-11-26 07:00:00', 'Soweto, Johannesburg', 15.00, 'Walk');

INSERT INTO Categories (EventID, CategoryName, MinAge, MaxAge) VALUES
(1, 'Open Senior (20-39)', 20, 39),
(1, 'Veterans (40-49)', 40, 49),
(2, 'Elite Cyclists', 18, 99),
(2, 'Sub-Veterans (30-39)', 30, 39),
(3, 'Fun Walk All Ages', 5, 99);

INSERT INTO Enrolments (ParticipantID, CategoryID, EnrolmentDate, Status) VALUES
(3, 1, GETDATE(), 'Confirmed'),
(4, 2, GETDATE(), 'Confirmed'),
(3, 3, GETDATE(), 'Confirmed');

INSERT INTO Results (EnrolmentID, FinishTime, OverallPosition, CategoryPosition) VALUES
(1, '06:22:34', 142, 45),
(2, '07:45:12', 1205, 310);
