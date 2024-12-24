using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyProject.Models;

namespace MyProject.ViewModels
{
    public class CategoryProductViewModel
    {
        public Category? Category { get; set; }
        public List<Product>? Products { get; set; }
    }
}