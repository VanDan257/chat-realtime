import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ClientApi } from '../client-api';
import { User } from '../models/user';

@Injectable({
    providedIn: 'root',
})

export class ChatBoardService {
    constructor(private http: HttpClient) { }

    getHistory() {
        return this.http.get(ClientApi.GetChatHistory);
    }

    getChatBoardInfo(groupCode: string) {
        return this.http.get(ClientApi.GetChatBoardInfo, {
          params: {
            groupCode,
          }
        });
    }

    searchGroup(keySearch: string){
        return this.http.get(ClientApi.SearchGroup, {
            params: {
                keySearch
            }
        });
    }

    accessGroup(groupCode: string){
        return this.http.get(ClientApi.AccessGroup, {
            params: {
                groupCode
            }
        });
    }

    addGroup(group: any) {
        return this.http.post(ClientApi.AddGroup, group);
    }

    OutGroup(groupCode: any){
        return this.http.delete(ClientApi.OutGroup,  {
            params: {
                groupCode: groupCode == null ? "" : groupCode
            }
        });
    }

    updateGroupName(groupCode: string, groupName: string){
        return this.http.put(ClientApi.UpdateGroupName, {
            Code: groupCode,
            Name: groupName
        });
    }

    addMembersToGroup(groupCode: string, members: User[]){
        return this.http.post(ClientApi.AddMembersToGroup, {
            Code: groupCode,
            Users : members
        })
    }

    sendMessage(groupCode: string, message: any) {
        return this.http.post(ClientApi.SendMessage, message, {
            params: {
                groupCode: groupCode == null ? "" : groupCode
            }
        });
    }

    sendMessageEncrypt(request: any) {
        return this.http.post(ClientApi.SendMessage, request);
    }

    getMessageByGroup(params: any) {
        let httpParams = new HttpParams();
        for (const key in params) {
            if (params.hasOwnProperty(key)) {
                httpParams = httpParams.append(key, params[key]);
            }
        }
        return this.http.get(ClientApi.GetMessageByGroup, { params: httpParams });
    }

    getMessageByContact(contactCode: string) {
        return this.http.get(ClientApi.GetMessageByContact + "/" + contactCode);
    }

    downloadFileAttachment(path: string) {
        return this.http.get(
          ClientApi.DownloadFile + "?path=" + path,
          {
            responseType: "blob",
          }
        );
    }

    updateGroupAvatar(request: any) {
        return this.http.post(ClientApi.UpdateGroupAvatar, request);
    }
}