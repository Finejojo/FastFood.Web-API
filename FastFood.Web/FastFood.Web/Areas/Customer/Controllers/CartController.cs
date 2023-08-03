using FastFood.Models;
using FastFood.Repository;
using FastFood.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FastFood.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public CartOrderViewModel details { get; set; }

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            details = new CartOrderViewModel()
            {
                OrderHeader = new FastFood.Models.OrderHeader()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            details.ListofCart = _context.Carts.Include(z => z.Item).Where(x => x.ApplicationUserId == claims.Value)
                .ToList();
            if (details.ListofCart != null)
            {
                foreach (var cart in details.ListofCart)
                {
                    details.OrderHeader.OrderTotal += (cart.Item.Price * cart.Count);
                }
            }
            return View(details);
        }

        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            details = new CartOrderViewModel()
            {
                ListofCart = _context.Carts.Include(x => x.Item).
                Where(x => x.ApplicationUserId == claims.Value).ToList(),
                OrderHeader = new OrderHeader()
            };

            details.OrderHeader.ApplicationUser = _context.ApplicationUsers
                .Where(x => x.Id == claims.Value).FirstOrDefault();

            details.OrderHeader.Name = details.OrderHeader.ApplicationUser.Name;
            details.OrderHeader.Phone = details.OrderHeader.ApplicationUser.PhoneNumber;
            details.OrderHeader.TimeofPick = DateTime.Now;
            foreach (var cart in details.ListofCart)
            {
                details.OrderHeader.OrderTotal += (cart.Item.Price * cart.Count);
            }
            return View(details);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {
            return View();
        }

        public async Task<IActionResult> plus(int id)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id);
            cart.Count += 1;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> minus(int id)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id);
            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
           
            if (cart.Count == 1)
            {
                _context.Carts.Remove(cart);
                _context.SaveChanges();
                var count = _context.Carts.Where(x => x.ApplicationUserId == claims.Value).ToList().Count();
                HttpContext.Session.SetInt32("SessionCart", count);
            }
            else
            {


                cart.Count -= 1;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> delete(int id)
        {


                var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
           

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == id);
                _context.Carts.Remove(cart);
                _context.SaveChanges();
            var count =  _context.Carts.Where(x=>x.ApplicationUserId==claims.Value).ToList().Count();
            HttpContext.Session.SetInt32("SessionCart", count);
            return RedirectToAction(nameof(Index));
        }

    }
}
