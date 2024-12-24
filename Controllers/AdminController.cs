using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Models;
using MyProject.ViewModels;

namespace MyProject.Controllers
{
    public class AdminController : Controller
    {

        private readonly AppDBContext _dbContext;
        public AdminController( AppDBContext dbContext)
        {

            _dbContext = dbContext;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var categories = _dbContext.Categories?.ToList();
    
            var viewModel = categories?.Select(category => new CategoryProductViewModel
            {
                Category = category,
                Products = _dbContext.Products!
                    .Where(p => p.CategoryId == category.Id)
                    .ToList()
            }).ToList();
            
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            Console.WriteLine(category.Id);
            if (ModelState.IsValid)
            {
                _dbContext.Add(category);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); 
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditCategory(int id)
        {
            var category = _dbContext.Categories
                .Include(c => c.Products) 
                .FirstOrDefault(c => c.Id == id);
    
            var viewModel = new CategoryProductViewModel
            {
                Category = category,
                Products = _dbContext.Products!
                    .Where(p => p.CategoryId == category!.Id)
                    .ToList()
            };
            if (category == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Update(category);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index)); 
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
         public IActionResult DeleteCategory(int id)
        {
            var category = _dbContext.Categories?.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _dbContext.Categories!.Remove(category);
                _dbContext.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddProduct(int categoryId)
        {
            var product = new Product { CategoryId = categoryId };
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Products!.Add(product);
                _dbContext.SaveChanges();
                return RedirectToAction("EditCategory", new { id = product.CategoryId });
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditProduct(int id)
        {
            var product = _dbContext.Products?.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Products!.Update(product);
                _dbContext.SaveChanges();
                return RedirectToAction("EditCategory", new { id = product.CategoryId });
            }
            return View(product);
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _dbContext.Products?.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _dbContext.Products!.Remove(product);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("EditCategory", new { id = product?.CategoryId });
        }

       
    }
}