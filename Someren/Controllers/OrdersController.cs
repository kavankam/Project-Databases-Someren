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
            DrinkOrderViewModel model = new DrinkOrderViewModel();
            model.Students = _studentRepository.GetAll(null);
            model.Drinks = _drinkRepository.GetAllDrinks();

            return View(model);
        }
        catch (Exception)
        {
            ViewData["ErrorMessage"] = "Something went wrong while loading students and drinks.";
            return View(new DrinkOrderViewModel());
        }
    }

    [HttpPost]
    public IActionResult ConfirmOrder(DrinkOrderViewModel model)
    {
        try
        {
            Student? student = _studentRepository.GetById(model.StudentID);
            Drink? drink = _drinkRepository.GetById(model.DrinkID);

            if (student == null || drink == null)
            {
                return RedirectToAction("Index");
            }

            model.SelectedStudent = student;
            model.SelectedDrink = drink;

            return View(model);
        }
        catch (Exception)
        {
            ViewData["ErrorMessage"] = "Something went wrong while confirming the order.";
            DrinkOrderViewModel newModel = new DrinkOrderViewModel();
            newModel.Students = _studentRepository.GetAll(null);
            newModel.Drinks = _drinkRepository.GetAllDrinks();
            return View("Index", newModel);
        }
    }

    [HttpPost]
    public IActionResult ProcessOrder(DrinkOrderViewModel model)
    {
        try
        {
            Drink? drink = _drinkRepository.GetById(model.DrinkID);
            Student? student = _studentRepository.GetById(model.StudentID);

            if (drink == null || student == null)
            {
                return RedirectToAction("Index");
            }

            if (model.Quantity <= 0)
            {
                ViewData["ErrorMessage"] = "Quantity must be greater than 0.";
                model.Students = _studentRepository.GetAll(null);
                model.Drinks = _drinkRepository.GetAllDrinks();
                return View("Index", model);
            }

            if (model.Quantity > drink.Stock)
            {
                ViewData["ErrorMessage"] = "Not enough stock available for this drink.";
                model.Students = _studentRepository.GetAll(null);
                model.Drinks = _drinkRepository.GetAllDrinks();
                return View("Index", model);
            }

            int newStock = drink.Stock - model.Quantity;
            Order order = new Order(model.StudentID, model.DrinkID, model.Quantity);
            _orderRepository.AddOrder(order);
            _drinkRepository.UpdateStock(model.DrinkID, newStock);

            ViewData["Message"] = $"Order processed: {model.Quantity} x {drink.Name} sold to {student.FirstName} {student.LastName}.";

            DrinkOrderViewModel newModel = new DrinkOrderViewModel();
            newModel.Students = _studentRepository.GetAll(null);
            newModel.Drinks = _drinkRepository.GetAllDrinks();

            return View("Index", newModel);
        }
        catch (Exception)
        {
            ViewData["ErrorMessage"] = "Something went wrong while processing the order.";
            DrinkOrderViewModel newModel = new DrinkOrderViewModel();
            newModel.Students = _studentRepository.GetAll(null);
            newModel.Drinks = _drinkRepository.GetAllDrinks();
            return View("Index", newModel);
        }
    }
}