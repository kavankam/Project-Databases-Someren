using Someren.Models;

namespace Someren.Repositories
{
    public interface IRoomRepository
    {
        List<Room> GetAllRooms(int? bedsCapacity);
        Room? GetById(int id);
        void Add(Room room);
        void Update(Room room);
        void Delete(int id);
    }
}