import { environment } from '../../environments/environment.development';

export class AdminApi {
    // auth
    static LoginAdmin = environment.apiUrl + "admin/auths/login";

    // users
    static GetAllUsers = environment.apiUrl + "users/get-all-users";
    static GetStaffs = environment.apiUrl + "users/get-staffs";
    static CreateStaff = environment.apiUrl + "users/create-staff";

    // chatboard
    static GetAllChatRoom = environment.apiUrl + "admin/chatboards/get-all-chatroom";
    static GetDetailChatRoom = environment.apiUrl + "admin/chatboards/get-detail-chatroom";
    static GetAllMessages = environment.apiUrl + "admin/chatboards/get-all-messages";
    static GetTrafficStatistics = environment.apiUrl + "admin/chatboards/traffic-statistics";
}