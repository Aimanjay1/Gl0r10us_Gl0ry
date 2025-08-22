namespace BizOpsAPI.DTOs
{
    public class CreateUserDto
    {
        public required string FullName { get; set; }
        public required string CompanyName { get; set; }
        public required string CompanyAddress { get; set; }
        public required string Email { get; set; }
        public required string ContactNumber { get; set; }
        public required string LogoUrl { get; set; }
        public required string Password { get; set; }
    }

    public class UpdateUserDto
    {
        public required string FullName { get; set; }
        public required string CompanyName { get; set; }
        public required string CompanyAddress { get; set; }
        public required string ContactNumber { get; set; }
        public required string LogoUrl { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public required string FullName { get; set; }
        public required string CompanyName { get; set; }
        public required string Email { get; set; }
        public required string LogoUrl { get; set; }
    }
}
