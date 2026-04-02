using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers;

public class LecturersController : Controller
{
    private readonly ILecturerRepository _lecturerRepository;

    public LecturersController(ILecturerRepository lecturerRepository)
    {
        _lecturerRepository = lecturerRepository;
    }

    public IActionResult Index()
    {
        List<Lecturer> lecturers = _lecturerRepository.GetAll();
        return View(lecturers);
    }
    
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Lecturer lecturer)
    {
        try
        {
            _lecturerRepository.Add(lecturer);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            return Content(ex.Message);
            //return View(lecturer);
        }

    }
    
    public IActionResult Edit(int id)
    {
        Lecturer? lecturer = _lecturerRepository.GetById(id);

        if (lecturer == null)
        {
            return NotFound();
        }

        return View(lecturer);
    }

    [HttpPost]
    public IActionResult Edit(Lecturer lecturer)
    {
        try
        {
            _lecturerRepository.Update(lecturer);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            return View(lecturer);
        }
    }
    
    [HttpGet]
    public IActionResult Delete(int? id)
    {
        if (id == null) //if no id is received in the URL
        {
            return NotFound();
        }

        Lecturer? lecturer = _lecturerRepository.GetById((int)id);

        if (lecturer == null) //if id is received in the URL but it is not in the database  
        {
            return NotFound();
        }

        return View(lecturer);
    }

    [HttpPost]
    public IActionResult Delete(Lecturer lecturer)
    {
        try
        {
            _lecturerRepository.Delete(lecturer);
            
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            return View(lecturer);
        }
    }
}


