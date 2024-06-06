using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using HardDelivery.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HardDelivery.Data;
using Microsoft.AspNetCore.Authorization;

namespace HardDelivery.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                    await _userManager.AddToRoleAsync(user, newRole);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "User not found.");
                }
            }
            return View("Index");
        }
    }
}
