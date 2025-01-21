using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Models;

namespace EventManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies() // Aktivieren Sie Lazy Loading, falls benötigt
                .UseInMemoryDatabase("TestDatabase"); // In-Memory-Datenbank
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primärschlüssel und Beziehungen für Ticket
            modelBuilder.Entity<Ticket>()
                .HasKey(t => t.Id); // Primärschlüssel definieren

            modelBuilder.Entity<Ticket>()
                .HasOne<Event>() // Ein Ticket gehört zu einem Event
                .WithMany(e => e.Tickets) // Ein Event hat viele Tickets
                .HasForeignKey("EventId")
                .OnDelete(DeleteBehavior.Cascade); // Tickets löschen, wenn Event gelöscht wird

            // Beziehung zwischen Customer und BookedTickets
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.BookedTickets)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            // Discriminator für Vererbung
            modelBuilder.Entity<Person>()
                .HasDiscriminator<string>("PersonType")
                .HasValue<Administrator>("Administrator")
                .HasValue<Customer>("Customer");

            // Beziehung zwischen Event und Venue
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict); // Keine automatische Löschung von Venues

            base.OnModelCreating(modelBuilder);
        }
    }
}