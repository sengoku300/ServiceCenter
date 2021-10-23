using CoreMVC_Exam.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.ViewModels
{
    public class ProductFormViewModel
    {
        public List<Category> Categories { get; set; }

        [Required]
        public Product Product { get; set; }
    }
}
