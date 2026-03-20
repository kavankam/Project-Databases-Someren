using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace Someren.Models
{
    public class Activity
    {
        [Key] //Attributes are in brackets
        public int ActivityID { get; set; }

        public string Name { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime Day { get; set; }

        public TimeSpan TimeSlot { get; set; } //timespan bc we use 10:00:00        



    }
}
