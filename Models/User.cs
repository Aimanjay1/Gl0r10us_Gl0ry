using System.Collections.Generic;

namespace BizOpsAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public required string CompanyName { get; set; }
        public required string CompanyAddress { get; set; }
        public required string Email { get; set; }
        public required string ContactNumber { get; set; }
        public required string LogoUrl { get; set; }
        public required string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Client> Clients { get; set; } = new List<Client>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }

}
