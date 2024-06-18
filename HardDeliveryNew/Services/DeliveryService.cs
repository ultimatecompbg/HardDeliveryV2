using HardDelivery.Data;
using HardDelivery.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HardDelivery.Services
{
    public interface IDeliveryService
    {
        Task<List<Delivery>> GetDeliveriesAsync();
        Task CreateDeliveryAsync(Delivery delivery);
        Task<Delivery> GetDeliveryByIdAsync(int id);
        Task UpdateDeliveryAsync(Delivery delivery);
        Task DeleteDeliveryAsync(int id);
    }

    public class DeliveryService : IDeliveryService
    {
        private readonly ApplicationDbContext _context;

        public DeliveryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Delivery>> GetDeliveriesAsync()
        {
            return await _context.Deliveries.ToListAsync();
        }

        public async Task CreateDeliveryAsync(Delivery delivery)
        {
            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();
        }

        public async Task<Delivery> GetDeliveryByIdAsync(int id)
        {
            return await _context.Deliveries.FindAsync(id);
        }

        public async Task UpdateDeliveryAsync(Delivery delivery)
        {
            _context.Entry(delivery).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDeliveryAsync(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery != null)
            {
                _context.Deliveries.Remove(delivery);
                await _context.SaveChangesAsync();
            }
        }
    }
}
