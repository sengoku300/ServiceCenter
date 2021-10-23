using CoreMVC_Exam.Areas.Identity.Data;
using CoreMVC_Exam.Data;
using CoreMVC_Exam.Models;
using CoreMVC_Exam.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMVC_Exam.Controllers
{
    public class AdminController : Controller
    {
        private readonly CoreMVC_ExamContext _context;
        private readonly UserManager<CoreMVC_ExamUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminController(CoreMVC_ExamContext context,
            UserManager<CoreMVC_ExamUser> userManager,
            IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostEnvironment;
        }

        public async Task<ActionResult> MasterEditAsync(string id)
        {
            if (id == null)
                return View("Index");

            var master = await _context.Users.FindAsync(id);

            MasterFormViewModel vm = new MasterFormViewModel()
            {
                Master = master
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(MasterFormViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if(vm.Picture != null)
                {
                    var savePath = _hostingEnvironment.WebRootPath
                                          + "\\Files\\" + vm.Picture.FileName;

                    // сохранение файла
                    using (var fileStream = new FileStream(savePath, FileMode.Create))
                    {
                        await vm.Picture.CopyToAsync(fileStream);
                    }

                    vm.Master.PathToImage = "\\Files\\" + vm.Picture.FileName;
                }

                _context.Update(vm.Master);
                await _context.SaveChangesAsync();
            }

            return View("Index");
        }
        public ActionResult MasterDetails(string id)
        {
            var master = _context.Users.Find(id); 
            
            var orders_m = _context.Orders.Include(t => t.Master).Where(t => t.Master.Id == master.Id).ToList();

            MasterDetailsViewModel master_vm = new MasterDetailsViewModel
            {
                Master = master,
                CountOrders = orders_m.Count
            };

            return View(master_vm);
        }

        public IActionResult Index()
        {
            var orders = _context.Orders
               .Include(t => t.Client)
               .Include(t => t.Master)
               .Include(t => t.Product)
               .ThenInclude(t => t.Category).ToList();

            var cat = _context.Categories.ToList();

            var clients = _context.Clients.ToList();

            var masters = (from user in _context.Users
                       where _context.Roles.Any(r => r.Name == "Master")
                       select user).ToList();

            List<MasterViewModel> masterViewModel = new List<MasterViewModel>();

            foreach (var item in masters)
            {
                var orders_m = _context.Orders.Include(t => t.Master).Where(t => t.Master.Id == item.Id).ToList();

                double earned = 0;

                if (orders_m.Count > 0)
                {
                    foreach (var ord in orders_m)
                    {
                        earned += ord.Price;
                    }
                }

                masterViewModel.Add(new MasterViewModel
                {
                    Master = item,
                    CountOrders = orders_m.Count,
                    EarnedMoney = earned
                });
            }

            AdminMasterViewModel vm = new AdminMasterViewModel()
            {
                Categories = cat,
                Orders = orders,
                Masters = masterViewModel,
                Clients = clients,
                ComboMasters = masters
            };

            return View(vm);
        }

        [HttpGet]
        [HttpPost]
        public ActionResult OrdersMasterList(string email)
        {
            IEnumerable<Order> filterOrders = null;

            if (email != null)
                filterOrders = _context.Orders.Include(t => t.Product)
                    .ThenInclude(t => t.Category).Include(t => t.Master).Include(t => t.Client)
                    .Where(t => t.Master.Email == email).ToList();
            else
                filterOrders = _context.Orders.Include(t => t.Client)
               .Include(t => t.Master)
               .Include(t => t.Product)
               .ThenInclude(t => t.Category);

            return PartialView("OrdersList", filterOrders);
        }

        public ActionResult MasterDelete(string id)
        {
            if (id == null)
                return View();

            var master = _context.Users.Where(t => t.Id == id).FirstOrDefault();

            _context.Users.Remove(master);
            _context.SaveChangesAsync();

            return View("Index");
        }


        [HttpGet]
        [HttpPost]
        public ActionResult OrdersList(string cat)
        {
            IEnumerable<Order> filterOrders = null;

            if (cat != null)
                filterOrders = _context.Orders.Include(t => t.Product)
                    .ThenInclude(t => t.Category).Where(t => t.Product.Category.Name == cat).ToList();
            else
                filterOrders = _context.Orders.Include(t => t.Client)
               .Include(t => t.Master)
               .Include(t => t.Product)
               .ThenInclude(t => t.Category);

            return PartialView(filterOrders);
        }
    }
}
