namespace Someren.Models
{
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public int Floor { get; set; }
        public string RoomType { get; set; }
        public int BedsCapacity { get; set; }
        public int BuildingID { get; set; }

        public Room()
        {
            RoomNumber = string.Empty;
            RoomType = string.Empty;
        }

        public Room(int roomID, string roomNumber, int floor, string roomType, int bedsCapacity, int buildingID)
        {
            RoomID = roomID;
            RoomNumber = roomNumber;
            Floor = floor;
            RoomType = roomType;
            BedsCapacity = bedsCapacity;
            BuildingID = buildingID;
        }
    }
}