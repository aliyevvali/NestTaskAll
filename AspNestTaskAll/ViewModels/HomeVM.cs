using AspNestTaskAll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNestTaskAll.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Category> PopularCategories { get; set; }
        public List<Category> RandomProducts { get; set; }
        public List<Product> Products { get; set; }
        public List<Product> RecentProducts { get; set; }
        public List<Product> TopRatedProducts { get; set; }
    }
}
