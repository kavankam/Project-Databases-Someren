using Someren.Models;

namespace Someren.Repositories;

public interface IActivitySupervisorRepository
{
    List<Lecturer> GetSupervisors(int activityId);
    List<Lecturer> GetNonSupervisors(int activityId);
    void AddSupervisor(int activityId, int lecturerId);
    void RemoveSupervisor(int activityId, int lecturerId);
}