using HardDelivery.Models;
using HardDelivery.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardDelivery.Controllers
{
    [Authorize]
    public class DeliveriesController : Controller
    {
        private readonly IDeliveryService _deliveryService;
        private readonly UserManager<User> _userManager;

        public DeliveriesController(IDeliveryService deliveryService, UserManager<User> userManager)
        {
            _deliveryService = deliveryService;
            _userManager = userManager;
        }

 
        public IActionResult Create()
        {
            ViewData["CourierId"] = new SelectList(_userManager.Users, "Id", "UserName");
            ViewData["ReceiverId"] = new SelectList(_userManager.Users, "Id", "UserName");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReceiverId,DeliveryPrice,Address,PaymentAmount,Weight")] Delivery delivery)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            await _deliveryService.CreateDeliveryAsync(delivery, user.Id);

            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(SentDeliveries));
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
                }
            }

            ViewData["CourierId"] = new SelectList(_userManager.Users, "Id", "UserName", delivery.CourierId);
            ViewData["ReceiverId"] = new SelectList(_userManager.Users, "Id", "UserName", delivery.ReceiverId);
            return RedirectToAction(nameof(SentDeliveries), "Deliveries");
        }

       
        public async Task<IActionResult> ReceivedDeliveries()
        {
            var user = await _userManager.GetUserAsync(User);
            var deliveries = await _deliveryService.GetReceivedDeliveriesAsync(user.Id);
            return View(deliveries);
        }

     
        public async Task<IActionResult> SentDeliveries()
        {
            var user = await _userManager.GetUserAsync(User);
            var deliveries = await _deliveryService.GetSentDeliveriesAsync(user.Id);
            return View(deliveries);
        }

        
        [Authorize(Roles = "courier,admin")]
        public async Task<IActionResult> AvailableDeliveries()
        {
            var deliveries = await _deliveryService.GetAvailableDeliveriesAsync();
            return View(deliveries);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "courier,admin")]
        public async Task<IActionResult> TakeDelivery(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            await _deliveryService.TakeDeliveryAsync(id, user.Id);
            return RedirectToAction(nameof(AvailableDeliveries));
        }

        [Authorize(Roles = "courier,admin")]
        public async Task<IActionResult> CourierDeliveries()
        {
            var user = await _userManager.GetUserAsync(User);
            var deliveries = await _deliveryService.GetCourierDeliveriesAsync(user.Id);
            return View(deliveries);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "courier,admin")]
        public async Task<IActionResult> MarkAsDelivered(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            await _deliveryService.MarkAsDeliveredAsync(id, user.Id);
            return RedirectToAction(nameof(CourierDeliveries));
        }

        
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AllDeliveries()
        {
            var deliveries = await _deliveryService.GetAllDeliveriesAsync();
            return View(deliveries);
        }
    }
}
