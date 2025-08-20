namespace BizOpsAPI.DTOs
{
    public class ClientCreateDto
    {
        public required string ClientName { get; set; }
        public required string CompanyName { get; set; }
        public required string CompanyAddress { get; set; }
        public required string Email { get; set; }
        public required string ContactNumber { get; set; }
    }

    public class ClientUpdateDto
    {
        public required string ClientName { get; set; }
        public required string CompanyName { get; set; }
        public required string CompanyAddress { get; set; }
        public required string Email { get; set; }
        public required string ContactNumber { get; set; }
    }

    public class ClientDto
    {
        public int ClientId { get; set; }
        public int UserId { get; set; }
        public required string ClientName { get; set; }
        public required string CompanyName { get; set; }
        public required string Email { get; set; }
    }
}
