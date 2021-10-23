using CoreMVC_Exam.Areas.Identity.Data;
using CoreMVC_Exam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.ViewModels
{
    public class OrderEditViewModel
    {
        public List<string> Clients { get; set; }

        public List<string> Masters { get; set; }

        public List<string> Products { get; set; }

        public string Temp_Client { get; set; }

        public string Temp_Master { get; set; }

        public string Temp_Product { get; set; }

        public Order Order { get; set; }
    }
}
