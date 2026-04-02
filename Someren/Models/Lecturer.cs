namespace Someren.Models;

public class Lecturer
{
    public int LecturerID { get; set; }
    public int Age { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }


    public Lecturer()
    {
    }

    public Lecturer(int lecturerID, int age, string firstName, string lastName, string phoneNumber)
    {
        LecturerID = lecturerID;
        Age = age;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }
}