using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using notip_server.Data;
using notip_server.Dto;
using notip_server.Hubs;
using notip_server.Models;
using notip_server.Interfaces;
using notip_server.Utils;
using RestSharp;

namespace notip_server.Service
{
    public class CallService : ICallService
    {
        private readonly DbChatContext chatContext;
        private readonly IWebHostEnvironment webHostEnvironment;
        private IHubContext<ChatHub> chatHub;
        private static readonly IDictionary<string, object> _configProperties = new Dictionary<string, object>()
        {
            { "properties",
                new {
                    enable_chat = true,
                    enable_advanced_chat = true,
                    enable_hand_raising = true,
                    enable_emoji_reactions = true,
                    enable_noise_cancellation_ui = true,
                    start_video_off = true,
                    start_audio_off = true,
                }
            }
        };
        public CallService(DbChatContext chatContext, IWebHostEnvironment webHostEnvironment, IHubContext<ChatHub> chatHub)
        {
            this.chatContext = chatContext;
            this.webHostEnvironment = webHostEnvironment;
            this.chatHub = chatHub;
        }

        /// <summary>
        /// Danh sách lịch sử cuộc gọi
        /// </summary>
        /// <param name="userSession">User hiện tại đang đăng nhập</param>
        /// <returns>Danh sách lịch sử cuộc gọi</returns>
        public async Task<List<GroupCallDto>> GetCallHistory(Guid userSession)
        {
            List<GroupCallDto> groupCalls = await chatContext.GroupCalls
                     .Where(x => x.Calls.Any(y => y.UserCode.Equals(userSession)))
                     .Select(x => new GroupCallDto()
                     {
                         Code = x.Code,
                         Name = x.Name,
                         Avatar = x.Avatar,
                         LastActive = x.LastActive,
                         Calls = x.Calls.OrderByDescending(y => y.Created)
                             .Select(y => new CallDto()
                             {
                                 UserCode = y.UserCode,
                                 User = new UserDto()
                                 {
                                     UserName = y.User.UserName,
                                     Avatar = y.User.Avatar
                                 },
                                 Created = y.Created,
                                 Status = y.Status
                             }).ToList()
                     }).ToListAsync();
            groupCalls.ForEach(grpCall =>
            {
                /// hiển thị tên cuộc gọi là người trong cuộc thoại (không phải người đang đang nhập)
                /// VD; A gọi cho B => Màn hình A sẽ nhìn trên danh sách cuộc gọi tên B và ngược lại.
                var us = grpCall.Calls.FirstOrDefault(x => !x.UserCode.Equals(userSession));
                grpCall.Name = us?.User.UserName;
                grpCall.Avatar = us?.User.Avatar;

                grpCall.LastCall = grpCall.Calls.FirstOrDefault();
            });

            return groupCalls.OrderByDescending(x => x.LastActive).ToList();
        }

        /// <summary>
        /// Thông tin chi tiết lịch sử cuộc gọi
        /// </summary>
        /// <param name="userSession">User hiện tại đang đăng nhập</param>
        /// <param name="groupCallCode">Mã cuộc gọi</param>
        /// <returns>Danh sách cuộc gọi</returns>
        public async Task<List<CallDto>> GetHistoryById(Guid userSession, Guid groupCallCode)
        {
            Guid friend = await chatContext.Calls
                .Where(x => x.GroupCallCode.Equals(groupCallCode) && x.UserCode != userSession)
                .Select(x => x.UserCode)
                .FirstOrDefaultAsync();

            return await chatContext.Calls
                .Where(x => x.UserCode.Equals(userSession) && x.GroupCallCode.Equals(groupCallCode))
                .OrderByDescending(x => x.Created)
                .Select(x => new CallDto()
                {
                    Status = x.Status,
                    Created = x.Created,
                    UserCode = friend
                })
                .ToListAsync();
        }

        /// <summary>
        /// Thực hiện cuộc gọi
        /// </summary>
        /// <param name="userSession">User hiện tại đang đăng nhập</param>
        /// <param name="callTo">Người được gọi</param>
        /// <returns>Đường link truy câp cuộc gọi</returns>
        public async Task<string> Call(Guid userSession, Guid callTo)
        {
            #region Gọi API tạo room - daily.co
            //var e = "{ properties: { enable_chat: true } }";
            //var d = new Dictionary<string, object>() {
            //    { "properties", new { enable_chat = true, enable_hand_raising = true } }
            //};
            var url = "https://api.daily.co/";  
            var client = new RestClient(url);
           
            var request = new RestRequest("v1/rooms", Method.Post);
            
            request.AddHeader("Authorization", $"Bearer {EnviConfig.DailyToken}");
            request.AddJsonBody(_configProperties);

            RestResponse response = await client.ExecutePostAsync(request);
            DailyResponse dailyRoomResp = JsonConvert.DeserializeObject<DailyResponse>(response.Content);
            #endregion

            // Lấy thông tin lịch sử cuộc gọi đã gọi cho user
            Guid grpCallCode = await chatContext.GroupCalls
                       .Where(x => x.Type.Equals(Constants.GroupType.SINGLE))
                       .Where(x => x.Calls.Any(y => y.UserCode.Equals(userSession) &&
                                   x.Calls.Any(y => y.UserCode.Equals(callTo))))
                       .Select(x => x.Code)
                       .FirstOrDefaultAsync();

            GroupCall groupCall = await chatContext.GroupCalls.FirstOrDefaultAsync(x => x.Code.Equals(grpCallCode));
            DateTime dateNow = DateTime.Now;

            User userCallTo = await chatContext.Users.FirstOrDefaultAsync(x => x.Id.Equals(callTo));

            // Kiểm tra lịch sử cuộc gọi đã tồn tại hay chưa. Nếu chưa => tạo nhóm gọi mới.
            if (groupCall == null)
            {
                groupCall = new GroupCall()
                {
                    Code = Guid.NewGuid(),
                    Created = dateNow,
                    CreatedBy = userSession,
                    Type = Constants.GroupType.SINGLE,
                    LastActive = dateNow
                };
                await chatContext.GroupCalls.AddAsync(groupCall);
            }

            /// Thêm danh sách thành viên trong cuộc gọi. Mặc định người gọi trạng thái OUT_GOING
            /// Người được gọi trạng thái MISSED. Nếu người được gọi join vào => CHuyển trạng thái IN_COMING

            List<Call> calls = new List<Call>(){
                new Call()
                {
                    GroupCallCode = groupCall.Code,
                    UserCode = userSession,
                    Status =Constants.CallStatus.OUT_GOING,
                    Created = dateNow,
                    Url =dailyRoomResp.url,
                },
                new Call()
                {
                    GroupCallCode = groupCall.Code,
                    UserCode = userCallTo.Id,
                    Status =Constants.CallStatus.MISSED,
                    Created = dateNow,
                    Url = dailyRoomResp.url,
                }
            };

            await chatContext.Calls.AddRangeAsync(calls);
            await chatContext.SaveChangesAsync();

            ///Truyền thông tin realtime cuộc gọi. Thông tin hubConnection của user.
            //if (!string.IsNullOrWhiteSpace(userCallTo.CurrentSession))
            //    await chatHub.Clients.Client(userCallTo.CurrentSession).SendAsync("callHubListener", dailyRoomResp.url);

            return dailyRoomResp.url;
        }

        /// <summary>
        /// Tham gia cuộc gọi. Cập nhật trạng thái cuộc gọi thành IN_COMING
        /// </summary>
        /// <param name="userSession">User hiện tại đang đăng nhập</param>
        /// <param name="url">Đường dẫn truy cập video call</param>
        public async Task JoinVideoCall(Guid userSession, string url)
        {
            Call call = await chatContext.Calls
                .Where(x => x.UserCode.Equals(userSession) && x.Url.Equals(url))
                .FirstOrDefaultAsync();

            if (call != null)
            {
                call.Status = Constants.CallStatus.IN_COMMING;
                await chatContext.SaveChangesAsync();
            }
        }


        /// <summary>
        /// Hủy cuộc gọi
        /// </summary>
        /// <param name="userCode">User hiện tại đang đăng nhập</param>
        /// <param name="url">Đường dẫn truy cập video call</param>
        public async Task CancelVideoCall(Guid userSession, string url)
        {
            string urlCall = await chatContext.Calls
                .Where(x => x.UserCode.Equals(userSession) && x.Url.Equals(url))
                .Select(x => x.Url)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(urlCall))
            {
                try
                {
                    #region gọi API xóa đường dẫn video call trên daily
                    var urlDelete = "https://api.daily.co/";
                    var client = new RestClient(urlDelete);

                    var request = new RestRequest($"v1/rooms/{urlCall.Split('/').Last()}", Method.Delete);
                    request.AddHeader("Authorization", $"Bearer {EnviConfig.DailyToken}");
                    RestResponse response = await client.ExecuteAsync(request);
                    #endregion
                }
                catch (Exception ex) { }
            }
        }
    }
}
