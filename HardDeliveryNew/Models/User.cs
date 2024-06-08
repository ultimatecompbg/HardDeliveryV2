using HardDelivery.Models.enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HardDelivery.Models
{
    public class User : IdentityUser<int>
    {
        [Required]
        public Role Role { get; set; }

        [Required]
        public string Name { get; set; }
        public decimal Balance { get; set; }

        public List<Delivery> Delivered { get; set; } = new List<Delivery>();
        public List<Delivery> Sended { get; set; } = new List<Delivery>();
        public List<Delivery> Received { get; set; } = new List<Delivery>();
        public List<Payment> ReceivedPayments { get; set; } = new List<Payment>();
        public List<Payment> SentPayments { get; set; } = new List<Payment>();
    }
}
