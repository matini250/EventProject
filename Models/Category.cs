using System;
using System.Collections.Generic;

namespace EventManagementSystem.Models
{
public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public virtual List<Event> Events { get; private set; } = new List<Event>();

    public IEnumerable<Event> GetEvents()
    {
        return Events;
    }
}
}