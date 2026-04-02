using Someren.Models;

namespace Someren.Repositories;

public interface ILecturerRepository
{
    List<Lecturer> GetAll();
    Lecturer? GetById(int id);
    void Add(Lecturer lecturer);
    void Update(Lecturer lecturer);
    void Delete( Lecturer lecturer);
}