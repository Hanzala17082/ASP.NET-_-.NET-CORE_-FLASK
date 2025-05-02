using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebappTASK2.Models;

namespace WebappTASK2
{
    public class EmployeeDataAccessLayer
    {
        private readonly string cs = "Server=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\muham\\Registration.mdf;Trusted_Connection=True;";

        // Method to get SQL connection
        public SqlConnection GetSqlConnection()
        {
            return new SqlConnection(cs);
        }

        // Method to get all employees
        public List<Employee> GetAllEmployees()
        {
            List<Employee> emp_list = new List<Employee>();

            try
            {
                using (SqlConnection con = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("spGetAllEmployee", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Employee emp = new Employee
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Age = Convert.ToInt32(reader["Age"]),
                            Designation = reader["Designation"].ToString(),
                            City = reader["City"].ToString(),
                            Date_Of_Joining = Convert.ToDateTime(reader["Date_Of_Joining"])
                        };
                        emp_list.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetAllEmployees: " + ex.Message);
            }
            return emp_list;
        }



        // Method to add employee
        public void AddEmployee(Employee emp)
        {
            using (SqlConnection conn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spAddEmployee", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters as per the stored procedure
                cmd.Parameters.AddWithValue("@name", emp.Name);
                cmd.Parameters.AddWithValue("@gender", emp.Gender);
                cmd.Parameters.AddWithValue("@age", emp.Age);
                cmd.Parameters.AddWithValue("@designation", emp.Designation);
                cmd.Parameters.AddWithValue("@city", emp.City);
                cmd.Parameters.AddWithValue("@doj", emp.Date_Of_Joining);

                conn.Open(); // Open the connection
                cmd.ExecuteNonQuery(); // Execute the stored procedure
            }
        }



        // Method to update employee
        public void UpdateEmployee(Employee emp)
        {
            try
            {
                using (SqlConnection con = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("spUpdateEmployee", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Id", emp.Id);
                    cmd.Parameters.AddWithValue("@Name", emp.Name);
                    cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                    cmd.Parameters.AddWithValue("@Age", emp.Age);
                    cmd.Parameters.AddWithValue("@Designation", emp.Designation);
                    cmd.Parameters.AddWithValue("@City", emp.City);
                    cmd.Parameters.AddWithValue("@Date_Of_Joining", emp.Date_Of_Joining);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateEmployee: " + ex.Message);
            }
        }

 
        

        // Method to delete employee
        public void DeleteEmployee(int id)
        {
            try
            {
                using (SqlConnection con = GetSqlConnection())
                {
                    SqlCommand cmd = new SqlCommand("spDeleteEmployee", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in DeleteEmployee: " + ex.Message);
            }
        }
    }
}
