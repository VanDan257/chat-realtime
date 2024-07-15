import { environment } from '../../environments/environment.development';

export class ClientApi {
  static Login = environment.apiUrl + 'auths/login';
  static SignUp = environment.apiUrl + 'auths/sign-up';
  //static Image = environment.apiUrl + "img";
  static DownloadFile = environment.apiUrl + 'file';
  static UserAccessHub = environment.apiUrl + 'user-access-hub';
  static ForgotPassword = environment.apiUrl + "auths/forgot-password";
  static ResetPassword = environment.apiUrl + "auths/reset-password";

  static GetChatHistory = environment.apiUrl + 'chatBoards/get-history';
  static SearchGroup = environment.apiUrl + 'chatBoards/search-group';
  static AccessGroup = environment.apiUrl + 'chatBoards/access-group';
  static GetChatBoardInfo = environment.apiUrl + 'chatBoards/get-info';
  static AddGroup = environment.apiUrl + 'chatBoards/groups';
  static OutGroup = environment.apiUrl + 'chatBoards/out-group';
  static SendMessage = environment.apiUrl + 'chatBoards/send-message';
  static GetMessageByGroup =
    environment.apiUrl + 'chatBoards/get-message-by-group';
  static GetMessageByContact =
    environment.apiUrl + 'chatBoards/get-message-by-contact';
  static AddMembersToGroup =
    environment.apiUrl + 'chatBoards/add-members-to-group';
  static UpdateGroupName = environment.apiUrl + 'chatBoards/update-group-name';
  static UpdateGroupAvatar =
    environment.apiUrl + 'chatBoards/update-photo-chat';

  static GetCallHistory = environment.apiUrl + 'calls/get-history';
  static GetCallHistoryById = environment.apiUrl + 'calls/get-history';
  static Call = environment.apiUrl + 'calls/call';
  static JoinVideoCall = environment.apiUrl + 'calls/join-video-call';
  static CancelVideoCall = environment.apiUrl + 'calls/cancel-video-call';

  static GetProfile = environment.apiUrl + 'users/profile';
  static UpdateProfile = environment.apiUrl + 'users/profile';
  static AddContact = environment.apiUrl + 'users/contacts';
  static GetContact = environment.apiUrl + 'users/get-contact';
  static UpdateAvatar = environment.apiUrl + 'users/update-avatar';

  static GetListFriend = environment.apiUrl + 'friend/get-list-friend';
  static GetListFriendInvite = environment.apiUrl + 'friend/get-list-friend-invite';
  static SendFriendRequest = environment.apiUrl + 'friend/send-friend-request';
  static AcceptFriendRequest =
    environment.apiUrl + 'friend/accept-friend-request';
  static CancelFriendRequest =
    environment.apiUrl + 'friend/cancel-friend-request';
  static BlockUser = environment.apiUrl + 'friend/block-user';
  static UnblockUser = environment.apiUrl + 'friend/unblock-user';
}
