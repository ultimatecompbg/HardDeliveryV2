using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HardDelivery.Data;
using HardDelivery.Models;

namespace HardDelivery.Controllers
{
    public class DeliveriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeliveriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Deliveries
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.deliveries.Include(d => d.Courier).Include(d => d.Receiver).Include(d => d.Sender);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Deliveries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.deliveries == null)
            {
                return NotFound();
            }

            var delivery = await _context.deliveries
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .Include(d => d.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // GET: Deliveries/Create
        public IActionResult Create()
        {
            ViewData["CourierId"] = new SelectList(_context.users, "Id", "Name");
            ViewData["ReceiverId"] = new SelectList(_context.users, "Id", "Name");
            ViewData["SenderId"] = new SelectList(_context.users, "Id", "Name");
            return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status,CourierId,SenderId,ReceiverId,DeliveryPrice")] Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                _context.Add(delivery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourierId"] = new SelectList(_context.users, "Id", "Name", delivery.CourierId);
            ViewData["ReceiverId"] = new SelectList(_context.users, "Id", "Name", delivery.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.users, "Id", "Name", delivery.SenderId);
            return View(delivery);
        }

        // GET: Deliveries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.deliveries == null)
            {
                return NotFound();
            }

            var delivery = await _context.deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }
            ViewData["CourierId"] = new SelectList(_context.users, "Id", "Name", delivery.CourierId);
            ViewData["ReceiverId"] = new SelectList(_context.users, "Id", "Name", delivery.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.users, "Id", "Name", delivery.SenderId);
            return View(delivery);
        }

        // POST: Deliveries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,CourierId,SenderId,ReceiverId,DeliveryPrice")] Delivery delivery)
        {
            if (id != delivery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(delivery.Id))
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
            ViewData["CourierId"] = new SelectList(_context.users, "Id", "Name", delivery.CourierId);
            ViewData["ReceiverId"] = new SelectList(_context.users, "Id", "Name", delivery.ReceiverId);
            ViewData["SenderId"] = new SelectList(_context.users, "Id", "Name", delivery.SenderId);
            return View(delivery);
        }

        // GET: Deliveries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.deliveries == null)
            {
                return NotFound();
            }

            var delivery = await _context.deliveries
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .Include(d => d.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        // POST: Deliveries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.deliveries == null)
            {
                return Problem("Entity set 'ApplicationDbContext.deliveries'  is null.");
            }
            var delivery = await _context.deliveries.FindAsync(id);
            if (delivery != null)
            {
                _context.deliveries.Remove(delivery);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryExists(int id)
        {
          return (_context.deliveries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
