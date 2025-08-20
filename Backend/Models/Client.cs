namespace BizOpsAPI.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public required string ClientName { get; set; }
        public required string CompanyName { get; set; }
        public required string CompanyAddress { get; set; }
        public required string Email { get; set; }
        public required string ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required User User { get; set; }
        public required ICollection<Invoice> Invoices { get; set; }
    }
}
