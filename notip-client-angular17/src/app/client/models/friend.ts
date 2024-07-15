import { User } from "./user";

export interface Friend extends User{
    // Là bạn bè với user hiện tại
    IsFriend: boolean;
    // Đã gửi lời mời kết bạn đến user hiện tại
    IsSentFriend: boolean;
    // Được user hiện tại gửi lời mời kết bạn
    IsBeenSentFriend: boolean;
    // Chặn tin nhắn và cuộc gọi nhười dùng hiện tại
    IsBlocked: boolean;
    // Bị user hiện tại block
    IsBeenBlocked: boolean;
}