namespace BizOpsAPI.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public int UserId { get; set; }
        public required string ItemName { get; set; }
        public required string Category { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public required string ReceiptUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
