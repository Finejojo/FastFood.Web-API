using FastFood.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastFood.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim =  claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(_context.ApplicationUsers.Where(x => x.Id != claim.Value).ToList());

          
        }
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.LockoutEnd = DateTime.Now.AddYears(5000);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UnLock(string id)
        {
            var user =  await _context.ApplicationUsers.FindAsync(id);
            if (user==null)
            {
                return NotFound();
            }
            user.LockoutEnd = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
