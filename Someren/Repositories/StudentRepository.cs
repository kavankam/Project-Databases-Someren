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

    public List<Student> GetAll(string? searchTerm)
    {
        List<Student> students = new();

        string query = @"
        SELECT StudentID, StudentNumber, Class, FirstName, LastName, PhoneNumber, RoomID
        FROM dbo.STUDENT";

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query += " WHERE LastName LIKE @SearchTerm";
        }

        query += " ORDER BY LastName, FirstName;";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
        }

        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            Student student = new()
            {
                StudentID = Convert.ToInt32(reader["StudentID"]),
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

    public void Add(Student student)
    {
        const string query = @"
        INSERT INTO dbo.STUDENT (StudentNumber, Class, FirstName, LastName, PhoneNumber, RoomID)
        VALUES (@StudentNumber, @Class, @FirstName, @LastName, @PhoneNumber, @RoomID);";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);

        command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
        command.Parameters.AddWithValue("@Class", student.Class);
        command.Parameters.AddWithValue("@FirstName", student.FirstName);
        command.Parameters.AddWithValue("@LastName", student.LastName);
        command.Parameters.AddWithValue("@PhoneNumber", (object?)student.PhoneNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@RoomID", student.RoomID);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public Student? GetById(int studentId)
    {
        const string query = @"
        SELECT StudentID, StudentNumber, Class, FirstName, LastName, PhoneNumber, RoomID
        FROM dbo.STUDENT
        WHERE StudentID = @StudentID;";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@StudentID", studentId);

        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        if (reader.Read())
        {
            return new Student
            {
                StudentID = Convert.ToInt32(reader["StudentID"]),
                StudentNumber = Convert.ToInt32(reader["StudentNumber"]),
                Class = reader["Class"].ToString() ?? string.Empty,
                FirstName = reader["FirstName"].ToString() ?? string.Empty,
                LastName = reader["LastName"].ToString() ?? string.Empty,
                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : reader["PhoneNumber"].ToString(),
                RoomID = Convert.ToInt32(reader["RoomID"])
            };
        }

        return null;
    }

    public void Update(Student student)
    {
        const string query = @"
        UPDATE dbo.STUDENT
        SET StudentNumber = @StudentNumber,
            Class = @Class,
            FirstName = @FirstName,
            LastName = @LastName,
            PhoneNumber = @PhoneNumber,
            RoomID = @RoomID
        WHERE StudentID = @StudentID;";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);

        command.Parameters.AddWithValue("@StudentID", student.StudentID);
        command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
        command.Parameters.AddWithValue("@Class", student.Class);
        command.Parameters.AddWithValue("@FirstName", student.FirstName);
        command.Parameters.AddWithValue("@LastName", student.LastName);
        command.Parameters.AddWithValue("@PhoneNumber", (object?)student.PhoneNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@RoomID", student.RoomID);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Delete(int studentId)
    {
        const string query = @"
        DELETE FROM dbo.STUDENT
        WHERE StudentID = @StudentID;";

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);

        command.Parameters.AddWithValue("@StudentID", studentId);

        connection.Open();
        command.ExecuteNonQuery();
    }
    
    public bool StudentNumberExists(int studentNumber, int? excludeStudentId = null)
    {
        string query = @"
        SELECT COUNT(*)
        FROM dbo.STUDENT
        WHERE StudentNumber = @StudentNumber";

        if (excludeStudentId.HasValue)
        {
            query += " AND StudentID <> @ExcludeStudentId";
        }

        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);

        command.Parameters.AddWithValue("@StudentNumber", studentNumber);

        if (excludeStudentId.HasValue)
        {
            command.Parameters.AddWithValue("@ExcludeStudentId", excludeStudentId.Value);
        }

        connection.Open();
        int count = (int)command.ExecuteScalar();

        return count > 0;
    }
}