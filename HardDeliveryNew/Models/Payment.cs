using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardDelivery.Models
{
	public class Payment
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public User Receiver { get; set; }
		[Required]
		public User Sender { get; set; }
		[Required]
		public int Sum { get; set; }
        [ForeignKey(nameof(Sender))]
        public int SenderId { get; set; }

        [ForeignKey(nameof(Receiver))]
        public int ReceiverId { get; set; }
    }
}
