using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EmployeeAPI;
using EmployeeAPI.Models;
using EmployeeAPI.Controllers;
using EmployeeAPI.Models;
using EmployeeAPI.Data;

namespace EmployeeAPI.Data
{
    public interface IEmployeeDataAccessLayer
    {
        List<Employees> GetAllEmployees();
        Employees GetEmployeeById(int id);
        void AddEmployee(Employees employee);
        void UpdateEmployee(Employees employee);
        bool DeleteEmployee(int id);
    }

    public class EmployeeDataAccessLayer : IEmployeeDataAccessLayer
    {
        private readonly string connectionString;

        public EmployeeDataAccessLayer()
        {
            connectionString = ConnectionString.dbcs;
        }

        public List<Employees> GetAllEmployees()
        {
            List<Employees> employees = new List<Employees>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllEmployees", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                employees.Add(new Employees
                                {
                                    Id = Convert.ToInt32(rdr["Id"]),
                                    Name = rdr["Name"].ToString(),
                                    Age = Convert.ToInt32(rdr["Age"]),
                                    Gender = rdr["Gender"].ToString(),
                                    DOJ = Convert.ToDateTime(rdr["DOJ"])
                                });
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            return employees;
        }

        public Employees GetEmployeeById(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                return new Employees
                                {
                                    Id = Convert.ToInt32(rdr["Id"]),
                                    Name = rdr["Name"].ToString(),
                                    Age = Convert.ToInt32(rdr["Age"]),
                                    Gender = rdr["Gender"].ToString(),
                                    DOJ = Convert.ToDateTime(rdr["DOJ"])
                                };
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }

            return null;
        }

        public void AddEmployee(Employees employee)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Age", employee.Age);
                    cmd.Parameters.AddWithValue("@Gender", employee.Gender);
                    cmd.Parameters.AddWithValue("@DOJ", employee.DOJ);

                    try
                    {
                        con.Open();
                        employee.Id = (int)cmd.ExecuteScalar();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        public void UpdateEmployee(Employees employee)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", employee.Id);
                    cmd.Parameters.AddWithValue("@Name", employee.Name);
                    cmd.Parameters.AddWithValue("@Age", employee.Age);
                    cmd.Parameters.AddWithValue("@Gender", employee.Gender);
                    cmd.Parameters.AddWithValue("@DOJ", employee.DOJ);

                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new KeyNotFoundException($"Employee with ID {employee.Id} not found.");
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        public bool DeleteEmployee(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteEmployee", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        internal static object SearchEmployees(string name)
        {
            throw new NotImplementedException();
        }
    }
}
