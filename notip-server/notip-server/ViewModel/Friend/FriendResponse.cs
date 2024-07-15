using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace notip_server.ViewModel.Friend
{
    public class FriendResponse
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string? Dob { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Avatar { get; set; }

        public string? Gender { get; set; }

        /// <summary>
        /// Là bạn bè với user hiện tại
        /// </summary>
        public bool IsFriend { get; set; }

        /// <summary>
        /// Đã gửi lời mời kết bạn đến user hiện tại
        /// </summary>
        public bool IsSentFriend { get; set; }

        /// <summary>
        /// Được user hiện tại gửi lời mời kết bạn
        /// </summary>
        public bool IsBeenSentFriend { get; set; }

        /// <summary>
        /// Chặn tin nhắn và cuộc gọi người dùng hiện tại
        /// </summary>
        public bool IsBlocked { get; set; }

        /// <summary>
        /// Bị user hiện tại block
        /// </summary>
        public bool IsBeenBlocked { get; set; }
    }
}