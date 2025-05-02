using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;

namespace WebApplication1
{
    public class EmployeeDataAccessLayer
    {
        private readonly List<Employees> _employees = new List<Employees>();

        public List<Employees> GetAllEmployees()
        {
            return _employees;
        }

        public Employees GetEmployee(int id)
        {
            return _employees.FirstOrDefault(e => e.Id == id);
        }

        public void AddEmployee(Employees employee)
        {
            employee.Id = _employees.Count + 1;
            _employees.Add(employee);
        }

        public void UpdateEmployee(Employees employee)
        {
            var existing = _employees.FirstOrDefault(e => e.Id == employee.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException();
            }

            var index = _employees.IndexOf(existing);
            _employees[index] = employee;
        }

        public bool DeleteEmployee(int id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
            {
                return false;
            }

            _employees.Remove(employee);
            return true;
        }
    }
}