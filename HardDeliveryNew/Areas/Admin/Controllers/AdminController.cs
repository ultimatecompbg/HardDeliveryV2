using HardDelivery.Models;
using HardDelivery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HardDelivery.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _adminService.GetAllUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
        {
            bool result = await _adminService.ChangeUserRoleAsync(userId, newRole);
            if (!result)
            {
               
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
