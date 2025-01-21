using System;
using System.Collections.Generic;

namespace EventManagementSystem.Models
{
    public class Administrator : Person
    {
        // Navigationseigenschaft mit virtual für Lazy Loading
        public virtual List<Event> ManagedEvents { get; set; } = new List<Event>();

        // Methode, um Events hinzuzufügen
        public void AddEvent(Event newEvent)
        {
            if (newEvent == null) throw new ArgumentNullException(nameof(newEvent));
            ManagedEvents.Add(newEvent);
        }
    }
}