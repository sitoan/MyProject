using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyProject.Areas.Identity.Data;

namespace MyProject.Models;
public class AppDBContext : IdentityDbContext<MyIdentityUser>
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
        
    }
    public new DbSet<User>? Users { get; set; }
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Cart>? Carts { get; set; }
    public DbSet<OrderDetail>? OrderDetails { get; set; }    
    public DbSet<Order>? Orders { get; set; }
    public DbSet<Product>? Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<User>()
            .HasOne(u => u.MyIdentityUser)
            .WithOne()
            .HasForeignKey<User>(u => u.MyIdentityUserId)
            .OnDelete(DeleteBehavior.Cascade);


        // Category -> Product (1-n)
        builder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> Orders (1-n)
        builder.Entity<User>()
            .HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Order -> OrderDetail (1-n)
        builder.Entity<Order>()
            .HasMany(o => o.OrderDetail)
            .WithOne(od => od.Order)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product -> OrderDetail (1-n)
        builder.Entity<Product>()
            .HasMany(p => p.OrderDetail)
            .WithOne(od => od.Product)
            .HasForeignKey(od => od.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // User - Cart (1-n)
        builder.Entity<User>()
            .HasMany(u => u.Carts)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); 
    }
    
}
