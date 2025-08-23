namespace BizOpsAPI.DTOs
{
    // For creating a new invoice
    public class CreateInvoiceDto
    {
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public required List<CreateInvoiceItemDto> Items { get; set; }
    }

    public class CreateInvoiceItemDto
    {
        public required string ItemName { get; set; }
        public required int Quantity { get; set; }
        public required decimal UnitPrice { get; set; }
    }

    // For updating an existing invoice
    public class UpdateInvoiceDto
    {
        public required string Status { get; set; } // Allow status changes (Paid/Unpaid)
        public DateTime DueDate { get; set; }
        public required List<UpdateInvoiceItemDto> Items { get; set; }
    }

    public class UpdateInvoiceItemDto
    {
        public int InvoiceItemId { get; set; }
        public required string ItemName { get; set; }
        public required int Quantity { get; set; }
        public required decimal UnitPrice { get; set; }
    }

    // For reading invoices (response)
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public required string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public required List<InvoiceItemDto> Items { get; set; }
    }

    public class InvoiceItemDto
    {
        public int InvoiceItemId { get; set; }
        public required string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
