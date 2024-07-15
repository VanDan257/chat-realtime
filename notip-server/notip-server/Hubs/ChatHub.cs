using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using notip_server.Data;
using notip_server.Dto;
using notip_server.Models;
using System.Collections.Concurrent;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace notip_server.Hubs
{
    public class ChatHub : Hub
    {
        //              <userCode, ConnectionId>
        private Dictionary<Guid, string> users = new Dictionary<Guid, string>();

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var param = httpContext.Request.Query["userCode"].ToString();
            Guid.TryParse(param, out var userCode);

            if (!string.IsNullOrEmpty(userCode.ToString()) && !users.ContainsKey(userCode))
            {
                users.Add(userCode, Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            RemoveByValue(users, Context.ConnectionId);
                //users.TryRemove(Context.ConnectionId, out user);
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Gửi tin nhắn
        /// </summary>
        /// <param name="receiverIds">danh sách Code người nhận tin nhắn</param>
        /// <param name="message">Tin nhắn</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task SendMessage(List<Guid> receiverIds, string payload)
        {
            try
            {
                foreach(var receiverId in receiverIds)
                {
                    if (users.TryGetValue(receiverId, out var connectionId))
                    {
                        await Clients.Client(connectionId).SendAsync("ReceiveMessage", payload);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Tìm kiếm user trực tuyến theo theo key
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool SearchByKey(Dictionary<string, string> dict, string key)
        {
            foreach (var item in dict)
            {
                if (item.Key == key)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Xoá Dictionary theo value
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="value"></param>
        private static void RemoveByValue(Dictionary<Guid, string> dictionary, string value)
        {
            // Tìm tất cả các key có giá trị tương ứng cần xóa
            var keysToRemove = dictionary.Where(kvp => kvp.Value == value).Select(kvp => kvp.Key).ToList();

            // Xóa các phần tử theo key
            foreach (var key in keysToRemove)
            {
                dictionary.Remove(key);
            }
        }

        //test hub
        /*
        public async Task AskServer(string textFromClient)
        {
            string tempString;

            if (textFromClient == "hello")
                tempString = "Message was: xin chao...";
            else
                tempString = "Message was: tam biet";

            await Clients.Clients(this.Context.ConnectionId).SendAsync("askServerRespone", tempString);
        }
        */
    }
}
