using Rock_Models;
using Rock_DataAccess;
using Rock_Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Rock;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Rock_Utility;

namespace LeaningShop.Controllers
{

    [Authorize(Roles = WConst.AdminRole)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _context.Products
                .Include(u=>u.Category)
                .Include(u=>u.ApplicationType);

            //foreach(var product in products)
            //{
            //    product.Category = _context.Categories.FirstOrDefault(u => u.Id == product.Id);
            //    product.ApplicationType = _context.ApplicationTypes.FirstOrDefault(u => u.Id == product.ApplicationTypeId);
            //}

            return View(products);
        }

        
        //GET - Upsert
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryDropDown = _context.Categories.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.CategoryID.ToString()
            //});
            //ViewBag.CategoryDropDown = CategoryDropDown;
            //Product product = new Product();

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _context.Categories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ApplicationTypeList = _context.ApplicationTypes.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _context.Products.Find(id);
                if(productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }


        //POST - Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if(productVM.Product.Id == 0)
                {
                    //creating
                    string upload = webRootPath + WConst.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;

                    _context.Products.Add(productVM.Product);
                }
                else
                {
                    //upload
                    var objFromDb = _context.Products.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);
                    if(files.Count > 0)
                    {
                        string upload = webRootPath + WConst.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _context.Products.Update(productVM.Product);
                }
                productVM.CategorySelectList = _context.Categories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                productVM.ApplicationTypeList = _context.ApplicationTypes.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET - DELETE
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            Product product = _context.Products
                .Include(u => u.Category)
                .Include(u => u.ApplicationType)
                .FirstOrDefault(u => u.Id == Id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int Id)
        {
            var product = _context.Products.Find(Id);
            if(product == null)
            {
                return NotFound();
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string upload = webRootPath + WConst.ImagePath;
            var oldFile = Path.Combine(upload, product.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
