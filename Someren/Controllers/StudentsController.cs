using Microsoft.AspNetCore.Mvc;
using Someren.Repositories;
using Someren.Models;

namespace Someren.Controllers;

public class StudentsController : Controller
{
    private readonly IStudentRepository _studentRepository;

    public StudentsController(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public IActionResult Index(string? searchTerm)
    {
        ViewBag.SearchTerm = searchTerm;
        var students = _studentRepository.GetAll(searchTerm);
        return View(students);
    }
    
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Student student)
    {
        if (!ModelState.IsValid)
        {
            return View(student);
        }
        
        if (_studentRepository.StudentNumberExists(student.StudentNumber))
        {
            ModelState.AddModelError("StudentNumber", "This student number already exists.");
            return View(student);
        }

        _studentRepository.Add(student);
        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult Edit(int id)
    {
        var student = _studentRepository.GetById(id);

        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }
    
    [HttpPost]
    public IActionResult Edit(Student student)
    {
        if (!ModelState.IsValid)
        {
            return View(student);
        }

        if (_studentRepository.StudentNumberExists(student.StudentNumber, student.StudentID))
        {
            ModelState.AddModelError("StudentNumber", "This student number already exists.");
            return View(student);
        }

        _studentRepository.Update(student);
        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult Delete(int id)
    {
        var student = _studentRepository.GetById(id);

        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int studentId)
    {
        _studentRepository.Delete(studentId);
        return RedirectToAction(nameof(Index));
    }
}

