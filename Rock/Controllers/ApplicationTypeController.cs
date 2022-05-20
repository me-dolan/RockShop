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
using Rock_Models;
using Rock_Utility;

namespace LeaningShop.Controllers
{
    [Authorize(Roles = WConst.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Index
        public IActionResult Index()
        {
            IEnumerable<ApplicationType> applicationTypes = _context.ApplicationTypes;
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
                _context.Add(applicationType);
                _context.SaveChanges();
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

            var applicationType = _context.ApplicationTypes.Find(id);
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
                _context.ApplicationTypes.Update(applicationType);
                _context.SaveChanges();
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

            var applicationType = _context.ApplicationTypes.Find(id);
            if (applicationType == null)
            {
                return NotFound();
            }

            return View(applicationType);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var applicationType = _context.ApplicationTypes.Find(id);
            _context.ApplicationTypes.Remove(applicationType);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool ApplicationTypeExists(int id)
        {
            return _context.ApplicationTypes.Any(e => e.Id == id);
        }
    }
}
