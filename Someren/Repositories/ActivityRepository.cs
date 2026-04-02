using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using Someren.Models;
using Microsoft.Extensions.Configuration;

namespace Someren.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly string _connectionString;

        public ActivityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenDatabase");
        }

        public Activity? GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT ActivityID, Name, Day, TimeSlot, DurationMinutes FROM Activity WHERE ActivityID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Activity
                {
                    ActivityID = (int)reader["ActivityID"],
                    Name = (string)reader["Name"],
                    Day = (DateTime)reader["Day"],
                    TimeSlot = (TimeSpan)reader["TimeSlot"],
                    DurationMinutes = (int)reader["DurationMinutes"]
                };
            }

            return null;
        }

        public IEnumerable<Activity> GetAll()
        {
            var activities = new List<Activity>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(
                "SELECT ActivityID, Name, Day, TimeSlot, DurationMinutes FROM Activity", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                activities.Add(new Activity
                {
                    ActivityID = (int)reader["ActivityID"],
                    Name = (string)reader["Name"],
                    Day = (DateTime)reader["Day"],
                    TimeSlot = (TimeSpan)reader["TimeSlot"],
                    DurationMinutes = (int)reader["DurationMinutes"]
                });
            }

            return activities;
        }

        public void Add(Activity activity)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(
                "INSERT INTO Activity (Name, Day, TimeSlot, DurationMinutes) VALUES (@name, @day, @timeslot, @duration)", conn);
            cmd.Parameters.AddWithValue("@name", activity.Name);
            cmd.Parameters.AddWithValue("@day", activity.Day);
            cmd.Parameters.AddWithValue("@timeslot", activity.TimeSlot);
            cmd.Parameters.AddWithValue("@duration", activity.DurationMinutes);
            cmd.ExecuteNonQuery();
        }

        public void Update(Activity activity)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(
                "UPDATE Activity SET Name = @name, Day = @day, TimeSlot = @timeslot, DurationMinutes = @duration WHERE ActivityID = @id", conn);
            cmd.Parameters.AddWithValue("@name", activity.Name);
            cmd.Parameters.AddWithValue("@day", activity.Day);
            cmd.Parameters.AddWithValue("@timeslot", activity.TimeSlot);
            cmd.Parameters.AddWithValue("@duration", activity.DurationMinutes);
            cmd.Parameters.AddWithValue("@id", activity.ActivityID);
            cmd.ExecuteNonQuery();
        }

        public void Delete(Activity activity)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(
                "DELETE FROM Activity WHERE ActivityID = @id", conn);
            cmd.Parameters.AddWithValue("@id", activity.ActivityID);
            cmd.ExecuteNonQuery();
        }

        public bool SaveChanges()
        {            
            return true; //automatic save apply already,
        }
    }
}