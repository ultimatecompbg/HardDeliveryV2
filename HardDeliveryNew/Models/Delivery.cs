

using HardDelivery.Model.enums;

namespace HardDelivery.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public int CourierId { get; set; }
        public int ReceiverId { get; set; }
        public int SenderId { get; set; }
        public decimal DeliveryPrice { get; set; }
        public Status Status { get; set; }
        public User Courier { get; set; }
        public User Receiver { get; set; }
        public User Sender { get; set; }
    }

}
