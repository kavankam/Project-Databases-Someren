using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;

namespace Someren.Controllers;

public class ActivitiesController : Controller
{
    private readonly IActivityRepository _activityRepo;

    public ActivitiesController(IActivityRepository activityRepo) => _activityRepo = activityRepo;

    public IActionResult Index(string? searchTerm) => View(_activityRepo.GetAll(searchTerm));

    // GET: Show Create Form
    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Activity activity)
    {
        if (!ModelState.IsValid) return View(activity);
        _activityRepo.Add(activity);
        return RedirectToAction(nameof(Index));
    }

    // GET: Show Edit Form
    public IActionResult Edit(int id)
    {
        var activity = _activityRepo.GetById(id);
        return activity == null ? NotFound() : View(activity);
    }

    [HttpPost]
    public IActionResult Edit(Activity activity)
    {
        if (!ModelState.IsValid) return View(activity);
        _activityRepo.Update(activity);
        return RedirectToAction(nameof(Index));
    }

    // GET: Show Delete Confirmation Page
    public IActionResult Delete(int id)
    {
        var activity = _activityRepo.GetById(id);
        return activity == null ? NotFound() : View(activity);
    }

    // POST: Actually perform the delete
    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        _activityRepo.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}