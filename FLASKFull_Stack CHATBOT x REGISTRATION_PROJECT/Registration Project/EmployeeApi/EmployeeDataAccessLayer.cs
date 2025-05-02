using System.Collections.Generic;
using System.Linq;
using EmployeeAPI.Models;

namespace EmployeeAPI.DataAccessLayer
{
    public class EmployeeDataAccessLayer
    {
        // List to hold employees (in-memory storage)
        private readonly List<Employees> _employees = new List<Employees>();

        // Lock object for thread safety
        private readonly object _lock = new object();

        // Method to get all employees
        public List<Employees> GetAllEmployee()
        {
            return _employees;
        }

        // Method to get a single employee by ID
        public Employees GetEmployee(int id)
        {
            return _employees.FirstOrDefault(e => e.Id == id);
        }

        // Method to add a new employee
        public void AddEmployee(Employees employee)
        {
            lock (_lock)
            {
                // Generate a new unique ID
                employee.Id = _employees.Any() ? _employees.Max(e => e.Id) + 1 : 1;
                _employees.Add(employee);
            }
        }

        // Method to update an existing employee
        public void UpdateEmployee(Employees employee)
        {
            lock (_lock)
            {
                // Find the existing employee by ID
                var existing = _employees.FirstOrDefault(e => e.Id == employee.Id);
                if (existing == null)
                {
                    throw new KeyNotFoundException("Employee not found");
                }

                // Replace the existing employee with the updated data
                var index = _employees.IndexOf(existing);
                _employees[index] = employee;
            }
        }

        // Method to delete an employee by ID
        public bool DeleteEmployee(int id)
        {
            lock (_lock)
            {
                // Find the employee by ID
                var employee = _employees.FirstOrDefault(e => e.Id == id);
                if (employee == null)
                {
                    return false;
                }

                // Remove the employee from the list
                _employees.Remove(employee);
                return true;
            }
        }
    }
}
