using notip_server.Dto;
using notip_server.Models;
using notip_server.ViewModel.Auth;
using Microsoft.AspNetCore.Identity.Data;

namespace notip_server.Interfaces
{
    public interface IAuthService
    {
        Task<AccessToken> Login(LoginRequest request);
        Task SignUp(SignUpRequest request);
        Task ForgetPassword(string email);
        Task<bool> ResetPassword(ResetPasswordRequest request);
        Task UserAccessHub(Guid userId);
        Task<AccessToken> LoginAdmin(LoginRequest request);
    }
}
