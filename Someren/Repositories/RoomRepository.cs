using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString;

        public RoomRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenDatabase");

            if (_connectionString == null)
            {
                throw new InvalidOperationException("Connection string could not be found.");
            }
        }

        public List<Room> GetAllRooms(int? bedsCapacity)
        {
            List<Room> rooms = new List<Room>();

            string query = @"
                SELECT RoomID, RoomNumber, Floor, RoomType, BedsCapacity, BuildingID
                FROM dbo.ROOM
                WHERE (@bedsCapacity IS NULL OR BedsCapacity = @bedsCapacity)
                ORDER BY TRY_CAST(RoomNumber AS INT), RoomNumber";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                if (bedsCapacity == null)
                {
                    command.Parameters.AddWithValue("@bedsCapacity", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@bedsCapacity", bedsCapacity);
                }
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Room room = ReadRoom(reader);
                    rooms.Add(room);
                }
            }

            return rooms;
        }

        public Room? GetById(int id)
        {
            string query = @"
                SELECT RoomID, RoomNumber, Floor, RoomType, BedsCapacity, BuildingID
                FROM dbo.ROOM
                WHERE RoomID = @id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return ReadRoom(reader);
                }
            }

            return null;
        }

        public void Add(Room room)
        {
            string query = @"
                INSERT INTO dbo.ROOM (RoomNumber, Floor, RoomType, BedsCapacity, BuildingID)
                VALUES (@roomNumber, @floor, @roomType, @bedsCapacity, @buildingId)";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@roomNumber", room.RoomNumber);
                command.Parameters.AddWithValue("@floor", room.Floor);
                command.Parameters.AddWithValue("@roomType", room.RoomType);
                command.Parameters.AddWithValue("@bedsCapacity", room.BedsCapacity);
                command.Parameters.AddWithValue("@buildingId", room.BuildingID);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void Update(Room room)
        {
            string query = @"
                UPDATE dbo.ROOM
                SET RoomNumber = @roomNumber,
                    Floor = @floor,
                    RoomType = @roomType,
                    BedsCapacity = @bedsCapacity,
                    BuildingID = @buildingId
                WHERE RoomID = @id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", room.RoomID);
                command.Parameters.AddWithValue("@roomNumber", room.RoomNumber);
                command.Parameters.AddWithValue("@floor", room.Floor);
                command.Parameters.AddWithValue("@roomType", room.RoomType);
                command.Parameters.AddWithValue("@bedsCapacity", room.BedsCapacity);
                command.Parameters.AddWithValue("@buildingId", room.BuildingID);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            string query = "DELETE FROM dbo.ROOM WHERE RoomID = @id";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                command.ExecuteNonQuery();
            }
        }

        private Room ReadRoom(SqlDataReader reader)
        {
            Room room = new Room();
            room.RoomID = (int)reader["RoomID"];
            room.RoomNumber = (string)reader["RoomNumber"];
            room.Floor = (int)reader["Floor"];
            room.RoomType = (string)reader["RoomType"];
            room.BedsCapacity = (int)reader["BedsCapacity"];
            room.BuildingID = (int)reader["BuildingID"];

            return room;
        }
    }
}