using Someren.Models;

namespace Someren.Repositories;

public interface IStudentRepository
{
    List<Student> GetAll();
}