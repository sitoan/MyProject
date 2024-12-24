
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProject.Models{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; } 
        [ForeignKey(nameof(UserId))]
        public User? User{ get; set; }
        public DateTime? CreatedAt { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();
    }
    
}
