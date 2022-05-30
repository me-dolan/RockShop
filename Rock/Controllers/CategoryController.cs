using Microsoft.AspNetCore.Mvc;
using Rock;
using Rock_Models;
using Rock_DataAccess;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Rock_Utility;
using Rock_DataAccess.Repository.IRepository;

namespace LeaningShop.Controllers
{
    [Authorize(Roles = WConst.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _catRepo;

        public CategoryController(ICategoryRepository catRepo)
        {
            _catRepo = catRepo;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _catRepo.GetAll();
            return View(categories);
        }

        
        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }


        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Add(category);
                _catRepo.Save();
                return RedirectToAction("Index");
            }
            return View(category);
        }


        // GET - EDIT
        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var category = _catRepo.Find(Id.GetValueOrDefault());
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Update(category);
                _catRepo.Save();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET - DELETE
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var category = _catRepo.Find(Id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int Id)
        {
            var category = _catRepo.Find(Id);
            if(category == null)
            {
                return NotFound();
            }
            _catRepo.Remove(category);
            _catRepo.Save();
            return RedirectToAction("Index");
        }
    }
}
