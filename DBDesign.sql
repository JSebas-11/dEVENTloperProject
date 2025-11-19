CREATE DATABASE EventsProject;
GO
USE EventsProject;
GO

--TABLES CREATION
CREATE TABLE UserAccount (
	UserId INT PRIMARY KEY IDENTITY(1, 1),
	Dni VARCHAR(12) UNIQUE NOT NULL,
	UserName VARCHAR(32) NOT NULL,
	HashPassword VARCHAR(60) NOT NULL,
	UserEmail VARCHAR(128) NOT NULL,
	UserImg VARBINARY(MAX) NULL,
	IsAdmin BIT NOT NULL
);

CREATE TABLE Category (
	CatId INT PRIMARY KEY IDENTITY(1, 1),
	CatName VARCHAR(16) UNIQUE NOT NULL,
	CatDescription VARCHAR(64) NOT NULL
);

CREATE TABLE EventState (
	EventStateId INT PRIMARY KEY,
	StateName VARCHAR(16) UNIQUE NOT NULL, --(Active, Pending, Cancelled, Finished, SoldOut)
);

CREATE TABLE EventInfo (
	EventId INT PRIMARY KEY IDENTITY(1, 1),
	Title VARCHAR(32) NOT NULL,
	EventDate DATETIME NOT NULL,
	EventPlace VARCHAR(32) NOT NULL,
	EventCity VARCHAR(32) NOT NULL,
	CatId INT NULL,
	InitialTime DATETIME NOT NULL,
	EndTime DATETIME NOT NULL,
	MinutesDuration INT NOT NULL,
	Artist VARCHAR(128) NOT NULL,
	EventDescription VARCHAR(256) NOT NULL,
	Capacity INT NOT NULL,
	EventStateId INT NULL,
	EventImg VARBINARY(MAX) NULL,
	CreatedById INT NULL,

	CONSTRAINT FK_EventInfo_Category FOREIGN KEY (CatId) REFERENCES Category(CatId) ON DELETE SET NULL,
	CONSTRAINT FK_EventInfo_EventState FOREIGN KEY (EventStateId) REFERENCES EventState(EventStateId) ON DELETE SET NULL,
	CONSTRAINT FK_EventInfo_UserAccount FOREIGN KEY (CreatedById) REFERENCES UserAccount(UserId) ON DELETE SET NULL
);          

CREATE TABLE UserEvent (
	UserEventId INT PRIMARY KEY IDENTITY(1, 1),
	UserId INT NOT NULL,
	EventId INT NOT NULL,
	EnrolledAt DATETIME NOT NULL,
	TicketsAmount INT NOT NULL,

	CONSTRAINT FK_UserEvent_UserAccount FOREIGN KEY (UserId) REFERENCES UserAccount(UserId) ON DELETE CASCADE,
	CONSTRAINT FK_UserEvent_EventInfo FOREIGN KEY (EventId) REFERENCES EventInfo(EventId) ON DELETE CASCADE
);

CREATE TABLE NotificationState (
	NotStateId INT PRIMARY KEY,
	StateName VARCHAR(16) UNIQUE NOT NULL, --(Read, UnRead, Removed)
);

CREATE TABLE NotificationInfo (
	NotificationId INT PRIMARY KEY IDENTITY(1, 1),
	NotStateId INT NULL,
	UserId INT NOT NULL,
	NotMessage VARCHAR(128) NOT NULL,
	CreatedAt DATETIME NOT NULL,

	CONSTRAINT FK_NotificationInfo_NotificationState FOREIGN KEY (NotStateId) REFERENCES NotificationState(NotStateId) ON DELETE SET NULL,
	CONSTRAINT FK_NotificationInfo_UserAccount FOREIGN KEY (UserId) REFERENCES UserAccount(UserId) ON DELETE CASCADE,
);

--TRIGGERS
CREATE TABLE UsersReg (
	UsersRegId INT PRIMARY KEY IDENTITY(1, 1),
	Dni VARCHAR(12) UNIQUE NOT NULL,
	IsAdmin BIT NOT NULL,
	createdAt DATETIME NOT NULL,
	deletedAt DATETIME NULL
);
GO

CREATE TRIGGER UsersReg_AI
	ON UserAccount AFTER INSERT AS
	BEGIN
		INSERT INTO UsersReg (Dni, IsAdmin, createdAt, deletedAt)
			SELECT Dni, IsAdmin, GETDATE(), NULL FROM inserted
	END;
GO

CREATE TRIGGER UsersReg_AD
	ON UserAccount AFTER DELETE AS
	BEGIN
		UPDATE UsersReg SET deletedAt = GETDATE()
		WHERE Dni = (SELECT Dni FROM deleted)
	END;
GO

--STATIC DATA INSERTION
INSERT INTO EventState
VALUES (1, 'Active'), (2, 'Pending'), (3, 'Cancelled'), (4, 'Finished'), (5, 'SoldOut');

INSERT INTO NotificationState
VALUES (1, 'Read'), (2, 'UnRead'), (3, 'Removed');

INSERT INTO Category (CatName, CatDescription)
VALUES
    ('Cinema', 'Movies and films screenings across various genres.'),
    ('Theater', 'Plays, musicals, and live theater performances.'),
    ('Comedy', 'Stand-up shows and comedy events.'),
    ('Dance', 'Dancing performances of any kind of genre.'),
    ('Concerts', 'Live music performances by bands, singers, or orchestras.'),
    ('Exhibitions', 'Art galleries, photography, and cultural showcases.'),
    ('Festivals', 'Music, cultural, or gastronomic festivals.'),
    ('Sports', 'Sporting events, tournaments, and competitions.'),
    ('Workshops', 'Interactive learning and training events.'),
    ('Tech Events', 'Conferences, networking, and tech talks.'),
    ('Other', 'Other kind of event.');

SELECT * FROM UserAccount;
SELECT * FROM UsersReg;
SELECT * FROM EventInfo;
SELECT * FROM UserEvent;
SELECT * FROM NotificationInfo;