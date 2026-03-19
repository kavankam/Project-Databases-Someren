using Someren.Models;

namespace Someren.Repositories;

public interface IStudentRepository
{
    List<Student> GetAll(string? searchTerm);
    void Add(Student student);
    Student? GetById(int studentId);
    void Update(Student student);
    void Delete(int studentId);
    bool StudentNumberExists(int studentNumber, int? excludeStudentId = null);
    
    /* mani */
}