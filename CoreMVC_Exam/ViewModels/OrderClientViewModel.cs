using CoreMVC_Exam.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.ViewModels
{
    public class OrderClientViewModel
    {
        public int Order_Id { get; set; }

        [Display(Name = "Товар")]
        public Product Product { get; set; }

        [Display(Name = "Клиент")]
        public Client Client { get; set; }

        [Display(Name = "Категории")]
        public List<Category> Categories { get; set; }
    }
}
