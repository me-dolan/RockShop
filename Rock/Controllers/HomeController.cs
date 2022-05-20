using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Rock_Models;
using Rock_Models.ViewModels;
using Microsoft.Extensions.Logging;
using Rock_DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Rock_Utility;
using Rock;

namespace LeaningShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _context.Products
                            .Include(u => u.Category)
                            .Include(u => u.ApplicationType),

                Categories = _context.Categories
            };
            return View(homeVM);
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WConst.SessionCart);
            }

            DetailsVM details = new DetailsVM() {
                Product = _context.Products
                                .Include(u => u.Category).Include(u => u.ApplicationType)
                                .Where(u => u.Id == id).FirstOrDefault(),
                ExistInCart = false
            };
            
            foreach(var shoppingCart in shoppingCartList)
            {
                if (shoppingCart.ProductId == id)
                {
                    details.ExistInCart = true;
                }
            }

            return View(details);
        }

        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WConst.SessionCart);
            }

            shoppingCartList.Add(new ShoppingCart { ProductId = id });
            HttpContext.Session.Set(WConst.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart).Count() > 0)
            {   
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WConst.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);
            if(itemToRemove != null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

            HttpContext.Session.Set(WConst.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}