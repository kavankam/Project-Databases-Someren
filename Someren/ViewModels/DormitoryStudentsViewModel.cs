using Someren.Models;

namespace Someren.ViewModels
{
    public class DormitoryStudentsViewModel
    {
        public Room Room { get; set; }
        public List<Student> DormitoryStudents { get; set; }
        public List<Student> RoomlessStudents { get; set; }

        public DormitoryStudentsViewModel()
        {
            Room = new Room();
            DormitoryStudents = new List<Student>();
            RoomlessStudents = new List<Student>();
        }
    }
}