using AspNestTaskAll.DAL;
using AspNestTaskAll.Models;
using AspNestTaskAll.Utility;
using AspNestTaskAll.Utility.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNestTaskAll.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private AppDbContext _context { get; }
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.Include(c => c.Products).ToListAsync();
            return View(categories);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (_context.Categories.FirstOrDefault(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim()) !=null ) return RedirectToAction(nameof(Index));
            if (category.Photo.CheckSize(400) || !category.Photo.CheckType("image/"))
            {
                return RedirectToAction(nameof(Index));
            }
            category.Logo = await category.Photo.SaveFileAsync(Path.Combine(Constant.ImagePath,"category"));
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Update(int Id, Category category)
        {
            Category category1 = _context.Categories.Find(Id);

            if (category1 == null || category == null) return NotFound();
            category1.Name = category.Name;
            category1.Logo = category.Logo;
            _context.Categories.Update(category1);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Category category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            if (category.IsDeleted)
            {
                _context.Remove(category);

            }
            category.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        
    }
}
