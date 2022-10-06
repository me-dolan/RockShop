#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rock_DataAccess;
using Rock_DataAccess.Repository.IRepository;
using Rock_Models;
using Rock_Utility;

namespace LeaningShop.Controllers
{
    [Authorize(Roles = WConst.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _appRepo;

        public ApplicationTypeController(IApplicationTypeRepository appRepo)
        {
            _appRepo = appRepo;
        }

        // GET: Index
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> applicationTypes = _appRepo.GetAll();
            return View(applicationTypes);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApplicationTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType applicationType)
        {
            if (ModelState.IsValid)
            {
                _appRepo.Add(applicationType);
                _appRepo.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(applicationType);
        }

        // GET: Edit
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var applicationType = _appRepo.Find(id.GetValueOrDefault());
            if (applicationType == null)
            {
                return NotFound();
            }
            return View(applicationType);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType applicationType)
        {
            if (ModelState.IsValid)
            {
                _appRepo.Update(applicationType);
                _appRepo.Save();
                return RedirectToAction("Index");
            }
            return View(applicationType);
        }

        // GET: Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var applicationType = _appRepo.Find(id.GetValueOrDefault());
            if (applicationType == null)
            {
                return NotFound();
            }

            return View(applicationType);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            var applicationType = _appRepo.Find(id.GetValueOrDefault());
            _appRepo.Remove(applicationType);
            _appRepo.Save();
            return RedirectToAction("Index");
        }
    }
}
