using System;

namespace EventManagementSystem.Models
{
public class Venue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public virtual Address Address { get; set; } = new Address();
    public int Capacity { get; set; }

    public int CalculateAvailableSeats(int bookedSeats)
    {
        return Capacity - bookedSeats;
    }
}
}