using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HardDelivery.Model.enums;

namespace HardDelivery.Models
{
    public class Delivery
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }
        public User Sender { get; set; }

        [Required]
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }

        public int CourierId { get; set; }
        public User Courier { get; set; }

        [Required]
        public decimal DeliveryPrice { get; set; }

        public Status Status { get; set; } = Status.pending;
    }
}
