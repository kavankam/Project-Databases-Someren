using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories;

public class ActivitySupervisorRepository : IActivitySupervisorRepository
{
    private readonly string _connectionString;

    public ActivitySupervisorRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SomerenDatabase");
    }

    public List<Lecturer> GetSupervisors(int activityId)
    {
        List<Lecturer> lecturers = new List<Lecturer>();

        using SqlConnection connection = new SqlConnection(_connectionString) ;
        connection.Open();

        string query = @"SELECT LECTURER.*
                        FROM LECTURER
                        JOIN SUPERVISES ON LECTURER.LecturerID = SUPERVISES.LecturerID
                        WHERE SUPERVISES.ActivityID = @activityId";
        
        SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@activityId", activityId);
        
        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            lecturers.Add(new Lecturer
            {
                LecturerID = (int)reader["LecturerID"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                PhoneNumber = (string)reader["PhoneNumber"],
                Age = (int)reader["Age"]
            });
        }

        return lecturers;
    }
    
    
    public List<Lecturer> GetNonSupervisors(int activityId)
    {
        List<Lecturer> lecturers = new List<Lecturer>();

        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        string query = @"SELECT *
                        FROM LECTURER
                        WHERE LecturerID NOT IN (
                        SELECT LecturerID FROM SUPERVISES WHERE ActivityID = @activityId)";

        SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@activityId", activityId);

        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            lecturers.Add(new Lecturer
            {
                LecturerID = (int)reader["LecturerID"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                PhoneNumber = (string)reader["PhoneNumber"],
                Age = (int)reader["Age"]
            });
        }

        return lecturers;
    }
    
    public void AddSupervisor(int activityId, int lecturerId)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        string query = @" INSERT INTO SUPERVISES (ActivityID, LecturerID)
                          VALUES (@activityId, @lecturerId)";

        SqlCommand command = new SqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@activityId", activityId);
        command.Parameters.AddWithValue("@lecturerId", lecturerId);

        command.ExecuteNonQuery();
    }
    
    public void RemoveSupervisor(int activityId, int lecturerId)
    {
        using SqlConnection connection = new SqlConnection(_connectionString);
        connection.Open();

        string query = @"DELETE FROM SUPERVISES
                         WHERE ActivityID = @activityId AND LecturerID = @lecturerId";

        SqlCommand command = new SqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@activityId", activityId);
        command.Parameters.AddWithValue("@lecturerId", lecturerId);

        command.ExecuteNonQuery();
    }
}