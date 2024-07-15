namespace notip_server.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string? Dob { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Avatar { get; set; }

        public string? Gender { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastLogin { get; set;}
    }
}
