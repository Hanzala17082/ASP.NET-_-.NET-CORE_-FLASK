using Microsoft.AspNetCore.Mvc;
using WebappTASK2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Diagnostics;

namespace WebappTASK2.Controllers
{
    public class HomeController : Controller
    {
        private readonly YourDbContext _context;

        // Injecting the DbContext into the constructor
        public HomeController(YourDbContext context)
        {
            _context = context;
        }

        // Action to show all employees in the Index view
        public IActionResult Index()
        {
            var employees = _context.Employees.ToList();  // Get all employees from the database
            return View(employees);  // Return the employees list to the Index view
        }

        // Action to handle employee creation (GET for displaying the form)
        public IActionResult Create()
        {
            return View();  // Render the Create form
        }

        // Action to handle employee creation (POST for saving the data)
        [HttpPost]
        public IActionResult Create(Employee emp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();  // Return a bad request if the model state is invalid
            }

            try
            {
                _context.Employees.Add(emp);  // Add the new employee
                _context.SaveChanges();       // Save changes to the database

                // Return the updated employee list as a partial view
                var employees = _context.Employees.ToList();
                return PartialView("_EmployeeListPartial", employees);
            }
            catch (Exception ex)
            {
                // Handle error
                ModelState.AddModelError("", "An error occurred while adding the employee.");
                return View(emp);  // Return to the Create view in case of error
            }
        }


        // Action to handle employee editing (GET)
        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);  // Return the Edit view with employee details
        }

        // Action to handle employee editing (POST for updating the data)
        [HttpPost]
        public IActionResult Edit(Employee emp)
        {
            if (!ModelState.IsValid)
            {
                return View(emp);  // Return the form back with validation errors
            }

            _context.Employees.Update(emp);  // Update the employee in the database
            _context.SaveChanges();          // Save changes to the database
            return RedirectToAction("Index");  // Redirect back to the Index page after updating
        }

        // Privacy view (static page)
        public IActionResult Privacy()
        {
            return View();
        }

        // Error handling view
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
