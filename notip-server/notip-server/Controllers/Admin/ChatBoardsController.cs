using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using notip_server.Dto;
using notip_server.Interfaces;
using notip_server.Middlewares;
using notip_server.ViewModel.ChatBoard;
using notip_server.ViewModel.Common;

namespace notip_server.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AdminRoleMiddleware))]
    public class ChatBoardsController : ControllerBase
    {
        private IChatBoardService _chatBoardService;

        public ChatBoardsController(IChatBoardService chatBoardService)
        {
            _chatBoardService = chatBoardService;
        }

        [HttpGet("get-all-chatroom")]
        public async Task<IActionResult> GetAllChatRoom([FromQuery]PagingRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                responseAPI.Data = await _chatBoardService.GetAllChatRoom(request);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpGet("get-detail-chatroom/{groupCode}")]
        public async Task<IActionResult> GetDetailChatRoom(string groupCode)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                Guid.TryParse(groupCode, out Guid code);
                responseAPI.Data = await _chatBoardService.GetDetailChatRoom(code);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpGet("traffic-statistics")]
        public async Task<IActionResult> TrafficStatistics([FromQuery]TrafficStatisticsRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                var data = await _chatBoardService.TrafficStatistics(request);
                responseAPI.Data = data;
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        //[HttpGet("get-all-messages")]
        //public async Task<IActionResult> GetAllMessages()
        //{
        //    ResponseAPI responseAPI = new ResponseAPI();
        //    try
        //    {
        //        var messages = _chatBoardService.GetAllMessages();
        //        responseAPI.Data = messages;
        //        return Ok(responseAPI);
        //    }
        //    catch (Exception ex)
        //    {
        //        responseAPI.Message = ex.Message;
        //        return BadRequest(responseAPI);
        //    }
        //}
    }
}
