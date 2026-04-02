using System.Data;
using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories;

public class StudentRepository : IStudentRepository
{
    private const string ConnectionStringName = "SomerenDatabase";
    private readonly string _connectionString;

    public StudentRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString(ConnectionStringName)
                            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public List<Student> GetAll(string? searchTerm)
    {
        string query = GetAllQuery(searchTerm);
        using SqlDataReader reader = GetReader(query, cmd => AddSearchParameter(cmd, searchTerm));
        return ReadStudents(reader);
    }

    public void Add(Student student)
    {
        const string query = @"INSERT INTO dbo.STUDENT (StudentNumber, Class, FirstName, LastName, PhoneNumber, RoomID)
VALUES (@StudentNumber, @Class, @FirstName, @LastName, @PhoneNumber, @RoomID);";
        ExecuteStudentQuery(query, student);
    }

    public Student? GetById(int studentId)
    {
        const string query = @"SELECT StudentID, StudentNumber, Class, FirstName, LastName, PhoneNumber, RoomID
FROM dbo.STUDENT
WHERE StudentID = @StudentID;";
        using SqlDataReader reader = GetReader(query, cmd => cmd.Parameters.AddWithValue("@StudentID", studentId));
        return reader.Read() ? MapStudent(reader) : null;
    }

    public void Update(Student student)
    {
        const string query = @"UPDATE dbo.STUDENT
SET StudentNumber = @StudentNumber,
    Class = @Class,
    FirstName = @FirstName,
    LastName = @LastName,
    PhoneNumber = @PhoneNumber,
    RoomID = @RoomID
WHERE StudentID = @StudentID;";
        ExecuteStudentQuery(query, student, true);
    }

    public void Delete(int studentId)
    {
        const string query = "DELETE FROM dbo.STUDENT WHERE StudentID = @StudentID;";
        ExecuteQuery(query, cmd => cmd.Parameters.AddWithValue("@StudentID", studentId));
    }

    public bool StudentNumberExists(int studentNumber, int? excludeStudentId = null)
    {
        string query = GetExistsQuery(excludeStudentId);
        int count = CountStudents(query, studentNumber, excludeStudentId);
        return count > 0;
    }

    private string GetAllQuery(string? searchTerm)
    {
        string query = @"SELECT StudentID, StudentNumber, Class, FirstName, LastName, PhoneNumber, RoomID
FROM dbo.STUDENT";
        if (!string.IsNullOrWhiteSpace(searchTerm)) query += " WHERE LastName LIKE @SearchTerm";
        return query + " ORDER BY LastName, FirstName;";
    }

    private string GetExistsQuery(int? excludeStudentId)
    {
        string query = "SELECT COUNT(*) FROM dbo.STUDENT WHERE StudentNumber = @StudentNumber";
        if (excludeStudentId.HasValue) query += " AND StudentID <> @ExcludeStudentId";
        return query;
    }

    private SqlDataReader GetReader(string query, Action<SqlCommand> addParameters)
    {
        SqlConnection connection = new(_connectionString);
        SqlCommand command = new(query, connection);
        addParameters(command);
        connection.Open();
        return command.ExecuteReader(CommandBehavior.CloseConnection);
    }

    private List<Student> ReadStudents(SqlDataReader reader)
    {
        List<Student> students = new();
        while (reader.Read()) students.Add(MapStudent(reader));
        return students;
    }

    private void ExecuteStudentQuery(string query, Student student, bool includeId = false)
    {
        ExecuteQuery(query, cmd => AddStudentParameters(cmd, student, includeId));
    }

    private void ExecuteQuery(string query, Action<SqlCommand> addParameters)
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);
        addParameters(command);
        connection.Open();
        command.ExecuteNonQuery();
    }

    private int CountStudents(string query, int studentNumber, int? excludeStudentId)
    {
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);
        AddExistsParameters(command, studentNumber, excludeStudentId);
        connection.Open();
        return (int)command.ExecuteScalar()!;
    }

    private void AddSearchParameter(SqlCommand command, string? searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
    }

    private void AddExistsParameters(SqlCommand command, int studentNumber, int? excludeStudentId)
    {
        command.Parameters.AddWithValue("@StudentNumber", studentNumber);
        if (excludeStudentId.HasValue)
            command.Parameters.AddWithValue("@ExcludeStudentId", excludeStudentId.Value);
    }

    private void AddStudentParameters(SqlCommand command, Student student, bool includeId)
    {
        if (includeId) command.Parameters.AddWithValue("@StudentID", student.StudentID);
        command.Parameters.AddWithValue("@StudentNumber", student.StudentNumber);
        command.Parameters.AddWithValue("@Class", student.Class);
        command.Parameters.AddWithValue("@FirstName", student.FirstName);
        command.Parameters.AddWithValue("@LastName", student.LastName);
        command.Parameters.AddWithValue("@PhoneNumber", (object?)student.PhoneNumber ?? DBNull.Value);
        command.Parameters.AddWithValue("@RoomID", student.RoomID);
    }

    private Student MapStudent(SqlDataReader reader)
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
}