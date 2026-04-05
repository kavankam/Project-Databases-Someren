using Someren.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IStudentRepository _studentRepository;

        public RoomsController(IRoomRepository roomRepository, IStudentRepository studentRepository)
        {
            _roomRepository = roomRepository;
            _studentRepository = studentRepository;
        }

        public IActionResult Index(int? bedsCapacity)
        {
            try
            {
                List<Room> rooms = _roomRepository.GetAllRooms(bedsCapacity);
                ViewData["BedsCapacity"] = bedsCapacity;
                return View(rooms);
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "Something went wrong while loading the rooms.";
                return View(new List<Room>());
            }
        }

        public IActionResult DormitoryStudents(int roomId)
        {
            try
            {
                Room? room = _roomRepository.GetById(roomId);
                if (room == null)
                {
                    return RedirectToAction("Index");
                }

                DormitoryStudentsViewModel viewModel = new DormitoryStudentsViewModel();
                viewModel.Room = room;
                viewModel.DormitoryStudents = _studentRepository.GetStudentsByRoomId(roomId);
                viewModel.RoomlessStudents = _studentRepository.GetStudentsWithoutRoom();

                return View(viewModel);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Dormitory students could not be loaded.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult AddDormitoryStudent(int roomId, int studentId)
        {
            try
            {
                _studentRepository.AddStudentToRoom(studentId, roomId);

                Student? student = _studentRepository.GetById(studentId);
                if (student != null)
                {
                    TempData["Message"] = student.FirstName + " " + student.LastName + " was added to the dormitory.";
                }

                return RedirectToAction("DormitoryStudents", new { roomId = roomId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "The student could not be added to the dormitory.";
                return RedirectToAction("DormitoryStudents", new { roomId = roomId });
            }
        }

        public IActionResult RemoveDormitoryStudent(int roomId, int studentId)
        {
            try
            {
                Student? student = _studentRepository.GetById(studentId);
                _studentRepository.RemoveStudentFromRoom(studentId);

                if (student != null)
                {
                    TempData["Message"] = student.FirstName + " " + student.LastName + " was removed from the dormitory.";
                }

                return RedirectToAction("DormitoryStudents", new { roomId = roomId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "The student could not be removed from the dormitory.";
                return RedirectToAction("DormitoryStudents", new { roomId = roomId });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Room room)
        {
            try
            {
                _roomRepository.Add(room);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "Something went wrong while adding the room.";
                return View(room);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                Room? room = _roomRepository.GetById(id);

                if (room == null)
                {
                    return RedirectToAction("Index");
                }

                return View(room);
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "Something went wrong while loading the room.";
                return View("Index", new List<Room>());
            }
        }

        [HttpPost]
        public IActionResult Edit(Room room)
        {
            try
            {
                _roomRepository.Update(room);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "Something went wrong while updating the room.";
                return View(room);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                Room? room = _roomRepository.GetById(id);
                if (room == null)
                {
                    return RedirectToAction("Index");
                }

                return View(room);
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "Something went wrong while loading the room.";
                return View("Index", new List<Room>());
            }
        }

        [HttpPost]
        public IActionResult Delete(Room room)
        {
            try
            {
                _roomRepository.Delete(room.RoomID);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "This room cannot be deleted because it is still linked to a student or lecturer.";
                return View(room);
            }
        }
    }
}