using CoreMVC_Exam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.ViewModels
{
    public class ProductIndexViewModel
    {
        public string[] Brands { get; set; }

        public IEnumerable<Product> Products { get; set; }
    }
}
