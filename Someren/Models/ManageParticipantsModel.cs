namespace Someren.Models
{

    public class ManageParticipantsModel
    {
        public Activity Activity { get; set; }

        public List<Student> Participants { get; set; }

        public List<Student> NonParticipants { get; set; }
    }
}

