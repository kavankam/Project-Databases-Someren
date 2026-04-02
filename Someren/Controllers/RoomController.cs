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

        public IActionResult Index()
        {
            List<Room> rooms = _roomRepository.GetAllRooms();
            return View(rooms);
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
            _roomRepository.Add(room);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Room? room = _roomRepository.GetById(id);

            if (room == null)
            {
                return RedirectToAction("Index");
            }

            return View(room);
        }

        [HttpPost]
        public IActionResult Edit(Room room)
        {
            _roomRepository.Update(room);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Room? room = _roomRepository.GetById(id);
            if (room == null)
            {
                return RedirectToAction("Index");
            }

            return View(room);
        }

        [HttpPost]
        public IActionResult Delete(Room room)
        {
            _roomRepository.Delete(room.RoomID);
            return RedirectToAction("Index");
        }
    }
}