using FastFood.Models;
using FastFood.Repository;
using FastFood.Web.Models;
using FastFood.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FastFood.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ItemListViewModel vm = new ItemListViewModel()
            {
                Items = await _context.Items.Include(x=>x.Category).Include(y=>y.SubCategory).ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                Coupons = await _context.Coupons.Where(c => c.IsActive == true).ToListAsync()

            };

              return View(vm);
        }
        public async Task<IActionResult> Details(int id)
        {
            var itemFromDb = await _context.Items.Include(x => x.Category)
                .Include(y => y.SubCategory).Where(x => x.Id == id).FirstOrDefaultAsync();
            var cart = new Cart()
            {
               
                Item = itemFromDb,
                ItemId = itemFromDb.Id,
                Count=1
            };
            return View(cart);
        }
               
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(Cart cart)
        {
            cart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claim.Value;

                var cartfromDb = await _context.Carts.Where(x => x.ApplicationUserId 
                == cart.ApplicationUserId
                && x.ItemId==cart.ItemId).FirstOrDefaultAsync();
                if (cartfromDb==null)
                {
                    _context.Carts.Add(cart);
                }
                else
                {
                    cartfromDb.Count = cartfromDb.Count + cart.Count;
                }

                _context.SaveChanges();

                var count = _context.Carts.Where(c => c.ApplicationUserId
                == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32("SessionCart", count);

                return RedirectToAction("Index");




            }
            return RedirectToAction("Index");
        }




    }
}
