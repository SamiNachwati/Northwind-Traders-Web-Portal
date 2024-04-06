using System.ComponentModel.DataAnnotations;
namespace Northwind.Models
{
    public class Category
    {
        [Key]
        public int categoryId { get; set; }

        [Display(Name = "Category")]
        public string categoryName { get; set; }
        public string description { get; set; }
        public List<Product> products { get; set; }

    }
}
