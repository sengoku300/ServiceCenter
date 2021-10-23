using CoreMVC_Exam.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.Models
{
    public class Order
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public Product Product { get; set; }

        public CoreMVC_ExamUser Master { get; set; }

        [Required]
        public Client Client { get; set; }

        public DateTime DateTimeStart { get; set; }

        public DateTime DateTimeEnd { get; set; }

        public double Price { get; set; }
    }
}
