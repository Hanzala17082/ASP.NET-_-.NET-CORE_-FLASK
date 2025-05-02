CREATE PROCEDURE spUpdateEmployee
(
    @Id INT,
    @name VARCHAR(50),
    @gender VARCHAR(50),
    @age INT,
    @designation VARCHAR(50),
    @city VARCHAR(50),
	@institute VARCHAR(100),
    @doj DATE -- Add parameter for Date of Joining
)
AS 
BEGIN 
    UPDATE Employees 
    SET 
        Name = @name,
        Gender = @gender,
        Age = @age,
        Designation = @designation,
        City = @city,
		Institute=@institute,
        Date_Of_Joining = @doj -- Update Date of Joining if needed
    WHERE Id = @Id
END