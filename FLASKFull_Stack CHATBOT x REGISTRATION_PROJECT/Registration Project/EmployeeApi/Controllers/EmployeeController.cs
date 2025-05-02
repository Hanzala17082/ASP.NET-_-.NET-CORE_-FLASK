using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeAPI.Models;
using EmployeeAPI.DataAccessLayer;


namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
         EmployeeDbContext _context; // Injected DbContext

        public EmployeeController(EmployeeDbContext context)
        {
            _context =  context;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employees>>> GetEmployees()
        {
            try
            {
                var employees = await _context.Employees.ToListAsync(); // Retrieving employee data from the database
                return Ok(employees); // Sending the data to the client
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Employees>> GetEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id); // Retrieve specific employee by ID
                if (employee == null)
                {
                    return NotFound(); // Employee not found
                }
                return Ok(employee); // Return the employee
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<Employees>> CreateEmployee(Employees employee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Employees.Add(employee); // Add new employee to context
                await _context.SaveChangesAsync(); // Save changes to the database

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee); // Created response
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/employees/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employees employee)
        {
            if (id != employee.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Entry(employee).State = EntityState.Modified; // Mark employee entity as modified
                await _context.SaveChangesAsync(); // Save changes to the database

                return NoContent(); // Successful update
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id); // Find employee by ID
                if (employee == null)
                    return NotFound(); // Employee not found

                _context.Employees.Remove(employee); // Remove employee from context
                await _context.SaveChangesAsync(); // Save changes to the database

                return NoContent(); // Successful deletion
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
