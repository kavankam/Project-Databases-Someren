namespace Someren.Models;

public class Student
{
    public int StudentID { get; set; }
    public int StudentNumber { get; set; }
    public string Class { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int? RoomID { get; set; }
}