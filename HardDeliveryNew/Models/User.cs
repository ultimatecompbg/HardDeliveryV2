using HardDelivery.Models.enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HardDelivery.Models
{
	public class User
	{
		[Key] 
		public int Id { get; set; }
		[Required]
		public Role Role { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public int Password { get; set; }
		public List<Delivery> Delivered = new List<Delivery>();
        public List<Delivery> Sended = new List<Delivery>();
        public List<Delivery> Received = new List<Delivery>();
        public List<Payment> ReceivedPayments = new List<Payment>();
        public List<Payment> SentPayments = new List<Payment>();

    }
}
