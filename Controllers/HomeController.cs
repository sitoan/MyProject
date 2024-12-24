using System.Data.Entity;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Models;
using MyProject.ViewModels;

namespace MyProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDBContext _dbContext;

    public HomeController(ILogger<HomeController> logger, AppDBContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

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

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }


    public IActionResult About()
    {
        return View();
    }
    public IActionResult Contact()
    {
        return View();
    }

}
