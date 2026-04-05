using Someren.Models;
using System.Collections.Generic;

namespace Someren.Repositories
{
    public interface IActivityParticipantRepository
    {
        List<Student> GetParticipants(int activityId);         
        List<Student> GetNonParticipants(int activityId);      
        void AddParticipant(int activityId, int studentId);    
        void RemoveParticipant(int activityId, int studentId); 
    }
}
