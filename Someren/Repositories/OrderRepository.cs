using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SomerenDatabase");

        if (_connectionString == null)
        {
            throw new InvalidOperationException("Connection string could not be found.");
        }
    }

    public void AddOrder(Order order)
    {
        string query = @"
            INSERT INTO dbo.ORDERS (StudentID, DrinkID, Quantity)
            VALUES (@studentId, @drinkId, @quantity)";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@studentId", order.StudentID);
            command.Parameters.AddWithValue("@drinkId", order.DrinkID);
            command.Parameters.AddWithValue("@quantity", order.Quantity);
            connection.Open();

            command.ExecuteNonQuery();
        }
    }
}