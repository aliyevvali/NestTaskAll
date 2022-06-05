using AspNestTaskAll.DAL;
using AspNestTaskAll.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNestTaskAll.Services
{
    public class LayoutServices
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _acces;

        public LayoutServices(AppDbContext context, IHttpContextAccessor acces)
        {
            _context = context;
            _acces = acces;
        }
        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(p => p.Key, p => p.Value);
        }
        public int GetBasketCount()
        {
            if (_acces.HttpContext.Request.Cookies["Basket"] == null)
            {
                return 0;
            }
            List<BasketVM> basket = JsonConvert.DeserializeObject<List<BasketVM>>(_acces.HttpContext.Request.Cookies["Basket"]);
            if (basket == null)
            {
                return 0;
            }
            return basket.Sum(b=>b.Count);
        }
    }
}
