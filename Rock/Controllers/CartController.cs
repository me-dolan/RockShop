using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Rock_DataAccess;
using Rock_Models;
using Rock_Models.ViewModels;
using Rock_Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        [BindProperty]
        public ProductUserVM ProductUserVM { get; set; }

        public CartController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,
                              IEmailSender emailSender, IConfiguration config)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _config = config;
        }

        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart) !=null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart).Count() > 0)
            {
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WConst.SessionCart);
            }

            List<int> productInCart = shoppingCarts.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _context.Products.Where(u => productInCart.Contains(u.Id));

            return View(productList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }

        public async Task<IActionResult> Summary()
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = User.FindFirstValue(ClaimTypes.Name);

            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart).Count() > 0)
            {
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WConst.SessionCart);
            }

            List<int> productInCart = shoppingCarts.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _context.Products.Where(u => productInCart.Contains(u.Id));

            ProductUserVM = new ProductUserVM()
            {
                ApplicationUser = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userId),
                ProductList = productList.ToList()
            };

            return View(ProductUserVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async  Task<IActionResult> SummaryPost(ProductUserVM productUserVM)
        {
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "Inquiry.html";

            var subject = "New inquiry";
            string htmlBody = "";
            using (StreamReader reader = System.IO.File.OpenText(PathToTemplate))
            {
                htmlBody = reader.ReadToEnd();
            }

            StringBuilder productListSB = new StringBuilder();
            foreach(var prod in ProductUserVM.ProductList)
            {
                productListSB.Append($" - Name: {prod.Name} <span style='font-size:14px;'> (ID: {prod.Id})</span><br />");
            }
            string messageBody = string.Format(htmlBody,
                ProductUserVM.ApplicationUser.FullName,
                ProductUserVM.ApplicationUser.Email,
                ProductUserVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            var email = _config.GetSection("EmailConfiguration").Get<EmailSettings>();

            await _emailSender.SendEmailAsync(productUserVM.ApplicationUser.Email, subject, messageBody);


            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WConst.SessionCart).Count() > 0)
            {
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WConst.SessionCart);
            }

            shoppingCarts.Remove(shoppingCarts.FirstOrDefault(i => i.ProductId == id));

            List<int> productInCart = shoppingCarts.Select(i => i.ProductId).ToList();
            IEnumerable<Product> productList = _context.Products.Where(u => productInCart.Contains(u.Id));

            HttpContext.Session.Set(WConst.SessionCart, shoppingCarts);

            return RedirectToAction(nameof(Index));
        }
    }
}
