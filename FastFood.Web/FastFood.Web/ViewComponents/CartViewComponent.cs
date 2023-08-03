using FastFood.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastFood.Web.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CartViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims != null)
            {
                if (HttpContext.Session.GetInt32("SessionCart") != null)
                {
                    return View(HttpContext.Session.GetInt32("SessionCart"));
                }

                else
                {
                    HttpContext.Session.SetInt32("SessionCart", _context
                        .Carts.Where(x => x.ApplicationUserId == claims.Value).ToList().Count);
                    return View(HttpContext.Session.GetInt32("SessionCart"));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }

        }
    }
}
