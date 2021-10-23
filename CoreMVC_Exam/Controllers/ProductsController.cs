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
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CoreMVC_Exam.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CoreMVC_ExamContext _context;

        private readonly ILogger<ProductsController> _logger;

        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductsController(CoreMVC_ExamContext context,
            ILogger<ProductsController> logger,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            HashSet<string> hash = new HashSet<string>();
            var prod = _context.Products.Select(t => t.Brand);

            foreach (var item in prod)
            {
                hash.Add(item);
            }
             

            return View(await _context.Products.Include(t => t.Category).ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> CreateAsync()
        {
            var vm = new ProductFormViewModel
            {
                Product = new Product(),
                Categories = await _context.Categories.ToListAsync()
            };

            return View("Create",vm);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(ProductFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Product.ID == 0)
                {
                    if (model.Product.Brand == null ||
                        model.Product.Model == null ||
                        model.Product.Category == null ||
                        model.Product.SerialNumber == null)
                        return RedirectToAction("Index", "Pictures");

                    model.Product.Category = _context.Categories.Where(t => t.Name == model.Product.Category.Name)
                        .FirstOrDefault();
                    _context.Products.Add(model.Product);
                    _context.SaveChanges();
                }
                else
                {
                    var imgInDB = _context.Products.Single(c => c.ID == model.Product.ID);
                    model.Product.Category = _context.Categories.Where(t => t.Name == model.Product.Category.Name)
                       .FirstOrDefault();

                    imgInDB.Brand = model.Product.Brand;
                    imgInDB.Model = model.Product.Model;
                    imgInDB.Price = model.Product.Price;
                    imgInDB.Category = model.Product.Category;
                    imgInDB.SerialNumber = model.Product.SerialNumber;

                    await _context.SaveChangesAsync();
                }
            }

            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Brand,Model,SerialNumber,Price,DateCreate")] Product product)
        {
            if (id != product.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }
    }
}
