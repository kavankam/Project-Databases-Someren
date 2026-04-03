using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories;

public class ActivityParticipantRepository : IActivityParticipantRepository
{
    private readonly string _connectionString;

    public ActivityParticipantRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SomerenDatabase");
    }

    public List<Student> GetParticipants(int activityId)
    {
        var students = new List<Student>();

        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        var cmd = new SqlCommand(@"
            SELECT S.*
            FROM STUDENT S
            JOIN PARTICIPATES_IN P ON S.StudentID = P.StudentID
            WHERE P.ActivityID = @activityId", conn);
        cmd.Parameters.AddWithValue("@activityId", activityId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            students.Add(new Student
            {
                StudentID = (int)reader["StudentID"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                Class = (string)reader["Class"]
            });
        }

        return students;
    }

    public List<Student> GetNonParticipants(int activityId)
    {
        var students = new List<Student>();

        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        var cmd = new SqlCommand(@"
            SELECT *
            FROM STUDENT
            WHERE StudentID NOT IN (
                SELECT StudentID FROM PARTICIPATES_IN WHERE ActivityID = @activityId
            )", conn);
        cmd.Parameters.AddWithValue("@activityId", activityId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            students.Add(new Student
            {
                StudentID = (int)reader["StudentID"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                Class = (string)reader["Class"]
            });
        }

        return students;
    }

    public void AddParticipant(int activityId, int studentId)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        var cmd = new SqlCommand(@"
            INSERT INTO PARTICIPATES_IN (ActivityID, StudentID)
            VALUES (@activityId, @studentId)", conn);
        cmd.Parameters.AddWithValue("@activityId", activityId);
        cmd.Parameters.AddWithValue("@studentId", studentId);

        cmd.ExecuteNonQuery();
    }

    public void RemoveParticipant(int activityId, int studentId)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        var cmd = new SqlCommand(@"
            DELETE FROM PARTICIPATES_IN
            WHERE ActivityID = @activityId AND StudentID = @studentId", conn);
        cmd.Parameters.AddWithValue("@activityId", activityId);
        cmd.Parameters.AddWithValue("@studentId", studentId);

        cmd.ExecuteNonQuery();
    }
}