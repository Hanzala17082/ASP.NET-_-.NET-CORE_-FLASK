CREATE PROCEDURE spAddEmployee
(
    @name VARCHAR(50),
    @gender VARCHAR(50),
    @age INT,
    @designation VARCHAR(50),
    @city VARCHAR(50),
	@institute VARCHAR(100),
    @doj DATE -- Add the Date of Joining parameter
)
AS
BEGIN
    INSERT INTO Employees (Name, Gender, Age, Designation, City,Institute, Date_Of_Joining) -- Include Date_Of_Joining in the insert statement
    VALUES (@name, @gender, @age, @designation, @city,@institute, @doj)
END