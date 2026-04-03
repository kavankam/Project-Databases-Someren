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