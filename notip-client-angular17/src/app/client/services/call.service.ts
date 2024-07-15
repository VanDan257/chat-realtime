import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ClientApi } from '../client-api';

@Injectable({
    providedIn: 'root',
})

export class CallService {
    constructor(private http: HttpClient) { }

    getCallHistory() {
        return this.http.get(ClientApi.GetCallHistory);
    }

    getCallHistoryById(key: string) {
        return this.http.get(ClientApi.GetCallHistoryById + "/" + key);
    }

    call(callTo: string) {
        return this.http.get(ClientApi.Call + "/" + callTo);
    }

    joinVideoCall(url: string) {
        return this.http.get(ClientApi.JoinVideoCall, {
            params: {
                url
            }
        });
    }

    cancelVideoCall(url: string) {
        return this.http.get(ClientApi.CancelVideoCall, {
          params: {
            url
          }
        });
    }
}