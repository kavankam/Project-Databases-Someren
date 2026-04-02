namespace Someren.Models;

public class Activity
{
    public int ActivityID { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Day { get; set; }
    public TimeSpan TimeSlot { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public DateTime StartDateTime => Day.Date + TimeSlot;
}
