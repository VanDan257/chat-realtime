using Microsoft.AspNetCore.Mvc;
using notip_server.Dto;
using notip_server.Interfaces;
using notip_server.ViewModel.Auth;
using Microsoft.AspNetCore.Identity.Data;
using notip_server.Middlewares;

namespace notip_server.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private IAuthService _authService;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthsController(IAuthService authService, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor contextAccessor)
        {
            _authService = authService;
            _webHostEnvironment = webHostEnvironment;
            _contextAccessor = contextAccessor;
        }


        [Route("auths/login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                AccessToken accessToken = await _authService.Login(request);
                responseAPI.Data = accessToken;
                
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("auths/sign-up")]
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                await _authService.SignUp(request);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpPost("admin/auths/login")]
        public async Task<IActionResult> LoginAdmin(LoginRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                AccessToken accessToken = await _authService.LoginAdmin(request);
                responseAPI.Data = accessToken;

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpGet("auths/forgot-password/{email}")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                await _authService.ForgetPassword(email);
                responseAPI.Status = 200;

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpPost("auths/reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                if(await _authService.ResetPassword(request))
                {
                    responseAPI.Message = "Lấy lại mật khẩu thành công";
                }
                else
                {
                    responseAPI.Message = "Lấy lại mật khẩu không thành công";
                    return BadRequest(responseAPI);
                }

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpGet("file")]
        public async Task<IActionResult> DownloadFile(string path)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", path);
                if (!System.IO.File.Exists(filePath))
                    return NotFound("File not found.");

                var fileName = filePath.Split("/");
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                return File(fileStream, "application/octet-stream", fileName[fileName.Length - 1]);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpPost("user-access-hub")]
        public async Task<IActionResult> UserAccessHub()
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                await _authService.UserAccessHub(userId);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
    }
}
