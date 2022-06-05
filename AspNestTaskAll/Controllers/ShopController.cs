using AspNestTaskAll.DAL;
using AspNestTaskAll.Models;
using AspNestTaskAll.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNestTaskAll.Controllers
{
    public class ShopController : Controller
    {
        private AppDbContext _context { get; set; }
        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? page)
        {
            //ViewBag.ProductCount = _context.Products.Where(p => p.IsDeleted == false).Count();
            ViewBag.Page = page;
            ViewBag.Categories = _context.Categories.Where(p => p.IsDeleted == false).Include(c => c.Products);
            return View();
        }
        public IActionResult LoadMore(int skip)
        {
            IQueryable<Product> p = _context.Products.Where(p => p.IsDeleted == false);
            int productCount = p.Count();
            return PartialView("_ShopPartial", p
                                    .OrderByDescending(p => p.Id)
                                    .Skip(skip)
                                    .Take(10)
                                    .Include(p => p.ProductImages)
                                    .Include(p => p.Category));

        }
        public IActionResult CategoryFilter(int CategoryId)
        {
            if (_context.Categories.Find(CategoryId) == null) return NotFound();
            return PartialView("_ShopPartial", _context.Products.Where(p => p.IsDeleted == false && p.CategoryId == CategoryId)
                                .OrderByDescending(p => p.Id)
                                .Include(p => p.ProductImages)
                                .Include(p => p.Category));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) return BadRequest();

            Product dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null) return NotFound();
            UpdateBasket((int)id);

            return RedirectToAction("Index");
        }
        
        private void UpdateBasket(int id)
        {
            List<BasketVM> basketItems = GetBasket();

            BasketVM basketitem = basketItems.Find(x => x.ProductId == id);
            if (basketitem != null)
            {
                basketitem.Count += 1;
            }
            else
            {
                basketitem = new BasketVM
                {
                    ProductId = id,
                    Count = 1
                };
                basketItems.Add(basketitem);
            }
            Response.Cookies.Append("Basket", JsonConvert.SerializeObject(basketItems));


        }
        private List<BasketVM> GetBasket()
        {
            List<BasketVM> basketItems = new List<BasketVM>();

            if (Request.Cookies["Basket"] != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            return basketItems;
        }
        public IActionResult Card()
        {
            List<BasketVM> basket = GetBasket();
            List<BasketItemVM> basketItems = new List<BasketItemVM>();
            foreach (var item in basket)
            {
                Product DbProduct = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == item.ProductId);
                if (DbProduct == null) continue;
                BasketItemVM basketItem = new BasketItemVM()
                {
                    ProductId = DbProduct.Id,
                    Image = DbProduct.ProductImages.FirstOrDefault(x => x.IsFront == true).Image,
                    Name = DbProduct.Name,
                    Price = DbProduct.SellPrice,
                    Raiting = DbProduct.Raiting,
                    IsActive = DbProduct.StockCount > 0 ? true : false,
                    Count = item.Count
                };
                basketItems.Add(basketItem);
            }
            return View(basketItems);
        }
    }
}
