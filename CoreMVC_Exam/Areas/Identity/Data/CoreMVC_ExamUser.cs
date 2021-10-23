using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CoreMVC_Exam.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the CoreMVC_ExamUser class
    public class CoreMVC_ExamUser : IdentityUser
    {
        [PersonalData]
        public string Passport { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public string PathToImage { get; set; }
    }
}
