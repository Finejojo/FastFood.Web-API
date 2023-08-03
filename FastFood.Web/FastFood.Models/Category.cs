using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FastFood.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<Item> Items { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<SubCategory> SubCategories { get; set; }
    }
}
