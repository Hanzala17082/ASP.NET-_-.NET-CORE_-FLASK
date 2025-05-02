USE [CrudADOdb]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetEmployeeById]
(
    @Id INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Gender, Age, DOJ
    FROM Employees
    WHERE Id = @Id;
END
GO
