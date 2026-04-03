using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories;

public class DrinkRepository : IDrinkRepository
{
    private readonly string _connectionString;

    public DrinkRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SomerenDatabase");
    }

    public List<Drink> GetAllDrinks()
    {
        List<Drink> drinks = new List<Drink>();

        string query = @"
            SELECT DrinkID, Name, Price, Stock, IsAlcoholic
            FROM dbo.DRINK
            ORDER BY Name";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Drink drink = ReadDrink(reader);
                drinks.Add(drink);
            }
        }

        return drinks;
    }

    public Drink? GetById(int id)
    {
        string query = @"
            SELECT DrinkID, Name, Price, Stock, IsAlcoholic
            FROM dbo.DRINK
            WHERE DrinkID = @id";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return ReadDrink(reader);
            }
        }

        return null;
    }

    public void UpdateStock(int drinkId, int newStock)
    {
        string query = @"
            UPDATE dbo.DRINK
            SET Stock = @newStock
            WHERE DrinkID = @drinkId";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@drinkId", drinkId);
            command.Parameters.AddWithValue("@newStock", newStock);
            connection.Open();

            command.ExecuteNonQuery();
        }
    }

    private Drink ReadDrink(SqlDataReader reader)
    {
        Drink drink = new Drink();
        drink.DrinkID = (int)reader["DrinkID"];
        drink.Name = (string)reader["Name"];
        drink.Price = (decimal)reader["Price"];
        drink.Stock = (int)reader["Stock"];
        drink.IsAlcoholic = (bool)reader["IsAlcoholic"];

        return drink;
    }
}