using AspNestTaskAll.DAL;
using AspNestTaskAll.Models;
using AspNestTaskAll.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNestTaskAll.ViewComponents
{
    public class ProductViewComponent:ViewComponent
    {
        private AppDbContext _context { get; }

        public ProductViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int page=1)
        {
            List<Product> products =await _context.Products.Where(x => x.IsDeleted == false)
                .Skip((page-1)*10).
                Take(10).
                Include(x=>x.ProductImages).
                Include(x => x.Category).ToListAsync();
            PaginateVM<Product> pagination = new PaginateVM<Product>
            {
                Items = products,
                ActivePage = page,
                PageCount = GetCountP(_context.Products.Count())

            };
            return View(await Task.FromResult(products));
        }
        public int GetCountP(int count)
        {

            return (int)Math.Ceiling((double)count/10);
        }
    }
}
