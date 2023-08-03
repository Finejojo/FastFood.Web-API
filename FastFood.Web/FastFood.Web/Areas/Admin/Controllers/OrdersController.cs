using FastFood.Models;
using FastFood.Repository;
using FastFood.Web.Utility;
using FastFood.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FastFood.Web.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string status)
        {
            IEnumerable<OrderHeader> order;
            if (User.IsInRole("Admin"))
            {
                order = _context.OrderHeaders.
                    Include(x => x.ApplicationUser).ToList();

            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims =  claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                order = _context.OrderHeaders.Where(x => x.ApplicationUserId==claims.Value); 
            }
            switch (status)
            {
                case "pending":
                    order = order.Where(x => x.PaymentStatus == PaymentStatus.StatusPending);
                    break;
                case "approved":
                    order = order.Where(x => x.PaymentStatus == PaymentStatus.StatusApproved);
                    break;
                case "underprocess":
                    order = order.Where(x => x.OrderStatus == OrderStatus.StatusInProcess);
                    break;
                case "shipped":
                    order = order.Where(x => x.OrderStatus == OrderStatus.StatusShpped);
                    break;

                default:
                    break;
            }
            return View(order);
        }
        public IActionResult OrderDetails(int id)
        {
            var OrderDetail = new OrderDetailsViewModel()
            {
                OrderHeader = _context.OrderHeaders.Include(x => x.ApplicationUser)
                .FirstOrDefault(x => x.Id == id),
                OrderDetails = _context.OrderDetails.Include(x => x.Item)
                .Where(Item => Item.Id == id).ToList()

            };
            return View(OrderDetail);
        }
    }
}
