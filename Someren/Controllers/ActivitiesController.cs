using Microsoft.AspNetCore.Mvc;
using Someren.Models;
using Someren.Repositories;
using System.Linq;

namespace Someren.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly IActivityRepository _activityRepo;
        private readonly IActivitySupervisorRepository _activitySupervisorRepository;
        private readonly IActivityParticipantRepository _activityParticipantRepository;


        public ActivitiesController(IActivityRepository activityRepo, IActivitySupervisorRepository activitySupervisorRepository, IActivityParticipantRepository activityParticipantRepository)
        {
            _activityRepo = activityRepo;
            _activitySupervisorRepository = activitySupervisorRepository;
            _activityParticipantRepository = activityParticipantRepository;

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
         //for Managing supervisors
        public IActionResult ManageSupervisors(int activityId)
        {
            var activity = _activityRepo.GetById(activityId);

            var supervisors = _activitySupervisorRepository.GetSupervisors(activityId);
            var nonSupervisors = _activitySupervisorRepository.GetNonSupervisors(activityId);

            var manageSupervisorModel = new ManageSupervisorsModel
            {
                Activity = activity,
                Supervisors = supervisors,
                NonSupervisors = nonSupervisors
            };

            return View(manageSupervisorModel);
        }
        
        [HttpPost]
        public IActionResult AddSupervisor(int activityId, int lecturerId)
        {
            _activitySupervisorRepository.AddSupervisor(activityId, lecturerId);
            return RedirectToAction("ManageSupervisors", new { activityId });
        }
        
        [HttpPost]
        public IActionResult RemoveSupervisor(int activityId, int lecturerId)
        {
            _activitySupervisorRepository.RemoveSupervisor(activityId, lecturerId);
            return RedirectToAction("ManageSupervisors", new { activityId });
        }

        //for managing participants

        public IActionResult ManageParticipants(int activityId)
        {
            var activity = _activityRepo.GetById(activityId);

            var participants = _activityParticipantRepository.GetParticipants(activityId);
            var nonParticipants = _activityParticipantRepository.GetNonParticipants(activityId);

            var manageParticipantsModel = new ManageParticipantsModel
            {
                Activity = activity,
                Participants = participants,
                NonParticipants = nonParticipants
            };

            return View(manageParticipantsModel);
        }

        [HttpPost]
        public IActionResult AddParticipant(int activityId, int studentId)
        {
            _activityParticipantRepository.AddParticipant(activityId, studentId);
            TempData["Message"] = "Successfully added as participant"; //confirmation message
            return RedirectToAction("ManageParticipants", new { activityId });
        }

        [HttpPost]
        public IActionResult RemoveParticipant(int activityId, int studentId)
        {
            _activityParticipantRepository.RemoveParticipant(activityId, studentId);
            TempData["Message"] = "Successfully removed as participant"; //confirmation message/delete
            return RedirectToAction("ManageParticipants", new { activityId });
        }
    }
}
