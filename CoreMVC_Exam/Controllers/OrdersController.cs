using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoreMVC_Exam.Data;
using CoreMVC_Exam.Models;
using CoreMVC_Exam.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using CoreMVC_Exam.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;

namespace CoreMVC_Exam.Controllers
{
    public class OrdersController : Controller
    {
        private readonly CoreMVC_ExamContext _context;

        private readonly UserManager<CoreMVC_ExamUser> _userManager;

        private readonly SmtpClient _client;

        public OrdersController(CoreMVC_ExamContext context, UserManager<CoreMVC_ExamUser> userManager)
        {
            _context = context;
            _userManager = userManager;

            // отправляем почту с учётной записи на сервере Google
            _client = new SmtpClient("smtp.gmail.com", 587);

            // настройка входа на сервер
            _client.Credentials = new NetworkCredential("sevicemenegment331@gmail.com", "service12345670");
            _client.EnableSsl = true;
        }

        private Task<CoreMVC_ExamUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Orders
        [Authorize(Roles = "Admin, Master")]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();

            var user_id = user?.Id;

            var my_orders = _context.Orders
                .Where(t => t.Master.Id == user_id)
                .Include(t => t.Client)
                .Include(t => t.Master)
                .Include(t => t.Product)
                .ThenInclude(t => t.Category)
                .ToList();

            var process_orders = _context.Orders
                .Where(t => t.Master != null)
                .Include(t => t.Client)
                .Include(t => t.Master)
                .Include(t => t.Product)
                .ThenInclude(t => t.Category)
                .ToList();

            var pending_orders = _context.Orders
                .Where(t => t.Master == null)
                .Include(t => t.Client)
                .Include(t => t.Master)
                .Include(t => t.Product)
                .ThenInclude(t => t.Category)
                .ToList();

            var categories = _context.Categories.ToList();

            double earned = 0;

            if (my_orders.Count > 0)
            {
                foreach (var item in my_orders)
                {
                    earned += item.Price;
                }
            }

            OrderViewModel orderViewModel = new OrderViewModel()
            {
                OrdersMy = my_orders,
                OrdersProccess = process_orders,
                OrdersPending = pending_orders,
                EarnedMoney = earned,
                Categories = categories
            };

            return View(orderViewModel);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpGet]
        [HttpPost]
        // метод возвращает PartialView, который содержит таблицу на Razor для возврата в исходную форму
        public async Task<ActionResult> OrdersListAsync(string brand)
        {
            IEnumerable<Order> orders = null;

            var user = await GetCurrentUserAsync();

            var user_id = user?.Id;

            orders = _context.Orders
                .Where(t => t.Master.Id == user_id)
                .Include(t => t.Client)
                .Include(t => t.Product)
                .ThenInclude(t => t.Category)
                .Where(t => t.Product.Brand == brand)
                .ToListAsync().Result;

            return PartialView("OrdersList",orders);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            OrderClientViewModel order = new OrderClientViewModel()
            {
                Categories = await _context.Categories.ToListAsync(),
                Product = new Product()
            };

            if (HttpContext.Session.GetString("client_id") != "")
            {
                int id = Convert.ToInt32(HttpContext.Session.GetString("client_id"));
                ViewBag.ClientID = id;

                if (id != 0)
                {
                    order.Client = _context.Clients.FirstOrDefault(t => t.ID == id);
                }
                else
                {

                    order.Client = new Client();
                }
            }

            return View(order);
        }

        public IActionResult Accept(int ID)
        {
            if (ID == 0)
                return View("Index");

            var order = _context.Orders.Where(t => t.ID == ID).Include(t => t.Product)
                .ThenInclude(t => t.Category).Include(t => t.Client).FirstOrDefault();
            
            return View(order);
        }


        private MailMessage CreateMailMessage(string from, string to, string subject, string body)
        {
            MailMessage mailMsg = new MailMessage();

            // заполнить поля почтового сообщения из полей окна
            mailMsg.From = new MailAddress(from);
            mailMsg.To.Add(new MailAddress(to));
            mailMsg.IsBodyHtml = false;
            mailMsg.Subject = subject;
            mailMsg.Body = body;

            return mailMsg;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOrder(Order order)
        {
            if(ModelState.IsValid)
            {
                var o = _context.Orders.Where(t => t.ID == order.ID).Include(t => t.Client)
                    .Include(t => t.Product)
                    .ThenInclude(t => t.Category)
                    .FirstOrDefault();

                var user = await GetCurrentUserAsync();

                o.Master = user;
                o.Price = order.Price;
                o.DateTimeStart = order.DateTimeStart;
                o.DateTimeEnd = order.DateTimeEnd;

                await _context.SaveChangesAsync();

                _client.Send(CreateMailMessage("sevicemenegment331@gmail.com", o.Client.Email, "Сервис Центр", $"Здравствуйте {o.Client.FirstName} {o.Client.LastName}, ваш заказ успешно принят! Ожидайте его к {o.DateTimeEnd.Date}. Сумма к оплате: {o.Price}"));
            }

            return View("Index", "Content");
        }


        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(OrderClientViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Order order1 = new Order();

                if (vm.Product.Category.Name != "")
                {
                    var c = _context.Categories.FirstOrDefault(t => t.Name == vm.Product.Category.Name);
                    vm.Product.Category = c;
                }

                order1.Product = vm.Product;
                order1.Client = vm.Client;

                _context.Add(order1);
                await _context.SaveChangesAsync();

                if (HttpContext.Session.GetString("client_id") == "0")
                {
                    HttpContext.Session.SetString("firstName", order1.Client.FirstName);

                    HttpContext.Session.SetString("lastName", order1.Client.LastName);

                    HttpContext.Session.SetString("client_id", order1.Client.ID.ToString());
                }

                return View("OrderSuccessfully", order1);
            }

            return View("Create",vm);
        }

        public ActionResult OrderSuccessfully(Order order)
        {
            if (order == null)
                return View("Create");

            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var products = (from p in _context.Products
                           select p.Brand + " " + p.Model).ToList();

            var clients = (from p in _context.Clients
                           select p.FirstName + " " + p.LastName).ToList();


            var masters = (from user in _context.Users
                           where _context.Roles.Any(r => r.Name == "Master")
                           select user.FirstName + " " + user.LastName).ToList();

            OrderEditViewModel orderEdit = new OrderEditViewModel()
            {
                Clients = clients,
                Masters = masters,
                Order = order,
                Products = products
            };

            return View(orderEdit);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderEditViewModel viewModel)
        {
            if (viewModel.Order.ID == 0)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string[] cl_name = viewModel.Temp_Client.Split(' ');

                    string[] ms_name = viewModel.Temp_Master.Split(' ');

                    string[] pr_name = viewModel.Temp_Product.Split(' ');

                    var client = _context.Clients.FirstOrDefault(t => t.FirstName == cl_name[0]
                    && t.LastName == cl_name[1]);

                    var masters = (from user in _context.Users
                                   where _context.Roles.Any(r => r.Name == "Master")
                                   select user).ToList();

                    var master = _context.Users.FirstOrDefault(t => t.FirstName == ms_name[0]
                    && t.LastName == ms_name[1]);

                    var product = _context.Products.FirstOrDefault(t => t.Brand == pr_name[0]
                    && t.Model == pr_name[1]);

                    viewModel.Order.Client = client;
                    viewModel.Order.Master = master;
                    viewModel.Order.Product = product;

                    _context.Update(viewModel.Order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(viewModel.Order.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index","Admin");
            }
            return View(viewModel.Order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Orders
                .Include(t => t.Client)
                .Include(t => t.Master)
                .Include(t => t.Product)
                .ThenInclude(t => t.Category)
                .FirstOrDefault(m => m.ID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.ID == id);
        }
    }
}
