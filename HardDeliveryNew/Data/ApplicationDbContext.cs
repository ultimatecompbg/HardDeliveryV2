using HardDelivery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HardDelivery.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Delivery>()
                .Property(d => d.DeliveryPrice)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Delivery>()
                .HasOne(d => d.Sender)
                .WithMany(u => u.Sended)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Delivery>()
                .HasOne(d => d.Receiver)
                .WithMany(u => u.Received)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Delivery>()
                .HasOne(d => d.Courier)
                .WithMany(u => u.Delivered)
                .HasForeignKey(d => d.CourierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.Sender)
                .WithMany(u => u.SentPayments)
                .HasForeignKey(p => p.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.Receiver)
                .WithMany(u => u.ReceivedPayments)
                .HasForeignKey(p => p.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
