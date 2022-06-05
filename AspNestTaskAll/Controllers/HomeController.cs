 using AspNestTaskAll.DAL;
using AspNestTaskAll.Models;
using AspNestTaskAll.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AspNestTaskAll.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _context { get; }
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IQueryable<Product> query = _context.Products.Include(p => p.ProductImages).Include(p => p.Category).AsQueryable();
            HomeVM homeVM = new HomeVM()
            {
                Sliders = await _context.Sliders.ToListAsync(),
                PopularCategories = await _context.Categories.Where(c => c.IsDeleted == false).OrderBy(c => c.Products.Count).Take(5).ToListAsync(),
                RandomProducts = await _context.Categories.Where(c => c.IsDeleted == false).OrderBy(c => Guid.NewGuid()).Take(10).Include(c=>c.Products).ToListAsync(),
                Products = await _context.Products.OrderByDescending(p => p.StockCount).Where(p=>p.StockCount>0).Take(10).Include(p => p.ProductImages).Include(p => p.Category).ToListAsync(),
                RecentProducts = await _context.Products.OrderByDescending(p => p.Id).Take(3).Include(p => p.ProductImages).Include(p => p.Category).ToListAsync(),
                TopRatedProducts = await _context.Products.OrderByDescending(p => p.Raiting).Take(3).Include(p => p.ProductImages).Include(p => p.Category).OrderByDescending(p => p.Raiting).Take(3).ToListAsync()
            };
            return View(homeVM);
        }
        public IActionResult Details(int Id)
        {
            DetailsVM detailsVM = new DetailsVM()
            {

                Categories = _context.Categories.ToList(),
                Products = _context.Products.Include(pi => pi.ProductImages).FirstOrDefault(p => p.Id == Id)
            };
            return View(detailsVM);
        }

    }
}
