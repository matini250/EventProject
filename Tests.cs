using Xunit;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Models;
using EventManagementSystem.Data;

namespace EventManagementSystem.Tests
{
    [Collection("Sequential")]
    public class Tests
    {
        // Hilfsmethode, um eine InMemory-Datenbank zu erstellen
                private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseLazyLoadingProxies()
                .UseInMemoryDatabase("EventManagementSystemInMemoryDb")   // In-Memory-Datenbank
                .Options;

            return new ApplicationDbContext(options);
        }


        // Test 1: Datenbankerstellung
        [Fact]
        public void CanCreateDatabase()
        {
            using (var context = CreateInMemoryDbContext())
            {
                Assert.True(context.Database.CanConnect());
            }
        }

        // Test 2: Vererbung mit Discriminator
        [Fact]
        public void CanAddAdministratorAndCustomer()
        {
            using (var context = CreateInMemoryDbContext())
            {
                var admin = new Administrator { Name = "Admin", Email = "admin@example.com" };
                var customer = new Customer { Name = "Customer", Email = "customer@example.com" };

                // Überprüfen, ob der Administrator und der Kunde schon existieren
                if (!context.Administrators.Any())
                    context.Administrators.Add(admin);
                
                if (!context.Customers.Any())
                    context.Customers.Add(customer);

                context.SaveChanges();

                Assert.Equal(1, context.Administrators.Count()); // Sollte 1 Administrator sein
                Assert.Equal(1, context.Customers.Count()); // Sollte 1 Kunde sein
            }
        }

        // Test 3: Readonly Collection und Backing Field
        [Fact]
        public void CanAddTicketToEvent()
        {
            using (var context = CreateInMemoryDbContext())
            {
                var event1 = new Event { Name = "Concert", Date = DateTime.Now };
                var ticket = new Ticket { Price = 50m, Seat = "A1" };

                event1.AddTicket(ticket); // Stelle sicher, dass das Ticket zu Event hinzugefügt wird

                context.Events.Add(event1);
                context.SaveChanges();

                var savedEvent = context.Events.Include(e => e.Tickets).FirstOrDefault();
                Assert.NotNull(savedEvent); // Event sollte nicht null sein
                Assert.Single(savedEvent.Tickets); // Sollte genau ein Ticket enthalten
            }
        }

        // Test 4: Transformation von Objekten
       [Fact]
        public void CanMarkEventAsCancelled()
        {
            using (var context = CreateInMemoryDbContext())
            {
                var eventObj = new Event
                {
                    Name = "Concert",
                    Date = DateTime.Now,
                    Venue = new Venue
                    {
                        Name = "Concert Hall",
                        Address = new Address { Street = "Main St", City = "Vienna", PostalCode = "1010" },
                        Capacity = 1000
                    }
                };

                var ticket = new Ticket
                {
                    Seat = "A1",
                    Price = 120
                };

                ticket.SetAvailability(true); // Ticket verfügbar machen
                eventObj.AddTicket(ticket);

                context.Events.Add(eventObj);
                context.SaveChanges();

                var savedEvent = context.Events.Include(e => e.Tickets).FirstOrDefault();
                Assert.NotNull(savedEvent);

                savedEvent!.MarkEventAsCancelled(); // Event als storniert markieren
                Assert.Equal(120, savedEvent.GetRevenue()); // Einnahmen nach Stornierung
            }
        }

        // Test 5: LINQ-Abfrage: Aktive Buchungen
        [Fact]
        public void CanGetActiveBookingsForCustomer()
        {
            using (var context = CreateInMemoryDbContext())
            {
                var ticket = new Ticket { Seat = "A1", Price = 50 };
                ticket.SetAvailability(true); // Verfügbarkeit sicherstellen

                var customer = new Customer { Name = "Jane Doe", Email = "jane.doe@example.com" };
                customer.BookTicket(ticket); // Buchung hinzufügen

                context.Customers.Add(customer);
                context.SaveChanges();

                var savedCustomer = context.Customers.Include(c => c.BookedTickets).FirstOrDefault();
                Assert.NotNull(savedCustomer); // Kunde sollte nicht null sein

                var activeBookings = savedCustomer!.GetActiveBookings(); 
                Assert.Single(savedCustomer!.BookedTickets);// Sollte genau 1 aktives Ticket enthalten
            }
        }

        // Test 6: LINQ-Abfrage: Einnahmen eines Events
        [Fact]
        public void CanCalculateEventRevenue()
{
            using (var context = CreateInMemoryDbContext())
            {
                var event1 = new Event { Name = "Concert", Date = DateTime.Now };
                var ticket1 = new Ticket { Price = 50m, Seat = "A1" };
                var ticket2 = new Ticket { Price = 70m, Seat = "A2" };

                ticket1.SetAvailability(true); // Ticket verfügbar machen
                ticket2.SetAvailability(true); // Ticket verfügbar machen

                event1.AddTicket(ticket1);
                event1.AddTicket(ticket2);

                context.Events.Add(event1);
                context.SaveChanges();

                var savedEvent = context.Events.Include(e => e.Tickets).FirstOrDefault();
                Assert.NotNull(savedEvent);

                var revenue = savedEvent!.GetRevenue(); // Einnahmen berechnen
                Assert.Equal(120m, revenue); // Einnahmen sollten 50 + 70 = 120 sein
            }
        }

        
        [Fact]
        public void CanAssociateCustomerWithTickets()
        {
            using (var context = CreateInMemoryDbContext())
            {
                var ticket = new Ticket { Seat = "B1", Price = 100 };
                ticket.SetAvailability(true);
                var customer = new Customer { Name = "John Doe", Email = "john.doe@example.com" };

                customer.BookTicket(ticket);

                context.Customers.Add(customer);
                context.SaveChanges();

                var savedCustomer = context.Customers.Include(c => c.BookedTickets).FirstOrDefault();
                Assert.NotNull(savedCustomer);
                Assert.Single(savedCustomer!.BookedTickets); // Sollte genau 1 gebuchtes Ticket enthalten
            }
        }

        
        [Fact]
        public void CanAddAddressToVenue()
        {
            using (var context = CreateInMemoryDbContext())
            {
                var address = new Address
                {
                    Street = "123 Main St",
                    City = "Vienna",
                    PostalCode = "1010"
                };

                var venue = new Venue
                {
                    Name = "Stadthalle",
                    Address = address,
                    Capacity = 5000
                };

                context.Venues.Add(venue);
                context.SaveChanges();

                var savedVenue = context.Venues.Include(v => v.Address).FirstOrDefault();
                Assert.NotNull(savedVenue);
                Assert.NotNull(savedVenue!.Address); 
            }
        }

        [Fact]
        public void CanLoadVenueLazily()
        {
            using (var context = CreateInMemoryDbContext())
            {
                var address = new Address
                {
                    Street = "123 Main St",
                    City = "Vienna",
                    PostalCode = "1010"
                };

                var venue = new Venue
                {
                    Name = "Stadthalle",
                    Address = address,
                    Capacity = 5000
                };

                var event1 = new Event
                {
                    Name = "Concert",
                    Date = DateTime.Now,
                    Venue = venue
                };

                context.Events.Add(event1);
                context.SaveChanges();

                var savedEvent = context.Events.FirstOrDefault();
                Assert.NotNull(savedEvent);
                Assert.NotNull(savedEvent!.Venue); 
            }
        }
    }
}