using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using notip_server.Dto;
using notip_server.Interfaces;
using notip_server.Middlewares;
using notip_server.ViewModel.Common;
using notip_server.ViewModel.User;

namespace notip_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AdminRoleMiddleware))]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUser([FromQuery]PagingRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                var users = await _userService.GetAllUser(request);
                responseAPI.Data = users;
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpGet("get-staffs")]
        public async Task<IActionResult> GetStaffs([FromQuery]PagingRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                var users = await _userService.GetStaffs(request);
                responseAPI.Data = users;
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateStaff(CreateAdminRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                await _userService.CreateStaff(request);
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
