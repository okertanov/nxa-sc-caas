using System;

namespace NXA.SC.Caas.Models
{
    public sealed class ApiToken
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Token { get; set; } = String.Empty;
        public DateTime ExpirationDate { get; set; }
        public bool Active { get; set; } = false;
        public int? ClientId { get; set; }
        public string? Description { get; set; }
        public int Rates { get; set; }
    }
}
