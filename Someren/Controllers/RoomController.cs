using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public IActionResult Index()
        {
            List<Room> rooms = _roomRepository.GetAllRooms();
            return View(rooms);
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