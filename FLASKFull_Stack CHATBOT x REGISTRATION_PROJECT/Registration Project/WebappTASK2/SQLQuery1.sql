USE [C:\USERS\MUHAM\REGISTRATION.MDF]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[spGetAllEmployee]

SELECT	@return_value as 'Return Value'

GO
