using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_test.Data;
using Project_test.Models;

namespace Project_test.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Project_testContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductsController(IWebHostEnvironment hostingEnvironment, Project_testContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        // GET: Products
        /*public async Task<IActionResult> Index()
        {
            var project_testContext = _context.Product.Include(p => p.Category);
            return View(await project_testContext.ToListAsync());
        }*/
        public async Task<IActionResult> Index(string searchString)
        {
            var products = _context.Product.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) || p.Description.Contains(searchString));
            }

            return View(await products.ToListAsync());
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [Authorize]
        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFileName)
        {
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (ImageFileName != null)
                {

                    product.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    // Save the image to a unique filename
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFileName.FileName;
                    var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", uniqueFileName);

                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await ImageFileName.CopyToAsync(fileStream);
                    }

                    // Update the product model with the image filename
                    product.ImagePath = uniqueFileName;
                }

                // Save the product to the database
                _context.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, return to the create view with the existing data
            return View(product);
        }

        [Authorize]
        [Route("Products/Edit/{id}")]
        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);

            if (product.UserId != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            {
                return Forbid(); // Or handle unauthorized access as needed
            }
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", product.CategoryID);
            ViewBag.ExistingImagePath = product.ImagePath;
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Products/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CategoryID,Name,Description,Price")] Product product, IFormFile ImageFileName)
        {
            if (id != product.ID)
            {
                return NotFound();
            }

            

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFileName != null)
                    {
                        // Save the image to a unique filename
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFileName.FileName;
                        var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", uniqueFileName);

                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await ImageFileName.CopyToAsync(fileStream);
                        }

                        // Update the product model with the image filename
                        product.ImagePath = uniqueFileName;
                    }

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

            // If the model state is not valid, return to the edit view with the existing data
            ViewData["CategoryID"] = new SelectList(_context.Category, "ID", "Name", product.CategoryID);
            return View(product);
        }


        [Authorize]
        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            if (product.UserId != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            {
                return Forbid(); // Or handle unauthorized access as needed
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ID == id);
        }
    }
}
