using CoreMVC_Exam.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.ViewModels
{
    public class MasterFormViewModel
    {
        public CoreMVC_ExamUser Master { get; set; }

        public IFormFile Picture { get; set; }
    }
}
