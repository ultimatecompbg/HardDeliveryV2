using HardDelivery.Data;
using HardDelivery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using HardDelivery.Models.enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using HardDelivery.Model.enums;

namespace HardDelivery.Controllers
{
    [Authorize]
    public class DeliveriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DeliveriesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index() => View(await _context.Deliveries.Include(d => d.Courier).Include(d => d.Receiver).Include(d => d.Sender).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var delivery = await _context.Deliveries.Include(d => d.Courier).Include(d => d.Receiver).Include(d => d.Sender).FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null) return NotFound();

            return View(delivery);
        }

        public IActionResult Create()
        {
            ViewData["CourierId"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourierId,ReceiverId,DeliveryPrice,Address")] Delivery delivery)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            delivery.SenderId = user.Id;
         

            if (ModelState.IsValid)
            {
                delivery.Status = Status.pending;
                delivery.Courier = user;
                delivery.Receiver = await _context.Users.FindAsync(delivery.ReceiverId);
                delivery.Sender = user;

                _context.Add(delivery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourierId"] = new SelectList(_context.Users, "Id", "UserName", delivery.CourierId);
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "UserName", delivery.ReceiverId);
            return View(delivery);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null) return NotFound();

            ViewData["CourierId"] = new SelectList(_context.Users, "Id", "UserName", delivery.CourierId);
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "UserName", delivery.ReceiverId);
            return View(delivery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourierId,ReceiverId,DeliveryPrice")] Delivery delivery)
        {
            if (id != delivery.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(delivery.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourierId"] = new SelectList(_context.Users, "Id", "UserName", delivery.CourierId);
            ViewData["ReceiverId"] = new SelectList(_context.Users, "Id", "UserName", delivery.ReceiverId);
            return View(delivery);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var delivery = await _context.Deliveries.Include(d => d.Courier).Include(d => d.Receiver).Include(d => d.Sender).FirstOrDefaultAsync(m => m.Id == id);
            if (delivery == null) return NotFound();

            return View(delivery);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryExists(int id) => _context.Deliveries.Any(e => e.Id == id);
    }
}
