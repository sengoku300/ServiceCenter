using CoreMVC_Exam.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.ViewModels
{
    public class MasterDetailsViewModel
    {
        public int CountOrders { get; set; }

        public CoreMVC_ExamUser Master { get; set; }
    }
}
