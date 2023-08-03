using FastFood.Models;
using FastFood.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FastFood.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CouponsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var coupons = _context.Coupons.ToList();
            return View(coupons);
        }
        [HttpGet]
        public IActionResult Create()
        {
           return View();
        }
        [HttpPost]
        public IActionResult Create(Coupon coupons)
        {
            if (ModelState.IsValid)
            {
                var files = Request.Form.Files;
               byte[] photo = null;
            using (var fileStream = files[0].OpenReadStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    photo = memoryStream.ToArray();
                }
            }
            coupons.CouponPicture = photo;
                _context.Coupons.Add(coupons);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(coupons);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var coupon = _context.Coupons.Where(x => x.Id == id).FirstOrDefault();
            if (coupon==null)
            {
                return NotFound();
            }
            _context.Coupons.Remove(coupon);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var coupon = _context.Coupons.Where(x => x.Id == id).FirstOrDefault();
            if (coupon == null)
            {
                return NotFound();
            }
            
            return View(coupon);
        }

        [HttpPost]
        public IActionResult Edit(Coupon model)
        {
            var coupon = _context.Coupons.Where(x => x.Id == model.Id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var files = Request.Form.Files;
                if (files.Count > 0)
                {


                    byte[] photo = null;
                    using (var fileStream = files[0].OpenReadStream())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);
                            photo = memoryStream.ToArray();
                        }
                    }
                    coupon.CouponPicture = photo;
                }
                coupon.MinimumAmount = model.MinimumAmount;
                coupon.Discount = model.Discount;
                coupon.IsActive = model.IsActive;
                coupon.Title = model.Title;
                coupon.Type = model.Type;
                _context.Coupons.Update(coupon);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
