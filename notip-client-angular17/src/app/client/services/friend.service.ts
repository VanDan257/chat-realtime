import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ClientApi } from '../client-api';

@Injectable({
  providedIn: 'root',
})
export class FriendService {
  constructor(private http: HttpClient) {}

  getListFriend(params: any) {
    let httpParams = new HttpParams();
    for (const key in params) {
      if (params.hasOwnProperty(key)) {
        httpParams = httpParams.append(key, params[key]);
      }
    }
    return this.http.get(ClientApi.GetListFriend, { params: httpParams });
  }

  getListFriendInvite(params: any) {
    let httpParams = new HttpParams();
    for (const key in params) {
      if (params.hasOwnProperty(key)) {
        httpParams = httpParams.append(key, params[key]);
      }
    }
    return this.http.get(ClientApi.GetListFriendInvite, {
      params: httpParams,
    });
  }

  sendFriendRequest(receiverCode: string) {
    return this.http.post(ClientApi.SendFriendRequest, {
      userCode: receiverCode,
    });
  }

  acceptFriendRequest(receiverCode: string) {
    return this.http.patch(ClientApi.AcceptFriendRequest, {
      userCode: receiverCode,
    });
  }

  cancelFriendRequest(receiverCode: string) {
    return this.http.patch(ClientApi.CancelFriendRequest, {
      userCode: receiverCode,
    });
  }

  blockUser(receiverCode: string) {
    return this.http.post(ClientApi.BlockUser, { userCode: receiverCode });
  }

  unblockUser(receiverCode: string) {
    return this.http.patch(ClientApi.UnblockUser, { userCode: receiverCode });
  }
}
