using FastFood.Models;

namespace FastFood.Web.ViewModels
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public Category  Category { get; set; }
        public SubCategory SubCategory { get; set; }
        public int SubCategoryId { get; set; }
    }
}
