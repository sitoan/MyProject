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
    public class CartController : Controller
    {
        private readonly AppDBContext _dbContext;
        private readonly UserManager<MyIdentityUser> _userManager;


        public CartController(UserManager<MyIdentityUser> userManager ,AppDBContext dbContext){
            _userManager = userManager;
            _dbContext = dbContext;
        }


        [Authorize]
        public IActionResult Index(){
            var CartProducts = (from c in _dbContext.Carts join p in _dbContext.Products! on c.ProductId equals p.Id 
                                select new CartDto {
                                    Id = c.Id,
                                    ProductId = p.Id,
                                    ProductName = p.Name,
                                    Image = p.Image,
                                    Quantity = c.Quantity,
                                    Price = p.Price,
                                    Inventory = p.Inventory,
                                    TotalPrice = c.Price,
                                }).ToList();

            return View(CartProducts);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCart([FromBody] Cart AddedProduct)
        {
            string userId = _userManager.GetUserId(User)!;

            try
            {
                Console.WriteLine("Added : " + AddedProduct.ProductId);

                var product = await _dbContext.Carts!.FirstOrDefaultAsync(c => c.ProductId == AddedProduct.ProductId);

                if (product == null)
                {   
                    await _dbContext.Carts!.AddAsync(AddedProduct);
                    AddedProduct.UserId = userId;
                }
                else
                {
                    product.Quantity += AddedProduct.Quantity;
                    product.Price += AddedProduct.Price;
                }
                await _dbContext.SaveChangesAsync();

                return Ok("Add to cart successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500, "An error occurred while adding to the cart");
            }
        }

        [HttpPut]        
        [Authorize]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityViewModel request)
        {
            var cartItem = await _dbContext.Carts!
                .Include(c => c.Product)
                .FirstOrDefaultAsync(item => item.Id == request.ItemId);

            if (cartItem == null)
            {
                return NotFound(new { message = "Cart item not found" });
            }

            if (request.Quantity <= 0)
            {
                _dbContext.Carts?.Remove(cartItem);
            }
            else
            {
                decimal productPrice = cartItem.Product?.Price ?? 0;
                cartItem.Quantity = request.Quantity;
                cartItem.Price = request.Quantity * productPrice;
            }


            try 
            {
                await _dbContext.SaveChangesAsync();

                var newTotalPrice = _dbContext.Carts!
                    .Sum(c => c.Price);

                var newTotalQuantity = _dbContext.Carts!
                    .Sum(c => c.Quantity);


                return Json(new 
                { 
                    success = true, 
                    message = request.Quantity <= 0 ? "Item removed successfully" : "Quantity updated successfully",
                    newProductQuantity = cartItem.Quantity,
                    newProductPrice = cartItem.Price,
                    newTotalPrice,
                    newTotalQuantity 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating quantity", error = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveItem(int id)
        {
            Console.WriteLine(" Remove item called: " + id);
            var cartItem = await _dbContext.Carts!
                .FirstOrDefaultAsync(item => item.Id == id);
            
            if (cartItem == null)
            {
                return NotFound(new { message = "Cart item not found" });
            }

            try 
            {
                _dbContext.Carts?.Remove(cartItem);
                await _dbContext.SaveChangesAsync();

                var newTotalPrice = _dbContext.Carts!
                    .Sum(c => c.Price);

                var newTotalQuantity = _dbContext.Carts!
                    .Sum(c => c.Quantity);

                return Json(new 
                { 
                    success = true, 
                    message = "Item removed successfully",
                    newTotalQuantity,
                    newTotalPrice
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error removing item", error = ex.Message });
            }
        }
    }
}