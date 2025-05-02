Create procedure [spGetAllEmployee]
as
begin
select *  from Employees order by Id
end