using System.ComponentModel.DataAnnotations.Schema;

namespace Project_test.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string? UserId { get; set; }
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [NotMapped]
        public IFormFile? ImageFileName { get; set; }
        public string? ImagePath { get; set; }
        public int Price { get; set; }
        public Category? Category { get; set; }
    }
}
