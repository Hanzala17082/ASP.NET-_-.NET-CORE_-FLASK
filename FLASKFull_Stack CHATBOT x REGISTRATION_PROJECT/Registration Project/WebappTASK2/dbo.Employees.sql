CREATE TABLE [dbo].[Employees] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (50)  NOT NULL DEFAULT hanzala,
    [Gender]          VARCHAR (50)  NULL DEFAULT male,
    [Age]             INT           NULL DEFAULT 21,
    [Designation]     VARCHAR (50)  NULL DEFAULT intern,
    [City]            VARCHAR (50)  NULL DEFAULT lahore,
    [Institute]       VARCHAR (100) NULL,
    [Date_Of_Joining] DATE          NULL DEFAULT 20-11-2024,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

