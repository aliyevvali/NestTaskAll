using AspNestTaskAll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNestTaskAll.ViewModels
{
    public class DetailsVM
    {
        public List<Category> Categories { get; set; }
        public Product Products { get; set; }
    }
}
