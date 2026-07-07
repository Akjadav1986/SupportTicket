-- Optional SQL reference only. Prefer EF Core migrations for real setup.
CREATE DATABASE SupportTicketClassifierDb;
GO

USE SupportTicketClassifierDb;
GO

CREATE TABLE Tickets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    CustomerEmail NVARCHAR(150) NOT NULL,
    ProductName NVARCHAR(100) NULL,
    Status NVARCHAR(30) NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE TicketClassifications (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TicketId INT NOT NULL UNIQUE,
    Category NVARCHAR(80) NOT NULL,
    Priority NVARCHAR(30) NOT NULL,
    RoutedTeam NVARCHAR(80) NOT NULL,
    Confidence DECIMAL(5,2) NOT NULL,
    Reason NVARCHAR(1000) NOT NULL,
    TagsJson NVARCHAR(MAX) NOT NULL,
    ClassifiedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_TicketClassifications_Tickets_TicketId
        FOREIGN KEY (TicketId) REFERENCES Tickets(Id) ON DELETE CASCADE
);
GO
