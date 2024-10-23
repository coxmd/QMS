using QueueManagementSystem.MVC.Models;
using QueueManagementSystem.MVC.Models.Smtp;
using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.MVC.Data.EntityConfigurations;
using ServiceProvider = QueueManagementSystem.MVC.Models.ServiceProvider;
using QueueManagementSystem.MVC.Models.SmsConfig;

namespace QueueManagementSystem.MVC.Data
{
    public class QueueManagementSystemContext : DbContext
    {
        public QueueManagementSystemContext(DbContextOptions<QueueManagementSystemContext> options) : base(options)
        {

        }

        public DbSet<Ticket> QueuedTickets {get; set;}
        public DbSet<Service> Services {get; set;}
        public DbSet<ServedTicket> ServedTickets {get; set;}
        public DbSet<ServiceProvider> ServiceProviders {get; set;}
        public DbSet<ServicePoint> ServicePoints {get; set;}
        public DbSet<Sms> SmsMessages { get; set; }
        public DbSet<EmailSettingsModel> SmptSettings { get; set; }
        public DbSet<NotificationMessage> Notifications { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Biometrics> Biodata { get; set; }
        
        public DbSet<Advert> Adverts { get; set; }
        public DbSet<SmsSettingsModel> SmsSettings { get; set; }
        public DbSet<Configuration> Configurations { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Configure the foreign key relationship between Service and ServiceCategory
            builder.Entity<Service>()
                .HasOne(s => s.ServiceCategory)
                .WithMany(sc => sc.Services)
                .HasForeignKey(s => s.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Optional: use Cascade or Restrict

            // Configure the Ticket and ServicePoint relationship
            builder.Entity<Ticket>()
                .HasOne(t => t.ServicePoint)
                .WithMany() 
                .HasForeignKey(t => t.ServicePointId)
                .OnDelete(DeleteBehavior.Restrict); // Optional, use Cascade or Restrict as needed

            //Configure the ServedTicket and ServicePoint relationship
            builder.Entity<ServedTicket>()
                .HasOne(t => t.ServicePoint)
                .WithMany()
                .HasForeignKey(t => t.ServicePointId)
                .OnDelete(DeleteBehavior.Restrict); // Optional, use Cascade or Restrict as needed

            // Configure the relationship between Ticket and Biometrics
            builder.Entity<Ticket>()
                .HasOne(t => t.Biometrics)
                .WithMany()
                .HasForeignKey(t => t.BiometricsId)
                .OnDelete(DeleteBehavior.Restrict); // Optional, use Cascade or Restrict as needed

            builder.ApplyConfiguration(new TicketEntityTypeConfiguration());
            builder.ApplyConfiguration(new ServiceEntityTypeConfiguration());
            builder.ApplyConfiguration(new ServedTicketEntityTypeConfiguration());
            builder.ApplyConfiguration(new ServiceProviderEntityTypeConfiguration());
            builder.ApplyConfiguration(new ServicePointEntityTypeConfiguration());

        }


    }
}