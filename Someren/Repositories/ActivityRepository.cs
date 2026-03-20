using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories;

// FIXED: Changed class name to ActivityRepository to match the file and interface
public class ActivityRepository : IActivityRepository
{
    private readonly string _connectionString;

    public ActivityRepository(IConfiguration configuration)
    {
        // 1. Setup: Finds the database connection string in appsettings.json
        _connectionString = configuration.GetConnectionString("SomerenDatabase")
                            ?? throw new InvalidOperationException("Connection string 'SomerenDatabase' not found.");
    }
    public List<Activity> GetAll(string? searchTerm)
    {
        string query = BuildSelectQuery(searchTerm);
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(query, connection);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

        connection.Open();
        return MapActivities(command.ExecuteReader());
    }
    public void Add(Activity activity)
    {
        const string sql = "INSERT INTO dbo.ACTIVITY (Name, Day, TimeSlot) VALUES (@Name, @Day, @TimeSlot)";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);

        SetActivityParams(command, activity);
        connection.Open();
        command.ExecuteNonQuery();
    }
    public Activity? GetById(int id)
    {
        const string sql = "SELECT ActivityID, Name, Day, TimeSlot FROM dbo.ACTIVITY WHERE ActivityID = @Id";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);

        command.Parameters.AddWithValue("@Id", id);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        // Use the helper we already made to keep this under 10 lines!
        return MapActivities(reader).FirstOrDefault();
    }
    public void Update(Activity activity)
    {
        const string sql = "UPDATE dbo.ACTIVITY SET Name=@Name, Day=@Day, TimeSlot=@TimeSlot WHERE ActivityID=@Id";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);

        SetActivityParams(command, activity, true);
        connection.Open();
        command.ExecuteNonQuery();
    }
    public void Delete(int id)
    {
        const string sql = "DELETE FROM dbo.ACTIVITY WHERE ActivityID = @Id";
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);

        command.Parameters.AddWithValue("@Id", id);
        connection.Open();
        command.ExecuteNonQuery();
    }
    public bool ActivityNameExists(string name, int? excludedId = null)
    {
        string sql = "SELECT COUNT(*) FROM dbo.ACTIVITY WHERE Name = @Name" + (excludedId.HasValue ? " AND ActivityID <> @Id" : "");
        using SqlConnection connection = new(_connectionString);
        using SqlCommand command = new(sql, connection);

        command.Parameters.AddWithValue("@Name", name);
        if (excludedId.HasValue) command.Parameters.AddWithValue("@Id", excludedId.Value);

        connection.Open();
        return (int)command.ExecuteScalar() > 0; // Returns true if count is 1 or more
    }
    // Helper to prevent SQL Injection
    private void SetActivityParams(SqlCommand cmd, Activity act, bool includeId = false)
    {
        cmd.Parameters.AddWithValue("@Name", act.Name);
        cmd.Parameters.AddWithValue("@Day", act.Day);
        cmd.Parameters.AddWithValue("@TimeSlot", act.TimeSlot);
        if (includeId) cmd.Parameters.AddWithValue("@Id", act.ActivityID);
    }

    // Helper to turn database rows into C# objects
    private List<Activity> MapActivities(SqlDataReader reader)
    {
        List<Activity> list = new();
        while (reader.Read())
        {
            list.Add(new Activity
            {
                ActivityID = (int)reader["ActivityID"],
                Name = reader["Name"].ToString() ?? "",
                Day = (DateTime)reader["Day"],
                TimeSlot = (TimeSpan)reader["TimeSlot"]
            });
        }
        return list;
    }

    // Helper to write the SQL query
    private string BuildSelectQuery(string? searchTerm)
    {
        string query = "SELECT ActivityID, Name, Day, TimeSlot FROM ACTIVITY";

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query += " WHERE Name LIKE @SearchTerm";
        }

        // This is the "Ordered by date/time" requirement (80 pts)
        query += " ORDER BY Day ASC, TimeSlot ASC";

        return query;

    }
}