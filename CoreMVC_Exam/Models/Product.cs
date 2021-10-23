using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        [Display(Name = "Категория")]
        public Category Category { get; set; }

        [Required(ErrorMessage = "Введите название бренда")]
        [StringLength(35, ErrorMessage = "Название бренда не должно превышать 35 символов, и не должно составлять меньше чем 2 символа", MinimumLength = 2)]
        [Display(Name = "Бренд")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Введите модель товара")]
        [StringLength(25, MinimumLength = 2)]
        [Display(Name = "Модель")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Введите серийный номер от вашего товара")]
        [StringLength(50)]
        [Display(Name = "Серийный номер")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Введите цену за которую покупали товар")]
        [Range(50, 5000000)]
        [Display(Name = "Цена")]
        public double Price { get; set; }
    }
}
