using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProject.Areas.Identity.Data;
using MyProject.Dto;
using MyProject.Models;
using MyProject.ViewModels;

namespace MyProject.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly AppDBContext _dbContext;
        public OrderController(AppDBContext dbContext, UserManager<MyIdentityUser> userManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        [Authorize]
        public IActionResult Index()
        {

            string userId = _userManager.GetUserId(User)!;

            var orders = _dbContext.Orders!
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetail)
                .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            return View(orders);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderViewModel request)
        {
            try
            {
                string userId = _userManager.GetUserId(User)!;
                Console.WriteLine("User ID: " + userId);

                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    var order = new Order
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _dbContext.Orders!.AddAsync(order);
                    await _dbContext.SaveChangesAsync();

                    foreach (var itemId in request.SelectedItemIds!)
                    {
                        var orderItem = await _dbContext.Carts!
                            .Include(c => c.Product)
                            .FirstOrDefaultAsync(c => c.Id == itemId && c.UserId == userId);

                        orderItem!.Product!.Inventory -= orderItem.Quantity;

                        if (orderItem == null)
                        {
                            throw new Exception($"Cart item {itemId} not found");
                        }

                        var orderDetail = new OrderDetail
                        {
                            OrderId = order.Id,
                            ProductId = orderItem.ProductId,
                            Quantity = orderItem.Quantity,
                            Price = orderItem.Price
                        };

                        _dbContext.OrderDetails?.AddAsync(orderDetail);

                        _dbContext.Carts?.Remove(orderItem);
                    }

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Json(new { 
                        success = true, 
                        message = "Order placed successfully", 
                        orderId = order.Id 
                    });
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = "An error occurred while placing the order: " + ex.Message 
                });
            }
        }
    }
}