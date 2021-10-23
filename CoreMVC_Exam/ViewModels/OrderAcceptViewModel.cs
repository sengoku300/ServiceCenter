using CoreMVC_Exam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.ViewModels
{
    public class OrderAcceptViewModel
    {
        public int OrderID { get; set; }

        public Order Order { get; set; }
    }
}
