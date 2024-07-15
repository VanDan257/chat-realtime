using Microsoft.AspNetCore.Identity;

namespace notip_server.Models
{
    public class User : IdentityUser<Guid>
    {
        public string PasswordSalt { get; set; }

        public string? Dob { get; set; }

        public string? Address { get; set; }

        public string? Avatar { get; set; }

        public string? Gender { get; set; }

        public DateTime? LastLogin { get; set; }

        public string? CurrentSession { get; set; }

        public DateTime? Created { get; set; }

        public virtual ICollection<Call> Calls { get; set; }

        public virtual ICollection<GroupUser> GroupUsers { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<LoginUserHistory> LoginUserHistories { get; set; }
    }
}
