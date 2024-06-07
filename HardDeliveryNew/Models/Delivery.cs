using HardDelivery.Model.enums;
using HardDelivery.Models.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace HardDelivery.Models
{
    public class Delivery
    {
        private double _deliveryPrice;
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey(nameof(Sender))]
        public int SenderId { get; set; }
        public User? Sender { get; set; }
        [ForeignKey(nameof(Courier))]
        public int? CourierId { get; set; }
        public User? Courier { get; set; }

        [Required]
        [ForeignKey(nameof(Receiver))]
        public int ReceiverId { get; set; }
        public User? Receiver { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Weight must be a positive number")]
        public int Weight { get; set; }
        public double DeliveryPrice
        {
            get
            {
                if (Weight <= 1)
                {
                    return 5;
                }
                else if (Weight <= 3)
                {
                    return 7;
                }
                else
                {
                    return 10;
                }
            }
            set { _deliveryPrice = value; }
        }
        [Required]
        public Status Status { get; set; }
        [Required]
        public string Address { get; set; }
        [Display(Name = "Cash on Delivery")]
        public decimal PaymentAmount { get; set; } 
    }
}

        
