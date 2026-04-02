using System.Data;
using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories;

public class LecturerRepository : ILecturerRepository
{
    private readonly string _connectionString;

    public LecturerRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SomerenDatabase");
    }

    private Lecturer ReadLecturer(SqlDataReader reader)
    {
        int lecturerId = (int)reader["LecturerID"];
        string firstName = (string)reader["FirstName"];
        string lastName = (string)reader["LastName"];
        string phoneNumber = (string)reader["PhoneNumber"];
        int age = (int)reader["Age"];

        return new Lecturer(lecturerId, age, firstName, lastName, phoneNumber);
    }

    public List<Lecturer> GetAll()
    {
        List<Lecturer> lecturers = new List<Lecturer>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT LecturerID, FirstName, LastName, PhoneNumber, Age " +
                           "FROM Lecturer " +
                           "ORDER BY LastName";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Lecturer lecturer = ReadLecturer(reader);

                lecturers.Add(lecturer);
            }
        }

        return lecturers;
    }

    public void Add(Lecturer lecturer)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "INSERT INTO Lecturer (FirstName, LastName, PhoneNumber, Age) " +
                           "VALUES (@FirstName, @LastName, @PhoneNumber, @Age)";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
            command.Parameters.AddWithValue("@LastName", lecturer.LastName);
            command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
            command.Parameters.AddWithValue("@Age", lecturer.Age);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    public Lecturer? GetById(int id)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT LecturerID, FirstName, LastName, PhoneNumber, Age " +
                           "FROM Lecturer " +
                           "WHERE LecturerID = @Id";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();


            if (reader.Read())
            {
                return ReadLecturer(reader);
            }

            return null;
        }
    }

    public void Update(Lecturer lecturer)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "UPDATE Lecturer SET FirstName = @FirstName, LastName = @LastName, " +
                           "PhoneNumber = @PhoneNumber, Age = @Age " +
                           "WHERE LecturerID = @Id";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
            command.Parameters.AddWithValue("@LastName", lecturer.LastName);
            command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
            command.Parameters.AddWithValue("@Age", lecturer.Age);
            command.Parameters.AddWithValue("@Id", lecturer.LecturerID);

            connection.Open();

            int nrOfRowsAffected = command.ExecuteNonQuery();
            if (nrOfRowsAffected == 0)
            {
                throw new Exception("No records updated!");
            }
        }
    }
    
    public void Delete(Lecturer lecturer)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "DELETE FROM Lecturer WHERE LecturerID = @Id";

            SqlCommand command = new SqlCommand(query, connection);
            
            command.Parameters.AddWithValue("@Id", lecturer.LecturerID);

            connection.Open();

            int nrOfRowsAffected = command.ExecuteNonQuery();
            if (nrOfRowsAffected == 0)
            {
                throw new Exception("No records deleted!");
            }
        }
    }
}