using notip_server.Models;
using System.Security.Claims;

namespace notip_server.Interfaces
{
    public interface IJwtService
    {
        ClaimsPrincipal DecodeJWT(string token, string secretAuthkey);
        string EncodeJWT(User user, string role);
    }
}
