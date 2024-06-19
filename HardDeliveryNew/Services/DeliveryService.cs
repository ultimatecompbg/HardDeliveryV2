using HardDelivery.Data;
using HardDelivery.Model.enums;
using HardDelivery.Models;
using HardDelivery.Models.enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardDelivery.Services
{
    public interface IDeliveryService
    {
        Task CreateDeliveryAsync(Delivery delivery, int senderId);
        Task<List<Delivery>> GetReceivedDeliveriesAsync(int receiverId);
        Task<List<Delivery>> GetSentDeliveriesAsync(int senderId);
        Task<List<Delivery>> GetAvailableDeliveriesAsync();
        Task TakeDeliveryAsync(int id, int courierId);
        Task<List<Delivery>> GetCourierDeliveriesAsync(int courierId);
        Task MarkAsDeliveredAsync(int id, int courierId);
        Task<List<Delivery>> GetAllDeliveriesAsync();
    }

    public class DeliveryService : IDeliveryService
    {
        private readonly ApplicationDbContext _context;

        public DeliveryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateDeliveryAsync(Delivery delivery, int senderId)
        {
            delivery.SenderId = senderId;
            delivery.Status = Status.pending;
            _context.Add(delivery);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Delivery>> GetReceivedDeliveriesAsync(int receiverId)
        {
            return await _context.Deliveries
                .Where(d => d.ReceiverId == receiverId)
                .Include(d => d.Courier)
                .Include(d => d.Sender)
                .ToListAsync();
        }

        public async Task<List<Delivery>> GetSentDeliveriesAsync(int senderId)
        {
            return await _context.Deliveries
                .Where(d => d.SenderId == senderId)
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .ToListAsync();
        }

        public async Task<List<Delivery>> GetAvailableDeliveriesAsync()
        {
            return await _context.Deliveries
                .Where(d => d.Status == Status.pending && d.CourierId == null)
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .Include(d => d.Sender)
                .ToListAsync();
        }

        public async Task TakeDeliveryAsync(int id, int courierId)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery != null && delivery.Status == Status.pending && delivery.CourierId == null)
            {
                delivery.CourierId = courierId;
                delivery.Status = Status.delivering;
                _context.Update(delivery);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Delivery>> GetCourierDeliveriesAsync(int courierId)
        {
            return await _context.Deliveries
                .Where(d => d.CourierId == courierId)
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .Include(d => d.Sender)
                .ToListAsync();
        }

        public async Task MarkAsDeliveredAsync(int id, int courierId)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery != null && delivery.CourierId == courierId && delivery.Status == Status.delivering)
            {
                delivery.Status = Status.delivered;
                _context.Update(delivery);
                await _context.SaveChangesAsync();

                if (delivery.PaymentAmount.HasValue)
                {
                    var paymentReceiver = await _context.Users.FindAsync(delivery.SenderId);
                    var payment = new Payment
                    {
                        SenderId = delivery.ReceiverId,
                        ReceiverId = delivery.SenderId,
                        Amount = (decimal)delivery.PaymentAmount
                    };

                    _context.Payments.Add(payment);
                    paymentReceiver.Balance += payment.Amount;
                    _context.Update(paymentReceiver);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<List<Delivery>> GetAllDeliveriesAsync()
        {
            return await _context.Deliveries
                .Include(d => d.Courier)
                .Include(d => d.Receiver)
                .Include(d => d.Sender)
                .ToListAsync();
        }
    }
}
