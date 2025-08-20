using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Student_Managment.Models;

namespace Student_Management.Data
{
    public class DataAccess
    {
        private readonly string _connectionString;

        public DataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // Helper method to map a DataRead
        private Student MapToStudent(MySqlDataReader reader)
        {
            return new Student
            {
                Id = reader.GetInt32("Id"),
                FirstName = reader.GetString("FirstName"),
                LastName = reader.GetString("LastName"),
                Email = reader.GetString("Email"),
                RollNumber = reader.GetString("RollNumber"),
                Department = reader.GetString("Department"),
                Year = reader.GetInt32("Year"),
                DateOfBirth = reader.GetDateTime("DateOfBirth"),
                CreatedAt = reader.GetDateTime("CreatedAt"),
                UpdatedAt = reader.GetDateTime("UpdatedAt")
            };
        }

        // GET: sttudent All or with name or eamil filter or rollno.
        public List<Student> GetStudents(string? name, string? email, string? rollNumber)
        {
            var students = new List<Student>();

            string query = "SELECT * FROM Student WHERE 1=1";
            var parameters = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(name))
            {
                query += " AND (FirstName LIKE @Name OR LastName LIKE @Name)";
                parameters.Add(new MySqlParameter("@Name", $"%{name}%"));
            }
            if (!string.IsNullOrEmpty(email))
            {
                query += " AND Email = @Email";
                parameters.Add(new MySqlParameter("@Email", email));
            }
            if (!string.IsNullOrEmpty(rollNumber))
            {
                query += " AND RollNumber = @RollNumber";
                parameters.Add(new MySqlParameter("@RollNumber", rollNumber));
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(MapToStudent(reader));
                        }
                    }
                }
            }
            return students;
        }

        // GET: Single student
        public Student? GetStudentById(int id)
        {
            Student? student = null;
            string query = "SELECT * FROM Student WHERE Id = @Id";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            student = MapToStudent(reader);
                        }
                    }
                }
            }
            return student;
        }

        // Add a new student
        public int CreateStudent(Student student)
        {
            int newId = 0;
            string query = @"INSERT INTO Student (FirstName, LastName, Email, RollNumber, Department, Year, DateOfBirth)
                             VALUES (@FirstName, @LastName, @Email, @RollNumber, @Department, @Year, @DateOfBirth);
                             SELECT LAST_INSERT_ID();";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@Email", student.Email);
                    command.Parameters.AddWithValue("@RollNumber", student.RollNumber);
                    command.Parameters.AddWithValue("@Department", student.Department);
                    command.Parameters.AddWithValue("@Year", student.Year);
                    command.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);

                    newId = Convert.ToInt32(command.ExecuteScalar());
                    student.Id = newId; // Update the student object with the new ID
                }
            }
            return newId;
        }

        // UPDATE: Update an existing student
        public void UpdateStudent(Student student)
        {
            string query = @"UPDATE Student SET
                             FirstName = @FirstName,
                             LastName = @LastName,
                             Email = @Email,
                             RollNumber = @RollNumber,
                             Department = @Department,
                             Year = @Year,
                             DateOfBirth = @DateOfBirth
                             WHERE Id = @Id";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", student.Id);
                    command.Parameters.AddWithValue("@FirstName", student.FirstName);
                    command.Parameters.AddWithValue("@LastName", student.LastName);
                    command.Parameters.AddWithValue("@Email", student.Email);
                    command.Parameters.AddWithValue("@RollNumber", student.RollNumber);
                    command.Parameters.AddWithValue("@Department", student.Department);
                    command.Parameters.AddWithValue("@Year", student.Year);
                    command.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);

                    command.ExecuteNonQuery();
                }
            }
        }

        // DELETE: Delete thw student
        public void DeleteStudent(int id)
        {
            string query = "DELETE FROM Student WHERE Id = @Id";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}