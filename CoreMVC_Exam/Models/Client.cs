using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.Models
{
    public class Client
    {
        [Key]
        public int ID { get; set; }

        [PersonalData]
        [Required(ErrorMessage = "Введите номер пасспорта")]
        [Display(Name = "Пасспорт")]
        public string Passport { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Введите фамилию")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Phone]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [EmailAddress]
        [Display(Name = "Эл.Почта")]
        [Required(ErrorMessage = "Введите название вашей эл.почты")]
        public string Email { get; set; }

        [Display(Name = "День рождения")]
        public DateTime Birthday { get; set; }
    }
}
