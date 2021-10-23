using CoreMVC_Exam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.ViewModels
{
    public class OrderViewModel
    {
        public List<Order> OrdersProccess { get; set; }

        public List<Order> OrdersMy { get; set; }

        public List<Order> OrdersPending { get; set; }

        public List<Category> Categories { get; set; }

        public double EarnedMoney { get; set; }
    }
}
