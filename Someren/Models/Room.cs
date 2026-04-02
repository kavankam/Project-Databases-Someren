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
    }
}