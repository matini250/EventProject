using System;
public class Ticket
{
    public Guid Id { get; set; } = Guid.NewGuid(); // Primärschlüssel
    public decimal Price { get; set; }
    public string Seat { get; set; } = string.Empty;
    private bool Available { get; set; } = true;

    public bool IsAvailable() => Available;

    public void SetAvailability(bool availability)
    {
        Available = availability;
    }

    // Methode zum Markieren als nicht verfügbar
    public void MarkAsUnavailable()
    {
        Available = false;
    }
}