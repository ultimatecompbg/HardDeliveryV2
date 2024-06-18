using HardDelivery.Models;
using HardDelivery.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HardDelivery.Areas.Deliveries.Controllers
{
    [Area("Deliveries")]
    public class DeliveriesController : Controller
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveriesController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public async Task<IActionResult> Index()
        {
            var deliveries = await _deliveryService.GetDeliveriesAsync();
            return View(deliveries);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                await _deliveryService.CreateDeliveryAsync(delivery);
                return RedirectToAction(nameof(Index));
            }
            return View(delivery);
        }
    }
}
