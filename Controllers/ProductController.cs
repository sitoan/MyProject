using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProject.Models;
using MyProject.ViewModels;
using X.PagedList;
using X.PagedList.Extensions;


namespace MyProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDBContext _dbContext;

        public ProductController(AppDBContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index(int? page, int? categoryId){
            
            int PageSize = 6;
            int pageNumber = page ?? 1;

            var productsQuery = _dbContext?.Products?.AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery!.Where(p => p.CategoryId == categoryId.Value);
            }

            // Fetch products, order them (e.g., by Id or Name), and apply pagination
            var products = productsQuery!
                .OrderBy(p => p.Id)
                .ToPagedList(pageNumber, PageSize);

            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.Categories = _dbContext?.Categories?.ToList();
            return View(products);  

        }            
        

        public IActionResult Details(int id){
            var product = _dbContext.Products?.FirstOrDefault(p => p.Id == id);
            return View(product);
        }


    }
}