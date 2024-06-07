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
        public async Task<IActionResult> Create([Bind("ReceiverId,DeliveryPrice,Address,Weight")] Delivery delivery)
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
                delivery.Receiver = await _context.Users.FindAsync(delivery.ReceiverId);
                delivery.Sender = user;

                _context.Add(delivery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }
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


        // Normal Users - Deliveries for receiving or received
        public async Task<IActionResult> ReceivedDeliveries()
        {
            var user = await _userManager.GetUserAsync(User);
            var deliveries = await _context.Deliveries
                .Where(d => d.ReceiverId == user.Id)
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .Include(d => d.Sender)
                .ToListAsync();
            return View(deliveries);
        }

        // Normal Users - Deliveries sent
        public async Task<IActionResult> SentDeliveries()
        {
            var user = await _userManager.GetUserAsync(User);
            var deliveries = await _context.Deliveries
                .Where(d => d.SenderId == user.Id)
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .Include(d => d.Sender)
                .ToListAsync();
            return View(deliveries);
        }

        // Couriers - Available deliveries
        [Authorize(Roles = "courier,admin")]
        public async Task<IActionResult> AvailableDeliveries()
        {
            var deliveries = await _context.Deliveries
                .Where(d => d.Status == Status.pending && d.CourierId == null)
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .Include(d => d.Sender)
                .ToListAsync();
            return View(deliveries);
        }
        [HttpPost]
        [Authorize(Roles = "courier,admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeDelivery(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            if (delivery.Status == Status.pending && delivery.CourierId == null)
            {
                delivery.CourierId = user.Id;
                delivery.Status = Status.delivering;
                _context.Update(delivery);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AvailableDeliveries));
        }
        // Couriers - Courier's deliveries
        [Authorize(Roles = "courier,admin")]
        public async Task<IActionResult> CourierDeliveries()
        {
            var user = await _userManager.GetUserAsync(User);
            var deliveries = await _context.Deliveries
                .Where(d => d.CourierId == user.Id)
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .Include(d => d.Sender)
                .ToListAsync();
            return View(deliveries);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsDelivered(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null || delivery.Courier != user)
            {
                return NotFound();
            }

            delivery.Status = Status.delivered;
            _context.Update(delivery);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CourierDeliveries));
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AllDeliveries()
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .Include(d => d.Sender)
                .ToListAsync();
            return View(deliveries);
        }
    }
}
