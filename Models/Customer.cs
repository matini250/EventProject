using System.Collections.Generic;
using System.Linq;

namespace EventManagementSystem.Models
{
    public class Customer : Person
    {
        private readonly List<Ticket> _bookedTickets = new List<Ticket>();
        public virtual IReadOnlyCollection<Ticket> BookedTickets => _bookedTickets.AsReadOnly();

        // Ticket buchen
        public void BookTicket(Ticket ticket)
        {
            if (ticket.IsAvailable() && !_bookedTickets.Contains(ticket))
            {
                _bookedTickets.Add(ticket);
                ticket.SetAvailability(false); // Setzt das Ticket als nicht verfügbar
            }
        }

        // Alle aktiven Buchungen abrufen
        public IEnumerable<Ticket> GetActiveBookings()
        {
            return _bookedTickets.Where(ticket => !ticket.IsAvailable());
        }

        // Buchung eines Tickets stornieren
        public void CancelBooking(Ticket ticket)
        {
            if (_bookedTickets.Contains(ticket))
            {
                ticket.SetAvailability(true); // Setzt das Ticket als verfügbar
                _bookedTickets.Remove(ticket);
            }
        }
    }
}