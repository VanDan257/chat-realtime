using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using notip_server.Dto;
using notip_server.Interfaces;
using notip_server.Middlewares;
using notip_server.ViewModel.Friend;
using notip_server.ViewModel.User;

namespace notip_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientRoleMiddleware))]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService friendService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;

        public FriendController(IFriendService friendService, IHttpContextAccessor contextAccessor, IUserService userService)
        {
            this.friendService = friendService;
            _userService = userService;
            _contextAccessor = contextAccessor;
        }

        [HttpGet("get-list-friend")]
        public async Task<IActionResult> GetListFriend([FromQuery] GetContactRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                responseAPI.Data = await friendService.GetListFriend(userId, request);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        
        [HttpGet("get-list-friend-invite")]
        public async Task<IActionResult> GetListFriendInvite([FromQuery]GetContactRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                responseAPI.Data = await friendService.GetListFriendInvite(userId, request);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("send-friend-request")]
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(FriendRequest receiver)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                await friendService.SendFriendRequest(userId, receiver.UserCode);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("accept-friend-request")]
        [HttpPatch]
        public async Task<IActionResult> AcceptFriendRequest(FriendRequest receiver)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                await friendService.AcceptFriendRequest(userId, receiver.UserCode);

                return Ok();
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("cancel-friend-request")]
        [HttpPatch]
        public async Task<IActionResult> CancelFriendRequest(FriendRequest receiver)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                await friendService.CancelFriendRequest(userId, receiver.UserCode);

                return Ok();
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("block-user")]
        [HttpPost]
        public async Task<IActionResult> BlockUser(FriendRequest receiver)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                await friendService.BlockUser(userId, receiver.UserCode);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("unblock-user")]
        [HttpPatch]
        public async Task<IActionResult> UnBlockUser(FriendRequest receiver)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                await friendService.UnBlockUser(userId, receiver.UserCode);

                return Ok();
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

    }
}
