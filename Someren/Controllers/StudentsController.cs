using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{

public class StudentsController : Controller
{
    private readonly IStudentRepository _studentRepository;
    private readonly IRoomRepository _roomRepository;

    public StudentsController(IStudentRepository studentRepository, IRoomRepository roomRepository)
    {
        _studentRepository = studentRepository;
        _roomRepository = roomRepository;
    }

    public IActionResult Index(string? searchTerm)
    {
        try
        {
            ViewData["SearchTerm"] = searchTerm;
            List<Student> students = _studentRepository.GetAll(searchTerm);
            return View(students);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Students could not be loaded.";
            return View(new List<Student>());
        }
    }

    public IActionResult Create()
    {
        try
        {
            ViewData["RoomOptions"] = GetRoomOptions();
            Student student = new Student();
            return View(student);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "The student form could not be loaded.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Create(Student student)
    {
        try
        {
            bool studentExists = StudentExists(student.StudentNumber, null);

            if (studentExists)
            {
                ViewData["StudentNumberError"] = "This student number already exists.";
                ViewData["RoomOptions"] = GetRoomOptions();
                return View(student);
            }

            _studentRepository.Add(student);
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            ViewData["ErrorMessage"] = "The student could not be added.";
            ViewData["RoomOptions"] = GetRoomOptions();
            return View(student);
        }
    }

    public IActionResult Edit(int id)
    {
        try
        {
            Student student = _studentRepository.GetById(id);
            if (student == null) return NotFound();
            ViewData["RoomOptions"] = GetRoomOptions();
            return View(student);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "The student could not be loaded.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Edit(Student student)
    {
        try
        {
            bool studentExists = StudentExists(student.StudentNumber, student.StudentID);

            if (studentExists)
            {
                ViewData["StudentNumberError"] = "This student number already exists.";
                ViewData["RoomOptions"] = GetRoomOptions();
                return View(student);
            }

            _studentRepository.Update(student);
            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            ViewData["ErrorMessage"] = "The student could not be changed.";
            ViewData["RoomOptions"] = GetRoomOptions();
            return View(student);
        }
    }

    public IActionResult Delete(int id)
    {
        try
        {
            Student student = _studentRepository.GetById(id);
            if (student == null) return NotFound();
            return View(student);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "The student could not be loaded.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int studentId)
    {
        try
        {
            _studentRepository.Delete(studentId);
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "This student cannot be deleted because it is still linked to other data.";
        }

        return RedirectToAction("Index");
    }

    private List<SelectListItem> GetRoomOptions()
    {
        List<SelectListItem> roomOptions = new List<SelectListItem>();
        List<Room> rooms = _roomRepository.GetAllRooms(null);

        foreach (Room room in rooms)
        {
            if (room.RoomType != null)
            {
                if (room.RoomType.ToLower() == "student")
                {
                    SelectListItem option = new SelectListItem();
                    option.Value = room.RoomID.ToString();
                    option.Text = room.RoomNumber;
                    roomOptions.Add(option);
                }
            }
        }

        return roomOptions;
    }

    private bool StudentExists(int studentNumber, int? excludeStudentId)
    {
        bool studentExists = _studentRepository.StudentNumberExists(studentNumber, excludeStudentId);
        return studentExists;
    }
}
}
