import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { ClientApi } from '../client-api';
import { environment } from "../../../environments/environment.development";

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  constructor(private http: HttpClient) {}

  public hubConnection!: signalR.HubConnection;

  public startConnection = (userCode: string) => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.chatHub}?userCode=${userCode}`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
        // accessTokenFactory: () => userId,
        // headers: {
        //   'User-ID': userId,
        //   "test-header": "abc"
        // }
      })
      .build();

    let http = this.http;

    this.hubConnection
      .start()
      .then(() => {
        console.log('Hub connection started');
        this.hubConnection
          .invoke('getConnectionId')
          .then(function (connectionId) {
            http
              .post(
                ClientApi.UserAccessHub,
                {},
                {
                  params: {
                    key: userCode,
                  },
                }
              )
              .subscribe({
                complete: () => console.log('connectionId: ', connectionId),
                error: (error) => console.log('error: ', error),
                next: (res) => console.log('next: ', res),
              });
          });
      })
      .catch((err) => console.log('Error while starting connection: ' + err));
  };

  public disconnected = () => {
    this.hubConnection.onclose(
      error => {
        if (error) {
          console.error('Connection closed with error: ', error);
        } else {
          console.log('Connection closed');
        }
      }
    )
  }
}
