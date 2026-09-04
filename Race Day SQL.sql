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