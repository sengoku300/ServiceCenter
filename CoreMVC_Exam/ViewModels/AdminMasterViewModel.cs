using CoreMVC_Exam.Areas.Identity.Data;
using CoreMVC_Exam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.ViewModels
{
    public class AdminMasterViewModel
    {
        public List<Order> Orders { get; set; }

        public List<Category> Categories { get; set; }

        public List<MasterViewModel> Masters { get; set; }

        public List<CoreMVC_ExamUser> ComboMasters { get; set; }

        public List<Client> Clients { get; set; }
    }
}
