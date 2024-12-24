using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject.Models{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Quote { get; set; }
        public decimal? Price { get; set; }
        public string? Image { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }
        public ICollection<OrderDetail>? OrderDetail { get; set; } = new List<OrderDetail>();
        public int? Inventory { get; set; }
    }
}
