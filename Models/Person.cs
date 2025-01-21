using System;

namespace EventManagementSystem.Models
{
    public abstract class Person
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}