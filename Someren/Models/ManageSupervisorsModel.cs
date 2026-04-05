namespace Someren.Models;

public class ManageSupervisorsModel
{
    public Activity Activity { get; set; } 
    public List<Lecturer> Supervisors { get; set; } 
    public List<Lecturer> NonSupervisors { get; set; } 
}