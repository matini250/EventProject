using EventManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
public class Event
{
    private readonly List<Ticket> _tickets = new List<Ticket>();
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public virtual Venue? Venue { get; set; } // Lazy Loading
    public virtual IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

    public void AddTicket(Ticket ticket)
    {
        if (!_tickets.Contains(ticket))
        {
            _tickets.Add(ticket);
        }
    }

    public void MarkEventAsCancelled()
    {
        foreach (var ticket in _tickets)
        {
            ticket.SetAvailability(false);
        }
    }

    public decimal GetRevenue()
    {
        return _tickets.Where(t => !t.IsAvailable()).Sum(t => t.Price);
    }
}