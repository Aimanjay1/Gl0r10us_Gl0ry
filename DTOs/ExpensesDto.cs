namespace BizOpsAPI.DTOs
{
    public class ExpensesDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class ExpenseCreateDto
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class ExpenseUpdateDto
    {
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
    }
}
