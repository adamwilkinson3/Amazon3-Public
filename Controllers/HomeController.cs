using Amazon3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace Amazon3.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        //private IHttpContextAccessor _httpContextAccessor;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, IUserStore<IdentityUser> userStore, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            //_httpContextAccessor = httpContextAccessor;
        }

        AppDbContext _context = new AppDbContext();
 
        public async Task<IActionResult> Index()
        {
            var appDbContext = await _context.Products.ToListAsync();

            return View(appDbContext);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public async Task<IActionResult> Product(string id)
        {
            var appDbContext = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (appDbContext != null) 
            { 
                ViewModel product = new ViewModel();
                product.Product = appDbContext;
                var productsList = await _context.Products.ToListAsync();
                ViewBag.productsList = productsList.Take(6);
                return View(product);
            }
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Product(string id, [Bind("Quantity")] ViewModel order)
        {
            var currentUserId = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUserId == null)
            {
                return View("NotLoggedIn");
            }
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product != null)
            {
                OrderItem orderItem = new OrderItem();
                orderItem.ProductId = id;
                orderItem.ProductName = product.ProductName;
                orderItem.Price = product.Price;
                orderItem.Date = DateTime.Now;
                orderItem.Url = product.Url;
                orderItem.Quantity = order.Quantity;
                orderItem.UserId = currentUserId.Id;
                _context.Update(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var currentUserId = user.Id.ToString();
                var cartItems = _context.Orders.Where(x => x.UserId == currentUserId);
                var cartList = await cartItems.ToListAsync();
                if (cartItems.Count() != 0)
                {
                    return View(cartList);
                }
                return View("CartEmpty");
            }
            return View("NotLoggedIn");
        }
        [HttpPost]
        public async Task<IActionResult> Cart(string id, [Bind("Quantity")] int quantity)
        {
            var orderItem = _context.Orders.Where(i => i.OrderId == id).FirstOrDefault();
            if (orderItem == null)
            {
                return RedirectToAction(nameof(Index));
            }
            orderItem.Quantity = quantity;

            _context.Orders.Update(orderItem);

            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Delete(string id)
        {
            var orderItem = await _context.Orders.FindAsync(id);
            //var orderItem = _context.Orders.Where(i => i.OrderId == id).FirstOrDefault();
            if (orderItem != null)
            {
                _context.Orders.Remove(orderItem);
                await _context.SaveChangesAsync();
            }
            
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> SubmitOrder()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserId = user.Id.ToString();
            _context.Orders.RemoveRange(_context.Orders.Where(x => x.UserId == currentUserId));
            await _context.SaveChangesAsync();

            return Redirect("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        public IActionResult CartEmpty()
        {
            return View();
        }
        public async Task<IActionResult> SignInWithTestUser()
        {
            await _signInManager.PasswordSignInAsync("test9@email.com", "12qw!@QW", false, lockoutOnFailure: false);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

//List<Product> productList = new List<Product>();
//productList.Add(new Product() { ProductId = "03513260-6945-4895-a08d-01efd8f5a4df", ProductName = "Longos - Lasagna Veg", Price = 17.52, Url = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=962&q=80" });
//productList.Add(new Product() { ProductId = "22709310-b4f7-4ab8-84d3-21c94592bc77", ProductName = "Vanilla Beans", Price = 17.61, Url = "https://images.unsplash.com/photo-1484723091739-30a097e8f929?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=898&q=80" });
//productList.Add(new Product() { ProductId = "e80777ef-b6f2-48da-b4df-1396a6f6d0b7", ProductName = "Shiro Miso", Price = 16.52, Url = "https://images.unsplash.com/photo-1497034825429-c343d7c6a68f?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxzZWFyY2h8Mjd8fGZvb2R8ZW58MHx8MHx8&auto=format&fit=crop&w=500&q=60" });
//productList.Add(new Product() { ProductId = "43e116f4-2b1f-4d06-992b-b62440b1463c", ProductName = "Rosemary - Fresh", Price = 5.59, Url = "https://images.unsplash.com/photo-1529042410759-befb1204b468?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=772&q=80" });
//productList.Add(new Product() { ProductId = "c1072e29-704b-4f65-9799-00f811e45465", ProductName = "Sambuca - Opal Nera", Price = 11.3, Url = "https://images.unsplash.com/photo-1481070555726-e2fe8357725c?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=870&q=80" });
//productList.Add(new Product() { ProductId = "bcdf11dc-89bd-4d65-8dd1-6301178d4324", ProductName = "Wine - Mondavi Coastal Private", Price = 16.41, Url = "https://images.unsplash.com/photo-1432139509613-5c4255815697?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=770&q=80" });
//productList.Add(new Product() { ProductId = "98c3c340-b860-4b5d-a496-f904aac39561", ProductName = "Syrup - Monin - Passion Fruit", Price = 10.62, Url = "https://images.unsplash.com/photo-1565299507177-b0ac66763828?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=844&q=80" });
//productList.Add(new Product() { ProductId = "776bcdd9-2c6c-4234-9963-bb8788be1f41", ProductName = "Fish - Halibut, Cold Smoked", Price = 11.91, Url = "https://images.unsplash.com/photo-1432139555190-58524dae6a55?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1752&q=80" });
//productList.Add(new Product() { ProductId = "16558bea-0757-4147-a2ea-cb6c4d91179b", ProductName = "Bread - Multigrain, Loaf", Price = 1.77, Url = "https://images.unsplash.com/photo-1592417817098-8fd3d9eb14a5?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=774&q=80" });
//productList.Add(new Product() { ProductId = "56dcf539-f5c9-4ad4-85be-7c44b8a3cc7b", ProductName = "Puree - Raspberry", Price = 9.92, Url = "https://images.unsplash.com/photo-1563379926898-05f4575a45d8?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1740&q=80" });
//productList.Add(new Product() { ProductId = "b61e2270-5b38-4591-ba28-330e01c46187", ProductName = "Schnappes Peppermint - Walker", Price = 15.51, Url = "https://images.unsplash.com/photo-1563805042-7684c019e1cb?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=654&q=80" });
//productList.Add(new Product() { ProductId = "b1b43dc7-b9b8-4dee-80b5-93f1b62e49ca", ProductName = "Wine - Marlbourough Sauv Blanc", Price = 18.88, Url = "https://images.unsplash.com/photo-1546554137-f86b9593a222?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=774&q=80" });

//_context.Products.AddRange(productList);
//await _context.SaveChangesAsync();