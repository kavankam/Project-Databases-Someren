using Microsoft.AspNetCore.Mvc;
using Someren.Repositories;

namespace Someren.Controllers;

public class StudentsController : Controller
{
    private readonly IStudentRepository _studentRepository;

    public StudentsController(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public IActionResult Index()
    {
        var students = _studentRepository.GetAll();
        return View(students);
    }
}