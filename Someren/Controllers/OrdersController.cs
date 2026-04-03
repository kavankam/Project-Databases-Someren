using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers;

public class OrdersController : Controller
{
    private readonly IStudentRepository _studentRepository;
    private readonly IDrinkRepository _drinkRepository;
    private readonly IOrderRepository _orderRepository;

    public OrdersController(IStudentRepository studentRepository, IDrinkRepository drinkRepository, IOrderRepository orderRepository)
    {
        _studentRepository = studentRepository;
        _drinkRepository = drinkRepository;
        _orderRepository = orderRepository;
    }

    [HttpGet]
    public IActionResult Index()
    {
        try
        {
            List<Student> students = _studentRepository.GetAll(null);
            List<Drink> drinks = _drinkRepository.GetAllDrinks();

            ViewData["Students"] = students;
            ViewData["Drinks"] = drinks;

            return View();
        }
        catch (Exception)
        {
            ViewData["ErrorMessage"] = "Something went wrong while loading students and drinks.";
            ViewData["Students"] = new List<Student>();
            ViewData["Drinks"] = new List<Drink>();
            return View();
        }
    }

    [HttpPost]
    public IActionResult ConfirmOrder(Order order)
    {
        try
        {
            Student? student = _studentRepository.GetById(order.StudentID);
            Drink? drink = _drinkRepository.GetById(order.DrinkID);

            if (student == null || drink == null)
            {
                return RedirectToAction("Index");
            }

            ViewData["Student"] = student;
            ViewData["Drink"] = drink;
            ViewData["Quantity"] = order.Quantity;

            return View(order);
        }
        catch (Exception)
        {
            ViewData["ErrorMessage"] = "Something went wrong while confirming the order.";
            ViewData["Students"] = _studentRepository.GetAll(null);
            ViewData["Drinks"] = _drinkRepository.GetAllDrinks();
            return View("Index");
        }
    }

    [HttpPost]
    public IActionResult ProcessOrder(Order order)
    {
        try
        {
            Drink? drink = _drinkRepository.GetById(order.DrinkID);
            Student? student = _studentRepository.GetById(order.StudentID);

            if (drink == null || student == null)
            {
                return RedirectToAction("Index");
            }

            if (order.Quantity <= 0)
            {
                ViewData["ErrorMessage"] = "Quantity must be greater than 0.";
                ViewData["Students"] = _studentRepository.GetAll(null);
                ViewData["Drinks"] = _drinkRepository.GetAllDrinks();
                return View("Index");
            }

            if (order.Quantity > drink.Stock)
            {
                ViewData["ErrorMessage"] = "Not enough stock available for this drink.";
                ViewData["Students"] = _studentRepository.GetAll(null);
                ViewData["Drinks"] = _drinkRepository.GetAllDrinks();
                return View("Index");
            }

            int newStock = drink.Stock - order.Quantity;
            _orderRepository.AddOrder(order);
            _drinkRepository.UpdateStock(order.DrinkID, newStock);

            ViewData["Message"] = $"Order processed: {order.Quantity} x {drink.Name} sold to {student.FirstName} {student.LastName}.";

            List<Student> students = _studentRepository.GetAll(null);
            List<Drink> drinks = _drinkRepository.GetAllDrinks();
            ViewData["Students"] = students;
            ViewData["Drinks"] = drinks;

            return View("Index");
        }
        catch (Exception)
        {
            ViewData["ErrorMessage"] = "Something went wrong while processing the order.";
            ViewData["Students"] = _studentRepository.GetAll(null);
            ViewData["Drinks"] = _drinkRepository.GetAllDrinks();
            return View("Index");
        }
    }
}