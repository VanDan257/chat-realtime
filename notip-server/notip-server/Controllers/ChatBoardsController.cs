using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using notip_server.Dto;
using notip_server.Interfaces;
using notip_server.Middlewares;
using notip_server.Service;
using notip_server.ViewModel.ChatBoard;
using notip_server.ViewModel.Common;
using System.Security.Cryptography;
using System.Text;

namespace notip_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ClientRoleMiddleware))] // Áp dụng middleware filter trên controller
    public class ChatBoardsController : ControllerBase
    {
        private IChatBoardService _chatBoardService;
        private readonly IHttpContextAccessor _contextAccessor;

        //private readonly string privateKey = System.IO.File.ReadAllText("private_key.pem");

        public ChatBoardsController(IChatBoardService chatBoardService, IHttpContextAccessor contextAccessor)
        {
            _chatBoardService = chatBoardService;
            _contextAccessor = contextAccessor;
        }

        [Route("get-history")]
        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                responseAPI.Data = await _chatBoardService.GetHistory(userId);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("search-group")]
        [HttpGet]
        public async Task<IActionResult> SearchChatGroup(string keySearch)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                responseAPI.Data = await _chatBoardService.SearchChatGroup(userId, keySearch);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        
        [Route("access-group")]
        [HttpGet]
        public async Task<IActionResult> AccessChatGroup(Guid groupCode)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                responseAPI.Data = await _chatBoardService.AccessChatGroup(userId, groupCode);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-info")]
        [HttpGet]
        public async Task<IActionResult> GetInfo(Guid groupCode)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                responseAPI.Data = await _chatBoardService.GetInfo(userId, groupCode);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }


        [Route("groups")]
        [HttpPost]
        public async Task<IActionResult> AddGroup(AddGroupRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                await _chatBoardService.AddGroup(userId, request);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-group-name")]
        [HttpPut]
        public async Task<IActionResult> UpdateGroupName(UpdateGroupNameRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                await _chatBoardService.UpdateGroupName(request);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("out-group")]
        [HttpDelete]
        public async Task<IActionResult> OutGroup(Guid groupCode)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                await _chatBoardService.OutGroup(userId, groupCode);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-group-avatar")]
        [HttpPut]
        public async Task<IActionResult> UpdateGroupAvatar(UpdateGroupAvatarRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                await _chatBoardService.UpdateGroupAvatar(request);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("add-members-to-group")]
        [HttpPost]
        public async Task<IActionResult> AddMembersToGroup(AddMembersToGroupRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                await _chatBoardService.AddMembersToGroup(request);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [HttpPost("update-photo-chat")]
        public async Task<IActionResult> UpdatePhotoChat(UpdateGroupAvatarRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();
            try
            {
                var data = await _chatBoardService.UpdatePhotoChat(request);
                responseAPI.Data = data;

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("send-message")]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromQuery] Guid groupCode)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                string jsonMessage = HttpContext.Request.Form["data"]!;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                };

                MessageDto message = JsonConvert.DeserializeObject<MessageDto>(jsonMessage, settings);
                message.Attachments = Request.Form.Files.ToList();

                string userSession = SystemAuthorization.GetCurrentUser(_contextAccessor);
                Guid.TryParse(userSession, out var userId);
                await _chatBoardService.SendMessage(userId, groupCode, message);

                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        //[Route("send-message")]
        //[HttpPost]
        //public IActionResult SendMessage([FromBody] EncryptedMessage encryptedMessage)
        //{
        //    try
        //    {
        //        // Giải mã khóa AES bằng khóa riêng tư RSA
        //        var aesKey = DecryptString(encryptedMessage.Key, privateKey);

        //        // Giải mã tin nhắn bằng khóa AES
        //        var decryptedMessage = DecryptAES(encryptedMessage.Message, aesKey);

        //        // Xử lý tin nhắn đã giải mã theo yêu cầu
        //        return Ok(new { message = decryptedMessage });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}


        //private string DecryptString(string cipherText, string privateKey)
        //{
        //    using (RSA rsa = RSA.Create())
        //    {
        //        rsa.ImportFromPem(privateKey.ToCharArray());
        //        var bytesToDecrypt = Convert.FromBase64String(cipherText);
        //        var decryptedBytes = rsa.Decrypt(bytesToDecrypt, RSAEncryptionPadding.Pkcs1);
        //        return Encoding.UTF8.GetString(decryptedBytes);
        //    }
        //}

        //private string DecryptAES(string cipherText, string key)
        //{
        //    var fullCipher = Convert.FromBase64String(cipherText);

        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = Encoding.UTF8.GetBytes(key);
        //        aesAlg.Mode = CipherMode.ECB;
        //        aesAlg.Padding = PaddingMode.PKCS7;

        //        using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
        //        {
        //            using (var msDecrypt = new MemoryStream(fullCipher))
        //            {
        //                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //                {
        //                    using (var srDecrypt = new StreamReader(csDecrypt))
        //                    {
        //                        return srDecrypt.ReadToEnd();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //public class EncryptedMessage
        //{
        //    public string Message { get; set; }
        //    public string Key { get; set; }
        //}

        [Route("get-message-by-group")]
        [HttpGet]
        public async Task<IActionResult> GetMessageByGroup([FromQuery] GetMessageRequest request)
        {
            ResponseAPI responseAPI = new ResponseAPI();

            try
            {
                responseAPI.Data = await _chatBoardService.GetMessageByGroup(request);

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
