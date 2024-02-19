using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebServiceApp.Data;
using WebServiceApp.Models;

namespace WebServiceApp.Controllers
{
    
    public class WebserviceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WebserviceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Webservice
        public async Task<IActionResult> Index()
        {
            return View(await _context.Webservices.ToListAsync());
        }

        // GET: Webservice/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webServiceModel = await _context.Webservices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (webServiceModel == null)
            {
                return NotFound();
            }

            return View(webServiceModel);
        }

        [Authorize]
        // GET: Webservice/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Webservice/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Url,ApiKeyRequired")] WebServiceModel webServiceModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(webServiceModel);

                // Add loggedin user to created by
                webServiceModel.CreatedBy = User.Identity?.Name ?? "Unkown";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(webServiceModel);
        }
[Authorize]
        // GET: Webservice/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webServiceModel = await _context.Webservices.FindAsync(id);
            if (webServiceModel == null)
            {
                return NotFound();
            }
            return View(webServiceModel);
        }

        // POST: Webservice/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Url,ApiKeyRequired")] WebServiceModel webServiceModel)
        {
            if (id != webServiceModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(webServiceModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WebServiceModelExists(webServiceModel.Id))
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
            return View(webServiceModel);
        }
        
[Authorize]
        // GET: Webservice/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var webServiceModel = await _context.Webservices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (webServiceModel == null)
            {
                return NotFound();
            }

            return View(webServiceModel);
        }

        // POST: Webservice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var webServiceModel = await _context.Webservices.FindAsync(id);
            if (webServiceModel != null)
            {
                _context.Webservices.Remove(webServiceModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WebServiceModelExists(int id)
        {
            return _context.Webservices.Any(e => e.Id == id);
        }
    }
}
