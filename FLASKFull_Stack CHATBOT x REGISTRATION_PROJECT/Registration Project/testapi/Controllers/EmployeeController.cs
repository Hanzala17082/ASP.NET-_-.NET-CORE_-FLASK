using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Net.Http.Json;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDataAccessLayer _employeeDataAccess;

        public EmployeeController(EmployeeDataAccessLayer employeeDataAccess)
        {
            _employeeDataAccess = employeeDataAccess;
        }

        // GET: api/Employee
        [HttpGet]
        public IActionResult GetEmployees()
        {
            var employees = _employeeDataAccess.GetAllEmployees();
            return Ok(employees);
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public IActionResult GetEmployee(int id)
        {
            var employee = _employeeDataAccess.GetEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        // POST: api/Employee
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employees employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _employeeDataAccess.AddEmployee(employee);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employees employee)
        {
            if (id != employee.Id)
            {
                return BadRequest();
            }

            try
            {
                _employeeDataAccess.UpdateEmployee(employee);
            }
            catch
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var result = _employeeDataAccess.DeleteEmployee(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}