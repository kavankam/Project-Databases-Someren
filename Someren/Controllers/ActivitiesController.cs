using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;
using System.Linq;

namespace Someren.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly IActivityRepository _activityRepo;

        public ActivitiesController(IActivityRepository activityRepo)
        {
            _activityRepo = activityRepo;
        }

        public IActionResult Index(string? searchTerm)
        {
            var activities = _activityRepo.GetAll();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                activities = activities
                    .Where(a => a.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var activitiesOrdered = activities
                .OrderBy(a => a.Day)
                .ThenBy(a => a.TimeSlot)
                .ToList();

            ViewBag.SearchTerm = searchTerm; //searches from the viewbag the term typed
            return View(activitiesOrdered);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //create submission
        public IActionResult Create(Activity activity)
        {
            if (ModelState.IsValid)
            {
                _activityRepo.Add(activity);
                _activityRepo.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        public IActionResult Edit(int id)
        {
            var activity = _activityRepo.GetById(id);
            if (activity == null)
                return NotFound();

            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Activity activity)
        {
            if (ModelState.IsValid)
            {
                _activityRepo.Update(activity);
                _activityRepo.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        public IActionResult Delete(int id)
        {
            var activity = _activityRepo.GetById(id);
            if (activity == null)
                return NotFound();

            return View(activity);
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int activityId) //delete confirmation page
        {
            var activity = _activityRepo.GetById(activityId);
            if (activity != null)
            {
                _activityRepo.Delete(activity);
                _activityRepo.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}