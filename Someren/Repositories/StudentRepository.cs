using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly string _connectionString;

    public StudentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SomerenDatabase")
                            ?? throw new InvalidOperationException("Connection string 'SomerenDatabase' not found.");
    }

    public List<Student> GetAll()
    {
        List<Student> students = new();

        const string query = @"
            SELECT PersonID, StudentNumber, Class, FirstName, LastName, PhoneNumber, RoomID
            FROM dbo.STUDENT
            ORDER BY LastName, FirstName;";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);

        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Student student = new()
            {
                PersonID = Convert.ToInt32(reader["PersonID"]),
                StudentNumber = Convert.ToInt32(reader["StudentNumber"]),
                Class = reader["Class"].ToString() ?? string.Empty,
                FirstName = reader["FirstName"].ToString() ?? string.Empty,
                LastName = reader["LastName"].ToString() ?? string.Empty,
                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : reader["PhoneNumber"].ToString(),
                RoomID = Convert.ToInt32(reader["RoomID"])
            };

            students.Add(student);
        }

        return students;
    }
}