using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MyProject.Areas.Identity.Data;

namespace MyProject.Models
{
    public class User
    {
        [Key]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public string? MyIdentityUserId { get; set; }
        [ForeignKey(nameof(MyIdentityUserId))]
        public MyIdentityUser? MyIdentityUser { get; set; }
    }
}