namespace notip_server.ViewModel.User
{
    public class UpdateProfileRequest
    {
        public string? UserName { get; set; }
        public string? Gender { get; set; }
        public string? Dob { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateAvatarRequest
    {
        public List<IFormFile> Image{ get; set; }
    }
}
