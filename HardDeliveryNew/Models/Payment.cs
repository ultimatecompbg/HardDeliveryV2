using HardDelivery.Models;

public class Payment
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public decimal Amount { get; set; }

    public User Sender { get; set; }
    public User Receiver { get; set; }
}
