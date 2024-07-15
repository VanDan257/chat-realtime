using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using notip_server.Data;
using notip_server.Dto;
using notip_server.Models;
using notip_server.Interfaces;
using notip_server.Utils;
using notip_server.ViewModel.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity.Data;

namespace notip_server.Service
{
    public class AuthService : IAuthService
    {
        private readonly DbChatContext chatContext;
        private readonly UserManager<User> _userManager;
        private readonly IPasswordService _passwordService;
        private readonly ISendMailService _sendMailService;

        public AuthService(DbChatContext chatContext, IPasswordService passwordService, ISendMailService sendMailService, UserManager<User> userManager)
        {
            this.chatContext = chatContext;
            _passwordService = passwordService;
            _sendMailService = sendMailService;
            _userManager = userManager;
        }

        /// <summary>
        /// Đăng nhập hệ thống
        /// </summary>
        /// <param name="user">Thông tin tài khoản người dùng</param>
        /// <returns>AccessToken</returns>
        public async Task<AccessToken> Login(LoginRequest request)
        {
            // 1. Validate the user exists
            if (await chatContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email) is not User user)
            {
                throw new Exception("Email không tồn tại");
            }

            // 2. Validate the password is correct
            if (!_passwordService.VerifyPassword(user.PasswordHash, user.PasswordSalt, request.Password))
            {
                user.AccessFailedCount = user.AccessFailedCount + 1;

                if(user.AccessFailedCount == 3) 
                {
                    string htmlBodyMail = "<h3>Có 1 truy cập đang cố gắng đăng nhập vào tài khoản của bạn! " +
                        "<br>Hãy đổi mật khẩu mạnh nếu đó không phải bạn.</h3>" +
                        "<h3>Nếu bạn quên mật khẩu, hãy lấy click vào đường link dưới đây để lấy lại mật khẩu</h3>" +
                        "<a href=\"http://localhost:4200/forgot-passsword\">Lấy lại mật khẩu</a>";
                    await _sendMailService.SendEmailAsync(user.Email, "Cảnh báo đăng nhập!", htmlBodyMail);
                }
                
                await chatContext.SaveChangesAsync();
                throw new Exception("Mật khẩu không chính xác.");
            }

            if(user.AccessFailedCount > 0)
            {
                user.AccessFailedCount = 0;
            }
            user.LastLogin = DateTime.Now;
            await chatContext.SaveChangesAsync();

            var role = await chatContext.UserRoles.Where(u => u.UserId == user.Id)
                .Join(chatContext.Roles,
                    userRoles => userRoles.RoleId,
                    roles => roles.Id,
                    (userRoles, roles) => roles.NormalizedName)
                .FirstOrDefaultAsync();

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(EnviConfig.SecretKey);

            DateTime expirationDate = DateTime.Now.Date.AddMinutes(EnviConfig.ExpirationInMinutes);
            long expiresAt = (long)(expirationDate - new DateTime(1970, 1, 1)).TotalSeconds;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.Expiration, expiresAt.ToString())

                }),
                Expires = expirationDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return new AccessToken
            {
                Id = user.Id,
                UserName = user.UserName,
                Avatar = user.Avatar,
                Token = jwtTokenHandler.WriteToken(token)
            };
        }

        /// <summary>
        /// Đăng ký tài khoản người dùng
        /// </summary>
        /// <param name="user">Thông tin tài khoản</param>
        public async Task SignUp(SignUpRequest request)
        {
            if (await chatContext.Users.AnyAsync(x => x.Email.Equals(request.Email)))
                throw new Exception("Email đã tồn tại");

            var saltPassword = _passwordService.GenerateSalt();
            var hashPassword = _passwordService.HashPassword(request.Password, saltPassword);

            User newUser = new User()
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.Phone,
                PasswordSalt = saltPassword,
                PasswordHash = hashPassword,
                Avatar = Constants.AVATAR_DEFAULT,
                NormalizedEmail = request.Email.ToLower(),
                NormalizedUserName = request.UserName.ToLower(),
            };

            var role = await chatContext.Roles.FirstOrDefaultAsync(x => x.NormalizedName == Constants.Role.CLIENT);
            UserRole userRole = new UserRole();
            if (role != null)
            {
                userRole.UserId = newUser.Id;
                userRole.RoleId = role.Id;
            }

            await chatContext.Users.AddAsync(newUser);
            await chatContext.UserRoles.AddAsync(userRole);
            await chatContext.SaveChangesAsync();

            await _userManager.UpdateSecurityStampAsync(newUser);
        }

        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task ForgetPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                    throw new Exception("Email không tồn tại!");

                // Kiểm tra nếu thuộc tính SecurityStamp của người dùng là null, cập nhật nó trước khi thay đổi email
                if (user.SecurityStamp == null)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                }

                string tokenConfirm = await _userManager.GeneratePasswordResetTokenAsync(user);

                string encodeToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tokenConfirm));

                string ressetPasswordUrl = $"http://localhost:4200/lay-lai-mat-khau?email={email}&token={encodeToken}";

                string htmlBodyMail = $"Hãy click vào đường dẫn sau để thiết lập lại mật khẩu: <a href=\"{ressetPasswordUrl}\">Reset password</a>";

                await _sendMailService.SendEmailAsync(email, "Thiết lập lại mật khẩu!", htmlBodyMail);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordRequest request) 
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if(user is null)
            {
                throw new Exception("Email không tồn tại!");
            }

            if (string.IsNullOrEmpty(request.ResetCode))
            {
                throw new Exception("Token không hợp lệ");
            }

            string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));

            var identityResult = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (identityResult.Succeeded)
            {
                return true;
            }
            else
            {
                foreach (var error in identityResult.Errors)
                {
                    Console.WriteLine("ResetPasswordAsync: ", error.Description);
                }
            }
            return false;
        }

        public async Task UserAccessHub(Guid userId)
        {
            try
            {
                var loginUser = new LoginUserHistory
                {
                    UserId = userId,
                    LoginTime = DateTime.Now
                };
                await chatContext.AddAsync(loginUser);
                await chatContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra!");
            }
        }

        /// <summary>
        /// Đăng nhập hệ thống
        /// </summary>
        /// <param name="user">Thông tin tài khoản người dùng</param>
        /// <returns>AccessToken</returns>
        public async Task<AccessToken> LoginAdmin(LoginRequest request)
        {
            // 1. Validate the user exists
            if (await chatContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email) is not User user)
            {
                throw new Exception("Email không tồn tại");
            }

            // 2. Validate the password is correct
            if (!_passwordService.VerifyPassword(user.PasswordHash, user.PasswordSalt, request.Password))
            {
                user.AccessFailedCount = user.AccessFailedCount + 1;

                if (user.AccessFailedCount == 3)
                {
                    string htmlBodyMail = "<h3>Có 1 truy cập đang cố gắng đăng nhập vào tài khoản của bạn! " +
                        "<br>Hãy đổi mật khẩu mạnh nếu đó không phải bạn.</h3>" +
                        "<h3>Nếu bạn quên mật khẩu, hãy lấy click vào đường link dưới đây để lấy lại mật khẩu</h3>" +
                        "<a href=\"http://localhost:4200/quen-mat-khau\">Lấy lại mật khẩu</a>";
                    await _sendMailService.SendEmailAsync(user.Email, "Cảnh báo đăng nhập!", htmlBodyMail);
                }

                await chatContext.SaveChangesAsync();
                throw new Exception("Mật khẩu không chính xác.");
            }

            if (user.AccessFailedCount > 0)
            {
                user.AccessFailedCount = 0;
            }
            
            var role = await chatContext.UserRoles.Where(u => u.UserId == user.Id)
                .Join(chatContext.Roles,
                    userRoles => userRoles.RoleId,
                    roles => roles.Id,
                    (userRoles, roles) => roles.NormalizedName)
                .FirstOrDefaultAsync();
            if(role != Constants.Role.ADMIN && role != Constants.Role.MODERATOR)
            {
                throw new Exception("Bạn không có quyền truy cập vào hệ thống");
            }

            user.LastLogin = DateTime.Now;

            await chatContext.SaveChangesAsync();

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(EnviConfig.SecretKey);

            DateTime expirationDate = DateTime.Now.Date.AddMinutes(EnviConfig.ExpirationInMinutes);
            long expiresAt = (long)(expirationDate - new DateTime(1970, 1, 1)).TotalSeconds;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.Expiration, expiresAt.ToString())

                }),
                Expires = expirationDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return new AccessToken
            {
                Id = user.Id,
                UserName = user.UserName,
                Avatar = user.Avatar,
                Token = jwtTokenHandler.WriteToken(token)
            };
        }
    }
}
