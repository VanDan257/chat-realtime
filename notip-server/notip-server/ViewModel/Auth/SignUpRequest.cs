using System.ComponentModel.DataAnnotations;

namespace notip_server.ViewModel.Auth
{
    public class SignUpRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }

        public string Phone { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
