using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers;

public class StudentsController : Controller
{
    private const string StudentRoomType = "Student";
    private readonly IStudentRepository _studentRepository;
    private readonly IRoomRepository _roomRepository;

    public StudentsController(IStudentRepository studentRepository, IRoomRepository roomRepository)
    {
        _studentRepository = studentRepository;
        _roomRepository = roomRepository;
    }

    public IActionResult Index(string? searchTerm)
    {
        ViewBag.SearchTerm = searchTerm;
        List<Student> students = _studentRepository.GetAll(searchTerm);
        return View(students);
    }

    public IActionResult Create()
    {
         ViewBag.RoomOptions = GetRoomOptions(); 
        Student student = new Student();
        return View(student);
    }

    [HttpPost]
    public IActionResult Create(Student student)
    {
        if (StudentExists(student.StudentNumber, null))
        {
            ModelState.AddModelError("StudentNumber", "This student number already exists.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.RoomOptions = GetRoomOptions(); 
            return View(student);
        }

        _studentRepository.Add(student);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        Student? student = _studentRepository.GetById(id);
        if (student == null) return NotFound();
        ViewBag.RoomOptions = GetRoomOptions(); 
        return View(student);
    }

    [HttpPost]
    public IActionResult Edit(Student student)
    {
        if (StudentExists(student.StudentNumber, student.StudentID))
        {
            ModelState.AddModelError("StudentNumber", "This student number already exists.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.RoomOptions = GetRoomOptions();
            return View(student);
        } 

        _studentRepository.Update(student);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        Student? student = _studentRepository.GetById(id);
        if (student == null) return NotFound();
        return View(student);
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

        return RedirectToAction(nameof(Index));
    }

    private List<SelectListItem> GetRoomOptions()
    {
        List<SelectListItem> roomOptions = new List<SelectListItem>();
        List<Room> rooms = _roomRepository.GetAllRooms();

        foreach (Room room in rooms)
        {
            if (room.RoomType != null &&
                room.RoomType.Equals(StudentRoomType, StringComparison.OrdinalIgnoreCase))
            {
                SelectListItem option = new SelectListItem();
                option.Value = room.RoomID.ToString();
                option.Text = room.RoomNumber;
                roomOptions.Add(option);
            }
        }

        return roomOptions;
    } 

    private bool StudentExists(int studentNumber, int? excludeStudentId)
    {
        return _studentRepository.StudentNumberExists(studentNumber, excludeStudentId);
    }
}
